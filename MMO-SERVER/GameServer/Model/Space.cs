﻿using System;
using System.Collections.Generic;
using System.Linq;
using GameServer.Manager;
using GameServer.Combat;
using Google.Protobuf;
using AOI;
using GameServer.Utils;
using System.Collections.Concurrent;
using Common.Summer.Core;
using HS.Protobuf.Scene;
using HS.Protobuf.SceneEntity;

namespace GameServer.Model
{
    /// <summary>
    /// 空间、地图、场景
    /// 本场景的操作需要线性化
    /// </summary>
    public class Space
    {
        public int SpaceId => def.SID;
        private SpaceDefine def { get; set; }

        public ConcurrentQueue<Action> actionQueue = new ConcurrentQueue<Action>();                 //任务队列,将space中的操作全部线性化，避免了并发问题
        private Dictionary<int, Character> characterDict = new Dictionary<int, Character>();        //当前地图中所有的Character<entityId，角色引用>
        public MonsterManager monsterManager = new MonsterManager();                                //怪物管理器，负责当前场景的怪物创建和销毁
        public SpawnManager spawnManager = new SpawnManager();                                      //怪物孵化器，负责怪物的孵化
        public FightManager fightManager = new FightManager();                                      //战斗管理器，负责技能、投射物、伤害、actor信息的更新
        public EItemManager itemManager = new EItemManager();                                       //物品管理器，管理场景中出现的物品
        public AoiZone aoiZone = new AoiZone(0.001f,0.001f);                                        //AOI管理器：十字链表空间(unity坐标系)
        private Vector2 viewArea = new(Config.Server.aoiViewArea, Config.Server.aoiViewArea);


        public Space(){}
        public Space(SpaceDefine spaceDefine)
        {
            def = spaceDefine;
            monsterManager.Init(this);
            spawnManager.Init(this);
            fightManager.Init(this);
            itemManager.Init(this);
        }

        public void Update()
        {
            spawnManager.Update();
            fightManager.OnUpdate(MyTime.deltaTime);
            while(actionQueue.TryDequeue(out var action))
            {
                action?.Invoke();
            }
        }

        /// <summary>
        ///   entity进入sapce
        /// </summary>
        /// <param name="entity"></param>
        public void EntityJoin(Entity entity)
        {
            actionQueue.Enqueue(() => {

                //加入aoi空间
                aoiZone.Enter(entity);

                //处理中心信息
                IMessage message = null;
                List<Entity> views = null;

                //1.角色
                if (entity is Character character)
                {
                    //1.将新加入的character交给当前场景来管理
                    character.OnEnterSpace(this);
                    characterDict[character.EntityId] = character;

                    //2.新上线的玩家需要获取场景中:全部的角色/怪物/物品的信息
                    SpaceEnterResponse resp = new SpaceEnterResponse();
                    resp.Character = character.Info;
                    var nearbyEntity = aoiZone.FindViewEntity(character.EntityId);
                    foreach (var ent in nearbyEntity)
                    {
                        if (ent is Actor acotr)
                        {
                            resp.CharacterList.Add(acotr.Info);
                        }
                        else if (ent is EItem item)
                        {
                            resp.EItemList.Add(item.NetEItem);
                        }
                    }
                    character.session.Send(resp);   //通知自己

                    //3.通知附近玩家。
                    var resp2 = new SpaceCharactersEnterResponse();
                    resp2.SpaceId = this.SpaceId;
                    resp2.CharacterList.Add(character.Info);

                    views = nearbyEntity.ToList();
                    message = resp2;
                }
                //2.怪物
                else if (entity is Monster mon)
                {
                    mon.OnEnterSpace(this);

                    var resp = new SpaceCharactersEnterResponse();
                    resp.SpaceId = SpaceId; //场景ID
                    resp.CharacterList.Add(mon.Info);

                    message = resp;
                    views = aoiZone.FindViewEntity(mon.EntityId).ToList();
                }
                //3.物品
                else if (entity is EItem ie)
                {
                    var resp = new SpaceItemEnterResponse();
                    resp.NetEItem = ie.NetEItem;

                    message = resp;
                    views = aoiZone.FindViewEntity(ie.EntityId).ToList();
                }

                //通知附近玩家，有entity加入
                foreach (var cc in views.OfType<Character>())
                {
                    cc.session.Send(message);
                }

            });
        }
        /// <summary>
        ///  entity离开space
        /// </summary>
        public void EntityLeave(Entity entity)
        {

            actionQueue.Enqueue(() =>
            {
                //不同类型在space空间中的清理工作
                if (entity is Character chr)
                {
                    characterDict.Remove(entity.EntityId);
                }
                else if (entity is Monster mon)
                {

                }
                else if (entity is EItem ie)
                {

                }

                //告诉其他人
                var views = aoiZone.FindViewEntity(entity.EntityId, false);
                SpaceEntityLeaveResponse resp = new SpaceEntityLeaveResponse();
                resp.EntityId = entity.EntityId;
                foreach (var cc in views.OfType<Character>())
                {
                    cc.session.Send(resp);
                }

                //退出aoi空间
                aoiZone.Exit(entity.EntityId);
            });
        }

        /// <summary>
        /// Actor更新的信息给其他玩家进行转发
        /// </summary>
        /// <param name="entitySync">位置+状态信息</param>
        public void SyncActor(NEntitySync entitySync,Actor self,bool isIncludeSelf = false)
        {
            actionQueue.Enqueue(() => {

                //1.更新Entity信息
                self.EntityData = entitySync.Entity;
                self.State = entitySync.State;
                
                //2.更新aoi空间里面我们的坐标
                var handle = aoiZone.Refresh(self.EntityId, self.AoiPos.x, self.AoiPos.y, viewArea);

                //3.广播同步self信息给视野范围内的玩家(排除自己)
                SpaceEntitySyncResponse resp = new SpaceEntitySyncResponse();
                resp.EntitySync = entitySync;
                var units = EntityManager.Instance.GetEntitiesByIds(handle.ViewEntity);
                foreach (var chr in units.OfType<Character>())
                {
                    chr.session.Send(resp);
                }

                //4.需要让自己的客户端强制位移
                if (isIncludeSelf)
                {
                    resp.EntitySync.Force = true;
                    Character cc = self as Character;
                    cc.session.Send(resp);
                }


                //6.这部分双向通知稍微麻烦
                //自己是chr
                if (self is Character selfChr)
                {
                    //新进入视野的单位，双向通知
                    var enterResp = new SpaceCharactersEnterResponse();
                    enterResp.SpaceId = this.SpaceId;
                    foreach (var key in handle.Newly)
                    {
                        Entity entity = EntityManager.Instance.GetEntityById((int)key);

                        //如果对方是Character
                        if (entity is Character targetChr)
                        {
                            //告诉对方自己已经进入对方视野
                            enterResp.CharacterList.Clear();
                            enterResp.CharacterList.Add(selfChr.Info);
                            targetChr.session.Send(enterResp);

                            //需要告诉自己,目标加入了我们的视野
                            enterResp.CharacterList.Clear();
                            enterResp.CharacterList.Add(targetChr.Info);
                            selfChr.session.Send(enterResp);
                        }
                        //如果对方是monster
                        else if(entity is Monster targetMon)
                        {
                            //需要告诉自己,目标加入了我们的视野
                            enterResp.CharacterList.Clear();
                            enterResp.CharacterList.Add(targetMon.Info);
                            selfChr.session.Send(enterResp);
                        }
                        //如果对方是物品
                        else if (entity is EItem ie)
                        {
                            var ieResp = new SpaceItemEnterResponse();
                            ieResp.NetEItem = ie.NetEItem;
                            selfChr.session.Send(ieResp);
                        }
                    }

                    //远离视野的单位，双向通知
                    var leaveResp = new SpaceEntityLeaveResponse();
                    foreach (var key in handle.Leave)
                    {
                        Entity entity = EntityManager.Instance.GetEntityById((int)key);

                        //如果对方是玩家
                        if (entity is Character targetChr)
                        {
                            //告诉他,自己已经离开他的视野
                            leaveResp.EntityId = self.EntityId;
                            targetChr.session.Send(leaveResp);

                            //同时告诉自己对方离开自己视野
                            leaveResp.EntityId = (int)key;
                            selfChr.session.Send(leaveResp);
                        }
                        else 
                        {
                            //同时告诉自己对方离开自己视野
                            leaveResp.EntityId = (int)key;
                            selfChr.session.Send(leaveResp);
                        }
                    }

                }
                else if(self is Monster selfMon)
                {
                    //新进入视野的单位，双向通知
                    var enterResp = new SpaceCharactersEnterResponse();
                    enterResp.SpaceId = this.SpaceId;
                    foreach (var key in handle.Newly)
                    {
                        Entity entity = EntityManager.Instance.GetEntityById((int)key);

                        //如果对方是Character
                        if (entity is Character targetChr)
                        {
                            //告诉对方自己已经进入对方视野
                            enterResp.CharacterList.Clear();
                            enterResp.CharacterList.Add(selfMon.Info);
                            targetChr.session.Send(enterResp);
                        }
                    }

                    //远离视野的单位，双向通知
                    var leaveResp = new SpaceEntityLeaveResponse();
                    foreach (var key in handle.Leave)
                    {
                        Entity entity = EntityManager.Instance.GetEntityById((int)key);

                        //如果对方是玩家
                        if (entity is Character targetChr)
                        {
                            //告诉他,自己已经离开他的视野
                            leaveResp.EntityId = self.EntityId;
                            targetChr.session.Send(leaveResp);
                        }

                    }
                }


                /*
                //5.新进入视野的单位，双向通知
                var enterResp = new SpaceCharactersEnterResponse();
                enterResp.SpaceId = this.SpaceId;
                foreach (var key in handle.Newly)
                {
                    Entity entity = EntityManager.Instance.GetEntityById((int)key);

                    if (entity is Actor target)
                    {
                        //如果对方是玩家，告诉对方自己已经进入对方视野
                        if (target is Character targetChr)
                        {
                            enterResp.CharacterList.Clear();
                            enterResp.CharacterList.Add(self.Info);
                            targetChr.session.Send(enterResp);
                        }

                        //如果自己是玩家，需要告诉自己,目标加入了我们的视野
                        if (self is Character selfChr)
                        {
                            enterResp.CharacterList.Clear();
                            enterResp.CharacterList.Add(target.Info);
                            selfChr.session.Send(enterResp);
                        }
                    }
                    else if (entity is ItemEntity ie && self is Character selfChr)
                    {
                        var ieResp = new SpaceItemEnterResponse();
                        ieResp.NetItemEntity = ie.NetItemEntity;
                        selfChr.session.Send(ieResp);
                    }

                }
                */

                /*
                //6.远离视野的单位，双向通知
                var leaveResp = new SpaceEntityLeaveResponse();
                foreach (var key in handle.Leave)
                {
                    Entity entity = EntityManager.Instance.GetEntityById((int)key);

                    //如果对方是玩家，告诉他,自己已经离开他的视野
                    if (entity is Character targetChr)
                    {
                        leaveResp.EntityId = self.EntityId;
                        targetChr.session.Send(leaveResp);
                    }
                    else
                    {

                    }

                    //如果自己是玩家，同时告诉自己对方离开自己视野
                    if (self is Character selfChr)
                    {
                        leaveResp.EntityId = (int)key;
                        selfChr.session.Send(leaveResp);
                    }

                }
                */

            });
        }

        /// <summary>
        /// AOI广播
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="msg"></param>
        /// <param name="includeSelf"></param>
        public void AOIBroadcast(Entity entity,IMessage msg, bool includeSelf = false)
        {
            //往aoi视野内进行广播(all Character)一个proto消息
            actionQueue.Enqueue(() => {
                var all = aoiZone.FindViewEntity(entity.EntityId, includeSelf);
                foreach (var chr in all.OfType<Character>())
                {
                    chr.session.Send(msg);
                }
            });

        }
        /// <summary>
        /// 全体广播
        /// </summary>
        /// <param name="msg"></param>
        public void Broadcast(IMessage msg)
        {
            //广播一个proto消息给场景的全体玩家
            actionQueue.Enqueue(() => {
                foreach (var kv in characterDict)
                {
                    kv.Value.session.Send(msg);
                }
            });

        }

        /// <summary>
        /// 传送
        /// </summary>
        /// <param name="targetSpace"></param>
        /// <param name="actor"></param>
        /// <param name="pos"></param>
        /// <param name="dir"></param>
        public virtual void TransmitTo(Space targetSpace, Actor actor,Vector3Int pos, Vector3Int dir = new Vector3Int())
        {
            actionQueue.Enqueue(() =>
            {
                //传送的不是同一场景
                if (this != targetSpace)
                {
                    //1.退出当前场景
                    this.EntityLeave(actor);

                    //设置坐标
                    actor.Position = pos;
                    actor.Direction = dir;
                    actor.currentSpace = targetSpace;

                    //2.进入新场景
                    targetSpace.EntityJoin(actor);

                }
                //传送的是同一场景
                else
                {
                    var entitySync = new NEntitySync();
                    entitySync.State = ActorState.Idle;
                    entitySync.Entity.Position = pos;
                    entitySync.Entity.Direction = dir;
                    SyncActor(entitySync, actor, true);
                }
            });
        }

        /// <summary>
        /// 寻找场景中最近的复活点
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public Vector3Int SearchNearestRevivalPoint(Character chr)
        {
            float comparativetGap = float.MaxValue;
            Vector3Int pos = chr.Position;
            foreach(var pointId in def.RevivalPointS)
            {
                var pointDef = DataManager.Instance.revivalPointDefindeDict[pointId];
                if (pointDef == null) continue;
                var tempPos = new Vector3Int(pointDef.X, pointDef.Y, pointDef.Z);
                var gap = Vector3Int.Distance(chr.Position, tempPos);   
                if(gap < comparativetGap)
                {
                    comparativetGap = gap;
                    pos = tempPos;
                }
            }
            return pos;
        }
    }
}

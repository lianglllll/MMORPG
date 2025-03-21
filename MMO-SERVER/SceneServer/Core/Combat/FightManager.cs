﻿using HS.Protobuf.Combat.Skill;
using HS.Protobuf.Scene;
using HS.Protobuf.SceneEntity;
using SceneServer.Core.Combat.Skills;
using SceneServer.Core.Model.Actor;
using SceneServer.Core.Scene;
using SceneServer.Core.Scene.Component;
using Serilog;
using System.Collections.Concurrent;

namespace SceneServer.Core.Combat
{
    /// <summary>
    /// 战斗管理器
    /// </summary>
    public class FightManager
    {
        // TODO 当前场景下的投射物列表
        public List<Missile> missiles = new List<Missile>();        

        // 待处理的技能施法队列：收集来自各个客户端的施法请求,线性处理，避免多线程并发问题。
        public ConcurrentQueue<CastInfo> castReqQueue = new ConcurrentQueue<CastInfo>();

        // 等待广播队列存在的意义：收集某帧的全部数据一起发送，减少数据包的发送频率
        // 等待广播：技能施法队列：通知各个客户端谁谁谁要施法技能
        public ConcurrentQueue<CastInfo> spellQueue = new ConcurrentQueue<CastInfo>();
        private SpellCastResponse spellCastResponse = new SpellCastResponse();

        // 等待广播：伤害队列：告诉客户端谁谁谁收到伤害了，让其播放一些动画/特效或者ui之类的。这里不做属性更新
        public ConcurrentQueue<Damage> damageQueue = new ConcurrentQueue<Damage>();
        private DamageResponse damageResponse = new DamageResponse();
        
        // 等待广播：人物属性更新的队列
        public ConcurrentQueue<PropertyUpdate> propertyUpdateQueue = new ConcurrentQueue<PropertyUpdate>();
        private PropertyUpdateRsponse propertyUpdateRsponse = new PropertyUpdateRsponse();
        
        public void Init()
        {

        }
        public void UnInit()
        {
        }
        public void Update(float deltaTime)
        {
            //处理施法请求
            while(castReqQueue.TryDequeue(out var cast))
            {
                RunCast(cast);
            }

            //处理飞行物的逻辑更新
            for (int i = 0; i < missiles.Count; i++)
            {
                missiles[i].OnUpdate(deltaTime);
            }

            //广播施法请求
            BroadcastSpellInfo();

            //广播伤害信息
            BroadcastDamage();
            
            //广播actor属性更新信息
            BroadcastProperties();
        }

        private void RunCast(CastInfo cast)
        {
            //1.判断施法者是否存在
            var caster = SceneEntityManager.Instance.GetSceneEntityById(cast.CasterId) as SceneActor;
            if (caster == null)
            {
                Log.Error("RunCast: Caster is null {0}", cast.CasterId);
                return;
            }
            //2.施法技能
            caster.SkillSpell.RunCast(cast);
        }
        private void BroadcastSpellInfo()
        {
            while(spellQueue.TryDequeue(out var item))
            {
                spellCastResponse.List.Add(item);
            }

            if(spellCastResponse.List.Count() > 0)
            {
                //找出所有受影响的玩家们,这里使用set是保证唯一性
                //谁需要看到这个施法的过程，当然是施法者aoi范围内的玩家。
                var hashSet = new HashSet<SceneCharacter>();
                foreach (var item in spellCastResponse.List)
                {
                    //当事人
                    var entity = SceneEntityManager.Instance.GetSceneEntityById(item.CasterId);
                    if (entity == null) continue;
                    var li = SceneManager.Instance.AoiZone.FindViewEntity(entity.EntityId, true);
                    if (li != null)
                    {
                        foreach (var cc in li.OfType<SceneCharacter>())
                        {
                            hashSet.Add(cc);
                        }
                    }
                }

                //广播
                foreach (var cc in hashSet)
                {
                    spellCastResponse.SessionId = cc.SessionId;
                    cc.Send(spellCastResponse);
                }

                spellCastResponse.List.Clear();
            }

        }
        private void BroadcastDamage()
        {
            while(damageQueue.TryDequeue(out var item))
            {
                damageResponse.List.Add(item);
            }
            if(damageResponse.List.Count > 0)
            {
                //找出所有受影响的玩家们,这里使用set是保证唯一性
                //谁需要看到这个伤害信息，当然是受伤害目标九宫格范围内的玩家。
                var hashSet = new HashSet<SceneCharacter>();
                foreach (var item in damageResponse.List)
                {
                    //当事人,你需要让这个给
                    var entity = SceneEntityManager.Instance.GetSceneEntityById(item.TargetId);
                    if (entity == null) continue;
                    var li = SceneManager.Instance.AoiZone.FindViewEntity(entity.EntityId, true);
                    if (li != null)
                    {
                        foreach (var cc in li.OfType<SceneCharacter>())
                        {
                            hashSet.Add(cc);
                        }
                    }
                }

                //广播
                //space.Broadcast(damageResponse);
                foreach (var cc in hashSet)
                {
                    // todo
                    // cc.session.Send(damageResponse);
                }

                damageResponse.List.Clear();
            }

        }
        private void BroadcastProperties()
        {
            while (propertyUpdateQueue.TryDequeue(out var item))
            {
                propertyUpdateRsponse.List.Add(item);
            }
            if (propertyUpdateRsponse.List.Count > 0)
            {
                //找出所有受影响的玩家们,这里使用set是保证唯一性
                //谁需要看到这个属性变换的过程，当然是属性变化者view范围内的玩家。
                var hashSet = new HashSet<SceneCharacter>();
                foreach(var item in propertyUpdateRsponse.List)
                {
                    //当事人
                    var entity = SceneEntityManager.Instance.GetSceneEntityById(item.EntityId);
                    if (entity == null) continue;
                    var li = SceneManager.Instance.AoiZone.FindViewEntity(entity.EntityId, true);
                    if (li != null)
                    {
                        foreach(var cc in li.OfType<SceneCharacter>())
                        {
                            hashSet.Add(cc);
                        }
                    }
                }

                //广播
                foreach (var cc in hashSet)
                {

                    // cc.session.Send(propertyUpdateRsponse);
                }

                propertyUpdateRsponse.List.Clear();
            }

        }

    }
}

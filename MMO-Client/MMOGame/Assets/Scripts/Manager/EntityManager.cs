using Assets.Script.Entities;
using BaseSystem.Tool.Singleton;
using GameServer.Model;
using HS.Protobuf.Scene;
using HS.Protobuf.SceneEntity;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GameClient.Entities
{
    /// <summary>
    /// entity对象管理器，只用于管理当前场景的entity
    /// </summary>
    public class EntityManager : SingletonNonMono<EntityManager>
    {
        //线程安全字典
        private ConcurrentDictionary <int, Entity> entityDict = new ConcurrentDictionary<int, Entity>();

        /// <summary>
        /// 构造函数
        /// </summary>
        public EntityManager() { }

        /// <summary>
        /// 推动entity的逻辑更新，此方法由unity主线程调用
        /// </summary>
        /// <param name="deltatime"></param>
        public virtual void OnUpdate(float deltatime)
        {
            foreach (var entity in entityDict.Values)
            {
                entity.OnUpdate(deltatime);
            }
        }


        /// <summary>
        /// 添加一个Entity
        /// </summary>
        /// <param name="entity"></param>
        public void AddEntity(Entity entity)
        {
            entityDict[entity.EntityId] = entity;
        }

        /// <summary>
        /// 移除一个Entity
        /// </summary>
        /// <param name="entityId"></param>
        public void RemoveEntity(int entityId)
        {
            if (entityDict.ContainsKey(entityId))
            {
                entityDict.Remove(entityId,out Entity entity);
            }
            Kaiyun.Event.FireOut("EntityLeave", entityId);   //GameObjectManager中实现
        }

        /// <summary>
        /// 清除全部entity
        /// </summary>
        public void Clear()
        {
            foreach(var entity in entityDict.Values)
            {
                if(entity is Actor actor)
                {
                    GameObjectManager.Instance.EntityLeave(actor.EntityId);
                }
            }
            entityDict.Clear();
        }

        /// <summary>
        /// 有一个actor进入当前场景
        /// </summary>
        /// <param name="nCharacter"></param>
        public void OnActorEnterScene(NetActor nActor)
        {
            //判断是否已经存在？
                if(entityDict.TryGetValue(nActor.Entity.Id,out var entity))
            {
                if(entity is Actor actor)
                {
                    actor.info = nActor;
                    return;
                }
            }


            //根据不同类型生成不同的actor：玩家角色、怪物、npc
            if(nActor.ActorType == ActorType.Character)
            {
                AddEntity(new Character(nActor));
            }else if(nActor.ActorType == ActorType.Monster)
            {
                AddEntity(new Monster(nActor));
            }
            else
            {
                Debug.Log("[EntityManager.OnActorEnterScene] 角色类型有误");
                return;
            }
            
            Kaiyun.Event.FireOut("CreateActorObject", nActor);//GameObjectManager中实现
        }

        /// <summary>
        /// 有一个itemEntity进入当前场景
        /// </summary>
        /// <param name="netEItem"></param>
        public void OnItemEnterScene(NetEItem netEItem)
        {
            //判断是否已经存在？
            if (entityDict.TryGetValue(netEItem.Entity.Id, out var itemEntity))
            {
                if (itemEntity is ItemEntity item )
                {
                    item.UpdateInfo(netEItem);
                    return;
                }
            }

            AddEntity(new ItemEntity(netEItem));
            Kaiyun.Event.FireOut("CreateItemObject", netEItem);//GameObjectManager中实现
        }



        /// <summary>
        /// entity位置信息同步
        /// </summary>
        /// <param name="nEntitySync"></param>
        public void OnEntitySync(NEntitySync nEntitySync)
        {
            //更新entity的信息，做备份
            Entity entity = entityDict.GetValueOrDefault(nEntitySync.Entity.Id);
            if (entity == null) return;
            entity.EntityData = nEntitySync.Entity;

            //根据更新的信息调整游戏对象的位置
            Kaiyun.Event.FireOut("EntitySync", nEntitySync);    //GameObjectManager中实现
        }

        /// <summary>
        /// 自己的entity位置信息同步
        /// </summary>
        /// <param name="nEntitySync"></param>
        public void OnCtlEntitySync(NEntitySync nEntitySync)
        {
            //更新entity的信息，做备份
            Actor owner = GameApp.character;
            if (owner == null) return;
            owner.EntityData = nEntitySync.Entity;

            //根据更新的信息调整游戏对象的位置
            Kaiyun.Event.FireOut("CtlEntitySync", nEntitySync);    //GameObjectManager中实现
        }

        /// <summary>
        /// itementity信息同步,目前只同步了amount
        /// </summary>
        /// <param name="netEItem"></param>
        public void OnItemEntitySync(NetEItem netEItem)
        {
            Entity entity = entityDict.GetValueOrDefault(netEItem.Entity.Id);
            if (entity == null) return;

            ItemEntity ientity = entity as ItemEntity;
            ientity.Amount = netEItem.ItemInfo.Amount;

            //暂时不需要同步其他的

        }



        /// <summary>
        /// 获取符合条件的entity
        /// </summary>
        /// <param name="match"></param>
        /// <returns></returns>
        public List<T> GetEntityList<T>(Predicate<T> match) where T : Entity
        {
            return entityDict.Values?
                .OfType<T>()                            //根据类型赛选
                .Where(entity => match.Invoke(entity))  //根据条件赛选
                .ToList();
        }

        /// <summary>
        /// 根据entityid获取一个entity，可以选择entity的子类类型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entityId"></param>
        /// <returns></returns>
        public T GetEntity<T>(int entityId) where T : Entity
        {
            return (T)entityDict.GetValueOrDefault(entityId);
        }

        /// <summary>
        /// 根据match模式筛选获取符合条件的entity
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="match"></param>
        /// <returns></returns>
        public List<T> GetEntities<T>(Predicate<T> match)
        {
            return entityDict.Values
                .OfType<T>()
                .Where(e => match.Invoke(e))
                .ToList();
        }

    }
}

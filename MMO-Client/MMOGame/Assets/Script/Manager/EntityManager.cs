using Assets.Script.Entities;
using Proto;
using Summer;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;





namespace GameClient.Entities
{
    /// <summary>
    /// entity对象管理器，只用于管理当前场景的entity
    /// </summary>
    public class EntityManager : Singleton<EntityManager>
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
        public void OnUpdate(float deltatime)
        {
            foreach (var entity in entityDict.Values)
            {
                var actor = entity as Actor;
                actor.skillManager.OnUpdate(deltatime);
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
            Kaiyun.Event.FireOut("CharacterLeave", entityId);   //GameObjectManager中实现
        }

        /// <summary>
        /// 清除全部东西
        /// </summary>
        public void Clear()
        {
            foreach(var entity in entityDict.Values)
            {
                if(entity is Actor actor)
                {
                    GameObjectManager.Instance.CharacterLeave(actor.EntityId);
                }
            }
            entityDict.Clear();
        }

        /// <summary>
        /// 有一个entity进入当前场景
        /// </summary>
        /// <param name="nCharacter"></param>
        public void OnEntityEnterScene(NetActor nActor)
        {
            //根据不同类型生成不同的actor：玩家角色、怪物、npc
            if(nActor.EntityType == EntityType.Character)
            {
                AddEntity(new Character(nActor));
            }else if(nActor.EntityType == EntityType.Monster)
            {
                AddEntity(new Monster(nActor));
            }
            else
            {
                Debug.Log("[EntityManager.OnEntityEnterScene] 角色类型有误");
                return;
            }
            
            Kaiyun.Event.FireOut("CreateActorObject", nActor);//GameObjectManager中实现
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
            entity.State = nEntitySync.State;
            entity.EntityData = nEntitySync.Entity;
            //根据更新的信息调整游戏对象的位置
            Kaiyun.Event.FireOut("EntitySync", nEntitySync);    //GameObjectManager中实现
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

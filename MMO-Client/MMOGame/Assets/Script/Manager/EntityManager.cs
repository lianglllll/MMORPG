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
    public class EntityManager : Singleton<EntityManager>
    {
        //线程安全字典
        private ConcurrentDictionary <int, Entity> entityDict = new ConcurrentDictionary<int, Entity>();

        public EntityManager() { }

        public void AddEntity(Entity entity)
        {
            UnityEngine.Debug.Log("AddEntity:" + entity.EntityId);
            entityDict[entity.EntityId] = entity;
        }

        public void RemoveEntity(int entityId)
        {
            UnityEngine.Debug.Log("RemoveEntity:" + entityId);
            if (entityDict.ContainsKey(entityId))
            {
                entityDict.Remove(entityId,out Entity entity);
            }
            Kaiyun.Event.FireOut("CharacterLeave", entityId);

        }

        public void OnEntityEnterScene(NetActor nCharacter)
        {
            if(nCharacter.EntityType == EntityType.Character)
            {
                AddEntity(new Character(nCharacter));
            }else if(nCharacter.EntityType == EntityType.Monster)
            {
                AddEntity(new Monster(nCharacter));
            }
            else
            {
                Debug.Log("[EntityManager.OnEntityEnterScene] 角色类型有误");
                return;
            }
            
            Kaiyun.Event.FireOut("CreateActorObject", nCharacter);//GameObjectManager中实现
        }
        
        public void OnEntitySync(NEntitySync nEntitySync)
        {
            Entity entity = entityDict.GetValueOrDefault(nEntitySync.Entity.Id);
            UnityEngine.Debug.Log("OnEntitySync:" + entity);
            if (entity == null) return;

            Debug.Log("[OnEntitySync]" + nEntitySync);

            entity.State = nEntitySync.State;
            entity.EntityData = nEntitySync.Entity;
            Kaiyun.Event.FireOut("EntitySync", nEntitySync);    //GameObjectManager中实现
        }

        public T GetEntity<T>(int entityId) where T : Entity
        {
            return (T)entityDict.GetValueOrDefault(entityId);
        }


        //根据match模式筛选获取entity
        public List<T> GetEntities<T>(Predicate<T> match)
        {
            return entityDict.Values
                .OfType<T>()
                .Where(e => match.Invoke(e))
                .ToList();
        }


        /// <summary>
        /// 此方法由unity主线程调用
        /// </summary>
        /// <param name="deltatime"></param>
        public void OnUpdate(float deltatime)
        {
            foreach(var entity in entityDict.Values)
            {
                var actor = entity as Actor;
                actor.skillManager.OnUpdate(deltatime);
            }
        }



    }
}

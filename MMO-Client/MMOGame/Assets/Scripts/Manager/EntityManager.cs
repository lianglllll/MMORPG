using HSFramework.MySingleton;
using HS.Protobuf.Scene;
using HS.Protobuf.SceneEntity;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Serilog;

namespace GameClient.Entities
{
    /// <summary>
    /// entity对象管理器，只用于管理当前场景的entity
    /// </summary>
    public class EntityManager : SingletonNonMono<EntityManager>
    {
        //线程安全字典
        private ConcurrentDictionary <int, Entity> entityDict = new ConcurrentDictionary<int, Entity>();

        public EntityManager() { }
        public virtual void OnUpdate(float deltatime)
        {
            foreach (var entity in entityDict.Values)
            {
                entity.Update(deltatime);
            }
        }
        public void AddEntity(Entity entity)
        {
            entityDict[entity.EntityId] = entity;
        }
        public void RemoveEntity(int entityId)
        {
            entityDict.TryRemove(entityId, out Entity entity);
            if (entity != null) {
                GameObjectManager.Instance.EntityLeave(entity);
            }

            if(entity is ClientItem)
            {
                Kaiyun.Event.FireOut("SceneItemChange");
            }
        }
        public void Clear()
        {
            foreach(var entity in entityDict.Values)
            {
                GameObjectManager.Instance.EntityLeave(entity);
            }
            entityDict.Clear();
        }
        public void OnActorEnterScene(NetActorNode netActorNode)
        {
            // 判断是否已经存在？
            if(entityDict.TryGetValue(netActorNode.EntityId,out _))
            {
                Log.Warning("msg entityId = {0}, 重复加入", netActorNode.EntityId);
                goto End;
            }

            //根据不同类型生成不同的actor：玩家角色、怪物、npc
            Actor actor;
            if(netActorNode.NetActorType == NetActorType.Character)
            {
                actor = new Character(netActorNode);
            }else if(netActorNode.NetActorType == NetActorType.Monster)
            {
                actor = new Monster(netActorNode);
            }
            else if(netActorNode.NetActorType == NetActorType.Npc)
            {
                actor = new Npc(netActorNode);
            }
            else
            {
                Debug.Log("[EntityManager.OnActorEnterScene] 角色类型有误");
                goto End;
            }
            AddEntity(actor);
            GameObjectManager.Instance.CreateActorObject(actor);
        End:
            return;
        }
        public void OnItemEnterScene(NetItemNode netItemNode)
        {
            //判断是否已经存在？
            if (entityDict.TryGetValue(netItemNode.EntityId, out _))
            {
                goto End;
            }
            ClientItem clientItem = new ClientItem(netItemNode);
            AddEntity(clientItem);
            GameObjectManager.Instance.CreateItemObject(clientItem);

            Kaiyun.Event.FireOut("SceneItemChange");
        End:
            return;
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
            // entity.EntityData = nEntitySync.Entity;

            //根据更新的信息调整游戏对象的位置
            GameObjectManager.Instance.EntitySync(nEntitySync);
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
            // owner.EntityData = nEntitySync.Entity;

            //根据更新的信息调整游戏对象的位置
            GameObjectManager.Instance.CtlEntitySync(nEntitySync);
        }
        /// <summary>
        /// itementity信息同步,目前只同步了amount
        /// </summary>
        /// <param name="netEItem"></param>
        public void OnItemEntitySync(NetEItem netEItem)
        {
            Entity entity = entityDict.GetValueOrDefault(netEItem.Entity.Id);
            if (entity == null) return;

            ClientItem ientity = entity as ClientItem;
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

        #region Tools
        public List<T> FindEntitiesWithinRadius<T>(Vector3 position, float radius) where T : Entity
        {
            List<T> result = new List<T>();
            float radiusSquared = radius * radius; // 预计算平方值优化性能

            foreach (var entity in entityDict.Values)
            {
                // 合并类型检查和空值判断
                if (entity is T item && item != null)
                {
                    // 使用转换后的item访问属性
                    Vector3 entityPos = item.Position;

                    // 计算平方距离避免开方运算
                    float distanceSquared = (entityPos - position).sqrMagnitude;

                    if (distanceSquared <= radiusSquared)
                    {
                        result.Add(item); // 添加转换后的对象
                    }
                }
            }

            return result;
        }
        #endregion
    }
}

using GameServer.Core;
using GameServer.Model;
using Summer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Manager
{
    /// <summary>
    /// Entity管理器,(character,怪物，npc,陷阱)
    /// 记录下这些之后，就能很快查找你附近有些什么人，什么怪物
    /// </summary>
    public class EntityManager : Singleton<EntityManager>
    {
        private int index = 1;
        //记录全部entity对象<entityId,entity>
        private Dictionary<int, Entity> allEntitiesDict = new Dictionary<int, Entity>();
        //记录单个场景中的entity列表，<spaceId,entityList>
        private Dictionary<int, List<Entity>> spaceEntitiesDict = new Dictionary<int, List<Entity>>();

        //获取唯一的entityId
        public int NewEntityId
        {
            get
            {
                lock (this)
                {
                    return index++;
                }
            }
        }


        public void AddEntity(int spaceId, Entity entity)
        {
            lock (this)
            {
                //给角色分配一个独一无二的id
                entity.EntityId = NewEntityId;//给entity里面的netObj赋值了
                allEntitiesDict[entity.EntityId] = entity;

                if (!spaceEntitiesDict.ContainsKey(spaceId))
                {
                    spaceEntitiesDict[spaceId] = new List<Entity>();
                }
                spaceEntitiesDict[spaceId].Add(entity);
            }

        }

        public void RemoveEntity(int spaceId, Entity entity)
        {
            lock (this)
            {
                allEntitiesDict.Remove(entity.EntityId);
                spaceEntitiesDict[spaceId].Remove(entity);
            }
        }


        public Entity GetEntity(int entityId)
        {
            return allEntitiesDict.GetValueOrDefault(entityId);
        }

        public void Update()
        {
            foreach (var entity in allEntitiesDict)
            {
                entity.Value.Update();
            }
        }

        //根据条件match查找entity对象(T),T:character||monster||...
        //比如说：生命值为0的，在某个范围内的，
        public List<T> GetEntityList<T>(int spaceId,Predicate<T> match)where T : Entity
        {
            return spaceEntitiesDict[spaceId]
                .OfType<T>()                            //根据类型赛选
                .Where(entity => match.Invoke(entity))  //根据条件赛选
                .ToList();
        }


        //查找坐标点最近的对象
        public T GetNearestEntity<T>(int sapceId,Vector3Int center,int range) where T:Entity
        {
            Predicate<T> match = (e) =>
            {
                return Vector3Int.Distance(center, e.Position) <= range;
            };

            T entity =  GetEntityList<T>(sapceId, match)
                .OrderBy(e => Vector3Int.Distance(center, e.Position))      //找第一个
                .FirstOrDefault();

            return entity;
        }

        //某个entity是否存在
        public bool Exist(int entityId)
        {
            return allEntitiesDict.ContainsKey(entityId);
        }




    }
}


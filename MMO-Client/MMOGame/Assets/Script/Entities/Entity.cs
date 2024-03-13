using Proto;
using Summer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace GameClient.Entities
{
    /// <summary>
    ///客户端entity数据的备份处理
    /// </summary>
    public class Entity
    {

        private Vector3 position;
        private Vector3 direction;
        private NetEntity netObj;                 //网络对象NetEntity
        public EntityState State;

        public int EntityId {
            get { return netObj.Id; }
            set { netObj.Id = value; }
        }
        public Vector3 Position
        {
            get { return position; }
            set {
                position = value;
                netObj.Position = V3.ToVec3(value); 
            }
        }
        public Vector3 Direction
        {
            get { return direction; }
            set {
                direction = value;
                netObj.Direction = V3.ToVec3(value);
            }
        }

        public Proto.NetEntity EntityData
        {
            get {
                return netObj;
            }
            set
            {
                Position = V3.ToVector3(value.Position);
                Direction = V3.ToVector3(value.Direction);
            }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="nEntity"></param>
        public Entity(NetEntity nEntity)
        {
            netObj = new NetEntity();
            netObj.Id = nEntity.Id;//设置id，因为entitydata赋值不会设置id
            this.EntityData = nEntity;
        }

        /// <summary>
        /// 推动entity的更新
        /// </summary>
        /// <param name="deltatime"></param>
        public virtual void OnUpdate(float deltatime)
        {

        }

    }
}


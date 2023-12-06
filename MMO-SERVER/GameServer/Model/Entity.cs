using Proto;
using Summer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameServer.Core;


namespace GameServer.Model
{
    /// <summary>
    /// 在MMO世界地图进行同步的实体
    /// </summary>
    public class Entity
    {

        private Vector3Int position;
        private Vector3Int direction;
        private int speed;
        private NetEntity netObj;                 //网络对象

        private long _lastUpdate;               //最后一次更新位置的时间戳


        public int EntityId { get { return netObj.Id; } set { netObj.Id = value; } }//entityid存放在netObj中,这个id的赋值在entitymanager中完成
        public Vector3Int Position
        {
            get { return position; }
            set {
                position = value;
                netObj.Position = value;  //重载了'='
                _lastUpdate = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            }
        }
        public Vector3Int Direction
        {
            get { return direction; }
            set {
                direction = value;
                netObj.Direction = value;
            }
        }
        public int Speed
        {
            get { return speed; }
            set
            {
                speed = value;
                netObj.Speed = speed;
            }
        }

    

        public Proto.NetEntity EntityData
        {
            get {
                return netObj;
            }
            set
            {
                Position = value.Position;
                Direction = value.Direction;
                Speed = value.Speed;
            }
        }

        //获取位置更新时间间隔
        public float PositionUpdateTimeDistance
        {
            get { return (DateTimeOffset.Now.ToUnixTimeMilliseconds() - _lastUpdate) * 0.001f; }
        }

        public Entity(Vector3Int pos, Vector3Int dir)
        {
            //赋值的同时，设置网络对象
            netObj = new NetEntity();
            Position = pos;
            Direction = dir;
        }

        public virtual void Update()
        {

        }

    }
}


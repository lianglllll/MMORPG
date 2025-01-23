using HS.Protobuf.SceneEntity;
using UnityEngine;

namespace GameClient.Entities
{
    // todo 目前这玩意只有一个功能，就是多态化
    public class Entity
    {
        private int m_entityId;
        private Vector3 m_position;
        private Vector3 m_rotation;
        private Vector3 m_sacle;

        public int EntityId
        {
            get{
                return m_entityId;
            }
        }

        // 正常Unity里面的坐标
        public Vector3 Position
        {
            get
            {
                return m_position;
            }
            set
            {
                m_position = value;
            }
        }
        public Vector3 Rotation
        {
            get
            {
                return m_rotation;
            }
            set
            {
                m_rotation = value;
            }
        }
        public Vector3 Scale
        {
            get
            {
                return m_sacle;
            }
            set
            {
                m_sacle = value;
            }
        }

        public Entity(int entityId, NetTransform transform)
        {
            m_entityId = entityId;
            NetVector3MoveToVector3(transform.Position, m_position);
            NetVector3MoveToVector3(transform.Rotation, m_rotation);
            NetVector3MoveToVector3(transform.Scale, m_sacle);
        }
        public virtual void Update(float deltatime)
        {

        }

        // tools 
        protected void NetVector3MoveToVector3(NetVector3 v1, Vector3 v2)
        {
            v2.x = v1.X * 0.001f;
            v2.y = v1.Y * 0.001f;
            v2.z = v1.Z * 0.001f;
        }
    }
}


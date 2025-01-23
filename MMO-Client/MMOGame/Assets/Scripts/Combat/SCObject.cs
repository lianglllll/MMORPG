using GameClient.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace GameClient
{
    /// <summary>
    /// Server-Client-Object,用于代理（一个人和一个坐标）
    /// </summary>
    public abstract class SCObject
    {
        protected object realObj;
        public SCObject(object realobj)
        {
            this.realObj = realobj;
        }

        public virtual int GetId => 0;
        public virtual object RealObj => realObj;
        public virtual Vector3 GetPosition() => Vector3.zero;
        public virtual Vector3 GetDirection() => Vector3.zero;

    }

    //定义SCEntity类,继承自SCObject
    public class SCEntity : SCObject
    {
        private Entity Obj { get => (Entity)realObj; }
        public SCEntity(Entity realobj) : base(realobj)
        {
        }

        public override int GetId => Obj.EntityId;


        public override Vector3 GetDirection() => Obj.Rotation;


        public override Vector3 GetPosition() => Obj.Position;

    }

    public class SCPosition : SCObject
    {
        public SCPosition(Vector3 realobj) : base(realobj)
        {
        }
        public override Vector3 GetPosition() => (Vector3)realObj;

    }
}

using HS.Protobuf.Scene;
using HSFramework.AI.StateMachine;
using Player.Controller;
using UnityEngine;

namespace Player
{
    public class TransformChangeDate
    {
        public Vector3 pos;
        public Vector3 rot;
        public float verticalSpeed;
        public float horizontalSpeed;
        public float hightSpeed;
        public bool isCorrection;   // 是否已经纠正了
        public long TimeStamp;
    }


    public class RemotePlayerState: StateBase
    {
        protected RemotePlayerController remotePlayer;
        protected StateMachineParameter StateMachineParameter;


        public override void Init(IStateMachineOwner owner)
        {
            base.Init(owner);
            remotePlayer = (RemotePlayerController)owner;
            StateMachineParameter = remotePlayer.StateMachineParameter;
        }

        // 同步位移数据接口
        public virtual void  SyncTransformData(ActorChangeTransformDataResponse resp)
        {
        }
    }
}

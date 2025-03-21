using HS.Protobuf.Common;
using HS.Protobuf.Scene;
using HS.Protobuf.SceneEntity;
using Player.Controller;
using UnityEngine;


namespace HSFramework.Net
{
    public class SyncEntityRecive : SyncEntity
    {
        private bool isStart;
        private RemotePlayerController syncController;
        [SerializeField]
        private Vector3 position;
        [SerializeField]
        private Vector3 direction;

        private void Start()
        {
            isStart = false;
        }

        public void Init(RemotePlayerController syncController, Vector3 pos, Vector3 dir)
        {
            if (isStart) return;
            isStart = true;
            this.syncController = syncController;
            this.position = pos;
            this.direction = dir;
        }


        private void Update()
        {
            //进行插值处理，而不是之间瞬移，看上去更加平滑
            //因为我们是0.2秒同步一次信息所以是5帧
            Move(Vector3.Lerp(transform.position, position, Time.deltaTime * 5f));

            //四元数，插值处理
            Quaternion targetQuaternion = Quaternion.Euler(direction);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetQuaternion, Time.deltaTime * 10f);
        }

        public void SyncEntity(NEntitySync nEntitySync)
        {
            SetValueTo(nEntitySync.Entity.Position,ref position);
            SetValueTo(nEntitySync.Entity.Direction, ref direction);

            if (nEntitySync.Force)
            {
                transform.rotation = Quaternion.Euler(direction);
                transform.position = position;
            }

            if(nEntitySync.State != ActorState.Constant)
            {
                // syncController.ChangeState(nEntitySync.State);
            }
        }
        private void SetValueTo(NetVector3 a, ref Vector3 b)
        {
            b.x = a.X * 0.001f;
            b.y = a.Y * 0.001f;
            b.z = a.Z * 0.001f;
        }
        public void Move(Vector3 target)
        {
            target.y = transform.position.y;
            syncController.CharacterController.Move(target - transform.position);
        }

    }
}


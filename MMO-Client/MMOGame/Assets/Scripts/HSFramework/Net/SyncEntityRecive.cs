using Player.Controller;
using Proto;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace HSFramework.Net
{
    public class SyncEntityRecive : MonoBehaviour
    {
        private bool isStart;
        private SyncController syncController;
        public Vector3 position;
        public Vector3 direction;


        private void Start()
        {
            isStart = false;
        }

        public void Init(SyncController syncController, Vector3 pos, Vector3 dir)
        {
            if (isStart) return;
            isStart = true;
            this.syncController = syncController;
            this.position = pos;
            this.direction = dir;
        }


        public void SyncPosAndRotaion(NetEntity nEntity, bool instantMove = false)
        {
            SetValueTo(nEntity.Position,position);
            //y值不变
            position.y = 0f;
            SetValueTo(nEntity.Direction, direction);

            //是否强制同步
            if (instantMove)
            {
                transform.rotation = Quaternion.Euler(direction);
                transform.position = position;
            }
            else
            {
                syncController.SyncPosAndRotaion(position, direction);
            }

        }
        private void SetValueTo(Vec3 a, Vector3 b)
        {
            b.x = a.X * 0.001f;
            b.y = a.Y * 0.001f;
            b.z = a.Z * 0.001f;
        }

    }

}


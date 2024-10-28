using GameClient.Entities;
using Player;
using Player.Controller;
using Proto;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HSFramework.Net
{
    public class SyncEntitySend : SyncEntity
    {
        //标志
        private bool isStart;
        private bool isStartCoroutine;
        private WaitForSeconds waitForSeconds = new WaitForSeconds(0.1f);//同步时间控制

        //entity信息
        public CtrlController ctrlController;
        private int entityId => ctrlController.Actor.EntityId;
        public Vector3 position;
        public Vector3 direction;

        private void Start()
        {
            isStart = false;
            isStartCoroutine = false;
        }

        private void Update()
        {
            //获取玩家控制的角色的位置和角度，我们自己的角色不受网络控制
            this.position = transform.position;
            this.direction = transform.rotation.eulerAngles;//记录的是欧拉角
        }

        public void Init(CtrlController ctrlController, Vector3 pos, Vector3 dir)
        {
            if (isStart) return;
            isStart = true;

            if (ctrlController == null)
            {
                Destroy(gameObject);
                return;
            }

            this.ctrlController = ctrlController;
            this.position = pos;
            this.direction = dir;

            //开启同步信息功能的协程
            //开启协程，每秒发送10次向服务器上传hero的属性
            StartCoroutine(AutoSendSyncRequest());

        }
        public void Stop()
        {
            if (!isStart) return;
            isStart = false;
            if (isStartCoroutine)
            {
                StopCoroutine(AutoSendSyncRequest());
            }
        }

        /// <summary>
        /// 发送同步信息协程
        /// </summary>
        /// <returns></returns>
        //优化,防止不断在堆中创建新对象
        SpaceEntitySyncRequest req = new SpaceEntitySyncRequest()
        {
            EntitySync = new NEntitySync()
            {
                Entity = new NetEntity()
                {
                    Position = new Vec3(),
                    Direction = new Vec3()
                }
            }
        };
        private IEnumerator AutoSendSyncRequest()
        {
            while (true)
            {
                //只有当主角移动的时候才会发生同步信息
                if (transform.hasChanged)
                {
                    SendSyncRequest();
                }
                yield return waitForSeconds;
            }

        }
        public void SendSyncRequest()
        {
            SetValueTo(transform.position, req.EntitySync.Entity.Position);
            SetValueTo(transform.rotation.eulerAngles, req.EntitySync.Entity.Direction);
            req.EntitySync.Entity.Id = entityId;
            req.EntitySync.State = ctrlController.GetEntityState(ctrlController.CurState);
            NetClient.Send(req);

            //重置
            transform.hasChanged = false;
            req.EntitySync.State = EntityState.NoneState;
        }
        private void SetValueTo(Vector3 a, Vec3 b)
        {
            a = a * 1000;
            b.X = (int)a.x;
            b.Y = (int)a.y;
            b.Z = (int)a.z;
        }

        //同步远端的强制数据
        public void SyncPosAndRotaion(NetEntity nEntity)
        {
            SetValueTo(nEntity.Position, position);
            //y值不变
            position.y = 0f;
            SetValueTo(nEntity.Direction, direction);

            transform.rotation = Quaternion.Euler(direction);
            transform.position = position;

        }
        private void SetValueTo(Vec3 a, Vector3 b)
        {
            b.x = a.X * 0.001f;
            b.y = a.Y * 0.001f;
            b.z = a.Z * 0.001f;
        }
    }
}


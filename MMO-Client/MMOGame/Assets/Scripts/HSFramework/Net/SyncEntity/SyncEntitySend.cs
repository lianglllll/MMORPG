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
        private bool m_isStart;
        private Coroutine m_AutoSendSyncRequestCorountine;
        private WaitForSeconds m_waitForSeconds = new WaitForSeconds(0.1f);//同步时间控制

        private CtrlController m_ctrlController;
        private Vector3 m_position;
        private Vector3 m_direction;
        private int EntityId => m_ctrlController.Actor.EntityId;

        private void Start()
        {
            m_isStart = false;
        }
        private void Update()
        {
            //获取玩家控制的角色的位置和角度，我们自己的角色不受网络控制
            this.m_position = transform.position;
            this.m_direction = transform.rotation.eulerAngles;//记录的是欧拉角
        }
        public void Init(CtrlController ctrlController, Vector3 pos, Vector3 dir)
        {
            if (m_isStart) return;
            m_isStart = true;

            if (ctrlController == null)
            {
                Destroy(gameObject);
                return;
            }

            this.m_ctrlController = ctrlController;
            this.m_position = pos;
            this.m_direction = dir;

            //开启同步信息功能的协程
            //开启协程，每秒发送10次向服务器上传hero的属性
            m_AutoSendSyncRequestCorountine = StartCoroutine(AutoSendSyncRequest());
        }
        private void _ResumeAutoSendSyncRequestCorountine()
        {
            if (m_isStart) return;
            m_isStart = true;
        }
        private void _StopAutoSendSyncRequestCorountine()
        {
            if (!m_isStart) return;
            m_isStart = false;
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
                if (m_isStart && transform.hasChanged)
                {
                    _SendSyncRequest();
                }
                yield return m_waitForSeconds;
            }

        }
        private void _SendSyncRequest()
        {
            SetValueTo(transform.position, req.EntitySync.Entity.Position);
            SetValueTo(transform.rotation.eulerAngles, req.EntitySync.Entity.Direction);
            req.EntitySync.Entity.Id = EntityId;
            req.EntitySync.State = m_ctrlController.CurState;
            NetClient.Send(req);

            //重置
            transform.hasChanged = false;
            req.EntitySync.State = ActorState.Constant;
        }
        private void SetValueTo(Vector3 a, Vec3 b)
        {
            b.X = (int)(a.x * 1000);
            b.Y = (int)(a.y * 1000);
            b.Z = (int)(a.z * 1000);
        }
        public void SendSyncRequest()
        {
            _StopAutoSendSyncRequestCorountine();
            _SendSyncRequest();
            _ResumeAutoSendSyncRequestCorountine();
        }

        //同步远端的强制数据
        public void SyncPosAndRotaion(NetEntity nEntity)
        {
            SetValueTo(nEntity.Position, m_position);
            //y值不变
            m_position.y = 0f;
            SetValueTo(nEntity.Direction, m_direction);

            transform.rotation = Quaternion.Euler(m_direction);
            transform.position = m_position;

        }
        private void SetValueTo(Vec3 a, Vector3 b)
        {
            b.x = a.X * 0.001f;
            b.y = a.Y * 0.001f;
            b.z = a.Z * 0.001f;
        }
    }
}


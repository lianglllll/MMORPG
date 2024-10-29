using Proto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Player
{
    public class SyncState_Move:SyncState
    {
        //移动和旋转需要的
        private Vector3 targetPos;
        private Quaternion targetRotation;
        private bool isMoveTo;
        private Action moveToOverAction;
        public float rotationThreshold = 0.01f; // 阈值
        public float positionThreshold = 0.01f; // 阈值



        public override void Enter()
        {
            isMoveTo = false;
            syncer.PlayAnimation("Move");
        }

        public override void Update()
        {
            if (isMoveTo)
            {
                _MoveToPostion();
            }
        }

        public override void Exit()
        {
        }

        /// <summary>
        /// 移动到某个点
        /// </summary>
        /// <param name="pos"></param>
        public void MoveToPostion(Vector3 pos,Vector3 rotation, Action action = null)
        {
            this.targetPos = pos;
            this.targetRotation = Quaternion.Euler(rotation);
            moveToOverAction = action;
            isMoveTo = true;
        }
        private float posDis;
        private float rotationDis;
        private void _MoveToPostion()
        {
            posDis = Vector3.Distance(targetPos, syncer.transform.position);
            rotationDis = Quaternion.Angle(targetRotation,syncer.transform.rotation);

            if (posDis < positionThreshold && rotationDis < rotationThreshold)
            {
                isMoveTo = false;
                syncer.transform.position = targetPos;
                syncer.transform.rotation = targetRotation;
                moveToOverAction?.Invoke();
                syncer.ChangeState(ActorState.Idle);
            }
            else
            {
                //平滑调制角色位移
                Vector3 dir = targetPos - syncer.transform.position;
                dir.y = 0;
                dir.Normalize();
                Quaternion targetRotation = Quaternion.LookRotation(dir);
                syncer.CharacterController.Move(dir * ShareParameter.curSpeed * Time.deltaTime);

                // 平滑地调整角色旋转
                syncer.transform.rotation = Quaternion.Lerp(syncer.transform.rotation, targetRotation, Time.deltaTime * ShareParameter.rotationSpeed);
            }
        }

    }
}

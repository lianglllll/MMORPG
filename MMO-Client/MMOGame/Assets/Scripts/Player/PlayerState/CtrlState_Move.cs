using Player;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Player
{
    public class CtrlState_Move: CtrlState
    {
        //自动移动需要的
        private Vector3 targetPos;
        private bool isMoveTo;
        private Action moveToOverAction;

        public override void Enter()
        {
            player.PlayAnimation("Move");
            isMoveTo = false;
        }

        public override void Update()
        {
            if (isMoveTo)
            {
                MoveTo();
            }



            //重力
            player.CharacterController.Move(new Vector3(0, ShareParameter.gravity * Time.deltaTime, 0));

        }


        /// <summary>
        /// 移动到某个点
        /// </summary>
        /// <param name="pos"></param>
        public void MoveToPostion(Vector3 pos, Action action = null)
        {
            this.targetPos = pos;
            moveToOverAction = action;
            isMoveTo = true;
        }
        private void MoveTo()
        {
            if (Vector3.Distance(targetPos, player.transform.position) < 0.1f)
            {
                isMoveTo = false;
                player.transform.position = targetPos;
                moveToOverAction?.Invoke();
                player.ChangeState(CommonSmallState.Idle);
            }
            else
            {
                Vector3 dir = targetPos - player.transform.position;
                dir.y = 0;
                dir.Normalize();
                // 插值计算目标旋转方向
                Quaternion targetRotation = Quaternion.LookRotation(dir);
                // 平滑地调整角色旋转
                player.transform.rotation = Quaternion.Lerp(player.transform.rotation, targetRotation, Time.deltaTime * ShareParameter.rotationSpeed);
                player.CharacterController.Move(dir * ShareParameter.curSpeed * Time.deltaTime);
            }
        }


    }
}

using HS.Protobuf.SceneEntity;
using System;
using UnityEngine;

namespace Player.PlayerState
{
    public class LocalPlayerState_Motion: LocalPlayerState
    {
        private enum MotionChildState
        {
            Walk, Run, WalkStop, RunStop
        }
        private bool isTransferToRun;
        private float walk2RunTransition;       // walk到run的过度程度 0-1
        private MotionChildState m_curMotionChildState;
        private MotionChildState MotionState
        {
            get => m_curMotionChildState;
            set
            {
                m_curMotionChildState = value;
                switch (m_curMotionChildState) {
                    case MotionChildState.Walk:
                        break;
                    case MotionChildState.Run:
                        break;
                    case MotionChildState.WalkStop:
                        player.PlayAnimation("WalkStop");
                        break;
                    case MotionChildState.RunStop:
                        player.PlayAnimation("RunStop");
                        break;
                }
            }
        }

        public override void Enter()
        {
            player.PlayAnimation("Motion");
            MotionState = MotionChildState.Walk;
            isTransferToRun = false;
            walk2RunTransition = 0;
            //注册根运动
            player.Model.SetRootMotionAction(OnRootMotion);
        }
        public override void Exit()
        {
            player.Model.Animator.speed = 1;
            player.Model.ClearRootMotionAction();
        }
        public override void Update()
        {
            //检测闪避
            if (GameInputManager.Instance.Shift)
            {
                player.ChangeState(NetActorState.Evade);
                return;
            }

            //检测跳跃
            if (GameInputManager.Instance.Jump)
            {
                player.ChangeState(NetActorState.Jumpup);
                return;
            }

            // 检测蹲下
            if (GameInputManager.Instance.Crouch)
            {
                player.ChangeState(NetActorState.Crouch);
                return;
            }

            // 移动逻辑
            switch (m_curMotionChildState)
            {
                case MotionChildState.Walk:
                    WalkOnUpdate();
                    break;
                case MotionChildState.Run:
                    RunOnUpdate();
                    break;
                case MotionChildState.WalkStop:
                    break;
                case MotionChildState.RunStop:
                    break;
            }

        }

        private void WalkOnUpdate()
        {
            float h = GameInputManager.Instance.Movement.x;
            float v = GameInputManager.Instance.Movement.y;
            if (h == 0 && v == 0)
            {
                player.ChangeState(NetActorState.Idle);
                return;
            }
            else
            {
                if (GameInputManager.Instance.SustainLeftShift || GameInputManager.Instance.Shift)
                {
                    isTransferToRun = true;
                }
                if (isTransferToRun)
                {
                    // 处理walk到run的过渡
                    walk2RunTransition = Mathf.Clamp(walk2RunTransition + Time.deltaTime * player.walk2RunTransitionSpeed, 0, 1);
                    if (walk2RunTransition >= 1)
                    {
                        MotionState = MotionChildState.Run;
                    }
                }

                // 设置动画变量
                player.Model.Animator.SetFloat("MotionSpeed", Mathf.Clamp(walk2RunTransition * player.runSpeed, player.walkSpeed, player.runSpeed));

                //处理旋转问题
                Vector3 input = new Vector3(h, 0, v);
                //获取相机旋转值y
                float y = Camera.main.transform.rotation.eulerAngles.y;
                //让四元数和向量相乘：让这个向量按照这个四元数所表达的角度进行旋转后得到的新向量。
                Vector3 targetDir = Quaternion.Euler(0, y, 0) * input;
                player.transform.rotation = Quaternion.Slerp(player.transform.rotation, Quaternion.LookRotation(targetDir),
                    Time.deltaTime * player.StateMachineParameter.rotationSpeed);
            }
        }
        private void RunOnUpdate()
        {
            float h = GameInputManager.Instance.Movement.x;
            float v = GameInputManager.Instance.Movement.y;
            if (h == 0 && v == 0)
            {
                player.ChangeState(NetActorState.Idle);
                return;
            }
            else
            {
                //处理旋转问题
                Vector3 input = new Vector3(h, 0, v);
                //获取相机旋转值y
                float y = Camera.main.transform.rotation.eulerAngles.y;
                //让四元数和向量相乘：让这个向量按照这个四元数所表达的角度进行旋转后得到的新向量。
                Vector3 targetDir = Quaternion.Euler(0, y, 0) * input;
                player.transform.rotation = Quaternion.Slerp(player.transform.rotation, Quaternion.LookRotation(targetDir),
                    Time.deltaTime * player.StateMachineParameter.rotationSpeed);
            }
        }

        private void OnRootMotion(Vector3 deltaPosition, Quaternion deltaRotation)
        {
            deltaPosition.y = player.gravity * Time.deltaTime;
            player.CharacterController.Move(deltaPosition);
        }

        public void SetRun()
        {
            MotionState = MotionChildState.Run;
            player.Model.Animator.SetFloat("MotionSpeed",player.runSpeed);
        }
    }
}

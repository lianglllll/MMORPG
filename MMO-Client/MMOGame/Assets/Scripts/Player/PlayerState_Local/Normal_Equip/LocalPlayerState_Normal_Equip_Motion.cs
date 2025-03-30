using HS.Protobuf.Scene;
using HS.Protobuf.SceneEntity;
using HSFramework.AI.StateMachine;
using System;
using UnityEngine;

namespace Player.PlayerState
{
    public class LocalPlayerState_Normal_Equip_Motion : LocalPlayerState
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

        private float timer = 0f;
        private const float SEND_INTERVAL = 0.2f; // 每100ms发送一次
        private ActorChangeTransformDataRequest actdReq;
        public override void Init(IStateMachineOwner owner)
        {
            base.Init(owner);
            actdReq = new ActorChangeTransformDataRequest();
            actdReq.PayLoad = new ActorChangeTransformDataPayLoad();
            actdReq.SessionId = NetManager.Instance.sessionId;
            actdReq.EntityId = player.Actor.EntityId;
            actdReq.OriginalTransform = new NetTransform();
            actdReq.OriginalTransform.Position = new();
            actdReq.OriginalTransform.Rotation = new();
            actdReq.OriginalTransform.Scale = new();
        }
        public override void Enter()
        {
            player.PlayAnimation("Normal_Equip_Motion");
            MotionState = MotionChildState.Walk;
            isTransferToRun = false;
            walk2RunTransition = 0;
            //注册根运动
            player.Model.SetRootMotionAction(OnRootMotion);

            // 发送状态改变请求
            player.NetworkActor.SendActorChangeStateRequest();

            // 定时发送位置信息
            timer = 0f;
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
            if (GameInputManager.Instance.Space)
            {
                // player.ChangeState(NetActorState.Jumpup);
                return;
            }

            // 检测蹲下
            if (GameInputManager.Instance.Crouch)
            {
                // player.ChangeState(NetActorState.Crouch);
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

            SendActorChangeTransformDataRequest();
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
                // 其实可以这样的,这个api是可以有插值的
                // player.Model.Animator.SetFloat("Normal_Vertical_Speed", 0, 0, Time.deltaTime);

                // 设置动画变量
                player.Model.Animator.SetFloat("Normal_Vertical_Speed", Mathf.Clamp(walk2RunTransition * player.runSpeed, player.walkSpeed, player.runSpeed));

                //处理旋转问题
                Vector3 input = new Vector3(h, 0, v);
                //获取相机旋转值y
                float y = Camera.main.transform.rotation.eulerAngles.y;
                //让四元数和向量相乘：让这个向量按照这个四元数所表达的角度进行旋转后得到的新向量。
                Vector3 targetDir = Quaternion.Euler(0, y, 0) * input;
                player.transform.rotation = Quaternion.Slerp(player.transform.rotation, Quaternion.LookRotation(targetDir),
                    Time.deltaTime * player.rotateSpeed);
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
                    Time.deltaTime * player.rotateSpeed);
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
            player.Model.Animator.SetFloat("Normal_Vertical_Speed", player.runSpeed);
        }
        private bool SendActorChangeTransformDataRequest()
        {
            timer += Time.deltaTime;
            if (timer >= SEND_INTERVAL)
            {
                timer = 0.0f;
                player.NetworkActor.V3ToNV3(player.gameObject.transform.position, actdReq.OriginalTransform.Position);
                player.NetworkActor.V3ToNV3(player.gameObject.transform.eulerAngles, actdReq.OriginalTransform.Rotation);
                player.NetworkActor.V3ToNV3(player.gameObject.transform.localScale, actdReq.OriginalTransform.Scale);
                actdReq.PayLoad.VerticalSpeed = (int)(player.Model.Animator.GetFloat("Normal_Vertical_Speed") * 1000);
                actdReq.Timestamp = NetworkTime.Instance.GetCurNetWorkTime();
                NetManager.Instance.Send(actdReq);
            }
            return true;
        }
    }
}

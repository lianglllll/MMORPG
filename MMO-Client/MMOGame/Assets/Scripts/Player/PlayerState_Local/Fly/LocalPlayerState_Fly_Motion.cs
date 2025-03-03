using HS.Protobuf.Scene;
using HS.Protobuf.SceneEntity;
using HSFramework.AI.StateMachine;
using UnityEngine;

namespace Player.PlayerState
{
    public class LocalPlayerState_Fly_Motion: LocalPlayerState
    {
        private enum MotionChildState
        {
            Walk, Run, WalkStop, RunStop
        }
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
            player.PlayAnimation("Fly_Motion");
            MotionState = MotionChildState.Walk;

            // 发送状态改变请求
            player.NetworkActor.SendActorChangeStateRequest();

            // 定时发送位置信息
            timer = 0f;
        }
        public override void Update()
        {
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

            float changeHightSpeed = 0f;
            Vector3 deltaPos = Vector3.zero;
            if (GameInputManager.Instance.SustainQ)
            {
                changeHightSpeed = player.flyChangeHightSpeed;
            }
            else if (GameInputManager.Instance.SustainE)
            {
                changeHightSpeed = -player.flyChangeHightSpeed;
            }
            deltaPos.y = changeHightSpeed * Time.deltaTime;
            player.CharacterController.Move(deltaPos);

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
                if (GameInputManager.Instance.SustainLeftShift && v > 0)
                {
                    MotionState = MotionChildState.Run;
                    return;
                }

                // 设置动画参数
                player.Model.Animator.SetFloat("Fly_Horizontal_Speed", h, 0.2f, Time.deltaTime);
                player.Model.Animator.SetFloat("Fly_Vertical_Speed", v, 0.2f, Time.deltaTime);

                Vector3 moveDirection = (player.transform.right * h + player.transform.forward * v).normalized;
                if (v > 0)
                {
                    // 方向计算（优化相机参考系处理）
                    Vector3 inputDir = new Vector3(h, 0, v).normalized;
                    Quaternion cameraYaw = Quaternion.Euler(0, Camera.main.transform.eulerAngles.y, 0);
                    moveDirection = cameraYaw * inputDir;
                    // 平滑旋转（增加方向锁定保护）
                    if (moveDirection != Vector3.zero)
                    {
                        Quaternion targetRot = Quaternion.LookRotation(moveDirection);
                        player.transform.rotation = Quaternion.Slerp(
                            player.transform.rotation,
                            targetRot,
                            Time.deltaTime * player.rotateSpeed
                        );
                    }
                }
                else
                {
                    // 保持角色的朝向始终和摄像机一致
                    float targetYaw = Camera.main.transform.eulerAngles.y;
                    Quaternion targetRot = Quaternion.Euler(0, targetYaw, 0);

                    player.transform.rotation = Quaternion.Slerp(
                        player.transform.rotation,
                        targetRot,
                        player.rotateSpeed * Time.deltaTime
                    );
                }

                // 处理位置
                float currentSpeed = player.flyWalkSpeed;
                Vector3 moveVector = moveDirection * currentSpeed * Time.deltaTime;
                player.CharacterController.Move(moveVector);
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
                // 设置动画参数（优化插值方式）
                player.Model.Animator.SetFloat("Fly_Horizontal_Speed", 0, 0.1f, Time.deltaTime);
                player.Model.Animator.SetFloat("Fly_Vertical_Speed", 2, 0.1f, Time.deltaTime);

                // 方向计算（优化相机参考系处理）
                Vector3 inputDir = new Vector3(h, 0, v).normalized;
                Quaternion cameraYaw = Quaternion.Euler(0, Camera.main.transform.eulerAngles.y, 0);
                Vector3 targetDir = cameraYaw * inputDir;
                // 平滑旋转（增加方向锁定保护）
                if (targetDir != Vector3.zero)
                {
                    Quaternion targetRot = Quaternion.LookRotation(targetDir);
                    player.transform.rotation = Quaternion.Slerp(
                        player.transform.rotation,
                        targetRot,
                        Time.deltaTime * player.rotateSpeed
                    );
                }

                // 处理位置
                float currentSpeed = player.flyRunSpeed;
                Vector3 moveVector = targetDir * currentSpeed * Time.deltaTime;
                player.CharacterController.Move(moveVector);
            }
        }

        private bool SendActorChangeTransformDataRequest()
        {
            timer += Time.deltaTime;
            if (timer >= SEND_INTERVAL)
            {
                timer = 0.0f;
                player.NetworkActor.V3ToNV3(player.gameObject.transform.position, actdReq.OriginalTransform.Position);
                player.NetworkActor.V3ToNV3(player.gameObject.transform.eulerAngles, actdReq.OriginalTransform.Rotation);
                // player.NetworkActor.V3ToNV3(player.gameObject.transform.localScale, actdReq.OriginalTransform.Scale);
                actdReq.PayLoad.HorizontalSpeed = (int)(player.Model.Animator.GetFloat("Fly_Horizontal_Speed") * 1000);
                actdReq.PayLoad.VerticalSpeed = (int)(player.Model.Animator.GetFloat("Fly_Vertical_Speed") * 1000);
                actdReq.Timestamp = NetworkTime.Instance.GetCurNetWorkTime();
                NetManager.Instance.Send(actdReq);
            }
            return true;
        }

    }
}

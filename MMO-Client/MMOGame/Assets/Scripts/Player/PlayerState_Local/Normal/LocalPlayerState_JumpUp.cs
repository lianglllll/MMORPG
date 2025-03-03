using HS.Protobuf.Scene;
using HS.Protobuf.SceneEntity;
using HSFramework.AI.StateMachine;
using UnityEngine;

namespace Player.PlayerState
{
    public class LocalPlayerState_JumpUp: LocalPlayerState
    {
        private float m_verticalVelocity;

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
            m_verticalVelocity = player.jumpVelocity;
            player.Model.Animator.SetFloat("Normal_Vertical_Speed", m_verticalVelocity);
            player.PlayAnimation("Jump");
            player.Model.SetRootMotionAction(OnRootMotion);

            // 发送状态改变请求
            StateMachineParameter.jumpVelocity = m_verticalVelocity;
            player.NetworkActor.SendActorChangeStateRequest();

            // 定时发送位置信息
            timer = 0f;
        }
        public override void Update()
        {
            if (m_verticalVelocity <= 0)
            {
                player.ChangeState(NetActorState.Falling);
                return;
            }

            // 重力影响
            m_verticalVelocity += player.gravity * Time.deltaTime;
            player.Model.Animator.SetFloat("Normal_Vertical_Speed", m_verticalVelocity);

            SendActorChangeTransformDataRequest();
        }
        public override void Exit()
        {
            player.Model.ClearRootMotionAction();
        }
        private void OnRootMotion(Vector3 deltaPosition, Quaternion deltaRotation)
        {
            deltaPosition.y += m_verticalVelocity * Time.deltaTime;
            Vector3 forwardOffset = Time.deltaTime * player.moveSpeedForJump * player.transform.forward;
            player.CharacterController.Move(deltaPosition + forwardOffset);
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
                // actdReq.PayLoad.VerticalSpeed = (int)(player.Model.Animator.GetFloat("Normal_Vertical_Speed") * 1000);
                actdReq.Timestamp = NetworkTime.Instance.GetCurNetWorkTime();
                NetManager.Instance.Send(actdReq);
            }
            return true;
        }


    }
}

using HS.Protobuf.SceneEntity;
using UnityEngine;

namespace Player.PlayerState
{
    public class LocalPlayerState_JumpUp: LocalPlayerState
    {
        private float m_verticalVelocity;

        public override void Enter()
        {
            m_verticalVelocity = player.jumpVelocity;
            player.Model.Animator.SetFloat("Normal_Vertical_Speed", m_verticalVelocity);
            player.PlayAnimation("Jump");
            player.Model.SetRootMotionAction(OnRootMotion);
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

    }
}

using HS.Protobuf.SceneEntity;
using UnityEngine;

namespace Player.PlayerState
{
    public class LocalPlayerState_Falling: LocalPlayerState
    {
        private float m_verticalVelocity;
        private LayerMask groundLayerMask = LayerMask.GetMask("Env");
        private float offset = 1f;
        public override void Enter()
        {
            m_verticalVelocity = 0;
            player.Model.Animator.SetFloat("VerticalSpeed", m_verticalVelocity);
            player.PlayAnimation("Jump");
            player.Model.SetRootMotionAction(OnRootMotion);
        }
        public override void Exit()
        {
            player.Model.ClearRootMotionAction();
        }
        public override void Update()
        {
            // 地面检测
            //if (Physics.Raycast(player.transform.position + new Vector3(0, 0.5f, 0), player.transform.up * (-1), offset, groundLayerMask))
            //{
            //    player.ChangeState(NetActorState.Idle);
            //    return;
            //}
            if(player.CharacterController.isGrounded || Physics.Raycast(player.transform.position + new Vector3(0, 0.5f, 0),
                player.transform.up * (-1), offset, groundLayerMask))
            {
                player.ChangeState(NetActorState.Idle);
                return;
            }


            // 重力及控制转向
            m_verticalVelocity += player.gravity * Time.deltaTime;
            player.Model.Animator.SetFloat("VerticalSpeed", m_verticalVelocity);
            AirControl();
        }

        private void AirControl()
        {
            float h = GameInputManager.Instance.Movement.x;
            float v = GameInputManager.Instance.Movement.y;

            //重力
            Vector3 motion = new Vector3(0, m_verticalVelocity * Time.deltaTime, 0);

            if (h != 0 || v != 0)
            {
                //处理空中位移
                Vector3 input = new Vector3(h, 0, v);
                Vector3 dir = Camera.main.transform.TransformDirection(input);
                motion.x = dir.x * player.moveSpeedForAirDown * Time.deltaTime;
                motion.z = dir.z * player.moveSpeedForAirDown * Time.deltaTime;

                //处理空中旋转
                //获取相机旋转值y
                float y = Camera.main.transform.rotation.eulerAngles.y;
                //让四元数和向量相乘：让这个向量按照这个四元数所表达的角度进行旋转后得到的新向量。
                Vector3 targetDir = Quaternion.Euler(0, y, 0) * input;
                player.transform.rotation = Quaternion.Slerp(player.transform.rotation, Quaternion.LookRotation(targetDir),
                    Time.deltaTime * player.rotateSpeed);
            }

            player.CharacterController.Move(motion);
        }
        private void OnRootMotion(Vector3 deltaPosition, Quaternion deltaRotation)
        {
            Vector3 forwardOffset = Time.deltaTime * player.moveSpeedForJump * player.transform.forward;
            player.CharacterController.Move(deltaPosition + forwardOffset);
        }
    }
}

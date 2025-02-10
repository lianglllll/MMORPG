using HS.Protobuf.SceneEntity;
using UnityEngine;

namespace Player.PlayerState
{
    public class LocalPlayerState_Crouch: LocalPlayerState
    {
        public override void Enter()
        {
            player.PlayAnimation("Crouch_Idle");
            //注册根运动
            player.Model.SetRootMotionAction(OnRootMotion);
        }
        public override void Exit()
        {
            player.Model.ClearRootMotionAction();
        }

        public override void Update()
        {
            if(GameInputManager.Instance.Crouch)
            {
                player.ChangeState(NetActorState.Idle);
                return;
            }

            // 玩家移动
            float h = GameInputManager.Instance.Movement.x;
            float v = GameInputManager.Instance.Movement.y;
            if (h != 0 || v != 0)
            {
                //处理旋转问题
                Vector3 input = new Vector3(h, 0, v);
                //获取相机旋转值y
                float y = Camera.main.transform.rotation.eulerAngles.y;
                //让四元数和向量相乘：让这个向量按照这个四元数所表达的角度进行旋转后得到的新向量。
                Vector3 targetDir = Quaternion.Euler(0, y, 0) * input;
                player.transform.rotation = Quaternion.Slerp(player.transform.rotation, Quaternion.LookRotation(targetDir),
                    Time.deltaTime * player.StateMachineParameter.rotationSpeed);

                player.PlayAnimation("Crouch_Walk");
            }
            else
            {
                player.PlayAnimation("Crouch_Idle");
            }
        }
        private void OnRootMotion(Vector3 deltaPosition, Quaternion deltaRotation)
        {
            deltaPosition.y = player.gravity * Time.deltaTime;
            player.CharacterController.Move(deltaPosition);
        }

    }
}

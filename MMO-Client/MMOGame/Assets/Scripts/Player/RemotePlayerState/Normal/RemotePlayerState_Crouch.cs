using HS.Protobuf.SceneEntity;
using UnityEngine;

namespace Player.PlayerState
{
    public class RemotePlayerState_Crouch: RemotePlayerState
    {
        public override void Enter()
        {
            remotePlayer.PlayAnimation("Crouch_Idle");
            //注册根运动
            remotePlayer.Model.SetRootMotionAction(OnRootMotion);
        }
        public override void Exit()
        {
            remotePlayer.Model.ClearRootMotionAction();
        }

        public override void Update()
        {
            if(GameInputManager.Instance.Crouch)
            {
                remotePlayer.ChangeState(NetActorState.Idle);
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
                remotePlayer.transform.rotation = Quaternion.Slerp(remotePlayer.transform.rotation, Quaternion.LookRotation(targetDir),
                    Time.deltaTime * remotePlayer.rotateSpeed);

                remotePlayer.PlayAnimation("Crouch_Walk");
            }
            else
            {
                remotePlayer.PlayAnimation("Crouch_Idle");
            }
        }
        private void OnRootMotion(Vector3 deltaPosition, Quaternion deltaRotation)
        {
            deltaPosition.y = remotePlayer.gravity * Time.deltaTime;
            remotePlayer.CharacterController.Move(deltaPosition);
        }

    }
}

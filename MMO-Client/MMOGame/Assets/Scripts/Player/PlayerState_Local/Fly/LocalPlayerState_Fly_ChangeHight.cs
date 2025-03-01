using HS.Protobuf.SceneEntity;
using UnityEngine;

namespace Player.PlayerState
{
    public class LocalPlayerState_Fly_ChangeHight : LocalPlayerState
    {
        private float changeHightSpeed = 0;
        private Vector3 deltaPos = new Vector3(0, 0, 0);

        public override void Enter()
        {
            player.PlayAnimation("Fly_ChangeHight");

            // 发送状态改变请求
            player.NetworkActor.SendActorChangeStateRequest();
        } 

        public override void Update()
        {
            if (GameInputManager.Instance.SustainQ)
            {
                player.Model.Animator.SetFloat("Fly_Hight_Speed", 1, 0.1f, Time.deltaTime);
                changeHightSpeed = player.flyChangeHightSpeed;
            }
            else if (GameInputManager.Instance.SustainE)
            {
                player.Model.Animator.SetFloat("Fly_Hight_Speed", -1, 0.1f, Time.deltaTime);
                changeHightSpeed = -player.flyChangeHightSpeed;
            }
            else
            {
                player.ChangeState(NetActorState.Idle);
                goto End;
            }
            deltaPos.y = changeHightSpeed * Time.deltaTime;
            player.CharacterController.Move(deltaPos);
        End:
            return;
        }
    }
}

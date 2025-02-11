using HS.Protobuf.SceneEntity;
using UnityEngine;

namespace Player.PlayerState
{
    public class LocalPlayerState_Idle: LocalPlayerState
    {
        public override void Enter()
        {
            player.PlayAnimation("Idle");

            // 发送状态改变请求
            player.NetworkActor.SendActorChangeStateRequest();
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

            // 玩家移动
            float h = GameInputManager.Instance.Movement.x;
            float v = GameInputManager.Instance.Movement.y;
            if (h != 0 || v != 0)
            {
                player.ChangeState(NetActorState.Motion);
                return;
            }

            //重力
            player.CharacterController.Move(new Vector3(0, player.gravity * Time.deltaTime, 0));
        }
    }
}

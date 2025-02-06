using HS.Protobuf.SceneEntity;
using UnityEngine;

namespace Player
{
    public class CtrlState_Idle: CtrlState
    {

        public override void Enter()
        {
            player.PlayAnimation("Idle1");
        } 

        public override void Update()
        {
            //检测玩家移动
            float h = GameInputManager.Instance.Movement.x;
            float v = GameInputManager.Instance.Movement.y;
            if (h != 0 || v != 0)
            {
                player.ChangeState(ActorState.Move);
                return;
            }

            //重力
            player.CharacterController.Move(new Vector3(0, ShareParameter.gravity * Time.deltaTime, 0));

        }

    }
}

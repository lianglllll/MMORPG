using HS.Protobuf.SceneEntity;

namespace Player.PlayerState
{
    public class LocalPlayerState_Fly_Idle: LocalPlayerState
    {
        public override void Enter()
        {
            player.PlayAnimation("Fly_Idle");

            // 发送状态改变请求
            player.NetworkActor.SendActorChangeStateRequest();
        } 

        public override void Update()
        {
            if (GameInputManager.Instance.SustainQ || GameInputManager.Instance.SustainE)
            {
                player.ChangeState(NetActorState.Changehight);
                goto End;
            }

            // 玩家移动
            float h = GameInputManager.Instance.Movement.x;
            float v = GameInputManager.Instance.Movement.y;
            if (h != 0 || v != 0)
            {
                player.ChangeState(NetActorState.Motion);
                goto End;
            }

        End:
            return;
        }
    }
}

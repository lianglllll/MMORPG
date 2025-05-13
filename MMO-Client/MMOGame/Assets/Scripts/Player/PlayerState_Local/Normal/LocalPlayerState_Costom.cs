namespace Player.PlayerState
{
    public class LocalPlayerState_Custom: LocalPlayerState
    {
        public override void Enter()
        {
            player.PlayAnimation("Custom");

            // 同时告知服务器，msg需要携带constom的动画名称

        }
    }
}

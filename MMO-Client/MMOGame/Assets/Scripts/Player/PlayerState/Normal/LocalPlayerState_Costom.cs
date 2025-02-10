namespace Player.PlayerState
{
    public class LocalPlayerState_Custom: LocalPlayerState
    {
        public override void Enter()
        {
            player.PlayAnimation("Custom");
        }
    }
}

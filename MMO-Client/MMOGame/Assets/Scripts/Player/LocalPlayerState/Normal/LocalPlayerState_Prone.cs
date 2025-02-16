namespace Player.PlayerState
{
    public class LocalPlayerState_Prone: LocalPlayerState
    {
        public override void Enter()
        {
            player.PlayAnimation("Prone");
        }
    }
}

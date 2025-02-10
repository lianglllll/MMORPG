namespace Player.PlayerState
{
    public class LocalPlayerState_Knock: LocalPlayerState
    {
        public override void Enter()
        {
            player.PlayAnimation("Knock");
        }
    }
}

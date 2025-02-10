namespace Player.PlayerState
{
    public class LocalPlayerState_Dizzy: LocalPlayerState
    {
        public override void Enter()
        {
            player.PlayAnimation("Dizzy");
        }
    }
}

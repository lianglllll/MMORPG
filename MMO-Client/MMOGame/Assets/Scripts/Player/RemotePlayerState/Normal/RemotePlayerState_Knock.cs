namespace Player.PlayerState
{
    public class RemotePlayerState_Knock: RemotePlayerState
    {
        public override void Enter()
        {
            remotePlayer.PlayAnimation("Knock");
        }
    }
}

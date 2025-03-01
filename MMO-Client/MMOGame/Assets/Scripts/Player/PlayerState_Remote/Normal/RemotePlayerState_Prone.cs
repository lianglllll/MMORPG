namespace Player.PlayerState
{
    public class RemotePlayerState_Prone: RemotePlayerState
    {
        public override void Enter()
        {
            remotePlayer.PlayAnimation("Prone");
        }
    }
}

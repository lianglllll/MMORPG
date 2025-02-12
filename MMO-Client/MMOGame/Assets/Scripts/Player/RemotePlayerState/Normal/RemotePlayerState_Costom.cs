namespace Player.PlayerState
{
    public class RemotePlayerState_Custom: RemotePlayerState
    {
        public override void Enter()
        {
            remotePlayer.PlayAnimation("Custom");
        }
    }
}

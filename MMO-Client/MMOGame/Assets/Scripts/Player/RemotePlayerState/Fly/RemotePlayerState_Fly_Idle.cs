namespace Player.PlayerState
{
    public class RemotePlayerState_Fly_Idle: RemotePlayerState
    {
        public override void Enter()
        {
            remotePlayer.PlayAnimation("Fly_Idle");
        } 
    }
}

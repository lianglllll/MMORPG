
namespace Player.PlayerState
{
    public class RemotePlayerState_Fly_Motion: RemotePlayerState
    {
        public override void Enter()
        {
            remotePlayer.PlayAnimation("Fly_Motion");
        }
    }
}

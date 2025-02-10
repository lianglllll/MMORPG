
namespace Player.PlayerState
{
    public class LocalPlayerState_Death: LocalPlayerState
    {
        public override void Enter()
        {
            player.PlayAnimation("Death");
            player.Actor.OnDeath();
        }

        public override void Exit()
        {
        }

    }
}

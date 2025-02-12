using HSFramework.AI.StateMachine;
using Player.Controller;

namespace Player
{
    public class RemotePlayerState: StateBase
    {
        protected RemotePlayerController remotePlayer;
        protected StateMachineParameter ShareParameter => remotePlayer.StateMachineParameter;

        public override void Init(IStateMachineOwner owner)
        {
            base.Init(owner);
            remotePlayer = (RemotePlayerController)owner;
        }


    }
}

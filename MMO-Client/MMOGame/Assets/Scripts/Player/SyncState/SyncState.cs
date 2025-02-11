using HSFramework.AI.StateMachine;
using Player.Controller;

namespace Player
{
    public class SyncState: StateBase
    {
        protected SyncController syncer;
        protected StateMachineParameter ShareParameter => syncer.StateMachineParameter;

        public override void Init(IStateMachineOwner owner)
        {
            base.Init(owner);
            syncer = (SyncController)owner;
        }


    }
}

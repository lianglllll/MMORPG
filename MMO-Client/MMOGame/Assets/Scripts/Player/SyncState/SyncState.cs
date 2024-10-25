using HSFramework.AI.StateMachine;
using Player.Controller;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Player.SyncState
{
    public class SyncState: StateBase
    {
        protected SyncController syncer;
        protected StateMachineParameter ShareParameter => syncer.stateMachine.ShareParameter;

        public override void Init(IStateMachineOwner owner)
        {
            base.Init(owner);
            syncer = (SyncController)owner;
        }


    }
}

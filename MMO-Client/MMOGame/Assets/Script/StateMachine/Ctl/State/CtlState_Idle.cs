using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Script.StateMachine.Ctl.State
{
    public class CtlState_Idle : CtlState
    {
        public CtlState_Idle(CtlStateMachine stateMachine)
        {
            Initialize(stateMachine);
        }

        public override void Enter()
        {
            animator.Play("Idle");
        }

    }
}

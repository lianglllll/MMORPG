using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Script.StateMachine.Ctl.State
{
    public class CtlState_Motion : CtlState
    {
        public CtlState_Motion(CtlStateMachine stateMachine)
        {
            Initialize(stateMachine);
        }

        public override void Enter()
        {
            //animator.SetFloat("Speed", stateMachine.parameter.owner.Speed * 0.001f);
            animator.Play("Motion");
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();
            //animator.SetFloat("Speed", stateMachine.parameter.owner.Speed * 0.001f);
        }

    }
}


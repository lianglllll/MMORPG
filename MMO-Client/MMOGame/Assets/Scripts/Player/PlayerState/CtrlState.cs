using HSFramework.AI.StateMachine;
using Player.Controller;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Player
{
    public enum CommonLargeState
    {
        Stand,Equip,SwordFlight
    }

    public enum CommonSmallState
    {
        AirDown, Death, Defense, Dizzy,Evade,Hurt,Idle, JumpUp, Move,Skill,None
    }

    public class CtrlState : StateBase
    {
        protected CtrlController player;
        protected StateMachineParameter ShareParameter => player.stateMachine.ShareParameter;

        public override void Init(IStateMachineOwner owner)
        {
            base.Init(owner);
            player = (CtrlController)owner;
        }

    }
}

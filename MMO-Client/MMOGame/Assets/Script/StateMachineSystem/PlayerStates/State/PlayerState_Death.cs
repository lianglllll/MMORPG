using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState_Death : PlayerState
{
    public PlayerState_Death(PlayerStateMachine stateMachine)
    {
        Initialize(stateMachine);
    }

    public override void Enter()
    {
        animator.Play("Death");
        if(GameApp.entityId == stateMachine.parameter.owner.EntityId)
        {
            stateMachine.parameter.owner.renderObj.GetComponent<PlayerMovementController>().enabled = false;
        }   
    }

    public override void LogicUpdate()
    {
        if (!stateMachine.parameter.owner.IsDeath)
        {
            stateMachine.SwitchState(ActorState.Idle);
        }
    }


    public override void Exit()
    {
        if (GameApp.entityId == stateMachine.parameter.owner.EntityId)
        {
            stateMachine.parameter.owner.renderObj.GetComponent<PlayerMovementController>().enabled = true;
        }
    }
}

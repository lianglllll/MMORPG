using Proto;
using Serilog;
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
        stateMachine.parameter.owner.OnDeath();


    }

    public override void LogicUpdate()
    {
        //监听
        if (stateMachine.parameter.owner.IsDeath != true)
        {
            stateMachine.SwitchState(EntityState.Idle, true);
        }

    }


    public override void Exit()
    {

    }
}

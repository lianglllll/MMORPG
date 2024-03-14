using Proto;
using Serilog;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState_Death : PlayerState
{
    float transitionDuration = 0.3f; // 过渡时间（秒）

    public PlayerState_Death(PlayerStateMachine stateMachine)
    {
        Initialize(stateMachine);
    }

    public override void Enter()
    {
        animator.CrossFade("Death", transitionDuration);
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

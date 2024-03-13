using Proto;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 眩晕状态,
/// </summary>
public class PlayerState_Dizzy : PlayerState
{
    public PlayerState_Dizzy(PlayerStateMachine stateMachine)
    {
        Initialize(stateMachine);
    }

    public override void Enter()
    {
        animator.Play("Dizzy");
    }

    public override void LogicUpdate()
    {
        //眩晕期间被打死
        if (stateMachine.parameter.owner.IsDeath){
            stateMachine.SwitchState(EntityState.Death, true);
        }

        //眩晕结束退出退出眩晕状态
        if (stateMachine.parameter.owner.entityState != EntityState.Dizzy)
        {
            stateMachine.SwitchState(EntityState.Idle, true);
        }
    }

    public override void Exit()
    {


    }
}

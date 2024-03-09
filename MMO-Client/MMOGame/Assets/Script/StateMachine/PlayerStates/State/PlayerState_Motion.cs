using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState_Motion : PlayerState
{
    public PlayerState_Motion(PlayerStateMachine stateMachine)
    {
        Initialize(stateMachine);
    }

    public override void Enter()
    {
        animator.SetFloat("Speed", stateMachine.parameter.owner.Speed*0.001f);
        animator.Play("Motion");
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
        animator.SetFloat("Speed", stateMachine.parameter.owner.Speed * 0.001f);


    }

}

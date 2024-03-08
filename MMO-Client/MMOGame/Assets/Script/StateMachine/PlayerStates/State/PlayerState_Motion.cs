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
        animator.Play("Motion");
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();


    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState_Walk : PlayerState
{
    public PlayerState_Walk(PlayerStateMachine stateMachine)
    {
        Initialize(stateMachine);
    }

    public override void Enter()
    {
        animator.Play("Walk");
    }


}

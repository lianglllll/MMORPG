using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState_StrikeAndBack : PlayerState
{

    public PlayerState_StrikeAndBack(PlayerStateMachine stateMachine)
    {
        Initialize(stateMachine);
    }

    public override void Enter()
    {
        animator.Play("StrikeAndBack");
    }


}

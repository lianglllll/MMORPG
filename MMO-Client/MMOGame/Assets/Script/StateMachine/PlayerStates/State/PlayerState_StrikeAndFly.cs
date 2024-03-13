using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState_StrikeAndFly : PlayerState
{

    public PlayerState_StrikeAndFly(PlayerStateMachine stateMachine)
    {
        Initialize(stateMachine);
    }

    public override void Enter()
    {
        animator.Play("StrikeAndFly");
    }


}

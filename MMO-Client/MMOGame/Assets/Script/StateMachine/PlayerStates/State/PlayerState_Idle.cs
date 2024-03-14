using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState_Idle : PlayerState
{
    float transitionDuration = 0.3f; // 过渡时间（秒）

    public PlayerState_Idle(PlayerStateMachine stateMachine)
    {
        Initialize(stateMachine);
    }

    public override void Enter()
    {
        animator.CrossFade("Idle", transitionDuration);
    }


}

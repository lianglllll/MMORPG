using GameClient.Combat;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState_SkillIntonate : PlayerState
{
    public PlayerState_SkillIntonate(PlayerStateMachine stateMachine)
    {
        Initialize(stateMachine);
    }

    public override void Enter()
    {
        animator.Play(stateMachine.parameter.skill.Define.IntonateAnimName);
    }


    public override void Exit()
    {

    }
}

using GameClient.Combat;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState_SkillActive : PlayerState
{
    public PlayerState_SkillActive(PlayerStateMachine stateMachine)
    {
        Initialize(stateMachine);
    }

    public override void Enter()
    {
        animator.Play(stateMachine.parameter.skill.Define.ActiveAnimName);
    }


    public override void LogicUpdate()
    {
        if (stateMachine.parameter.skill.Stage != SkillStage.Active)
        {
            stateMachine.SwitchState(ActorState.Idle);
        }
    }
    public override void Exit()
    {
        stateMachine.parameter.skill = null;
    }

}

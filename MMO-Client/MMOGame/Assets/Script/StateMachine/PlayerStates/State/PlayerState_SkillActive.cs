using GameClient.Combat;
using Proto;
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
        if(stateMachine.parameter.skill == null)
        {
            stateMachine.SwitchState(EntityState.Idle);
            return;
        }
        animator.Play(stateMachine.parameter.skill.Define.ActiveAnimName);
    }

    public override void LogicUpdate()
    {
        if (stateMachine.parameter.skill.Stage != SkillStage.Active)
        {
            stateMachine.currentEntityState = EntityState.NoneState;
            stateMachine.SwitchState(EntityState.Idle);
        }
    }

    public override void PhysicUpdate()
    {

    }

    public override void Exit()
    {
        stateMachine.parameter.skill = null;
    }

}

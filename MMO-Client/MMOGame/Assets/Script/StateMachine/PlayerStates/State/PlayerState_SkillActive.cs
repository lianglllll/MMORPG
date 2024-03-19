using GameClient.Combat;
using Proto;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState_SkillActive : PlayerState
{
    float transitionDuration = 0.3f; // 过渡时间（秒）

    private bool isTargetSkill;

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
        if (stateMachine.parameter.skill.Define.ActiveAnimName.Equals("None")) return; 
        //animator.CrossFade(stateMachine.parameter.skill.Define.ActiveAnimName, transitionDuration);
        animator.Play(stateMachine.parameter.skill.Define.ActiveAnimName);

    }

    public override void LogicUpdate()
    {
        //当技能阶段不是active的时候就退出
        if (stateMachine.parameter.skill.Stage != SkillStage.Active)
        {
            stateMachine.SwitchState(EntityState.Idle,true);
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

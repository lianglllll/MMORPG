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
        //动画
        animator.Play(stateMachine.parameter.skill.Define.IntonateAnimName);

        //自身特效
        if(stateMachine.parameter.skill.Define.ID == 2002)
        {
            var prefab = Resources.Load<GameObject>("Effects/MagicCircles/Magic circle");
            var ins = GameObject.Instantiate(prefab, stateMachine.parameter.owner.renderObj.transform);
            GameObject.Destroy(ins, stateMachine.parameter.skill.Define.IntonateTime);
        }

        //声音


    }

    public override void LogicUpdate()
    {

    }

    public override void Exit()
    {

    }
}

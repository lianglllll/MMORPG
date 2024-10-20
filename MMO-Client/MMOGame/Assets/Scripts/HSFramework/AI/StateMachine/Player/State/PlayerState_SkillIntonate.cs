using GameClient.Combat;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState_SkillIntonate : PlayerState
{
    float transitionDuration = 0.3f; // 过渡时间（秒）

    public PlayerState_SkillIntonate(PlayerStateMachine stateMachine)
    {
        Initialize(stateMachine);
    }

    public override void Enter()
    {
        //动画
        var skill = stateMachine.parameter.skill;
        animator.CrossFade(skill.Define.IntonateAnimName, transitionDuration);

        //自身特效
        if(skill.Define.IntonateArt != "")
        {
            var prefab = Res.LoadAssetSync<GameObject>(skill.Define.IntonateArt);
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

using GameClient;
using Proto;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState_Hit : PlayerState
{
    float transitionDuration = 0.3f; // 过渡时间（秒）

    public PlayerState_Hit(PlayerStateMachine stateMachine)
    {
        Initialize(stateMachine);
    }

    public override void Enter()
    {
        animator.Play("Hit",-1,0f);
        animator.Update(0f); // 立即更新动画到起始帧

        //将来可以进行分离状态机
        if (GameApp.entityId == stateMachine.parameter.owner.EntityId)
        {
            //看向敌人
            var target = stateMachine.parameter.attacker;
            if (target != null)
            {
                stateMachine.parameter.owner.LookTarget(target.renderObj.transform.position);
                stateMachine.parameter.attacker = null;
            }
        }
    }

    public override void LogicUpdate()
    {
        //参考一梦江湖，受击状态是不强制你不能操作的
        //在移动的时候，你收到伤害甚至不会播放受击动作。
        //只有当你站着不动的时候，收到伤害时，才会播放受击动画，
        //并且面向攻击者(面向这个动作得在自身不在特殊状态如眩晕的情况下)。并且这个受击动画状态是可以被移动打断的。
        //如果是ai的话，我们得让它的受机变得像眩晕那样了，得有一个僵直时间，起码要把动画播放完毕。

        //这里一旦动作播放完毕就退出到idle
        if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
        {
            // 动画播放完毕
            // 注意: normalizedTime 可以大于1，特别是在循环动画的情况下，因此可能需要额外的条件来处理循环
            stateMachine.SwitchState(EntityState.Idle,true);
        }

    }

    public override void Exit()
    {
    }


}

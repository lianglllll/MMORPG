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
        animator.CrossFade("Hit", transitionDuration);
    }

    public override void LogicUpdate()
    {
        //监听hit，这是是否监听都不需要呢？只是播放一个动画？
        //或许可以给技能添加一个造成僵直的时间，这段时间被打这个不能动弹
    }

    public override void Exit()
    {
    }


}

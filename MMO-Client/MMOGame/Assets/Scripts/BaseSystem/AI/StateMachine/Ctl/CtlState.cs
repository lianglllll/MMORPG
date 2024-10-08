using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CtlState : IState
{
    protected Animator animator;                                  //动画器用于动画播放
    protected CtlStateMachine stateMachine;                     //用于状态的切换

    public void Initialize(CtlStateMachine stateMachine)
    {
        this.animator = stateMachine.parameter.animator;
        this.stateMachine = stateMachine;
    }

    public virtual void Enter()
    {
    }

    public virtual void Exit()
    {
    }

    public virtual void LogicUpdate()
    {
    }

    public virtual void PhysicUpdate()
    {
    }
}

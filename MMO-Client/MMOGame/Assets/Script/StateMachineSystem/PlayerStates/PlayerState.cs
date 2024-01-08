using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState : IState
{
    protected Animator animator;                                   //动画器用于动画切换
    protected PlayerStateMachine stateMachine;                     //用于状态的切换

    public void Initialize(PlayerStateMachine stateMachine)
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

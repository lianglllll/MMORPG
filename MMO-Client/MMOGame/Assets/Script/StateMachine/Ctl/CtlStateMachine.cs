using Assets.Script.StateMachine.Ctl.State;
using GameClient.Combat;
using GameClient.Entities;
using Proto;
using Serilog;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



/// <summary>
/// 状态间共享的参数
/// </summary>
public class CtlParameter
{
    public Actor owner;
    public Animator animator;
    public Skill skill;
}

/// <summary>
/// 玩家状态机,只做动画的同步，不做其他的逻辑处理
/// </summary>
public class CtlStateMachine : StateMachine
{
    public CtlParameter parameter;
    public EntityState currentEntityState;
    private Dictionary<EntityState, IState> stateTable = new();
    private void Awake()
    {
        parameter = new CtlParameter();
        parameter.animator = GetComponent<Animator>();

        //添加状态
        stateTable.Add(EntityState.Idle, new CtlState_Idle(this));
        stateTable.Add(EntityState.Motion, new CtlState_Motion(this));
    }

    private void Start()
    {
        //空闲状态启动
        currentEntityState = EntityState.Idle;
        SwitchOn(stateTable[EntityState.Idle]);
    }

    /// <summary>
    /// 逻辑更新
    /// </summary>
    protected override void Update()
    {
        base.Update();
    }

    //公有的状态切换
    public void SwitchState(EntityState state, bool enforce = false)
    {
        //切换
        currentEntityState = state;
        SwitchState(stateTable[currentEntityState]);
    }

}

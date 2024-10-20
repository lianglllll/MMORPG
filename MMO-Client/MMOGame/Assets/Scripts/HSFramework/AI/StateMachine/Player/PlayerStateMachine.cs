using BaseSystem.AI;
using GameClient.Combat;
using GameClient.Entities;
using Proto;
using Serilog;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



//我们的同步状态机中，可以同步一些个需要状态连贯性的动画比如说蓄力动画和技能激活动画
//蓄力state的update中可以监测skill中的skill阶段来选择是否跳跃到下一个节点(激活state)
//而激活state中我们可以在updata中检测skill中的阶段变化(即就是等到激活时间结束skill进入冷却阶段），我们可以跳跃回idleState
//状态机可以分离state之间的逻辑，让其只关注本state的情况
//反正比之前那个好看一点(本质其实没啥变化)，再不济也可以当成一个动画切换器来使用

/// <summary>
/// 状态间共享的参数
/// </summary>
public class Parameter
{
    public Actor owner;
    public Animator animator;
    public Skill skill;
    //hit的时候有用
    public Actor attacker;
}

/// <summary>
/// 玩家状态机,只做动画的同步，不做其他的逻辑处理
/// </summary>
public class PlayerStateMachine : StateMachine
{
    public Parameter parameter;
    public EntityState currentEntityState;
    private Dictionary<EntityState, IState> stateTable = new();
    private void Awake()
    {
        parameter = new Parameter();
        parameter.animator = GetComponent<Animator>();
        //添加状态
        stateTable.Add(EntityState.Idle, new PlayerState_Idle(this));
        stateTable.Add(EntityState.Motion, new PlayerState_Motion(this));
        stateTable.Add(EntityState.Hit, new PlayerState_Hit(this));
        stateTable.Add(EntityState.Death, new PlayerState_Death(this));
        stateTable.Add(EntityState.SkillIntonate, new PlayerState_SkillIntonate(this));
        stateTable.Add(EntityState.SkillActive, new PlayerState_SkillActive(this));
        stateTable.Add(EntityState.Dizzy, new PlayerState_Dizzy(this));

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
        //死人就不更新了，等强制退出死亡状态
        if (currentEntityState != EntityState.Death && parameter.owner.IsDeath)
        {
            SwitchState(EntityState.Death);
        }
        base.Update();
    }

    //公有的状态切换
    public void SwitchState(EntityState state, bool enforce = false,bool reentry = false)
    {
        //空状态，我们维持当前状态
        if (state == EntityState.NoneState) return;

        //相同的状态，我们维持当前状态
        if (currentEntityState == state && reentry == false) return;

        //技能action状态，不可被切换
        if (currentEntityState == EntityState.SkillActive && enforce == false) return;

        //技能眩晕状态，不可被切换
        if (currentEntityState == EntityState.Dizzy && enforce == false) return;

        //当前为死亡状态，不可被切换
        if (currentEntityState == EntityState.Death && enforce == false) return;

        //切换
        currentEntityState = state;
        SwitchState(stateTable[currentEntityState]);
    }

    //是否在特殊状态:玩家是不能操作的状态
    public bool IsSpecialState()
    {
        if(currentEntityState == EntityState.Death ||
            currentEntityState == EntityState.SkillIntonate ||
            currentEntityState == EntityState.SkillActive ||
            currentEntityState == EntityState.Dizzy ||
            currentEntityState == EntityState.Hit)
        {
            return true;
        }
        return false;
    }
}

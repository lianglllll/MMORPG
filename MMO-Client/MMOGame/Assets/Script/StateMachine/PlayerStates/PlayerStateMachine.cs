using GameClient.Combat;
using GameClient.Entities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



//我们的同步状态机中，可以同步一些个需要状态连贯性的动画比如说蓄力动画和技能激活动画
//蓄力state的update中可以监测skill中的skill阶段来选择是否跳跃到下一个节点(激活state)
//而激活state中我们可以在updata中检测skill中的阶段变化(即就是等到激活时间结束skill进入冷却阶段），我们可以跳跃回idleState
//状态机可以分离state之间的逻辑，让其只关注本state的情况
//反正比之前那个好看一点(本质其实没啥变化)，再不济也可以当成一个动画切换器来使用


/// <summary>
/// actor的状态
/// </summary>
public enum ActorState
{
    None = 0,
    Idle = 1,
    Walk = 2,
    Run = 3,
    Jump = 4,

    Death = 5,
    Hit = 6,
    SkillIntonate = 7,      //技能蓄气
    SkillActive = 8,        //技能激活
    Swordflight = 9,
}

/// <summary>
/// 状态间共享的参数
/// </summary>
public class Parameter
{
    public Actor owner;
    public Animator animator;
    public Skill skill;
}

/// <summary>
/// 玩家状态机,只做动画的同步，不做其他的逻辑处理
/// </summary>
public class PlayerStateMachine : StateMachine
{
    public ActorState currentActorState;
    private Dictionary<ActorState, IState> stateTable = new Dictionary<ActorState, IState>();
    public Parameter parameter;
    private void Awake()
    {
        parameter = new Parameter();
        parameter.animator = GetComponent<Animator>();
        //添加状态
        stateTable.Add(ActorState.Idle, new PlayerState_Idle(this));
        stateTable.Add(ActorState.Walk, new PlayerState_Walk(this));
        stateTable.Add(ActorState.Death, new PlayerState_Death(this));
        stateTable.Add(ActorState.SkillIntonate, new PlayerState_SkillIntonate(this));
        stateTable.Add(ActorState.SkillActive, new PlayerState_SkillActive(this));

    }

    private void Start()
    {
        //空闲状态启动
        currentActorState = ActorState.Idle;
        SwitchOn(stateTable[ActorState.Idle]);
    }

    //公有的状态切换
    public  void SwitchState(ActorState state)
    {
        //空
        if (state == ActorState.None) return;

        //相同的
        if (currentActorState == state) return;

        //action状态，不可打断状态，之间返回不允许切换。
        //这里再优化吧
        if (currentActorState == ActorState.SkillActive && parameter.skill != null) return;

        //切换
        currentState?.Exit();
        currentActorState = state;
        IState newState = stateTable[currentActorState];
        SwitchOn(newState);
    }

    //是否在特殊状态
    public bool IsSpecialState()
    {
        if(currentActorState == ActorState.Death || currentActorState == ActorState.SkillIntonate || currentActorState == ActorState.SkillActive)
        {
            return true;
        }
        return false;
    }
}

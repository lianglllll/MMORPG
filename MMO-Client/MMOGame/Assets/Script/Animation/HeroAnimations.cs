using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Proto;
using Animancer;
using GameClient.Combat;

public class HeroAnimations : MonoBehaviour
{
    public enum HState
    {
        None = 0,
        Idle = 1,
        Run = 2,
        Attack = 3,
        Die = 4,
        Gethit = 5,
        SkillIntonate,      //技能蓄气
        SkillActive,        //技能激活
    }

    public HState state = HState.Idle;    //当前角色的状态
    private GameEntity gameEntity;
    private NamedAnimancerComponent _animancer;
    private Skill skill;

    private void Awake()
    {
        gameEntity = GetComponent<GameEntity>();
        _animancer = GetComponent<NamedAnimancerComponent>();
    }
    private void Start()
    {
        Kaiyun.Event.RegisterOut("OnSkillIntonate", this, "OnSkillIntonate");
        Kaiyun.Event.RegisterOut("OnSkillActive", this, "OnSkillActive");
    }
    private void OnDestroy()
    {
        Kaiyun.Event.UnregisterOut("OnSkillIntonate",this, "OnSkillIntonate");
    }

    private void Update()
    {
        if(skill != null)
        {
            //施法动作
            if(skill.Stage == SkillStage.Intonate)
            {
                Play(skill.Define.IntonateAnimName);
            }else if(skill.Stage == SkillStage.Active)
            {
                Play(skill.Define.ActiveAnimName);
            }
            //恢复动作
            if(state == HState.SkillIntonate && skill.Stage != SkillStage.Intonate)
            {
                state = HState.Idle;
                PlayIdle();
            }
            else if (state == HState.SkillActive && skill.Stage != SkillStage.Active)
            {
                state = HState.Idle;
                PlayIdle();
            }
        }

    }


    /// <summary>
    /// 技能蓄气
    /// </summary>
    /// <param name="skill"></param>
    public void OnSkillIntonate(Skill skill)
    {
        if (gameEntity.entityId != skill.Owner.EntityId) return;
        this.skill = skill;
        this.state = HState.SkillIntonate;
    }

    /// <summary>
    /// 技能激活
    /// </summary>
    /// <param name="skill"></param>
    public void OnSkillActive(Skill skill)
    {
        if (gameEntity.entityId != skill.Owner.EntityId) return;
        this.skill = skill;
        this.state = HState.SkillActive;
    }

    //todo 用于网络同步使用
    public void switchState(EntityState entityState)
    {
        switch (entityState)
        {
            case EntityState.Idle:
                PlayIdle();
                break;
            case EntityState.Walk:
                PlayRun();
                break;
            default:
                PlayIdle();
                break;
        }
        //gameEntity.lastEntityState = entityState;
    }

    //通用的播放动画
    public void Play(string animationName,Action OnEnd=null)
    {
        if (animationName == null) return;
        AnimancerState state = _animancer.TryPlay(animationName);  //它会从组件中挂载的动画中，根据名字来找
        if(state == null)
        {
            Debug.LogError($"animation[{animationName}] does not exist.");
        }
        else
        {
            if(OnEnd != null)//添加动画结束回调
            {
                state.Events.OnEnd = OnEnd;
            }
        }

    }

    public void PlayIdle()
    {
        if (state == HState.Attack || state == HState.Gethit || state == HState.SkillIntonate)
            return;
        Play("Idle");
        state = HState.Idle;
        //gameEntity.entityState = EntityState.Idle;
    }

    public void PlayRun()
    {
        if (state == HState.Attack || state == HState.SkillIntonate)
            return;
        Play("Walk");
        state = HState.Run;
        //gameEntity.entityState = EntityState.Walk;
    }

    public void PlayAttack1()
    {
        Play("Attack01", Attack01End);
        state = HState.Attack;
    }
    public void Attack01End()
    {
        state = HState.None;
        PlayIdle();
    }

    public void PlayDie()
    {
        Play("Die");
        state = HState.Die;
    }

    public void PlayGethit()
    {
        Play("Gethit", GethitEnd);
        state = HState.Gethit;
    }

    public void GethitEnd()
    {
        state = HState.None;
        PlayIdle();
    }




}

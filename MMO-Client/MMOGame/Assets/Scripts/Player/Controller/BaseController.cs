using GameClient.Combat;
using GameClient.Entities;
using HSFramework.AI.StateMachine;
using Proto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Player
{

    /// <summary>
    /// 状态间共享的参数
    /// </summary>
    public class StateMachineParameter
    {
        public float gravity = -9.8f;
        public float rotationSpeed = 8f;
        public float curSpeed = 1f;
        public Actor attacker;
        public Skill curSkill;
    }

    public abstract class BaseController: MonoBehaviour,IStateMachineOwner
    {
        //角色模型
        private ModelBase model;
        public ModelBase Model { get => model; }

        //声音源
        protected AudioSource audioSource;

        //状态机
        public StateMachine stateMachine { get; protected set; }
        protected CommonSmallState curState;
        public CommonSmallState CurState => curState;
        private StateMachineParameter stateMachineParameter;
        public StateMachineParameter StateMachineParameter => stateMachineParameter;


        //角色控制器
        private CharacterController characterController;
        public CharacterController CharacterController { get => characterController; }

        //ui控制
        public UnitUIController unitUIController;

        //信息
        private Actor actor;
        public Actor Actor => actor;

        protected virtual void Awake()
        {
            model = transform.Find("Model").GetComponent<ModelBase>();
            characterController = GetComponent<CharacterController>();
            audioSource = GetComponent<AudioSource>();
            unitUIController = GetComponent<UnitUIController>();
        }
        public virtual void Init(Actor actor)
        {
            this.actor = actor;
            Model.Init();
            stateMachine = new StateMachine();
            stateMachineParameter = new StateMachineParameter();
            stateMachine.Init(this, stateMachineParameter);
        }

        //状态改变
        public EntityState GetEntityState(CommonSmallState state)
        {
            switch (state)
            {
                case CommonSmallState.Idle:
                    return EntityState.Idle;
                case CommonSmallState.Move:
                    return EntityState.Motion;
                default:
                    return EntityState.NoneState;
            }
        }
        public CommonSmallState GetCommonSmallState(EntityState state)
        {
            switch (state)
            {
                case EntityState.Idle:
                    return CommonSmallState.Idle;
                case EntityState.Motion:
                    return CommonSmallState.Move;
                default:
                    return CommonSmallState.None;
            }
        }


        public virtual void ChangeState(CommonSmallState state, bool reCurrstate = false)
        {

        }
        private string currentAnimationName;
        public void PlayAnimation(string animationName, bool reState = false, float fixedTransitionDuration = 0.25f)
        {
            //同名动画问题
            if (currentAnimationName == animationName && !reState)
            {
                return;
            }
            currentAnimationName = animationName;
            Model.Animator.CrossFadeInFixedTime(animationName, fixedTransitionDuration);
        }
        public void PlayAudio(AudioClip audioClip)
        {
            if (audioClip == null) return;
            audioSource.PlayOneShot(audioClip);
        }
        public void OnFootStep()
        {

        }


    }
}

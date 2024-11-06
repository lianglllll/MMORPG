using GameClient.Combat;
using GameClient.Entities;
using HSFramework.AI.StateMachine;
using HSFramework.Net;
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
        public float moveSpeed = 1f;
        public float rotationSpeed = 8f;
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

        //角色控制器
        private CharacterController characterController;
        public CharacterController CharacterController { get => characterController; }

        //ui控制
        public UnitUIController unitUIController;

        //信息
        private Actor actor;
        public Actor Actor => actor;

        //初始化
        protected virtual void Awake()
        {
            model = transform.Find("Model").GetComponent<ModelBase>();
            characterController = GetComponent<CharacterController>();
            audioSource = GetComponent<AudioSource>();
            unitUIController = GetComponent<UnitUIController>();
        }
        public virtual void Init(Actor actor, SyncEntity syncEntity)
        {
            this.actor = actor;
            Model.Init();
            unitUIController.Init(actor);
            stateMachine = new StateMachine();
            stateMachineParameter = new StateMachineParameter();
            stateMachine.Init(this, stateMachineParameter);
        }


        #region 状态机

        public StateMachine stateMachine { get; protected set; }
        protected ActorState curState;
        public ActorState CurState => curState;
        private StateMachineParameter stateMachineParameter;
        public StateMachineParameter StateMachineParameter => stateMachineParameter;

        public virtual void ChangeState(ActorState state, bool reCurrstate = false)
        {

        }


        #endregion

        #region 动画相关

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

        #endregion

        #region 工具

        public void DirectLookTarget(Vector3 pos)
        {

            // 计算角色应该朝向目标点的方向
            Vector3 targetDirection = pos - transform.position;

            // 限制在Y轴上的旋转
            targetDirection.y = 0;

            // 计算旋转方向
            Quaternion targetRotation = Quaternion.LookRotation(targetDirection);

            // 将角色逐渐旋转到目标方向
            //float rotationSpeed = 5f;
            //renderObj.transform.rotation = Quaternion.Slerp(renderObj.transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            // 立即将角色转向目标方向
            transform.rotation = targetRotation;
        }

        #endregion

    }
}

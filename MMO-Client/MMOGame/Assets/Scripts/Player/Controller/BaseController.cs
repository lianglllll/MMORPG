using GameClient.Combat;
using GameClient.Entities;
using HS.Protobuf.SceneEntity;
using HSFramework.AI.StateMachine;
using HSFramework.Net;
using UnityEngine;

namespace Player
{

    /// <summary>
    /// 状态间共享的参数
    /// </summary>
    public class StateMachineParameter
    {
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

        #region Player配置信息

        [Header("Player配置")]
        public float gravity = -9.8f;

        public float rotateSpeed = 5f;

        public float walk2RunTransitionSpeed = 1f;
        public float walkSpeed = 1.5f;
        public float runSpeed = 5f;

        public float jumpVelocity = 5f;
        public float moveSpeedForJump = 1f;
        public float moveSpeedForAirDown = 1f;

        public float needPlayEndAnimationHeight = 5f;                  //如果空中检测到距离地面有3米则启动翻滚
        public float playEndAnimationHeight = 1.8f;                    //End动画播放需要的高度
        public float rollPower = 1f;
        public float rotateSpeedForAttack = 5f;
        public float DefenceTime;
        public float WaitCounterAttackTime;

        #endregion



        //初始化
        protected virtual void Awake()
        {
            model = transform.Find("Model").GetComponent<ModelBase>();
            characterController = GetComponent<CharacterController>();
            audioSource = GetComponent<AudioSource>();
            unitUIController = GetComponent<UnitUIController>();
        }
        public virtual void Init(Actor actor, NetworkActor networkActor)
        {
            this.actor = actor;
            Model.Init();
            unitUIController.Init(actor);
            stateMachine = new StateMachine();
            stateMachineParameter = new StateMachineParameter();
            stateMachine.Init(this);
        }


        #region 状态机

        public StateMachine stateMachine { get; protected set; }
        protected NetActorState curState;
        public NetActorState CurState => curState;
        private StateMachineParameter stateMachineParameter;
        public StateMachineParameter StateMachineParameter => stateMachineParameter;
        public virtual void ChangeState(NetActorState state, bool reCurrstate = false)
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

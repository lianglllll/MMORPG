using GameClient.Combat;
using GameClient.Entities;
using HS.Protobuf.Scene;
using HS.Protobuf.SceneEntity;
using HSFramework.AI.StateMachine;
using System.Collections.Generic;
using UnityEngine;

namespace Player
{

    /// <summary>
    /// 状态间共享的参数
    /// </summary>
    public class StateMachineParameter
    {
        public Actor attacker;
        public Skill curSkill;

        // jump
        public float jumpVelocity;

        // evade相关共享参数
        public NetAcotrEvadeStatePayload evadeStatePayload;
        public Vector3 evadeRotation;
    }

    public abstract class BaseController: MonoBehaviour,IStateMachineOwner
    {
        //角色模型
        private ModelBase model;
        public ModelBase Model { get => model; }

        //角色控制器
        private CharacterController characterController;
        public CharacterController CharacterController { get => characterController; }

        //ui控制
        [HideInInspector]
        public UnitUIController unitUIController;

        // 声音控制
        private UnitAudioManager m_unitAudioManager;

        // 特效控制
        [HideInInspector]
        public UnitEffectManager m_unitEffectManager;


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

        public float flyWalkSpeed = 10f;
        public float flyRunSpeed = 20f;
        public float flyChangeHightSpeed = 8f;

        public List<AudioClip> normal_Motion_AudioClip = new();

        #endregion

        //初始化
        protected virtual void Awake()
        {
            model = transform.Find("Model").GetComponent<ModelBase>();
            characterController = GetComponent<CharacterController>();
            unitUIController = GetComponent<UnitUIController>();
            m_unitAudioManager = transform.Find("UnitAudioManager").GetComponent<UnitAudioManager>();
            m_unitEffectManager = transform.Find("UnitEffectManager").GetComponent<UnitEffectManager>();
        }
        public virtual void Init(Actor actor, NetworkActor networkActor)
        {
            this.actor = actor;
            // Model.Init();
            unitUIController.Init(actor);
            m_unitAudioManager.Init();
            m_unitEffectManager.Init();
            stateMachine = new StateMachine();
            m_stateMachineParameter = new StateMachineParameter();
            stateMachine.Init(this);
        }
        public virtual void UnInit()
        {
            stateMachine.UnInit();
        }
        protected virtual void Update()
        {
            if (isTransitioning)
            {
                lerpTime += Time.deltaTime;
                float t = lerpTime / transitionDuration;
                transform.position = Vector3.Lerp(transform.position, targetPosition, t);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, t);

                // 检查是否接近目标位置
                if (Vector3.Distance(transform.position, targetPosition) < positionThreshold &&
                    Quaternion.Angle(transform.rotation, targetRotation) < rotationThreshold)
                {
                    isTransitioning = false;
                    // 确保最终位置和旋转准确
                    transform.position = targetPosition;
                    transform.rotation = targetRotation;
                    return;
                }
            }
        }


        #region 状态机

        public StateMachine stateMachine { get; protected set; }
        private StateMachineParameter m_stateMachineParameter;
        protected NetActorState m_curState;
        protected NetActorMode m_curMode;
        public  NetActorState CurState => m_curState;
        public NetActorMode CurMode => m_curMode;
        public StateMachineParameter StateMachineParameter => m_stateMachineParameter;
        public virtual void ChangeState(NetActorState state, bool reCurrstate = false)
        {
        }

        public bool IsTransitioning => isTransitioning;
        public float transitionDuration = 0.2f; // 转换持续时间
        private Vector3 targetPosition;
        private Quaternion targetRotation;
        private float lerpTime;
        private bool isTransitioning = false;
        // 定义接近目标值的阈值
        float positionThreshold = 0.01f; // 可根据需要调整
        float rotationThreshold = 1.0f;  // 单位为度数，可根据需要调整

        protected ActorChangeStateResponse curActorChangeStateResponse;
        public void AdjustToOriginalTransform(NetActorState oldNetActorState, ActorChangeStateResponse message)
        {
            curActorChangeStateResponse = message;
            targetPosition = actor.Position;
            if(oldNetActorState == NetActorState.Falling)
            {
                targetPosition.y = transform.position.y;
            }
            targetRotation = Quaternion.Euler(actor.Rotation);

            // 重置插值计时器
            lerpTime = 0;
            isTransitioning = true;
        }
        public virtual void ChangeMode(NetActorMode mode)
        {
            if (m_curMode != mode) {
                m_curMode = mode;
                ChangeState(NetActorState.Idle,true);
            }
        }
        public void InitModeAndState(NetActorMode mode, NetActorState state = NetActorState.None)
        {
            m_curMode = mode;
            if(state != NetActorState.None)
            {
                ChangeState(state);
            }
            else
            {
                ChangeState(NetActorState.Idle);
            }
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
        public void OnFootStep()
        {
            m_unitAudioManager.PlayFootAudioClip();
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

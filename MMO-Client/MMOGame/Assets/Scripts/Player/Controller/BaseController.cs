using GameClient;
using GameClient.Combat;
using GameClient.Combat.LocalSkill.Config;
using GameClient.Entities;
using HS.Protobuf.Scene;
using HS.Protobuf.SceneEntity;
using HSFramework.AI.StateMachine;
using HSFramework.MyDelayedTaskScheduler;
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
        // 角色模型
        private ModelBase model;
        public ModelBase Model { get => model; }

        // 角色控制器
        private CharacterController characterController;
        public CharacterController CharacterController { get => characterController; }

        // 武器
        private WeaponManager m_weaponManaager;

        // ui控制
        [HideInInspector]
        public UnitUIController unitUIController;

        // 声音控制
        private UnitAudioManager m_unitAudioManager;

        // 特效控制
        [HideInInspector]
        public UnitEffectManager m_unitEffectManager;

        // 信息
        private Actor m_actor;
        public Actor Actor => m_actor;

        // 网络相关
        private NetworkActor m_networkActor;
        public NetworkActor NetworkActor => m_networkActor;

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

        // 初始化
        protected virtual void Awake()
        {
            model = transform.Find("Model").GetComponent<ModelBase>();
            m_weaponManaager = transform.Find("Model").GetComponent<WeaponManager>();
            characterController = GetComponent<CharacterController>();
            unitUIController = GetComponent<UnitUIController>();
            m_unitAudioManager = transform.Find("UnitAudioManager").GetComponent<UnitAudioManager>();
            m_unitEffectManager = transform.Find("UnitEffectManager").GetComponent<UnitEffectManager>();
            m_networkActor = GetComponent<NetworkActor>();
        }
        private void Start()
        {
            model.Init(this);
            m_unitAudioManager.Init();
            m_weaponManaager.Init();
        }


        public virtual void Init(Actor actor)
        {
            m_actor = actor;

            unitUIController.Init(actor);
            m_unitEffectManager.Init();
            m_networkActor.Init(this);
            stateMachine.Init(this);

            isTransitioning = false;
            CurSkill = null;
        }
        public virtual void UnInit()
        {
            unitUIController.UnInit();
            m_unitEffectManager.Init();
            m_networkActor.UnInit();
            stateMachine.UnInit();

            isTransitioning = false;
            CurSkill = null;
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

        public StateMachine stateMachine = new();
        private StateMachineParameter m_stateMachineParameter = new();
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
            targetPosition = m_actor.Position;
            if(oldNetActorState == NetActorState.Falling || (CurMode != NetActorMode.Fly && CurMode != NetActorMode.FlyEquip))
            {
                targetPosition.y = transform.position.y;
            }
            targetRotation = Quaternion.Euler(m_actor.Rotation);

            // 重置插值计时器
            lerpTime = 0;
            isTransitioning = true;
        }
        public virtual void ChangeMode(NetActorMode mode)
        {
            if (m_curMode == mode) {
                goto End;
            }

            var old_Mode = m_curMode;
            m_curMode = mode;
            if(old_Mode == NetActorMode.NormalEquip || old_Mode == NetActorMode.FlyEquip || old_Mode == NetActorMode.MountedEquip)
            {
                if (m_curMode != NetActorMode.NormalEquip && m_curMode != NetActorMode.FlyEquip && m_curMode != NetActorMode.MountedEquip)
                {
                    model.Animator.SetBool("UnEquiping", true);
                }
            }
            else
            {
                if (m_curMode == NetActorMode.NormalEquip || m_curMode == NetActorMode.FlyEquip || m_curMode == NetActorMode.MountedEquip)
                {
                    model.Animator.SetBool("Equiping", true);
                }
            }

            ChangeState(NetActorState.Idle, true);
        End:
            return;
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
        public void EquipAction()
        {
            // 设置武器显隐等
            m_weaponManaager.ShowCurWeapon();
        }
        public void EquipEndAction()
        {
            model.Animator.SetBool("Equiping", false);
        }
        public void UnEquipAction()
        {
            // 设置武器显隐等
            m_weaponManaager.HideCurWeapon();
        }
        public void UnEquipEndAction()
        {
            model.Animator.SetBool("UnEquiping", false);
        }

        #endregion

        #region Skill
        public Skill CurSkill
        {
            get;
            set;
        }
        public bool IsCanCancelSkill
        {
            get;
            set;
        }
        public bool IsCanSwitchSkill
        {
            get;
            set;
        }
        public bool UseSkill(Skill skill)
        {
            if(m_actor.EntityId == GameApp.character.EntityId)
            {
                // 有可能是在技能没结束的时候变招了，也有可能是打断
                if(CurSkill != null)
                {
                    CurSkill.CancelSkill();
                }

                CurSkill = skill;
                IsCanCancelSkill = false;
                IsCanSwitchSkill = false;
            }
            m_stateMachineParameter.curSkill = skill;

            return true;
        }
        public void OnCanCancelSkill()
        {
            IsCanCancelSkill = true;
        }
        public void OnCanSwitchSkill()
        {
            IsCanSwitchSkill = true;
        }
        public void OnSkillOver(Skill skill)
        {
            Kaiyun.Event.FireIn("SkillEnterColdDown");
            if (CurSkill != skill) return;
            CurSkill = null;
            IsCanCancelSkill = false;
            IsCanSwitchSkill = false;
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
        public void CreateSpawnObjectAtPos(LocalSkill_SpawnObject spawnObj, Vector3 spawnPoint)
        {
            if (spawnObj != null && spawnObj.prefab != null)
            {
                DelayedTaskScheduler.Instance.AddDelayedTask(spawnObj.delayTime, () => {
                    var tmp = Instantiate(spawnObj.prefab);
                    tmp.transform.position = spawnPoint + spawnObj.pos;
                    tmp.transform.eulerAngles += spawnObj.rotation;
                    tmp.transform.localScale += spawnObj.rotation;
                    tmp.transform.LookAt(Camera.main.transform);
                    PlayAudio(spawnObj.audioClip);
                });
            }

        }
        public void CreateSpawnObjectAroundOwner(LocalSkill_SpawnObject spawnObj)
        {
            if (spawnObj.prefab != null)
            {
                DelayedTaskScheduler.Instance.AddDelayedTask(spawnObj.delayTime, () => {
                    // 一般特效的生成位置是相对于主角的
                    var obj = Instantiate(spawnObj.prefab);
                    obj.transform.position = transform.TransformPoint(spawnObj.pos);                
                    obj.transform.rotation = transform.rotation * Quaternion.Euler(spawnObj.rotation);
                    Destroy(obj, 2f);
                    if (spawnObj.audioClip != null)
                    {
                        PlayAudio(spawnObj.audioClip);
                    }
                });
            }
        }
        public void PlayAudio(AudioClip audioClip)
        {
            if (audioClip == null) return;
            m_unitAudioManager.PlayAudio(audioClip);
        }

        #endregion

    }
}

using GameClient.Combat;
using GameClient.Combat.Buffs;
using GameClient.Manager;
using HS.Protobuf.Combat.Skill;
using HS.Protobuf.Scene;
using HS.Protobuf.SceneEntity;
using Player;
using Player.Controller;
using Serilog;
using UnityEngine;

namespace GameClient.Entities
{
    public class Actor:Entity
    {
        private bool m_isInit;
        protected GameObject        m_renderObj;
        private UnitDefine          m_unitDefine;                                               
        private NetActorNode        m_netActorNode;                                     
        public BaseController       m_baseController;
        public SkillManager         m_skillManager;
        private BuffManager         m_buffManager;

        public GameObject RenderObj {
            get
            {
                return m_renderObj;
            }
            set
            {
                m_renderObj = value;
            }
        }
        public UnitDefine UnitDefine => m_unitDefine;
        public BuffManager BuffManager => m_buffManager;
        public NetActorMode NetActorMode
        {
            get => m_netActorNode.NetActorMode;
            set => m_netActorNode.NetActorMode = value;
        }
        public NetActorState NetActorState
        {
            get => m_netActorNode.NetActorState;
            set => m_netActorNode.NetActorState = value;
        }
        public bool IsDeath => m_netActorNode.NetActorState == NetActorState.Death;
        public string ActorName => m_netActorNode.ActorName;
        public int Hp => m_netActorNode.Hp;
        public int MaxHp => m_netActorNode.MaxHp;
        public int Mp => m_netActorNode.Mp;
        public int MaxMp => m_netActorNode.MaxMp;
        public int CurSceneId => m_netActorNode.SceneId;
        public int Level => m_netActorNode.Level;
        public long Exp => m_netActorNode.Exp;
        public float Speed { get => m_netActorNode.Speed * 0.001f;}

        public Actor(NetActorNode netAcotrNode) :base(netAcotrNode.EntityId, netAcotrNode.Transform)
        {
            m_netActorNode = netAcotrNode;
            m_unitDefine = LocalDataManager.Instance.m_unitDefineDict[netAcotrNode.ProfessionId];

            m_buffManager = new();
            m_buffManager.Init(this, netAcotrNode.Buffs);

            // m_equipManager = new();
            // m_equipManager.Init(this, netAcotrNode.WornEquipments);

            m_skillManager = new();
            if(GameApp.entityId == EntityId)
            {
                m_skillManager.Init(this, netAcotrNode.FixedSkillGroupInfo.Skills);
            }
            else
            {
                m_skillManager.Init(this, null);
            }
        }
        public void Init(BaseController baseController)
        {
            m_baseController = baseController;
            RenderObj = m_baseController.gameObject;
            m_isInit = true;
        }
        public override void Update(float deltatime)
        {
            m_skillManager.Update(deltatime);
            m_buffManager.Update(deltatime);
        }

        public void OnRecvDamage(Damage damage)
        {
            // 受伤，被别人打了，播放一下特效或者ui。不做数值更新
            if (m_renderObj == null) return;
            if (IsDeath) return;

            // ui
            var ownerPos = m_renderObj.transform.position;
            if (damage.IsImmune)
            {
                DynamicTextManager.CreateText(ownerPos, "免疫", DynamicTextManager.missData);
            }
            else if (damage.IsMiss)
            {   // 闪避了，显示一下闪避ui
                DynamicTextManager.CreateText(ownerPos, "Miss", DynamicTextManager.missData);
            }
            else{
                // 伤害飘字
                if(damage.DamageType == DameageType.Magical)
                {
                    DynamicTextManager.CreateText(ownerPos, damage.Amount.ToString("0"), DynamicTextManager.Spell);
                }
                else if(damage.DamageType == DameageType.Physical)
                {
                    DynamicTextManager.CreateText(ownerPos, damage.Amount.ToString("0"), DynamicTextManager.Physical);
                }
                else if (damage.DamageType == DameageType.Real)
                {
                    DynamicTextManager.CreateText(ownerPos, damage.Amount.ToString("0"), DynamicTextManager.Real);
                }
                else
                {
                    DynamicTextManager.CreateText(ownerPos, damage.Amount.ToString("0"));
                }

                // 暴击做一些处理，震屏..
                if (damage.IsCrit)
                {
                    DynamicTextManager.CreateText(ownerPos, "Crit!", DynamicTextManager.critData);
                }

                // 被技能击中的粒子效果
                if (damage.SkillId != 0)
                {
                    var skillDef = LocalDataManager.Instance.m_skillDefineDict[damage.SkillId];
                    if (skillDef != null)
                    {
                        GameEffectManager.AddEffectTarget(skillDef.HitArt, m_renderObj, new Vector3(0, 1, 0));
                    }
                }

                // 被击中的音效



                // 切换到挨打的动作
                if (m_baseController.CurState != NetActorState.Motion)
                {
                    m_baseController.StateMachineParameter.attacker = EntityManager.Instance.GetEntity<Actor>(damage.AttackerId);
                    m_baseController.ChangeState(NetActorState.Hurt, true);
                }

                if(EntityId == GameApp.entityId)
                {
                    Kaiyun.Event.FireIn("EnterCombatEvent");
                }
            }
        }
        public void OnHpChanged(int oldHp,int newHp)
        {
            m_netActorNode.Hp = newHp;
            if(EntityId != GameApp.entityId)
            {
                m_baseController.unitUIController.UpdateHpBar();
            }
        }
        public void OnMpChanged(int old_value, int new_value)
        {
            this.m_netActorNode.Mp = new_value;
        }
        public void OnLevelChanged(int old_value, int new_value)
        {
            //更新当前actor的数据
            m_netActorNode.Level = new_value;
        }
        public void OnExpChanged(long old_value, long new_value)
        {
            //更新当前actor的数据
            m_netActorNode.Exp = new_value;
        }
        public void OnMaxHpChanged(int old_value, int new_value)
        {
            m_netActorNode.MaxHp = new_value;
        }
        public void OnMaxMpChanged(int old_value, int new_value)
        {
            m_netActorNode.MaxMp = new_value;
        }
        public void OnSpeedChanged(int old_value, int new_value)
        {
            // Speed = new_value;
        }
        public virtual void OnDeath()
        {
        }
        public virtual void OnRevive() { }

        // tools
        public void HandleActorChangeModeResponse(ActorChangeModeResponse message)
        {
            NetActorMode = message.Mode;
            m_baseController.ChangeMode(NetActorMode);
        }
        public void HandleActorChangeStateResponse(ActorChangeStateResponse message)
        {
            if (m_baseController == null)
            {
                Log.Warning("Actor:HandleActorChangeStateResponse m_baseController is null");
                return;
            }

            // 缓存transform信息
            NetVector3MoveToVector3(message.OriginalTransform.Position, ref m_position);
            NetVector3MoveToVector3(message.OriginalTransform.Rotation, ref m_rotation);
            m_baseController.AdjustToOriginalTransform(NetActorState,message);

            // 状态切换
            NetActorState = message.State;
            m_baseController.ChangeState(NetActorState);
        }
        public void HandleActorChangeTransformDate(ActorChangeTransformDataResponse message)
        {
            if (!m_isInit)
            {
                Log.Warning("Actor:HandleActorChangeTransformDate m_baseController is null");
                return;
            }

            // 缓存transform信息
            NetVector3MoveToVector3(message.OriginalTransform.Position, ref m_position);
            NetVector3MoveToVector3(message.OriginalTransform.Rotation, ref m_rotation);

            var state = m_baseController.stateMachine.CurState as RemotePlayerState;
            state.SyncTransformData(message);
        }
    }
}

using GameClient.Combat;
using GameClient.InventorySystem;
using GameClient.Manager;
using Google.Protobuf.Collections;
using HS.Protobuf.Combat.Buff;
using HS.Protobuf.Combat.Skill;
using HS.Protobuf.Game.Backpack;
using HS.Protobuf.SceneEntity;
using Player;
using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;

namespace GameClient.Entities
{
    public class Actor:Entity
    {
        private GameObject      m_renderObj;
        private UnitDefine      m_define;                                               
        private NetActorNode    m_netActorNode;                                     
        public BaseController   m_baseController;
        public SkillManager     m_skillManager;
        public BuffManager      m_buffManager;
        public EquipManager     m_equipManager;

        public bool IsDeath => m_netActorNode.NetActorState == NetActorState.Death;
        public int Level => m_netActorNode.Level;
        public long Exp => m_netActorNode.Exp;
        public int Speed { get => m_netActorNode.Speed; set => m_netActorNode.Speed = value; }

        public Actor(NetActorNode netAcotrNode) :base(netAcotrNode.EntityId, netAcotrNode.Transform)
        {
            m_netActorNode = netAcotrNode;
            m_define = DataManager.Instance.unitDefineDict[netAcotrNode.ProfessionId];
            m_skillManager = new();
            m_buffManager = new();
            m_equipManager = new();
            m_skillManager.Init(this, netAcotrNode.EquippedSkills);
            m_buffManager.Init(this, netAcotrNode.Buffs);
            m_equipManager.Init(this, netAcotrNode.WornEquipments);
        }
        public void Init(BaseController baseController)
        {
            this.m_baseController = baseController;
        }
        public override void Update(float deltatime)
        {
            m_skillManager.Update(deltatime);
            m_buffManager.Update(deltatime);
        }


        public virtual void OnModeChanged(ActorMode old_value, ActorMode new_value)
        {
            this.actorMode = new_value;
        }
        public virtual void OnCombatModeChanged(ActorCombatMode old_value, ActorCombatMode new_value)
        {
            this.actorCombatMode = new_value;
        }
        public virtual void recvDamage(Damage damage)
        {
            //受伤，被别人打了，播放一下特效或者ui。不做数值更新

            if (m_renderObj == null) return;
            if (IsDeath) return;
            //ui
            var ownerPos = m_renderObj.transform.position;
            if (damage.IsImmune)
            {
                DynamicTextManager.CreateText(ownerPos, "免疫", DynamicTextManager.missData);
            }
            else if (damage.IsMiss)
            {   //闪避了，显示一下闪避ui
                DynamicTextManager.CreateText(ownerPos, "Miss", DynamicTextManager.missData);
            }
            else{
                //伤害飘字
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

                //暴击做一些处理，震屏..
                if (damage.IsCrit)
                {
                    DynamicTextManager.CreateText(ownerPos, "Crit!", DynamicTextManager.critData);
                }

                //被技能击中的粒子效果
                if (damage.SkillId != 0)
                {
                    var skillDef = DataManager.Instance.skillDefineDict[damage.SkillId];
                    if (skillDef != null)
                    {
                        GameEffectManager.AddEffectTarget(skillDef.HitArt, m_renderObj, new Vector3(0, 1, 0));
                    }
                }


                //被击中的音效


                //切换到挨打的动作
                if(m_baseController.CurState != ActorState.Move)
                {
                    m_baseController.StateMachineParameter.attacker = GameTools.GetActorById(damage.AttackerId);
                    m_baseController.ChangeState(ActorState.Hurt);
                }

            }
        }
        public void OnHpChanged(float oldHp,float newHp)
        {
            //Debug.Log($"current hp = {newHp}");
            this.m_netActorNode.Hp = newHp;
            LocalOrTargetAcotrPropertyChange();
        }
        public void OnMpChanged(float old_value, float new_value)
        {
            this.m_netActorNode.Mp = new_value;
            LocalOrTargetAcotrPropertyChange();
        }
        public virtual void OnDeath()
        {
            //如果当前actor被关注，则需要通知
            if (GameApp.target == this)
            {
                Kaiyun.Event.FireIn("TargetDeath");
            }

        }
        public void OnLevelChanged(int old_value, int new_value)
        {
            //更新当前actor的数据
            this.m_netActorNode.Level = new_value;
            //事件通知，level数据发送变化（可能某些ui组件需要这个信息）
            LocalOrTargetAcotrPropertyChange();
        }
        public void OnHpmaxChanged(float old_value, float new_value)
        {
            this.m_netActorNode.HpMax = new_value;
            LocalOrTargetAcotrPropertyChange();
        }
        public void OnMpmaxChanged(float old_value, float new_value)
        {
            this.m_netActorNode.MpMax = new_value;
            LocalOrTargetAcotrPropertyChange();
        }
        public void OnSpeedChanged(int old_value, int new_value)
        {
            this.Speed = new_value;
        }



        /// <summary>
        /// 本地玩家或者目标玩家的属性发送变化
        /// </summary>
        public void LocalOrTargetAcotrPropertyChange()
        {
            if (this == GameApp.character || this == GameApp.target)
            {
                //CombatPanelScript、这个事件给需要更新本地chr和targetChr的UI用的
                Kaiyun.Event.FireOut("SpecificAcotrPropertyUpdate", this);
            }
        }
    }
}

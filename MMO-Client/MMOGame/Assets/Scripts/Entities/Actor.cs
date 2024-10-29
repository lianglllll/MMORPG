using GameClient.InventorySystem;
using GameClient.Manager;
using Google.Protobuf.Collections;
using Player;
using Proto;
using Serilog;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace GameClient.Entities
{
    public class Actor:Entity
    {
        public ActorMode actorMode;
        public ActorCombatMode actorCombatMode;
        public ActorState actorState;
        public NetActor info;                                                   //网络信息
        public UnitDefine define;                                               //actor默认def
        public SkillManager skillManager;                                       //技能管理
        public ConcurrentDictionary<EquipsType, Equipment> equipsDict = new();  //actor持有的装备
        public ConcurrentDictionary<int, Buff> buffsDict = new();               //actor持有的buff<实例id,buff>
        public GameObject renderObj;                                            //actor中对应的游戏对象
        public BaseController baseController;

        public bool IsDeath => actorMode == ActorMode.Dead;
        public int Level => info.Level;
        public long Exp => info.Exp;
        public int Speed { get => info.Speed; set => info.Speed = value; }

        //初始化相关
        public Actor(NetActor info) :base(info.Entity)
        {
            this.info = info;
            this.define = DataManager.Instance.unitDict[info.Tid];
            this.skillManager = new SkillManager(this);
            this.LoadEquips(info.EquipList);
            this.LoadBuffs(info.BuffsList);
        }
        public override void OnUpdate(float deltatime)
        {
            skillManager.OnUpdate(deltatime);
            BuffUpdate(deltatime);
        }
        public void Init(BaseController baseController)
        {
            this.baseController = baseController;
        }
        public void LoadEquips(RepeatedField<ItemInfo> itemInfos)
        {
            equipsDict.Clear();
            foreach (var itemInfo in itemInfos)
            {
                var item = new Equipment(itemInfo);
                equipsDict[item.EquipsType] = item;
            }
        }
        private void LoadBuffs(RepeatedField<BuffInfo> buffsList)
        {
            foreach (var buffInfo in buffsList)
            {
                new Buff().Init(buffInfo, this);
            }
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

            if (renderObj == null) return;
            if (IsDeath) return;
            //ui
            var ownerPos = renderObj.transform.position;
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
                        GameEffectManager.AddEffectTarget(skillDef.HitArt, renderObj, new Vector3(0, 1, 0));
                    }
                }


                //被击中的音效


                //切换到挨打的动作
                if(baseController.CurState != ActorState.Move)
                {
                    baseController.StateMachineParameter.attacker = GameTools.GetActorById(damage.AttackerId);
                    baseController.ChangeState(ActorState.Hurt);
                }

            }
        }
        public void OnHpChanged(float oldHp,float newHp)
        {
            //Debug.Log($"current hp = {newHp}");
            this.info.Hp = newHp;
            LocalOrTargetAcotrPropertyChange();
        }
        public void OnMpChanged(float old_value, float new_value)
        {
            this.info.Mp = new_value;
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
            this.info.Level = new_value;
            //事件通知，level数据发送变化（可能某些ui组件需要这个信息）
            LocalOrTargetAcotrPropertyChange();
        }
        public void OnHpmaxChanged(float old_value, float new_value)
        {
            this.info.HpMax = new_value;
            LocalOrTargetAcotrPropertyChange();
        }
        public void OnMpmaxChanged(float old_value, float new_value)
        {
            this.info.MpMax = new_value;
            LocalOrTargetAcotrPropertyChange();
        }
        public void OnSpeedChanged(int old_value, int new_value)
        {
            this.Speed = new_value;
        }

        //buff相关，//todo应该用一个buffmananger来管理
        public void AddBuff(Buff buff)
        {
            buffsDict[buff.ID] = buff;
            if(GameApp.character == this || GameApp.target == this)
            {
                Kaiyun.Event.FireOut("SpecialActorAddBuff", buff);
            }
        }
        public void RemoveBuff(int id)
        {
            if(buffsDict.TryRemove(id, out _))
            {
                if (GameApp.character == this || GameApp.target == this)
                {
                    Kaiyun.Event.FireOut("SpecialActorRemoveBuff", this, id);
                }
            }

        }
        private List<int> removeKey = new();
        private void BuffUpdate(float deltatime)
        {
            if (buffsDict.Count <= 0) return;

            Buff temBuf;
            removeKey.Clear();
            foreach (var item in buffsDict)
            {
                temBuf = item.Value;
                temBuf.ResidualDuration -= deltatime;
                //降级
                if(temBuf.ResidualDuration <= 0)
                {
                    --(temBuf.CurrentLevel);
                }
                //删除
                if (temBuf.CurrentLevel <= 0)
                {
                    removeKey.Add(item.Key);
                    continue;
                }

            }

            //删除无效的ui
            foreach (var key in removeKey)
            {
                RemoveBuff(key);
            }

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

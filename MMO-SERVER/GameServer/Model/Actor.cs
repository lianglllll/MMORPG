﻿using Summer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Proto;
using GameServer.Core;
using GameServer.Combat;
using GameServer.Manager;
using Serilog;
using Common.Summer.core;

namespace GameServer.Model
{

    public class Actor : Entity
    {
        public UnitDefine Define;                                                                   //单位的一些静态数据
        public Space currentSpace;                                                                  //当前的场景
        public NetActor info  = new NetActor();                                                     //NetActor 网络对象
        public EntityState State;                                                                   //actor状态：跑、走、跳、死亡、
        public Attributes Attr = new Attributes();                                                  //actor属性
        public SkillManager skillManager;                                                           //actor技能管理器
        public Spell spell;                                                                         //actor技能释放器
        public UnitState unitState;                                                                 //单位状态:死亡、空闲、战斗

        /// <summary>
        /// 判断actor死亡看的是状态而不是hp
        /// </summary>
        public bool IsDeath => unitState == UnitState.Dead;                                         
        /// <summary>
        /// actorId:characterId、monsterId因为它们都在同一个unit配置文件里面
        /// </summary>
        public int Id { 
            get { return info.Id; } 
            set { info.Id = value; } 
        }                                                                           
        /// <summary>
        /// actor的名
        /// </summary>
        public string Name { 
            get { return info.Name; } 
            set { info.Name = value; } 
        }
        /// <summary>
        /// actor的类型：角色、怪物、npc
        /// </summary>
        public EntityType Type { 
            get { return info.EntityType; } 
            set { info.EntityType = value; }
        }                                                                  
        /// <summary>
        /// 场景id
        /// </summary>
        public int SpaceId
        {
            get
            {
                return currentSpace.SpaceId;
            }
        }
        /// <summary>
        /// acotr的Hp
        /// </summary>
        public float Hp { get { return info.Hp; }}
        /// <summary>
        /// actor的Mp
        /// </summary>
        public float Mp { get { return info.Mp; }}

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="type"></param>
        /// <param name="TID"></param>
        /// <param name="level"></param>
        /// <param name="position"></param>
        /// <param name="direction"></param>
        public Actor( EntityType type,int TID,int level,Vector3Int position, Vector3Int direction) : base(position, direction)
        {
            //加载define的默认数据
            this.Define = DataManager.Instance.unitDefineDict[TID];     //todo 可能会空，可能会有人篡改json配置文件
            
            //先更新entity中的speed字段
            this.Speed = this.Define.Speed;

            //再更新NetActor网络对象的信息
            this.info.Tid = TID;                                        //TID:区分相同实体类型，不同身份。可以通过tid去找define
            this.info.EntityType = type;                                //unit类型
            this.info.Entity = this.EntityData;                         //entity的基本数据：pos dir speed entityId
            this.info.Name = Define.Name;                               //defind中的角色默认名字，可以给子类进行覆盖
            this.info.Hp = (int)this.Define.HPMax;                      //hp
            this.info.Mp = (int)this.Define.MPMax;                      //mp
            this.info.Level = level;                                    //level

            //给当前actor添加相对应的组件
            this.skillManager = new SkillManager(this);                 //技能管理器
            this.spell = new Spell(this);                               //技能施法器
            this.Attr.Init(this);                                       //actor属性初始化
        }

        /// <summary>
        /// 推动actor再mmo世界的运行
        /// </summary>
        public override void Update()
        {
            this.skillManager.Update();//驱动技能系统
        }

        /// <summary>
        /// actor进入某个场景，更新自己记录的space信息
        /// </summary>
        /// <param name="space"></param>
        public void OnEnterSpace(Space space)
        {
            this.currentSpace = space;
            this.info.SpaceId = space.SpaceId;
        }

        /// <summary>
        /// actor复活，回血回蓝
        /// </summary>
        public void Revive()
        {
            if (!IsDeath) return;
            SetHp(Attr.final.HPMax);
            SetMP(Attr.final.MPMax);
            SetState(UnitState.Free);
            OnAfterRevive();
        }

        /// <summary>
        /// 复活后处理
        /// </summary>
        /// <param name="killerID"></param>
        protected virtual void OnAfterRevive()
        {

        }

        /// <summary>
        /// 当前actor收到扣血通知
        /// </summary>
        /// <param name="damage"></param>
        public void RecvDamage(Damage damage)
        {
            Log.Information("Actor:RecvDamage[{0}]", damage);
            //添加广播，一个伤害发生了
            currentSpace.fightManager.damageQueue.Enqueue(damage);
            //扣血
            if(Hp > damage.Amount)
            {
                SetHp(Hp - damage.Amount);
            }
            else
            {
                Die(damage.AttackerId);
            }
        }

        /// <summary>
        /// 设置actor的hp，并广播
        /// </summary>
        /// <param name="hp"></param>
        private void SetHp(float hp)
        {

            if (MathC.Equals(info.Hp, hp)) return;
            if(hp <= 0)
            {
                hp = 0;
                this.unitState = UnitState.Dead;
            }
            if (hp > Attr.final.HPMax)
            {
                hp = Attr.final.HPMax;
            }
            float oldValue = info.Hp;
            info.Hp = hp;

            //发包
            PropertyUpdate po = new PropertyUpdate()
            {
                EntityId = EntityId,
                Property = PropertyUpdate.Types.Prop.Hp,
                OldValue = new() { FloatValue = oldValue },
                NewValue = new() { FloatValue = info.Hp },
            };
            currentSpace.fightManager.propertyUpdateQueue.Enqueue(po);
        }

        /// <summary>
        /// 设置actor的mp，并广播
        /// </summary>
        /// <param name="mp"></param>
        private void SetMP(float mp)
        {
            if (MathC.Equals(info.Mp, mp)) return;
            if (mp <= 0)
            {
                mp = 0;
            }
            if (mp > Attr.final.MPMax)
            {
                mp = Attr.final.MPMax;
            }
            float oldValue = info.Mp;
            info.Mp = mp;

            //发包
            PropertyUpdate po = new PropertyUpdate()
            {
                EntityId = EntityId,
                Property = PropertyUpdate.Types.Prop.Mp,
                OldValue = new() { FloatValue = oldValue },
                NewValue = new() { FloatValue = info.Mp },
            };
            currentSpace.fightManager.propertyUpdateQueue.Enqueue(po);
        }

        /// <summary>
        /// 设置actor的unitstate,并广播
        /// </summary>
        /// <param name="unitState"></param>
        private void SetState(UnitState unitState)
        {
            if (this.unitState == unitState) return;
            UnitState oldValue = this.unitState;
            this.unitState = unitState;
            PropertyUpdate po = new PropertyUpdate()
            {
                EntityId = EntityId,
                Property = PropertyUpdate.Types.Prop.State,
                OldValue = new() { StateValue = oldValue },
                NewValue = new() { StateValue = unitState } 
            };
            currentSpace.fightManager.propertyUpdateQueue.Enqueue(po);
        }

        /// <summary>
        /// 当前actor死亡
        /// </summary>
        /// <param name="killerID"></param>
        public virtual void Die(int killerID)
        {
            if (IsDeath) return;
            OnBeforeDie(killerID);
            SetState(UnitState.Dead);
            SetHp(0);
            SetMP(0);
            OnAfterDie(killerID);
        }

        /// <summary>
        /// 死亡前的处理
        /// </summary>
        /// <param name="killerID"></param>
        protected virtual void OnBeforeDie(int killerID)
        {
        }

        /// <summary>
        /// 死亡后的处理
        /// </summary>
        /// <param name="killerID"></param>
        protected virtual void OnAfterDie(int killerID)
        {

        }

        /// <summary>
        /// 传送
        /// </summary>
        public virtual void TransmitSpace(Space targetSpace,Vector3Int pos,Vector3Int dir = new Vector3Int())
        {
            if (this is not Character chr) return;
            //传送的不是同一场景
            if(currentSpace != targetSpace)
            {
                //1.退出当前场景
                currentSpace.CharacterLeave(chr);
                //设置坐标
                chr.Position = pos;
                chr.Direction = dir;
                //2.进入新场景
                targetSpace.CharaterJoin(chr);
            }
            //传送的是同一场景
            else
            {

            }
        }
    }
}

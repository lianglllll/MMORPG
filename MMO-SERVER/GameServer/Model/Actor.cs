using Summer;
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
        public UnitDefine Define;                                                                   //怪物中：山贼，土匪？
        public NetActor info  = new NetActor();                                                     //作为存放网络信息的tmp
        public EntityState State;                                                                   //actor状态：跑、走、跳
        public Space currentSpace;                                                                  //todo,可以修改space的同时去修改info.spaceid
        public Attributes Attr = new Attributes();                                                  //actor属性
        public SkillManager skillManager;                                                           //actor技能管理器
        public Spell spell;                                                                         //actor技能释放器
        public UnitState unitState;                                                                 //单位状态:死亡、空闲、战斗
        public bool IsDeath => unitState == UnitState.Dead;                                         //判断actor死亡看的是状态而不是hp



        public int Id { 
            get { return info.Id; } 
            set { info.Id = value; } 
        }                                                                           //actorId
        public string Name { 
            get { return info.Name; } 
            set { info.Name = value; } 
        }
        public EntityType Type { 
            get { return info.EntityType; } 
            set { info.EntityType = value; }
        }                                                                  //角色，怪物，npc？
        public int SpaceId
        {
            get
            {
                return currentSpace.SpaceId;
            }
        }
        public float Hp { get { return info.Hp; }}
        public float Mp { get { return info.Mp; }}




        //TID:区分相同实体类型，不同身份。可以通过tid去找define
        public Actor( EntityType type,int TID,int level,Vector3Int position, Vector3Int direction) : base(position, direction)
        {
            //加载define的默认数据
            this.Define = DataManager.Instance.unitDefineDict[TID];//todo 可能会空，可能会有人篡改json配置文件
            this.info.Tid = TID;
            this.info.EntityType = type;
            this.info.Entity = this.EntityData;
            this.info.Name = Define.Name;               //默认名，可以给子类进行覆盖
            this.info.Hp = (int)this.Define.HPMax;
            this.info.Mp = (int)this.Define.MPMax;
            this.info.Level = level;
            this.Speed = this.Define.Speed;
            

            this.skillManager = new SkillManager(this);
            this.Attr.Init(this);
            this.spell = new Spell(this);
        }

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
        protected virtual void OnBeforeDie(int killerID)
        {

        }
        protected virtual void OnAfterDie(int killerID)
        {

        }
    }
}

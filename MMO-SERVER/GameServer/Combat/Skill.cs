using GameServer.Core;
using GameServer.Model;
using Proto;
using Serilog;
using Summer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Combat
{
    //技能施法的过程：
    //开始 - 前摇 - 激活 - 结束
    public enum Stage
    {
        None,               //无状态
        Intonate,           //吟唱
        Active,             //已激活
        Colding             //冷却中
    }

    public class Skill
    {
        public SkillDefine Define;          //技能定义
        public Actor Owner;                 //技能归属者
        public float ColdDown;              //冷却倒计时，0表示技能可用
        private float RunTime;              //技能运行时间
        public Stage State;                 //当前技能状态
        public bool IsPassive;              //是否是被动技能

        public Skill(Actor owner,int skid)
        {
            this.Owner = owner;
            Define = DataManager.Instance.skillDefineDict[skid];
        }

        public void Update()
        {
            if (State == Stage.None && ColdDown == 0) return;


            if (ColdDown > 0) ColdDown -= Time.deltaTime;
            if (ColdDown < 0) ColdDown = 0;
            RunTime += Time.deltaTime;

            //如果当前的蓄气状态且蓄气已经达到目标值，就切换到激活状态
            if(State == Stage.Intonate && RunTime >= Define.IntonateTime)
            {
                State = Stage.Active;
                ColdDown = Define.CD;//此时真正进入冷却
                OnActive();
            }


            //active状态达到最大值,进入冷却
            if(State == Stage.Active)
            {
                if(RunTime >= Define.IntonateTime + Define.HitDelay.Max())
                {
                    State = Stage.Colding;
                }
            }

            //冷却
            if(State == Stage.Colding)
            {
                if(ColdDown == 0)
                {
                    RunTime = 0;
                    State = Stage.None;
                    OnFinish();
                }
            }


        }


        public bool IsNoneTarget
        {
            get => Define.TargetType == "None";
        }
        public bool IsUnitTarget
        {
            get => Define.TargetType == "单位";
        }
        public bool IsPointTarget
        {
            get => Define.TargetType == "点";
        }

        //检查技能是否可用
        public CastResult CanUse(SCObject sco)
        {
            //被动技能
            if (IsPassive)
                return CastResult.IsPassive;
            //MP不足
            else if (Owner.info.Mp < Define.Cost)
                return CastResult.MpLack;
            //正在进行
            else if (State != Stage.None)
                return CastResult.Running;
            //冷却中
            else if (ColdDown > 0)
                return CastResult.ColdDown;
            //Entity已经死亡
            else if (Owner.IsDeath)
                return CastResult.EntityDead;
            //目标已死亡
            else if (sco is SCEntity && (sco.RealObj as Actor).IsDeath)
                return CastResult.EntityDead;
            //施法者和目标的距离超过限制
            var dist = Vector3Int.Distance(Owner.EntityData.Position, sco.GetPosition());
            if (dist > Define.SpellRange)
                return CastResult.OutOfRange;

            //可用
            return CastResult.Success;
        }

        //使用技能
        public CastResult Use(SCObject sco)
        {
            RunTime = 0;
            State = Stage.Intonate;
            return CastResult.Success;
        }


        private void OnActive()
        {
            Log.Information("Skill Active Owner[{0}],skill[{1}]", Owner.EntityId,Define.Name);
        }
        private void OnFinish()
        {
            Log.Information("技能结束：Owner[{0}],skill[{1}]",Owner.EntityId, Define.Name);
        }

    }
}

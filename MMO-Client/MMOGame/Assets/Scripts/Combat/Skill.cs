using GameClient.Entities;
using HS.Protobuf.SceneEntity;
using Serilog;
using UnityEngine;

namespace GameClient.Combat
{
    //todo   蓄气技能在蓄气阶段被其他人打死，如果有投射物还是会发出。

    //技能施法的阶段
    //开始 - 蓄气 - 激活 - 结束
    public enum SkillStage
    {
        None,               //无状态
        Intonate,           //吟唱/蓄气
        Active,             //已激活
        Colding             //冷却中
    }

    public class Skill
    {
        public SkillDefine Define;          //技能定义
        public Actor Owner;                 //技能归属者
        public float ColddownTime;          //冷却倒计时，0表示技能可用
        private float RunTime;              //技能运行时间
        public SkillStage Stage;            //当前技能执行到的阶段
        private SCObject _sco;              //技能的目标,Use触发时设置

        public int SkillId => Define.ID;

        /// <summary>
        /// 聚气进度 0-1
        /// </summary>
        public float IntonateProgress => RunTime / Define.IntonateTime;

        /// <summary>
        /// 是否为无目标技能
        /// </summary>
        public bool IsNoneTarget
        {
            get => Define.TargetType == "None";
        }

        //无目标但是有指向，估计要归到无目标技能哪里去，到时候服务器直接用那个角色的方向作为施法方向即可
        //也可以携带方向过去过去哈

        /// <summary>
        /// 是否为单位目标技能
        /// </summary>
        public bool IsUnitTarget
        {
            get => Define.TargetType == "单位";
        }

        /// <summary>
        /// 是否为点目标技能
        /// </summary>
        public bool IsPointTarget
        {
            get => Define.TargetType == "点";
        }

        /// <summary>
        /// 是不是普通攻击
        /// </summary>
        public bool IsNormal => Define.Type == "普通攻击";
        /// <summary>
        /// 是否是被动技能
        /// </summary>
        public bool IsPassive => Define.Type == "被动技能";

        /// <summary>
        /// 是不是主动技能
        /// </summary>
        public bool IsActiveSkill =>  !(IsPassive || IsNormal);

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="skid"></param>
        public Skill(Actor owner, int skid)
        {
            this.Owner = owner;
            Define = DataManager.Instance.skillDefineDict[skid];
        }

        /// <summary>
        /// SkillManager调用，推动技能的阶段
        /// </summary>
        /// <param name="deltatime"></param>
        public void OnUpdate(float deltatime)
        {
            //1.技能从被使用那一阶段开始推动
            if (Stage == SkillStage.None && ColddownTime == 0) return;

            //2.记录技能的运行时间
            RunTime += deltatime;

            //3.如果当前的蓄气状态且蓄气已经达到目标值，就切换到激活状态
            if (Stage == SkillStage.Intonate && RunTime >= Define.IntonateTime)
            {
                Stage = SkillStage.Active;
                OnActive();
            }

            //4.active状态达到最大值,进入冷却
            if (Stage == SkillStage.Active)
            {
                //技能执行结束
                if (RunTime >= Define.IntonateTime + Define.Duration)
                {
                    Stage = SkillStage.Colding;

                    OnColdDown();

                }
            }

            //5.技能处于激活状态时，冷却就开始倒计时了
            if (Stage == SkillStage.Colding)
            {
                if (ColddownTime > 0) ColddownTime -= Time.deltaTime;
                if (ColddownTime <= 0)
                {
                    ColddownTime = 0;
                    RunTime = 0;
                    Stage = SkillStage.None;
                    OnFinish();
                }
            }

        }

        /// <summary>
        /// 使用技能
        /// </summary>
        /// <param name="target"></param>
        public void Use(SCObject target)
        {
            //只有本机玩家才会用到这个值
            if (Owner.EntityId == GameApp.character.EntityId)
            {
                GameApp.CurrSkill = this;
            }

            if (Owner.m_baseController.StateMachineParameter.curSkill == null)
            {
                Owner.m_baseController.StateMachineParameter.curSkill = this;
            }
            else
            {
                //有东西要中断当前技能active


                Log.Error("当前状态机skill参数不为空！！");
                return;
            }

            _sco = target;
            RunTime = 0;

            if (Define.IntonateTime > 0)
            {
                //技能阶段从none切换到蓄气阶段
                Stage = SkillStage.Intonate;
                OnIntonate();
            }
            else
            {
                Stage = SkillStage.Active;
                OnActive();
            }
            Owner.m_baseController.ChangeState(ActorState.Skill);
        }

        /// <summary>
        /// 蓄气阶段，让游戏对象切换动画状态
        /// </summary>
        public void OnIntonate()
        {

            UnityMainThreadDispatcher.Instance().Enqueue(() =>
            {
                //蓄气转向,
                if (_sco is SCEntity)
                {
                    var target = _sco.RealObj as Actor;
                    if (target != Owner)
                    {
                        Owner.m_baseController.DirectLookTarget(target.RenderObj.transform.position);
                    }
                }
            });

        }

        /// <summary>
        /// 技能处于激活阶段
        /// </summary>
        private void OnActive()
        {
            ColddownTime = Define.CD;//此时真正进入冷却

            //如果有飞行物,就生成飞行物
            if (Define.IsMissile)
            {
                var target = _sco.RealObj as Actor;
                GameObject myObjcet = new GameObject("MyMissile");
                var missile = myObjcet.AddComponent<Missile>();
                missile.Init(this, Owner.RenderObj.transform.position, target.RenderObj);
            }

        }

        /// <summary>
        /// 冷却阶段
        /// </summary>
        private void OnColdDown()
        {
            //需要刷新一些个ui
            if (Owner == GameApp.character)
            {
                GameApp.CurrSkill = null;
                Kaiyun.Event.FireIn("SkillEnterColdDown");
                Kaiyun.Event.FireIn("SkillActiveEnd");
            }
        }

        /// <summary>
        /// 技能完成了它的生命周期
        /// </summary>
        private void OnFinish()
        {
            //Log.Information("技能结束：Owner[{0}],skill[{1}]", Owner.EntityId, Define.Name);
        }

        /// <summary>
        /// 获取描述文本
        /// </summary>
        /// <returns></returns>
        public virtual string GetDescText()
        {
            var content = $"<color=#ffffff>{this.Define.Name}</color>\n" +
                          $"<color=yellow>{this.Define.Description}</color>\n\n" +
                          $"<color=bulue>技能冷却时间：{this.Define.CD}</color>";
            return content;
        }
    }
}

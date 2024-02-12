using GameClient.Manager;
using Proto;
using Serilog;
using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace GameClient.Entities
{
    public class Actor:Entity
    {

        public NetActor info;
        public UnitDefine define;
        public SkillManager skillManager;
        public GameObject renderObj;        //actor中对应的游戏对象
        public UnitState unitState;
        public PlayerStateMachine StateMachine;
        public bool IsDeath => unitState == UnitState.Dead;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="info"></param>
        public Actor(NetActor info) :base(info.Entity)
        {
            this.info = info;
            this.define = DataManager.Instance.unitDict[info.Tid];
            this.skillManager = new SkillManager(this);
        }

        



        /// <summary>
        /// 受伤了，改一下ui，播放一下动画
        /// </summary>
        /// <param name="damage"></param>
        public void recvDamage(Damage damage)
        {
            //ui
            var _textPos = renderObj.transform.position;

            //闪避了，显示一下闪避ui
            if (damage.IsMiss)
            {
                DynamicTextManager.CreateText(_textPos, "Miss", DynamicTextManager.missData);
            }
            else{
                //伤害飘字
                DynamicTextManager.CreateText(_textPos, damage.Amount.ToString("0"));
                if (damage.IsCrit)
                {
                    //暴击做一些处理，震屏..
                    DynamicTextManager.CreateText(_textPos, "Crit!", DynamicTextManager.critData);
                }
            }


            var attacker = GameTools.GetUnit(damage.AttackerId);
            var skill = attacker.skillManager.GetSkill(damage.SkillId);
            //这个skill的hit特效
            var ps = Resources.Load<ParticleSystem>(skill.Define.HitArt);
            if(ps != null)
            {
                var pos = renderObj.transform.position;
                var dir = renderObj.transform.rotation;
                ParticleSystem psObj = GameObject.Instantiate(ps, pos, dir);
                GameObject.Destroy(psObj.gameObject, psObj.main.duration);
            }
            else
            {
                Log.Error("Failed to load particle system");
            }


        }

        /// <summary>
        /// 更新当前actor的hp
        /// </summary>
        /// <param name="oldHp"></param>
        /// <param name="newHp"></param>
        public void OnHpChanged(float oldHp,float newHp)
        {
            this.info.Hp = newHp;
        }

        /// <summary>
        /// 更新当前actor的mp
        /// </summary>
        /// <param name="old_value"></param>
        /// <param name="new_value"></param>
        public void OnMpChanged(float old_value, float new_value)
        {
            this.info.Mp = new_value;
        }

        /// <summary>
        /// 状态更改
        /// </summary>
        /// <param name="old_value"></param>
        /// <param name="new_value"></param>
        public virtual void OnStateChanged(UnitState old_value, UnitState new_value)
        {
            this.unitState = new_value;
        }

    }
}

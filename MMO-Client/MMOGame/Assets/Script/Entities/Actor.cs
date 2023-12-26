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
        public bool IsDeath => unitState == UnitState.Dead;




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
        /// 设置当前actor的hp
        /// </summary>
        /// <param name="oldHp"></param>
        /// <param name="newHp"></param>
        public void OnHpChanged(float oldHp,float newHp)
        {
            Debug.Log("hp-change");
            this.info.Hp = newHp;
        }


        public void OnMpChanged(float old_value, float new_value)
        {
            Debug.Log("mp-change");
            this.info.Mp = new_value;
        }


        public void OnStateChanged(UnitState old_value, UnitState new_value)
        {
            Debug.Log("状态-change");
            this.unitState = new_value;
            if (IsDeath)
            {
                if (renderObj == null) return;
                var ani = renderObj.GetComponent<HeroAnimations>();
                ani.PlayDie();
                GameTimerManager.Instance.TryUseOneTimer(3f, _HideElement);
            }
            else
            {
                renderObj?.SetActive(true);
            }

        }

        /// <summary>
        /// 隐藏当前有限对象
        /// </summary>
        /// <returns></returns>
        public void  _HideElement()
        {
            //如果单位死亡，将其隐藏
            //这里判断是防止在死亡的3秒内本actor复活了
            if (IsDeath) {
                renderObj?.SetActive(false);
            }
        }


    }
}

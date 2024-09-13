using BaseSystem.MyDelayedTaskScheduler;
using GameClient.Entities;
using Proto;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Script.Entities
{
    public class Monster : Actor
    {
        public Monster(NetActor nCharacter) : base(nCharacter)
        {

        }


        public override void OnStateChanged(UnitState old_value, UnitState new_value)
        {
            base.OnStateChanged(old_value, new_value);

            if(old_value == UnitState.Dead)
            {
                OnRevive();
            }
        }

        public override void OnDeath()
        {
            base.OnDeath();
            if (renderObj == null) return;

            //隐藏怪物实体
            DelayedTaskScheduler.Instance.AddDelayedTask(3f, () =>
            {
                //如果单位死亡，将其隐藏
                //这里判断是防止在死亡的3秒内本actor复活了
                if (IsDeath)
                {
                    renderObj?.SetActive(false);
                }
            });

            /*
            GameTimerManager.Instance.TryUseOneTimer(3f, () => {
                //如果单位死亡，将其隐藏
                //这里判断是防止在死亡的3秒内本actor复活了
                if (IsDeath)
                {
                    renderObj?.SetActive(false);
                }
            });
            */
        }


        /// <summary>
        /// 复活
        /// </summary>
        public void OnRevive()
        {
            renderObj?.SetActive(true);

        }

    }
}

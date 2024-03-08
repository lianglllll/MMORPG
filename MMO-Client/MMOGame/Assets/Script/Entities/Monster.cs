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

            //处理死亡逻辑
            if (IsDeath)
            {
                Log.Information("1.死亡状态更新");
                if (renderObj == null) return;
                StateMachine.SwitchState(EntityState.Death);

                GameTimerManager.Instance.TryUseOneTimer(5f, ()=> {
                    //如果单位死亡，将其隐藏
                    //这里判断是防止在死亡的3秒内本actor复活了
                    if (IsDeath)
                    {
                        renderObj?.SetActive(false);
                    }
                });

            }
            else if(old_value == UnitState.Dead)
            {
                renderObj?.SetActive(true);
                StateMachine.SwitchState(EntityState.Idle);
            }


        }


    }
}

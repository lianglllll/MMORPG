using GameClient.Entities;
using Proto;
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
            if(IsDeath)
                GameTimerManager.Instance.TryUseOneTimer(3f, _HideElement);
        }


        /// <summary>
        /// 隐藏当前有限对象
        /// </summary>
        /// <returns></returns>
        public void _HideElement()
        {
            //如果单位死亡，将其隐藏
            //这里判断是防止在死亡的3秒内本actor复活了
            if (IsDeath)
            {
                renderObj?.SetActive(false);
            }
        }
    }
}

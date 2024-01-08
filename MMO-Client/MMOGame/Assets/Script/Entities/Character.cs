using GameClient.Entities;
using Proto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Script.Entities
{
    public class Character:Actor
    {
        public Character(NetActor nCharacter) : base(nCharacter)
        {

        }

        /// <summary>
        /// 角色的状态发生变化
        /// </summary>
        /// <param name="old_value"></param>
        /// <param name="new_value"></param>
        public override void OnStateChanged(UnitState old_value, UnitState new_value)
        {
            base.OnStateChanged(old_value, new_value);
            //本机玩家死亡要显示消息面板
            if (IsDeath && GameApp.character == this)
            {
                GameApp.combatPanelScript.ShowDeathBox();
            }
        }

        /// <summary>
        /// 发送玩家复活请求
        /// </summary>
        public void _Revive()
        {
            ReviveRequest req = new ReviveRequest();
            req.EntityId = EntityId;
            NetClient.Send(req);
        }

    }
}

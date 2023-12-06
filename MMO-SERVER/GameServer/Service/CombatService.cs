using GameServer.core;
using GameServer.Model;
using Proto;
using Serilog;
using Summer;
using Summer.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Service
{
    public class CombatService : Singleton<CombatService>
    {



        public void Start()
        {
            MessageRouter.Instance.Subscribe<SpellCastRequest>(_SpellCastRequest);
        }


        private void _SpellCastRequest(Connection conn, SpellCastRequest message)
        {
            Log.Information("技能施法请求：{0}", message);
            //判断技能施法是否成功
            Session session  = conn.Get<Session>();
            Character chr = session.character;
            if(chr.EntityId != message.Info.CasterId)
            {
                Log.Error("施法者ID错误");
                return;
            }
            //成功，则将其放入当前场景的战斗管理器的缓冲队列中
            chr.currentSpace.fightManager.castInfoQueue.Enqueue(message.Info);
        }

    }
}

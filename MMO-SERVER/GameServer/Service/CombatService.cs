using GameServer.core;
using GameServer.Manager;
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
            MessageRouter.Instance.Subscribe<ReviveRequest>(_ReviveRequest);

        }


        /// <summary>
        /// 复活请求
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="message"></param>
        private void _ReviveRequest(Connection conn, ReviveRequest message)
        {
            var actor = EntityManager.Instance.GetEntity(message.EntityId);
            if(actor != null && actor is Character chr&& chr.IsDeath&& chr.conn == conn)
            {
                //设置当前角色的位置
                chr.Position = new Core.Vector3Int(283000, 4000, 185000);
                chr.Revive();
                //给客户端广播发送更新位置的数据
                SpaceEntitySyncResponse resp = new SpaceEntitySyncResponse();
                NEntitySync nEntitySync = new NEntitySync();
                nEntitySync.Entity = conn.Get<Session>().character.EntityData;
                nEntitySync.State = EntityState.Idle;
                resp.EntitySync = nEntitySync;
                conn.Send(resp);
            }
        }


        /// <summary>
        /// 施法技能请求
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="message"></param>
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

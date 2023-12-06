using GameServer.Manager;
using GameServer.Model;
using Proto;
using Serilog;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Combat
{
    /// <summary>
    /// 战斗管理器
    /// </summary>
    public class FightManager
    {
        private Space space;

        //技能施法队列
        //等到下一帧开始运行的时候再处理
        public ConcurrentQueue<CastInfo> castInfoQueue = new ConcurrentQueue<CastInfo>();

        //技能的等待广播队列
        public ConcurrentQueue<CastInfo> spellQueue = new ConcurrentQueue<CastInfo>();

        //施法信息响应包,每帧发送一次
        private SpellCastResponse spellCastResponse = new SpellCastResponse();


        public void OnUpdate(float deltaTime)
        {
            while(castInfoQueue.TryDequeue(out var cast))
            {
                Log.Information("执行施法：{0}", cast);
                RunCast(cast);
            }
            BroadcastSpellInfo();
        }

        //广播施法信息
        private void BroadcastSpellInfo()
        {
            while(spellQueue.TryDequeue(out var item))
            {
                spellCastResponse.List.Add(item);
            }

            if(spellCastResponse.List.Count() == 0)
            {
                return;
            }
            space.Broadcast(spellCastResponse);
            spellCastResponse.List.Clear();
        }

        public void Init(Space space)
        {
            this.space = space;
        }


        private void RunCast(CastInfo cast)
        {
            //1.判断施法者是否存在
            var caster = EntityManager.Instance.GetEntity(cast.CasterId) as Actor;
            if(caster == null)
            {
                Log.Error("RunCast: Caster is null {0}", cast.CasterId);
                return;
            }
            //2.施法技能
            caster.spell.RunCast(cast);

        }




    }
}

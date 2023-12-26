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


        //等待处理的施法请求队列,等到下一帧开始运行的时候再处理
        public ConcurrentQueue<CastInfo> castInfoQueue = new ConcurrentQueue<CastInfo>();

        //等待广播的技能施法队列
        public ConcurrentQueue<CastInfo> spellQueue = new ConcurrentQueue<CastInfo>();
        //等待广播的施法信息==响应包
        private SpellCastResponse spellCastResponse = new SpellCastResponse();

        //当前场景下的投射物列表
        public List<Missile> missiles = new List<Missile>();

        //等待广播的伤害队列
        public ConcurrentQueue<Damage> damageQueue = new ConcurrentQueue<Damage>();
        private DamageResponse damageResponse = new DamageResponse();
        
        //等待广播：人物属性更新的队列
        public ConcurrentQueue<PropertyUpdate> propertyUpdateQueue = new ConcurrentQueue<PropertyUpdate>();
        private PropertyUpdateRsponse propertyUpdateRsponse = new PropertyUpdateRsponse();
        

        public void Init(Space space)
        {
            this.space = space;
        }

        public void OnUpdate(float deltaTime)
        {
            while(castInfoQueue.TryDequeue(out var cast))
            {
                Log.Information("执行施法：{0}", cast);
                RunCast(cast);
            }

            BroadcastSpellInfo();
            BroadcastDamage();
            BroadcastProperties();

            for (int i = 0; i < missiles.Count; i++)
            {
                missiles[i].OnUpdate(deltaTime);
            }


        }

        //广播施法信息
        private void BroadcastSpellInfo()
        {
            while(spellQueue.TryDequeue(out var item))
            {
                spellCastResponse.List.Add(item);
            }

            if(spellCastResponse.List.Count() > 0)
            {
                space.Broadcast(spellCastResponse);
                spellCastResponse.List.Clear();
            }

        }

        //广播伤害信息
        private void BroadcastDamage()
        {
            while(damageQueue.TryDequeue(out var item))
            {
                damageResponse.List.Add(item);
            }
            if(damageResponse.List.Count > 0)
            {
                space.Broadcast(damageResponse);
                damageResponse.List.Clear();
            }

        }

        //广播人物某个属性
        private void BroadcastProperties()
        {
            while (propertyUpdateQueue.TryDequeue(out var item))
            {
                propertyUpdateRsponse.List.Add(item);
            }
            if (propertyUpdateRsponse.List.Count > 0)
            {
                space.Broadcast(propertyUpdateRsponse);
                propertyUpdateRsponse.List.Clear();
            }

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

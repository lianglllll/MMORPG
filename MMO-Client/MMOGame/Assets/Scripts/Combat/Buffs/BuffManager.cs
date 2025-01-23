using GameClient.Entities;
using Google.Protobuf.Collections;
using HS.Protobuf.Combat.Buff;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace GameClient.Combat.Buffs
{
    public class BuffManager
    {
        private Actor m_owner;
        public ConcurrentDictionary<int, Buff> m_buffsDict = new();               //<InstanceId, buff>
        private List<int> m_removeKey = new();

        public bool Init(Actor owner, RepeatedField<BuffInfo> buffsList)
        {
            m_owner = owner;

            foreach (var buffInfo in buffsList)
            {
                var buff = new Buff();
                buff.Init(buffInfo, m_owner);
                AddBuff(buff);
            }
            return true;
        }
        public void Update(float deltatime)
        {
            if (m_buffsDict.Count <= 0) return;

            // 驱动buff,并且找出要消失的buff的key
            Buff temBuf;
            m_removeKey.Clear();
            foreach (var item in m_buffsDict)
            {
                temBuf = item.Value;
                temBuf.RemainingTime -= deltatime;
                //降级
                if (temBuf.RemainingTime <= 0)
                {
                    temBuf.CurLevel -= 1;
                }
                //删除
                if (temBuf.CurLevel <= 0)
                {
                    m_removeKey.Add(item.Key);
                    continue;
                }

            }

            // 删除过期的buff
            foreach (var key in m_removeKey)
            {
                RemoveBuff(key);
            }

        }

        public void AddBuff(Buff buff)
        {
            m_buffsDict[buff.InstanceId] = buff;
            if (GameApp.character == m_owner || GameApp.target == m_owner)
            {
                Kaiyun.Event.FireOut("SpecialActorAddBuff", buff);
            }
        }
        public void RemoveBuff(int id)
        {
            if (m_buffsDict.TryRemove(id, out _))
            {
                if (GameApp.character == m_owner || GameApp.target == m_owner)
                {
                    Kaiyun.Event.FireOut("SpecialActorRemoveBuff", m_owner, id);
                }
            }
        }
        public List<Buff> GetBuffs()
        {
            return m_buffsDict.Values.ToList();
        } 
    }
}

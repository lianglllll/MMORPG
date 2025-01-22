
using GameClient.Entities;
using Google.Protobuf.Collections;
using HS.Protobuf.Combat.Buff;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace GameClient.Combat
{
    public class BuffManager
    {
        private Actor m_owner;
        public ConcurrentDictionary<int, Buff> m_buffsDict = new();               //actor持有的buff<实例id,buff>
        private List<int> removeKey = new();

        public bool Init(Actor owner, RepeatedField<BuffInfo> buffsList)
        {
            m_owner = owner;

            foreach (var buffInfo in buffsList)
            {
                new Buff().Init(buffInfo, m_owner);
            }


            return true;
        }
        public void Update(float deltatime)
        {
            if (m_buffsDict.Count <= 0) return;

            Buff temBuf;
            removeKey.Clear();
            foreach (var item in m_buffsDict)
            {
                temBuf = item.Value;
                temBuf.ResidualDuration -= deltatime;
                //降级
                if (temBuf.ResidualDuration <= 0)
                {
                    --(temBuf.CurrentLevel);
                }
                //删除
                if (temBuf.CurrentLevel <= 0)
                {
                    removeKey.Add(item.Key);
                    continue;
                }

            }

            //删除无效的ui
            foreach (var key in removeKey)
            {
                RemoveBuff(key);
            }

        }

        public void AddBuff(Buff buff)
        {
            m_buffsDict[buff.ID] = buff;
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
    }
}

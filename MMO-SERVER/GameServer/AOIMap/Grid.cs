
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using GameServer.Model;

namespace AOIMap
{
    /// <summary>
    /// ����
    /// </summary>
    public class Grid
    {
        public int GID { get; private set; }
        private HashSet<IAOIUnit> aoiSet;           //������Ҫͬ���ĵ�λ����ɫ�����npc����Ʒ
        private ReaderWriterLockSlim pIDLock;       //��д��

        public Grid(int id)
        {
            GID = id;
            aoiSet = new();
            pIDLock = new ();
        }

        /// <summary>
        /// �����Ҫͬ���ĵ�λ
        /// </summary>
        /// <param name="obj"></param>
        public void Add(IAOIUnit obj)
        {
            pIDLock.EnterReadLock();
            try
            {
                if (!aoiSet.Contains(obj))
                {
                    aoiSet.Add(obj);
                }
            }
            finally
            {
                pIDLock.ExitReadLock();
            }
            
        }

        /// <summary>
        /// �Ƴ���Ҫͬ���ĵ�λ
        /// </summary>
        /// <param name="obj"></param>
        public void Remove(IAOIUnit obj)
        {
            pIDLock.EnterReadLock();
            try
            {
                aoiSet.Remove(obj);
            }
            finally
            {
                pIDLock.ExitReadLock();
            }
            
        }

        /// <summary>
        /// ��ȡ��ǰ������ȫ����ͬ����λ
        /// </summary>
        /// <returns></returns>
        public List<IAOIUnit> GetEntities()
        {
            pIDLock.EnterReadLock();
            try
            {
                return aoiSet.ToList();
            }
            finally
            {
                pIDLock.ExitReadLock();
            }
        }

    }
}

﻿using Common.Summer.Tools;
using HS.Protobuf.Combat.Buff;
using SceneServer.Core.Model.Actor;

namespace GameServer.Buffs
{
    /// <summary>
    /// 每个角色都有自己的Buff管理器
    /// 更新buff的逻辑
    /// </summary>
    public class BuffManager
    {
        //用于分配全局唯一的id
        private static IdGenerator _idGenerator = new IdGenerator();

        public SceneActor Owner { get; protected set; }
        public List<BuffBase> buffs { get; protected set; }
        public Action<BuffBase> Observer;

        public bool Init(SceneActor Owner)
        {
            this.Owner = Owner;
            buffs = new();
            return true;
        }

        /// <summary>
        /// 推动buff运行
        /// </summary>
        /// <param name="delta"></param>
        public void Update(float delta)
        {
            //所有Buff执行Update
            foreach (BuffBase item in buffs)
            {
                if (item.CurrentLevel > 0 && item.Owner != null)
                {
                    item.Update(delta);
                }
            }

            //降低持续时间,清理无用buff
            //这里从尾部开始，避免破坏list顺序
            for (int i = buffs.Count - 1; i >= 0; i--)
            {
                var buff = buffs[i];
                //降低持续时间
                buff.ResidualDuration -= delta;


                //如果持续时间为0，则降级,
                //降级后如果等级为0则移除，否则刷新持续时间
                if (buff.ResidualDuration <= 0)
                {
                    buff.CurrentLevel -= buff.Demotion;
                    if (buff.CurrentLevel <= 0)
                    {
                        RemoveBuff(buff);
                        continue;
                    }
                    else
                    {
                        buff.ResidualDuration = buff.MaxDuration;
                    }
                }


            }

        }

        /// <summary>
        /// 添加一个buff
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="provider">提供者</param>
        /// <param name="level"></param>
        public void AddBuff<T>(SceneActor provider, int level = 1) where T : BuffBase, new()
        {
            //遍历看看目标身上有没有已存在的要挂的buff。
            List<T> temp01 = buffs.OfType<T>().ToList();

            //如果没有直接挂一个新buff就行了
            //如果有已存在的要挂的buff，就要进行冲突处理了
            if (temp01.Count == 0)
            {
                AddNewBuff<T>(provider, level);
            }
            else
            {
                switch (temp01[0].BuffConflict)
                {
                    //如果是独立存在，那也直接挂buff
                    case BuffConflict.Separate:
                        bool temp = true;
                        foreach (T item in temp01)
                        {
                            if (item.Provider == provider)
                            {
                                item.CurrentLevel += level;
                                temp = false;
                                continue;
                            }
                        }
                        if (temp)
                        {
                            AddNewBuff<T>(provider, level);
                        }
                        break;
                    //如果是合并，则跟已有的buff叠层。
                    case BuffConflict.Combine:
                        temp01[0].CurrentLevel += level;
                        break;
                    //如果是覆盖，则移除旧buff，然后添加这个buff。
                    case BuffConflict.Cover:
                        RemoveBuff(temp01[0]);
                        AddNewBuff<T>(provider, level);
                        break;
                }
            }

        }
        private void AddNewBuff<T>(SceneActor provider, int level) where T : BuffBase, new()
        {
            T buff = new T();
            buff.ID = _idGenerator.GetId();
            buff.Init(Owner, provider);
            buffs.Add(buff);
            buff.ResidualDuration = buff.MaxDuration;
            buff.CurrentLevel = level;
            buff.OnGet();
            Observer?.Invoke(buff);

            BuffsChangePostProcessing();

            //广播通知客户端
            var resp = new BuffsAddResponse();
            resp.List.Add(buff.Info);
            // Owner?.currentSpace?.AOIBroadcast(Owner, resp,true);

        }

        /// <summary>
        /// 移除单位身上指定的一个buff
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="buff"></param>
        /// <returns>是否成功，如果失败说明目标不存在</returns>
        public bool RemoveBuff(BuffBase buff)
        {
            //Log.Information("移除的buff" + buff.Name);
            var item = buffs.FirstOrDefault(x => x == buff);
            if (item != null)
            {
                item.CurrentLevel -= item.CurrentLevel;
                item.OnLost();
                buffs.Remove(item);
                _idGenerator.ReturnId(item.ID);

                BuffsChangePostProcessing();

                //广播通知客户端
                var resp = new BuffsRemoveResponse();
                resp.List.Add(item.Info);
                // Owner?.currentSpace?.AOIBroadcast(Owner, resp, true);

                return true;
            }
            return false;
        }
        public void RemoveAllBuff()
        {
            int len = buffs.Count;
            var resp = new BuffsRemoveResponse();
            for (int i = 0;i < len; ++i) {
                var item = buffs[i];
                item.OnLost();
                _idGenerator.ReturnId(item.ID);
                resp.List.Add(item.Info);
            }
            buffs.Clear();
            // Owner?.currentSpace?.AOIBroadcast(Owner, resp, true);
        }

        /// <summary>
        /// 查询身上是否有指定类型的buff存在
        /// </summary>
        /// <param name="bid"></param>
        /// <returns></returns>
        public bool HasBuffByBid(int bid)
        {
            return buffs.Any(x=>x.BID == bid);
        }

        /// <summary>
        /// 获得单位身上指定类型的buff的列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Owner"></param>
        /// <returns></returns>
        public List<T> GetBuffs<T>() where T : BuffBase, new()
        {
            List<T> result = buffs.OfType<T>().ToList();
            return result;
        }

        /// <summary>
        /// bufflist变更的后处理
        /// </summary>
        private void BuffsChangePostProcessing()
        {
            //更新actor网络对象上面的buff信息
            //Owner.Info.BuffsList.Clear();
            //foreach (BuffBase item in buffs)
            //{
            //    Owner.Info.BuffsList.Add(item.Info);
            //}
        }
    }
}

using GameClient.Entities;
using HS.Protobuf.Combat.Buff;
using System;
using System.Collections.Generic;

namespace GameClient.Combat.Buffs{
    public class Buff
    {
        private BuffInfo m_buffInfo;
        private BuffDefine m_buffDefine;

        private BuffType m_BuffType;
        private BuffConflict m_BuffConflict = BuffConflict.Cover;

        public Actor m_owner;
        public Actor m_provider;

        public Actor Owner => m_owner;
        public Actor Provider => m_provider;
        public int CurLevel {
            get
            {
                return m_buffInfo.CurrentLevel;
            }
            set
            {
                m_buffInfo.CurrentLevel = value;
            }
        }
        public float RemainingTime
        {
            get
            {
                return m_buffInfo.ResidualDuration;
            }
            set
            {
                m_buffInfo.ResidualDuration = value;
            }
        } 
        public int InstanceId => m_buffInfo.Id;

        public float MaxDuration => m_buffDefine.MaxDuration;
        public int MaxLevel => m_buffDefine.MaxLevel;
        public int Demotion => m_buffDefine.Demotion;
        public bool Dispellable => m_buffDefine.Dispellable;
        public string IconPath => m_buffDefine.IconPath;

        public void Init(BuffInfo info, Actor actor = null)
        {
            m_buffInfo = info;

            if (actor != null)
            {
                m_owner = actor;
            }
            else
            {
                m_owner = EntityManager.Instance.GetEntity<Actor>(info.OwnerId);
            }
            m_provider = EntityManager.Instance.GetEntity<Actor>(info.ProviderId);

            m_buffDefine = LocalDataManager.Instance.m_buffDefineDict.GetValueOrDefault(info.Bid, null);
            if (m_buffDefine != null)
            {
                m_BuffType = m_buffDefine.BuffType switch
                {
                    "正增益" => BuffType.Buff,
                    "负增益" => BuffType.Debuff,
                    _ => BuffType.Buff,
                };

                /// 当两个不同单位向同一个单位施加同一个buff时的冲突处理
                /// 分为三种：
                /// combine,合并为一个buff，叠层（提高等级）
                /// separate,独立存在
                /// cover, 覆盖，后者覆盖前者
                m_BuffConflict = m_buffDefine.BuffConflict switch
                {
                    "合并" => BuffConflict.Combine,
                    "独立" => BuffConflict.Separate,
                    "覆盖" => BuffConflict.Cover,
                    _ => throw new Exception("Buff BuffConflict Not Found ：" + m_buffDefine.BuffConflict),
                };

            }
        }
        public void UpdataInfo(BuffInfo buffInfo)
        {
            m_buffInfo = buffInfo;
        }

        public virtual void OnGet() { }
        public virtual void OnLost() { }
        public virtual void OnUpdate(float delta) { }
        protected virtual void OnLevelChange(int change) { }
    }

}

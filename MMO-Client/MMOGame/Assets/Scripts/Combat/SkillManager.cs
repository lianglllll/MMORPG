using GameClient.Combat;
using GameClient.Entities;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace GameClient.Manager
{

    /// <summary>
    /// 技能管理器，每一个Actor都有独立的技能管理器
    /// </summary>
    public class SkillManager
    {
        private Actor m_owner;        
        public List<Skill> Skills = new List<Skill>();
        
        public void Init(Actor owner, Google.Protobuf.Collections.RepeatedField<int> equippedSkills)
        {
            m_owner = owner;
            //初始化技能信息，处理服务器传送过来的
            foreach (var skillId in equippedSkills)
            {
                var skill = new Skill(owner, skillId);
                Skills.Add(skill);
            }
        }
        public void Update(float deltatime)
        {
            foreach(Skill skill in Skills)
            {
                skill.OnUpdate(deltatime);
            }
        }

        public Skill GetSkillBySkillId(int skillId)
        {
            return Skills.FirstOrDefault(s => s.Define.ID == skillId);
        }
        public List<Skill> GetCommonSkills()
        {
            return Skills.Where(skill => skill.IsNormal).ToList();
        }
        public List<Skill> GetActiveSkills()
        {
            return Skills.Where(skill => skill.IsActiveSkill).ToList();
        }

    }
}

using GameClient.Combat;
using GameClient.Entities;
using Google.Protobuf.Collections;
using HS.Protobuf.Combat.Skill;
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
        // public List<Skill> Skills = new();
        private Dictionary<int, Skill> skillDict = new();       // <skillId, Skill>
        private Dictionary<int, Skill> fixedSkillDict = new();  // <pos, Skill>
        
        public void Init(Actor owner, RepeatedField<FixedSkillInfo> skills)
        {
            m_owner = owner;

            AddSkillArsenal(m_owner.UnitDefine.weaponSkillArsenalId);

            // 初始化固定技能组技能信息，处理服务器传送过来的
            if(skills != null)
            {
                foreach (var item in skills)
                {
                    var skill = new Skill(owner, item.SkillId);
                    skillDict.Add(skill.SkillId, skill);
                    fixedSkillDict.Add(item.Pos, skill);
                }
            }
        }
        public void Update(float deltatime)
        {
            foreach(Skill skill in skillDict.Values)
            {
                skill.OnUpdate(deltatime);
            }
        }

        public Skill GetSkillBySkillId(int skillId)
        {
            if(skillDict.TryGetValue(skillId, out var skill))
            {
                return skill;
            }
            return null;
        }
        public Dictionary<int, Skill> GetFixedSkills()
        {
            return fixedSkillDict;
        }
        public void AddSkillArsenal(int weaponSkillArsenalId)
        {
            var tmpDef = LocalDataManager.Instance.
                            WeaponSkillArsenalDefineDict[weaponSkillArsenalId];
            var baseSkillIds = tmpDef.SkillIds;
            foreach (var skillId in baseSkillIds)
            {
                var skill = new Skill(m_owner, skillId);
                skillDict.Add(skill.SkillId, skill);
            }
        }
        public void AddSkill(Skill skill)
        {
            if(skill == null){
                goto End;
            }
            skillDict.Add(skill.SkillId, skill);
        End:
            return;
        }
    }
}

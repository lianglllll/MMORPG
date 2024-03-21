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
        private Actor owner;        //归属者
        public List<Skill> Skills = new List<Skill>();
        
        public SkillManager(Actor owner)
        {
            this.owner = owner;
            Init();
        }
        
        private void Init()
        {
            //初始化技能信息，处理服务器传送过来的
            foreach(var info in owner.info.Skills)
            {
                var skill = new Skill(owner, info.Id);
                Skills.Add(skill);
            }
        }


        //推动技能运转    
        public void OnUpdate(float deltatime)
        {
            foreach(Skill skill in Skills)
            {
                skill.OnUpdate(deltatime);
            }
        }

        /// <summary>
        /// 根据id获取技能
        /// </summary>
        /// <param name="skillId"></param>
        /// <returns></returns>
        public Skill GetSkill(int skillId)
        {
            return Skills.FirstOrDefault(s => s.Define.ID == skillId);
        }

        /// <summary>
        /// 获取普通攻击skill
        /// </summary>
        /// <returns></returns>
        public List<Skill> GetCommonSkills()
        {
            return Skills.Where(skill => skill.IsNormal).ToList();
        }

        /// <summary>
        /// 获取主动技能
        /// </summary>
        public List<Skill> GetActiveSkills()
        {
            return Skills.Where(skill => skill.IsActiveSkill).ToList();
        }

    }
}

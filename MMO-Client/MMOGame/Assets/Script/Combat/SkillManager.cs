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
                //Log.Information($"角色[{owner.define.Name}]加载技能[{skill.Define.ID}-{skill.Define.Name}]");
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

        public Skill GetSkill(int skillId)
        {
            return Skills.FirstOrDefault(s => s.Define.ID == skillId);
        }
    }
}

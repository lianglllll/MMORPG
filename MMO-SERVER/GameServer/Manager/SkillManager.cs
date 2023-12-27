using GameServer.Combat;
using GameServer.Model;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Manager
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
            InitSkills();
        }
        
        public void InitSkills()
        {
            //初始化技能信息，正常是通过读取数据库来加载技能信息的

            if(this.owner.Define.TID == 0)      //战士
            {
                loadSkill(1,2,3);
            }else if(this.owner.Define.TID == 1)//法师
            {
                loadSkill(101, 102);
            }else if(this.owner.Define.TID == 1001)
            {
                loadSkill(1001);
            }
        }

        private void loadSkill(params int[] ids)
        {
            foreach(int skid in ids)
            {
                owner.info.Skills.Add(new Proto.SkillInfo() { Id = skid });
                var skill = new Skill(owner, skid);
                Skills.Add(skill);
                Log.Information("角色[{0}]加载技能[{1}-{2}]", owner.Name, skill.Define.ID, skill.Define.Name);
            }
        }


        public Skill GetSkill(int skillId)
        {
            foreach (var skill in Skills) { 
                if(skill.Define.ID == skillId)
                {
                    return skill;
                }
            }
            return null;
        }

        //推动技能运转    
        public void Update()
        {
            foreach(Skill skill in Skills)
            {
                skill.Update();
            }
        }

    }
}

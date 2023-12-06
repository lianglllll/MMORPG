using GameClient.Manager;
using Proto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameClient.Entities
{
    public class Actor:Entity
    {

        public NetActor info;
        public UnitDefine define;
        public SkillManager skillManager; 

        public Actor(NetActor info) :base(info.Entity)
        {
            this.info = info;
            this.define = DataManager.Instance.unitDict[info.Tid];
            this.skillManager = new SkillManager(this);
        }

    }
}

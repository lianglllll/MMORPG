﻿using GameServer.core.FSM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.AI.State
{
    /// <summary>
    /// 死亡状态
    /// </summary>
    public class DeathState : IState<Param>
    {
        public DeathState(FSM<Param> fsm)
        {
            this.fsm = fsm;
        }

        public override void OnEnter()
        {

        }

        public override void OnUpdate()
        {
            //寻找退出死亡状态时间
            var monster = param.owner;
            if (!monster.IsDeath)
            {
                fsm.ChangeState("patrol");
            }
        }

    }
}
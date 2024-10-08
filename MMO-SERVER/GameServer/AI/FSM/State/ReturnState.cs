﻿using GameServer.AI.FSM;
using GameServer.Core;
using Proto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.AI.FSM.State
{
    /// <summary>
    /// 返回状态
    /// </summary>
    public class ReturnState : IState<Param>
    {
        public ReturnState(FSM<Param> fsm)
        {
            this.fsm = fsm;
        }

        public override void OnEnter()
        {
            param.owner.StartMoveTo(param.owner.initPosition);
        }

        public override void OnUpdate()
        {
            var monster = param.owner;

            //返回时被眩晕
            if (monster.State == EntityState.Dizzy) return;

            //有问题，我们切换为巡逻状态
            if (monster.State != EntityState.Motion)
            {
                fsm.ChangeState("patrol");
                return;
            }

            //接近到出生点就切换为巡逻状态
            if (Vector3.Distance(monster.initPosition, monster.Position) < 100)
            {
                fsm.ChangeState("patrol");
            }
        }

    }
}

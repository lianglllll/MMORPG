﻿using GameServer.core.FSM;
using GameServer.Model;
using Proto;
using Serilog;
using Summer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.AI.State
{
    public class HitState : IState<Param>
    {

        public HitState(FSM<Param> fsm)
        {
            this.fsm = fsm;
        }

        public override void OnEnter()
        {
            //如果再移动就先停下来
            var monster = param.owner;
            if (monster.State == EntityState.Motion)
            {
                monster.StopMove();
            }
            //看向目标，开始原地罚站

        }

        public override void OnUpdate()
        {
            //退出当前状态的条件
            if (param.remainHitWaitTime <= 0)
            {
                param.remainHitWaitTime = 0;
                var monster = param.owner;
                //如果当前怪物没有死亡，就应该去追击伤害来源的玩家
                if(!monster.IsDeath)
                {
                    if (monster.target != null && !monster.target.IsDeath)
                    {
                        monster.AI.fsm.ChangeState("chase");
                    }
                    else
                    {
                        monster.target = null;
                        monster.AI.fsm.ChangeState("return");
                    }
                }
            }
            param.remainHitWaitTime -= Time.deltaTime;
            //Log.Information("[受击后摇傻站]" + param.remainHitWaitTime.ToString());

        }
    }
}
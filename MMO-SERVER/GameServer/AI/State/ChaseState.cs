﻿using GameServer.core.FSM;
using GameServer.Core;
using GameServer.Manager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.AI.State
{
    /// <summary>
    /// 追击状态
    /// </summary>
    public class ChaseState : IState<Param>
    {
        public ChaseState(FSM<Param> fsm)
        {
            this.fsm = fsm;
        }

        public override void OnUpdate()
        {
            var monster = param.owner;

            //追击目标失效切换为返回状态
            if (monster.target == null || monster.target.IsDeath || !EntityManager.Instance.Exist(monster.target.EntityId))
            {
                monster.target = null;
                fsm.ChangeState("return");
                return;
            }

            //计算距离
            float brithDistance = Vector3.Distance(monster.initPosition, monster.Position);
            float targetDistance = Vector3.Distance(monster.target.Position, monster.Position);

            //当超过我们的活动范围或者追击范围，切换返回状态
            if (brithDistance > param.walkRange || targetDistance > param.chaseRange)
            {
                monster.target = null;
                fsm.ChangeState("return");
                return;
            }

            //攻击距离不够，我们继续靠近目标
            if (targetDistance > 2000)
            {
                monster.MoveTo(monster.target.Position);
                return;
            }

            //在技能后摇结束之前，我们不能再次攻击
            if (monster.curentSkill != null) return;

            //到这里就符合攻击条件了
            //符合攻击条件的同时其实也可以同时攻击，也就是说行走和攻击的动画需要混合。
            //后面客户端可能就会使用原本的动画状态机了，然后网络传送传送动画的变量。。
            //因为动画之间的切换好生硬了
            //这里如果切换到idle在攻击，会发送idle和walk抖动
            if (monster.State == Proto.EntityState.Motion)
            {
                monster.StopMove();
            }

            monster.Attack(monster.target);

        }
    }
}
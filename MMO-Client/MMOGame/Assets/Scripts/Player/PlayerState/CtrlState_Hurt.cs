using Player;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Player
{
    public class CtrlState_Hurt: CtrlState
    {
        public override void Enter()
        {
            player.PlayAnimation("Hurt");

            //看向敌人
            var target = ShareParameter.attacker;
            if (target != null)
            {
                player.DirectLookTarget(target.renderObj.transform.position);
                ShareParameter.attacker = null;
            }

        }

        public override void Update()
        {
            //todo,僵直时间应该像眩晕那样由服务器控制
            if(CheckAnimatorStateName("Hurt",out var time) && time > 0.9f)
            {
                player.ChangeState(CommonSmallState.Idle);
            }
        }

        public override void Exit()
        {
        }

    }
}

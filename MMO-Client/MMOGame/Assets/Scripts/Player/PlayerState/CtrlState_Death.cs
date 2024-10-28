using Player;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Player
{
    public class CtrlState_Death: CtrlState
    {
        public override void Enter()
        {
            player.PlayAnimation("Death");
            player.Actor.OnDeath();
        }

        public override void Exit()
        {
        }

    }
}

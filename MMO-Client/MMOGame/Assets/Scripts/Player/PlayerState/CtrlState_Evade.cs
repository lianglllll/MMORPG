using Player;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Player
{
    public class CtrlState_Evade: CtrlState
    {
        public override void Enter()
        {
            player.PlayAnimation("Evade");
        }

        public override void Update()
        {
        }

        public override void Exit()
        {
        }

    }
}
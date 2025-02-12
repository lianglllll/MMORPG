using Player;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Player
{
    public class RemotePlayerState_Evade : RemotePlayerState
    {
        public override void Enter()
        {
            remotePlayer.PlayAnimation("Evade");
        }

        public override void Update()
        {
        }

        public override void Exit()
        {
        }

    }
}

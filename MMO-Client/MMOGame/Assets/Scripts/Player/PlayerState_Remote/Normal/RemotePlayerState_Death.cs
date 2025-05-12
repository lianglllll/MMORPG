using Player;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Player
{
    public class RemotePlayerState_Death : RemotePlayerState
    {
        public override void Enter()
        {
            remotePlayer.PlayAnimation("Death");
            remotePlayer.Actor.OnDeath();
        }
        public override void Exit()
        {
            remotePlayer.Actor.OnRevive();
        }

    }
}

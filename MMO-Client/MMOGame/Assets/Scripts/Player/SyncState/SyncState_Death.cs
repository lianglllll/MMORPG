using Player;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Player
{
    public class SyncState_Death : SyncState
    {
        public override void Enter()
        {
            syncer.PlayAnimation("Death");
            syncer.Actor.OnDeath();
        }
        public override void Exit()
        {
        }

    }
}

using Player;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Player
{
    public class SyncState_Dizzy : SyncState
    {
        public override void Enter()
        {
            syncer.PlayAnimation("Dizzy");
        }

    }
}

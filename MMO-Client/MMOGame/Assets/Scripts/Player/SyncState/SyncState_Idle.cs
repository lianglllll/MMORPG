using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Player
{
    public class SyncState_Idle:SyncState
    {
        public override void Enter()
        {
            syncer.PlayAnimation("Idle");
        }
    }
}

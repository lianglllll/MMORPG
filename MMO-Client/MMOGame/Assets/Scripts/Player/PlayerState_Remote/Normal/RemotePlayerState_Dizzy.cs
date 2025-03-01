using Player;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Player
{
    public class RemotePlayerState_Dizzy : RemotePlayerState
    {
        public override void Enter()
        {
            remotePlayer.PlayAnimation("Dizzy");
        }

    }
}

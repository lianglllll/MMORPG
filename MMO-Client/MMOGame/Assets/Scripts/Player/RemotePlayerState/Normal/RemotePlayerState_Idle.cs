using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Player
{
    public class RemotePlayerState_Idle:RemotePlayerState
    {
        public override void Enter()
        {
            remotePlayer.PlayAnimation("Idle");
        }

        public override void Update()
        {
            //重力
            remotePlayer.CharacterController.Move(new Vector3(0, remotePlayer.gravity * Time.deltaTime, 0));
        }
    }
}

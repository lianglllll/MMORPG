using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Player
{
    public class SyncState_Idle:SyncState
    {
        public override void Enter()
        {
            syncer.PlayAnimation("Idle");
        }

        public override void Update()
        {
            //重力
            syncer.CharacterController.Move(new Vector3(0, ShareParameter.gravity * Time.deltaTime, 0));
        }
    }
}

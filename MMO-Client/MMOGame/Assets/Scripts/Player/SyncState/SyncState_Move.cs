using Proto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Player
{
    public class SyncState_Move:SyncState
    {
        public override void Enter()
        {
            syncer.PlayAnimation("Move");
        }
        public override void Update()
        {
            //重力
            syncer.CharacterController.Move(new Vector3(0, ShareParameter.gravity * Time.deltaTime, 0));
        }
        public override void Exit()
        {
        }

    }
}

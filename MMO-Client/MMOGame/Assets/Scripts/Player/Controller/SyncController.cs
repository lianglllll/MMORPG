 using GameClient.Entities;
using HSFramework.Net;
using Player;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Player.Controller
{
    public class SyncController : BaseController
    {
        private SyncEntityRecive syncEntityRecive;

        public override void Init(Actor actor,SyncEntity syncEntity)
        {
            base.Init(actor, syncEntity);
            syncEntityRecive = syncEntity as SyncEntityRecive;
        }

        public override void SStart()
        {
            ChangeState(CommonSmallState.Idle);
        }


        public override void ChangeState(CommonSmallState state, bool reCurrstate = false)
        {
            if (curState == state && !reCurrstate) return;
            if (state == CommonSmallState.None) return;
            curState = state;
            switch (state)
            {
                case CommonSmallState.AirDown:
                    stateMachine.ChangeState<SyncState_AirDown>(reCurrstate);
                    break;
                case CommonSmallState.Death:
                    stateMachine.ChangeState<SyncState_Death>(reCurrstate);
                    break;
                case CommonSmallState.Defense:
                    stateMachine.ChangeState<SyncState_Defense>(reCurrstate);
                    break;
                case CommonSmallState.Dizzy:
                    stateMachine.ChangeState<SyncState_Dizzy>(reCurrstate);
                    break;
                case CommonSmallState.Evade:
                    stateMachine.ChangeState<SyncState_Evade>(reCurrstate);
                    break;
                case CommonSmallState.Hurt:
                    stateMachine.ChangeState<SyncState_Hurt>(reCurrstate);
                    break;
                case CommonSmallState.Idle:
                    stateMachine.ChangeState<SyncState_Idle>(reCurrstate);
                    break;
                case CommonSmallState.JumpUp:
                    stateMachine.ChangeState<SyncState_JumpUp>(reCurrstate);
                    break;
                case CommonSmallState.Move:
                    stateMachine.ChangeState<SyncState_Move>(reCurrstate);
                    break;
                case CommonSmallState.Skill:
                    stateMachine.ChangeState<SyncState_Skill>(reCurrstate);
                    break;
            }
        }

        public void SyncPosAndRotaion(Vector3 pos,Vector3 rotation)
        {
            ChangeState(CommonSmallState.Move);
            var state = stateMachine.CurState as SyncState_Move;
            state.MoveToPostion(pos,rotation);
        }

    }
}

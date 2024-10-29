 using GameClient.Entities;
using HSFramework.Net;
using Player;
using Proto;
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
            ChangeState(ActorState.Idle);
        }


        public override void ChangeState(ActorState state, bool reCurrstate = false)
        {
            if (curState == state && !reCurrstate) return;
            if (state == ActorState.Constant) return;
            curState = state;
            switch (state)
            {
                case ActorState.AirDown:
                    stateMachine.ChangeState<SyncState_AirDown>(reCurrstate);
                    break;
                case ActorState.Death:
                    stateMachine.ChangeState<SyncState_Death>(reCurrstate);
                    break;
                case ActorState.Defense:
                    stateMachine.ChangeState<SyncState_Defense>(reCurrstate);
                    break;
                case ActorState.Dizzy:
                    stateMachine.ChangeState<SyncState_Dizzy>(reCurrstate);
                    break;
                case ActorState.Evade:
                    stateMachine.ChangeState<SyncState_Evade>(reCurrstate);
                    break;
                case ActorState.Hurt:
                    stateMachine.ChangeState<SyncState_Hurt>(reCurrstate);
                    break;
                case ActorState.Idle:
                    stateMachine.ChangeState<SyncState_Idle>(reCurrstate);
                    break;
                case ActorState.JumpUp:
                    stateMachine.ChangeState<SyncState_JumpUp>(reCurrstate);
                    break;
                case ActorState.Move:
                    stateMachine.ChangeState<SyncState_Move>(reCurrstate);
                    break;
                case ActorState.Skill:
                    stateMachine.ChangeState<SyncState_Skill>(reCurrstate);
                    break;
            }
        }

        public void SyncPosAndRotaion(Vector3 pos,Vector3 rotation)
        {
            ChangeState(ActorState.Move);
            var state = stateMachine.CurState as SyncState_Move;
            state.MoveToPostion(pos,rotation);
        }

    }
}

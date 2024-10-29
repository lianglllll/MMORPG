using GameClient.Entities;
using HSFramework.Net;
using Proto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Player.Controller
{
    public class CtrlController:BaseController
    {
        private SyncEntitySend syncEntitySend;

        public override void Init(Actor actor,SyncEntity syncEntity)
        {
            base.Init(actor,syncEntity);
            this.syncEntitySend = syncEntity as SyncEntitySend;
        }

        public override void SStart()
        {
            ChangeState(ActorState.Idle);
        }


        public override void ChangeState(ActorState state, bool reCurrstate = false)
        {
            if (curState == state && !reCurrstate) return;

            //发送状态变更包
            curState = state;
            syncEntitySend.SendSyncRequest();
            
            switch (state)
            {
                case ActorState.AirDown:
                    stateMachine.ChangeState<CtrlState_Idle>(reCurrstate);
                    break;
                case ActorState.Death:
                    stateMachine.ChangeState<CtrlState_Death>(reCurrstate);
                    break;
                case ActorState.Defense:
                    stateMachine.ChangeState<CtrlState_Defense>(reCurrstate);
                    break;
                case ActorState.Dizzy:
                    stateMachine.ChangeState<CtrlState_Dizzy>(reCurrstate);
                    break;
                case ActorState.Evade:
                    stateMachine.ChangeState<CtrlState_Evade>(reCurrstate);
                    break;
                case ActorState.Hurt:
                    stateMachine.ChangeState<CtrlState_Hurt>(reCurrstate);
                    break;
                case ActorState.Idle:
                    stateMachine.ChangeState<CtrlState_Idle>(reCurrstate);
                    break;
                case ActorState.JumpUp:
                    stateMachine.ChangeState<CtrlState_JumpUp>(reCurrstate);
                    break;
                case ActorState.Move:
                    stateMachine.ChangeState<CtrlState_Move>(reCurrstate);
                    break;
                case ActorState.Skill:
                    stateMachine.ChangeState<CtrlState_Skill>(reCurrstate);
                    break;
            }
        }

        public void MoveToPostion(Vector3 pos)
        {
            ChangeState(ActorState.Move);
            var state = stateMachine.CurState as CtrlState_Move;
            state.MoveToPostion(pos);
        }

    }
}

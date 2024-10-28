using GameClient.Entities;
using HSFramework.Net;
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
            ChangeState(CommonSmallState.Idle);
        }


        public override void ChangeState(CommonSmallState state, bool reCurrstate = false)
        {
            if (curState == state && !reCurrstate) return;

            //发送状态变更包
            curState = state;
            syncEntitySend.SendSyncRequest();
            
            switch (state)
            {
                case CommonSmallState.AirDown:
                    stateMachine.ChangeState<CtrlState_Idle>(reCurrstate);
                    break;
                case CommonSmallState.Death:
                    stateMachine.ChangeState<CtrlState_Death>(reCurrstate);
                    break;
                case CommonSmallState.Defense:
                    stateMachine.ChangeState<CtrlState_Defense>(reCurrstate);
                    break;
                case CommonSmallState.Dizzy:
                    stateMachine.ChangeState<CtrlState_Dizzy>(reCurrstate);
                    break;
                case CommonSmallState.Evade:
                    stateMachine.ChangeState<CtrlState_Evade>(reCurrstate);
                    break;
                case CommonSmallState.Hurt:
                    stateMachine.ChangeState<CtrlState_Hurt>(reCurrstate);
                    break;
                case CommonSmallState.Idle:
                    stateMachine.ChangeState<CtrlState_Idle>(reCurrstate);
                    break;
                case CommonSmallState.JumpUp:
                    stateMachine.ChangeState<CtrlState_JumpUp>(reCurrstate);
                    break;
                case CommonSmallState.Move:
                    stateMachine.ChangeState<CtrlState_Move>(reCurrstate);
                    break;
                case CommonSmallState.Skill:
                    stateMachine.ChangeState<CtrlState_Skill>(reCurrstate);
                    break;
            }
        }

        public void MoveToPostion(Vector3 pos)
        {
            ChangeState(CommonSmallState.Move);
            var state = stateMachine.CurState as CtrlState_Move;
            state.MoveToPostion(pos);
        }

    }
}

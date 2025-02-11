using GameClient.Entities;
using HS.Protobuf.SceneEntity;
using HSFramework.Net;
using Player.PlayerState;

namespace Player.Controller
{
    public class LocalPlayerController:BaseController
    {
        // private SyncEntitySend syncEntitySend;
        private NetworkActor m_networkActor;
        public NetworkActor NetworkActor => m_networkActor;

        public override void Init(Actor actor, NetworkActor networkActor)
        {
            base.Init(actor, networkActor);
            m_networkActor = networkActor;
            // this.syncEntitySend = syncEntity as SyncEntitySend;
            ChangeState(NetActorState.Idle);
        }
        public override void ChangeState(NetActorState state, bool reCurrstate = false)
        {
            if (curState == state && !reCurrstate) return;
            curState = state;
            
            switch (state)
            {
                case NetActorState.None:
                    break;
                case NetActorState.Idle:
                    stateMachine.ChangeState<LocalPlayerState_Idle>(reCurrstate);
                    break;
                case NetActorState.Motion:
                    stateMachine.ChangeState<LocalPlayerState_Motion>(reCurrstate);
                    break;
                case NetActorState.Jumpup:
                    stateMachine.ChangeState<LocalPlayerState_JumpUp>(reCurrstate);
                    break;
                case NetActorState.Falling:
                    stateMachine.ChangeState<LocalPlayerState_Falling>(reCurrstate);
                    break;
                case NetActorState.Crouch:
                    stateMachine.ChangeState<LocalPlayerState_Crouch>(reCurrstate);
                    break;
                case NetActorState.Prone:
                    stateMachine.ChangeState<LocalPlayerState_Prone>(reCurrstate);
                    break;
                case NetActorState.Hurt:
                    stateMachine.ChangeState<LocalPlayerState_Hurt>(reCurrstate);
                    break;
                case NetActorState.Dizzy:
                    stateMachine.ChangeState<LocalPlayerState_Dizzy>(reCurrstate);
                    break;
                case NetActorState.Knock:
                    stateMachine.ChangeState<LocalPlayerState_Dizzy>(reCurrstate);
                    break;
                case NetActorState.Death:
                    stateMachine.ChangeState<LocalPlayerState_Death>(reCurrstate);
                    break;
                case NetActorState.Defense:
                    stateMachine.ChangeState<LocalPlayerState_Defense>(reCurrstate);
                    break;
                case NetActorState.Evade:
                    stateMachine.ChangeState<LocalPlayerState_Evade>(reCurrstate);
                    break;
                case NetActorState.Skill:
                    stateMachine.ChangeState<LocalPlayerState_Skill>(reCurrstate);
                    break;
                case NetActorState.Custom:
                    stateMachine.ChangeState<LocalPlayerState_Skill>(reCurrstate);
                    break;
            }

            // 发送状态变更包
            // syncEntitySend.SendSyncRequest();
        }
    }
}

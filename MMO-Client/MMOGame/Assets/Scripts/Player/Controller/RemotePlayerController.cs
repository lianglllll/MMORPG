 using GameClient.Entities;
using HS.Protobuf.SceneEntity;
using Player.PlayerState;

namespace Player.Controller
{
    public class RemotePlayerController : BaseController
    {
        private NetworkActor m_networkActor;
        public NetworkActor NetworkActor => m_networkActor;
        public override void Init(Actor actor, NetworkActor networkActor)
        {
            base.Init(actor, networkActor);
            m_networkActor = networkActor;
            ChangeState(NetActorState.Idle);
        }
        public override void ChangeState(NetActorState state, bool reCurrstate = false)
        {
            if (m_curState == state && !reCurrstate) return;
            m_curState = state;
            switch (state)
            {
                case NetActorState.None:
                    break;
                case NetActorState.Idle:
                    stateMachine.ChangeState<RemotePlayerState_Idle>(reCurrstate);
                    break;
                case NetActorState.Motion:
                    stateMachine.ChangeState<RemotePlayerState_Motion>(reCurrstate);
                    break;
                case NetActorState.Jumpup:
                    stateMachine.ChangeState<RemotePlayerState_JumpUp>(reCurrstate);
                    break;
                case NetActorState.Falling:
                    stateMachine.ChangeState<RemotePlayerState_Falling>(reCurrstate);
                    break;
                case NetActorState.Crouch:
                    stateMachine.ChangeState<RemotePlayerState_Crouch>(reCurrstate);
                    break;
                case NetActorState.Prone:
                    stateMachine.ChangeState<RemotePlayerState_Prone>(reCurrstate);
                    break;
                case NetActorState.Hurt:
                    stateMachine.ChangeState<RemotePlayerState_Hurt>(reCurrstate);
                    break;
                case NetActorState.Dizzy:
                    stateMachine.ChangeState<RemotePlayerState_Dizzy>(reCurrstate);
                    break;
                case NetActorState.Knock:
                    stateMachine.ChangeState<RemotePlayerState_Knock>(reCurrstate);
                    break;
                case NetActorState.Death:
                    stateMachine.ChangeState<RemotePlayerState_Death>(reCurrstate);
                    break;
                case NetActorState.Defense:
                    stateMachine.ChangeState<RemotePlayerState_Defense>(reCurrstate);
                    break;
                case NetActorState.Evade:
                    stateMachine.ChangeState<RemotePlayerState_Evade>(reCurrstate);
                    break;
                case NetActorState.Skill:
                    stateMachine.ChangeState<RemotePlayerState_Skill>(reCurrstate);
                    break;
                case NetActorState.Custom:
                    stateMachine.ChangeState<RemotePlayerState_Custom>(reCurrstate);
                    break;
            }
        }
    }
}

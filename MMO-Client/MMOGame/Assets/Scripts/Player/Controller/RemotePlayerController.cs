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
            if(actor.NetActorMode != NetActorMode.None)
            {
                ChangeMode(actor.NetActorMode);
            }
            else
            {
                ChangeMode(NetActorMode.Normal);
            }
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
                    if(CurMode == NetActorMode.Normal)
                    {
                        stateMachine.ChangeState<RemotePlayerState_Idle>(reCurrstate);
                    }else if(CurMode == NetActorMode.FlyNormal)
                    {
                        stateMachine.ChangeState<RemotePlayerState_Fly_Idle>(reCurrstate);
                    }
                    break;
                case NetActorState.Motion:
                    if (CurMode == NetActorMode.Normal)
                    {
                        stateMachine.ChangeState<RemotePlayerState_Motion>(reCurrstate);
                    }
                    else if (CurMode == NetActorMode.FlyNormal)
                    {
                        stateMachine.ChangeState<RemotePlayerState_Fly_Motion>(reCurrstate);
                    }
                    break;
                case NetActorState.Jumpup:
                    StateMachineParameter.jumpVelocity = curActorChangeStateResponse.PayLoad.JumpVerticalVelocity * 0.001f;
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
                    StateMachineParameter.evadeStatePayload = curActorChangeStateResponse.PayLoad.EvadePayLoad;
                    stateMachine.ChangeState<RemotePlayerState_Evade>(reCurrstate);
                    break;
                case NetActorState.Skill:
                    stateMachine.ChangeState<RemotePlayerState_Skill>(reCurrstate);
                    break;
                case NetActorState.Custom:
                    stateMachine.ChangeState<RemotePlayerState_Custom>(reCurrstate);
                    break;
                case NetActorState.Changehight:
                    if (m_curMode == NetActorMode.Normal)
                    {

                    }
                    else if (m_curMode == NetActorMode.FlyNormal)
                    {
                        stateMachine.ChangeState<RemotePlayerState_Fly_ChangeHight>(reCurrstate);
                    }
                    break;
            }
        }
    }
}

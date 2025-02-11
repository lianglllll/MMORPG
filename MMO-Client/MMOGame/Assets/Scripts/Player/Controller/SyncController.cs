 using GameClient.Entities;
using HS.Protobuf.SceneEntity;
using HSFramework.Net;

namespace Player.Controller
{
    public class SyncController : BaseController
    {
        private SyncEntityRecive syncEntityRecive;

        public override void Init(Actor actor, NetworkActor networkActor)
        {
            base.Init(actor, networkActor);
            //syncEntityRecive = syncEntity as SyncEntityRecive;
            ChangeState(NetActorState.Idle);
        }

        public override void ChangeState(NetActorState state, bool reCurrstate = false)
        {
            if (curState == state && !reCurrstate) return;
            curState = state;
            switch (state)
            {
                case NetActorState.Falling:
                    stateMachine.ChangeState<SyncState_AirDown>(reCurrstate);
                    break;
                case NetActorState.Death:
                    stateMachine.ChangeState<SyncState_Death>(reCurrstate);
                    break;
                case NetActorState.Defense:
                    stateMachine.ChangeState<SyncState_Defense>(reCurrstate);
                    break;
                case NetActorState.Dizzy:
                    stateMachine.ChangeState<SyncState_Dizzy>(reCurrstate);
                    break;
                case NetActorState.Evade:
                    stateMachine.ChangeState<SyncState_Evade>(reCurrstate);
                    break;
                case NetActorState.Hurt:
                    stateMachine.ChangeState<SyncState_Hurt>(reCurrstate);
                    break;
                case NetActorState.Idle:
                    stateMachine.ChangeState<SyncState_Idle>(reCurrstate);
                    break;
                case NetActorState.Jumpup:
                    stateMachine.ChangeState<SyncState_JumpUp>(reCurrstate);
                    break;
                case NetActorState.Motion:
                    stateMachine.ChangeState<SyncState_Move>(reCurrstate);
                    break;
                case NetActorState.Skill:
                    stateMachine.ChangeState<SyncState_Skill>(reCurrstate);
                    break;
            }
        }
    }
}

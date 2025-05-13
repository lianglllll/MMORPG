using GameClient.Entities;
using HS.Protobuf.SceneEntity;
using Player.PlayerState;
using Serilog;

namespace Player.Controller
{
    public class LocalPlayerController:BaseController
    {
        protected override void Update()
        {
            base.Update();

            if (GameInputManager.Instance.GI_Tab)
            {
                if(m_curMode == NetActorMode.Normal)
                {
                    ChangeMode(NetActorMode.Fly);
                }
                else if(m_curMode == NetActorMode.NormalEquip)
                {
                    ChangeMode(NetActorMode.FlyEquip);
                }
                else if(m_curMode == NetActorMode.Fly)
                {
                    ChangeMode(NetActorMode.Normal);
                }
                else if(m_curMode == NetActorMode.FlyEquip)
                {
                    ChangeMode(NetActorMode.NormalEquip);
                }
            }
        }

        public override void Init(Actor actor)
        {
            base.Init(actor);
            ChangeMode(NetActorMode.Normal);
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
                    if(m_curMode == NetActorMode.Normal)
                    {
                        stateMachine.ChangeState<LocalPlayerState_Idle>(reCurrstate);
                    }else if(m_curMode == NetActorMode.NormalEquip)
                    {
                        stateMachine.ChangeState<LocalPlayerState_Normal_Equip_Idle>(reCurrstate);
                    }
                    else if (m_curMode == NetActorMode.Fly)
                    {
                        stateMachine.ChangeState<LocalPlayerState_Fly_Idle>(reCurrstate);
                    }
                    break;
                case NetActorState.Motion:
                    if (m_curMode == NetActorMode.Normal)
                    {
                        stateMachine.ChangeState<LocalPlayerState_Motion>(reCurrstate);
                    }else if(m_curMode == NetActorMode.NormalEquip)
                    {
                        stateMachine.ChangeState<LocalPlayerState_Normal_Equip_Motion>(reCurrstate);
                    }
                    else if (m_curMode == NetActorMode.Fly)
                    {
                        stateMachine.ChangeState<LocalPlayerState_Fly_Motion>(reCurrstate);
                    }
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
                    if(m_curMode == NetActorMode.Normal)
                    {
                        stateMachine.ChangeState<LocalPlayerState_Evade>(reCurrstate);
                    }else if(m_curMode == NetActorMode.NormalEquip)
                    {
                        stateMachine.ChangeState<LocalPlayerState_Normal_Equip_Evade>(reCurrstate);
                    }
                    break;
                case NetActorState.Skill:
                    stateMachine.ChangeState<LocalPlayerState_Skill>(reCurrstate);
                    break;
                case NetActorState.Custom:
                    stateMachine.ChangeState<LocalPlayerState_Skill>(reCurrstate);
                    break;
                case NetActorState.Changehight:
                    if (m_curMode == NetActorMode.Normal)
                    {

                    }
                    else if (m_curMode == NetActorMode.Fly)
                    {
                        stateMachine.ChangeState<LocalPlayerState_Fly_ChangeHight>(reCurrstate);
                    }
                    break;
            }

            // 发送状态变更包
            // syncEntitySend.SendSyncRequest();
        }
        public override void ChangeMode(NetActorMode mode)
        {
            if (m_curMode == mode)
            {
                goto End;
            }

            if(CurSkill != null && !IsCanCancelSkill)
            {
                goto End;
            }

            base.ChangeMode(mode);

            // 事件触发
            Kaiyun.Event.FireIn("LocalPlayerModeChange");

            // 告诉服务器
            NetworkActor.SendActorChangeModeRequest();
        End:
            return;
        }
    }
}

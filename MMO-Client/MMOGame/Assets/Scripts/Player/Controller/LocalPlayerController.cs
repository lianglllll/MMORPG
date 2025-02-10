using GameClient.Entities;
using HS.Protobuf.SceneEntity;
using HSFramework.Net;
using Player.PlayerState;
using UnityEngine;

namespace Player.Controller
{
    public class LocalPlayerController:BaseController
    {
        private SyncEntitySend syncEntitySend;

        #region Player配置信息

        [Header("Player配置")]
        public float gravity = -9.8f;

        public float rotateSpeed = 5f;

        public float walk2RunTransitionSpeed = 1f;
        public float walkSpeed = 1.5f;
        public float runSpeed = 5f;

        public float jumpVelocity = 5f;
        public float moveSpeedForJump = 1f;
        public float moveSpeedForAirDown = 1f;


        public float needPlayEndAnimationHeight = 5f;                  //如果空中检测到距离地面有3米则启动翻滚
        public float playEndAnimationHeight = 1.8f;                    //End动画播放需要的高度
        public float rollPower = 1f;
        public float rotateSpeedForAttack = 5f;
        public float DefenceTime;
        public float WaitCounterAttackTime;

        #endregion

        public override void Init(Actor actor,SyncEntity syncEntity)
        {
            base.Init(actor,syncEntity);
            this.syncEntitySend = syncEntity as SyncEntitySend;
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

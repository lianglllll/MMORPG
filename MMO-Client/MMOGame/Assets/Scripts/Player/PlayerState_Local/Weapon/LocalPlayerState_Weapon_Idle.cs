using HS.Protobuf.SceneEntity;
using HSFramework.AI.StateMachine;
using UnityEngine;

namespace Player.PlayerState
{

    public class LocalPlayerState_Weapon_Idle : LocalPlayerState
    {
        private enum IdleFomat
        {
            Normal, Extra
        }

        private int m_extraIdleCnt;
        private float m_switchExtraIdleTime;
        private IdleFomat m_idleFomat;
        private float m_switchExtraIdleInterval = 10f;
        private string m_curExtraIdleName;

        public override void Init(IStateMachineOwner owner)
        {
            base.Init(owner);

            m_extraIdleCnt = player.Model.GetComponent<ModelAnimationData>().ExtraIdleCnt;

        }
        public override void Enter()
        {
            player.PlayAnimation("Weapon_Idle");
            m_switchExtraIdleTime = 0f;
            m_idleFomat = IdleFomat.Normal;

            // 发送状态改变请求
            player.NetworkActor.SendActorChangeStateRequest();
        } 
        public override void Update()
        {
            //检测闪避
            if (GameInputManager.Instance.Shift)
            {
                player.ChangeState(NetActorState.Evade);
                goto End;
            }

            //检测跳跃
            if (GameInputManager.Instance.Jump)
            {
                player.ChangeState(NetActorState.Jumpup);
                goto End;
            }

            // 检测蹲下
            if (GameInputManager.Instance.Crouch)
            {
                player.ChangeState(NetActorState.Crouch);
                goto End;
            }



            // 玩家移动
            float h = GameInputManager.Instance.Movement.x;
            float v = GameInputManager.Instance.Movement.y;
            if (h != 0 || v != 0)
            {
                player.ChangeState(NetActorState.Motion);
                goto End;
            }

            //重力
            player.CharacterController.Move(new Vector3(0, player.gravity * Time.deltaTime, 0));

            //检测下落
            if (player.CharacterController.isGrounded == false)
            {
                player.ChangeState(NetActorState.Falling);
                goto End;
            }

            // 播放其他的的idle动画
            if (m_extraIdleCnt == 0)
            {
                goto End;
            }
            m_switchExtraIdleTime += Time.deltaTime;
            if (m_idleFomat == IdleFomat.Normal)
            {
                if (m_switchExtraIdleTime > m_switchExtraIdleInterval)
                {
                    m_idleFomat = IdleFomat.Extra;

                    int randomIdx = Random.Range(1, m_extraIdleCnt + 1);
                    m_curExtraIdleName = "Idle_" + randomIdx;
                    player.PlayAnimation(m_curExtraIdleName);
                }
            }
            else if(m_idleFomat == IdleFomat.Extra)
            {
                if (CheckAnimatorStateName(m_curExtraIdleName, out var time) && time >= 0.95f)
                {
                    m_idleFomat = IdleFomat.Normal;
                    player.PlayAnimation("Idle");
                    m_switchExtraIdleTime = 0f;
                }
            }
        End:
            return;
        }
    }
}

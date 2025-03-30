using HS.Protobuf.SceneEntity;
using HSFramework.AI.StateMachine;
using System.Collections.Generic;
using UnityEngine;

namespace Player.PlayerState
{

    public class LocalPlayerState_Idle: LocalPlayerState
    {
        private enum IdleFomat
        {
            Normal, Extra
        }

        private int m_extraIdleCnt;
        private string m_curExtraIdleName;
        private IdleFomat m_idleFomat;
        private float m_switchExtraIdleTime;
        private float m_switchExtraIdleInterval = 10f;

        // 洗牌算法
        private List<int> m_shuffledIndices = new List<int>();
        private int m_currentShuffleIndex = 0;
        private bool m_needReshuffle = true;


        public override void Init(IStateMachineOwner owner)
        {
            base.Init(owner);

            m_extraIdleCnt = player.Model.GetComponent<ModelAnimationData>().ExtraIdleCnt;

        }

        public override void Enter()
        {
            player.PlayAnimation("Idle");
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
            if (GameInputManager.Instance.Space)
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
            if (m_idleFomat == IdleFomat.Normal && m_switchExtraIdleTime > m_switchExtraIdleInterval)
            {
                UpdateShuffleList(); // 按需洗牌

                int randomIdx = m_shuffledIndices[m_currentShuffleIndex];
                m_currentShuffleIndex = (m_currentShuffleIndex + 1) % m_shuffledIndices.Count;

                // 完成一轮后标记需要重新洗牌
                if (m_currentShuffleIndex == 0)
                {
                    m_needReshuffle = true;
                }

                m_curExtraIdleName = "Idle_" + randomIdx;
                player.PlayAnimation(m_curExtraIdleName);
                m_idleFomat = IdleFomat.Extra;
            }
            else if(m_idleFomat == IdleFomat.Extra)
            {
                if (CheckAnimatorStateName(m_curExtraIdleName, out var time) && time >= 0.95f)
                {
                    player.PlayAnimation("Idle");
                    m_switchExtraIdleTime = 0f;
                    m_idleFomat = IdleFomat.Normal;
                }
            }
        End:
            return;
        }

        // tools
        void UpdateShuffleList()
        {
            if (m_needReshuffle || m_shuffledIndices.Count != m_extraIdleCnt)
            {
                m_shuffledIndices.Clear();
                for (int i = 1; i <= m_extraIdleCnt; i++)
                {
                    m_shuffledIndices.Add(i);
                }

                // Fisher-Yates洗牌算法优化版
                for (int i = m_shuffledIndices.Count - 1; i > 0; i--)
                {
                    int j = Random.Range(0, i + 1);
                    int temp = m_shuffledIndices[i];
                    m_shuffledIndices[i] = m_shuffledIndices[j];
                    m_shuffledIndices[j] = temp;
                }

                m_currentShuffleIndex = 0;
                m_needReshuffle = false;
            }
        }


    }
}

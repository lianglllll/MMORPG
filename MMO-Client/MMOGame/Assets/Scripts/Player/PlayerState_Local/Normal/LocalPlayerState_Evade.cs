using HS.Protobuf.SceneEntity;
using System.Collections;
using UnityEngine;

namespace Player.PlayerState
{
    public class LocalPlayerState_Evade: LocalPlayerState
    {
        private Coroutine rollCorotine;
        private bool isRotate = false;
        private string curAnimationName = "Roll";
        private float rotationRate = 0;

        public override void Enter()
        {
            player.m_unitEffectManager.StartCloneTrailFX();

            //根据输入进行翻滚
            float h = GameInputManager.Instance.Movement.x;
            float v = GameInputManager.Instance.Movement.y;
            Vector3 inputDir = new Vector3(h, 0, v).normalized;
            float y = Camera.main.transform.rotation.eulerAngles.y;     // 获取相机旋转值y
            NetAcotrEvadeStatePayload payload = NetAcotrEvadeStatePayload.NetActorEvadeStatePayloadFront;
            Quaternion targetRotation;
            if (h != 0 && v != 0)
            {
                // 让四元数和向量相乘：让这个向量按照这个四元数所表达的角度进行旋转后得到的新向量。
                Vector3 targetDir = Quaternion.Euler(0, y, 0) * inputDir;
                targetRotation = Quaternion.LookRotation(targetDir);
                //四个斜方向，自动转向使用向前翻滚即可
                rollCorotine = MonoManager.Instance.StartCoroutine(DoRotate(targetRotation));
            }
            else if (h != 0 || v != 0)
            {
                targetRotation = Quaternion.Euler(0, y, 0);
                if (h == 0 && v == 1)
                {
                    payload = NetAcotrEvadeStatePayload.NetActorEvadeStatePayloadFront;
                }
                else if (h == 0 && v == -1)
                {
                    payload = NetAcotrEvadeStatePayload.NetActorEvadeStatePayloadBack;
                }
                else if (h == -1 && v == 0)
                {
                    payload = NetAcotrEvadeStatePayload.NetActorEvadeStatePayloadLeft;
                }
                else if (h == 1 && v == 0)
                {
                    payload = NetAcotrEvadeStatePayload.NetActorEvadeStatePayloadRight;
                }

                //四个正方向
                rollCorotine = MonoManager.Instance.StartCoroutine(DoRotate2(payload, targetRotation));
            }
            else
            {
                //没有输入不旋转模型，让其按原来模型的方向向后翻滚
                curAnimationName = "Roll_back";
                player.PlayAnimation("Roll_back");
                player.Model.SetRootMotionAction(OnRootMotion);

                targetRotation = player.transform.rotation;
                payload = NetAcotrEvadeStatePayload.NetActorEvadeStatePayloadBack;
            }

            // 发包
            ShareParameter.evadeStatePayload = payload;
            ShareParameter.evadeRotation = targetRotation.eulerAngles;
            player.NetworkActor.SendActorChangeStateRequest();
        }
        public override void Update()
        {
            if (isRotate) return;

            if (CheckAnimatorStateName(curAnimationName, out var time) && time >= 0.8f)
            {
                float h = GameInputManager.Instance.Movement.x;
                float v = GameInputManager.Instance.Movement.y;
                if (h != 0 || v != 0)
                {
                    player.ChangeState(NetActorState.Motion);
                    var state = (LocalPlayerState_Motion)player.stateMachine.CurState;
                    state.SetRun();
                }
                else
                {
                    player.ChangeState(NetActorState.Idle);
                }
            }
        }
        public override void Exit()
        {
            player.Model.ClearRootMotionAction();
            if (rollCorotine != null)
            {
                MonoManager.Instance.StopCoroutine(rollCorotine);
            }
            player.m_unitEffectManager.StopCloneTrailFX();
        }

        private void OnRootMotion(Vector3 deltaPosition, Quaternion deltaRotation)
        {
            // deltaPosition *= Mathf.Clamp(moveStatePower, 1, 1.2f) * player.rollPower;
            deltaPosition.y = player.gravity * Time.deltaTime;
            player.CharacterController.Move(deltaPosition);
        }

        private IEnumerator DoRotate(Quaternion targetRotation)
        {
            //先旋转，再播放
            isRotate = true;
            rotationRate = 0;
            while (rotationRate < 1)
            {
                rotationRate += Time.deltaTime * 10;//10倍速旋转
                player.transform.rotation = Quaternion.Slerp(player.transform.rotation, targetRotation, rotationRate);
                yield return null;
            }
            isRotate = false;

            curAnimationName = "Roll";
            player.PlayAnimation("Roll");
            player.Model.SetRootMotionAction(OnRootMotion);
        }
        private IEnumerator DoRotate2(NetAcotrEvadeStatePayload payload, Quaternion targetRotation)
        {
            //先将角色模型的旋转旋转到和摄像机一致
            isRotate = true;
            rotationRate = 0;
            while (rotationRate < 1)
            {
                rotationRate += Time.deltaTime * 10;//10倍速旋转
                player.transform.rotation = Quaternion.Slerp(player.transform.rotation, targetRotation, rotationRate);
                yield return null;
            }
            isRotate = false;

            //四个正方向的移动
            if (payload == NetAcotrEvadeStatePayload.NetActorEvadeStatePayloadFront)
            {
                curAnimationName = "Roll";
                player.PlayAnimation("Roll");
            }
            else if (payload == NetAcotrEvadeStatePayload.NetActorEvadeStatePayloadBack)
            {
                curAnimationName = "Roll_back";
                player.PlayAnimation("Roll_back");
            }
            else if (payload == NetAcotrEvadeStatePayload.NetActorEvadeStatePayloadLeft)
            {
                curAnimationName = "Roll_left";
                player.PlayAnimation("Roll_left");
            }
            else if (payload == NetAcotrEvadeStatePayload.NetActorEvadeStatePayloadRight)
            {
                curAnimationName = "Roll_right";
                player.PlayAnimation("Roll_right");
            }
            player.Model.SetRootMotionAction(OnRootMotion);
        }
    }
}

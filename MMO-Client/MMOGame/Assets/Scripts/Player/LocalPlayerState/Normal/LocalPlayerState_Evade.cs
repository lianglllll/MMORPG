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

        public override void Enter()
        {
            //根据输入进行翻滚
            float h = GameInputManager.Instance.Movement.x;
            float v = GameInputManager.Instance.Movement.y;
            if (h != 0 && v != 0)
            {
                //四个斜方向，自动转向使用向前翻滚即可
                Vector3 inputDir = new Vector3(h, 0, v).normalized;
                rollCorotine = MonoManager.Instance.StartCoroutine(DoRotate(inputDir));
            }
            else if (h != 0 || v != 0)
            {
                //四个正方向
                Vector3 inputDir = new Vector3(h, 0, v).normalized;
                rollCorotine = MonoManager.Instance.StartCoroutine(DoRotate2(inputDir));
            }
            else
            {
                //没有输入不旋转模型，让其按原来模型的方向向后翻滚
                curAnimationName = "Roll_back";
                player.PlayAnimation("Roll_back");
                player.Model.SetRootMotionAction(OnRootMotion);
            }

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
        }

        private void OnRootMotion(Vector3 deltaPosition, Quaternion deltaRotation)
        {
            // deltaPosition *= Mathf.Clamp(moveStatePower, 1, 1.2f) * player.rollPower;
            deltaPosition.y = player.gravity * Time.deltaTime;
            player.CharacterController.Move(deltaPosition);
        }

        private IEnumerator DoRotate(Vector3 input)
        {
            //先旋转，再播放
            isRotate = true;
            //获取相机旋转值y
            float y = Camera.main.transform.rotation.eulerAngles.y;
            //让四元数和向量相乘：让这个向量按照这个四元数所表达的角度进行旋转后得到的新向量。
            Vector3 targetDir = Quaternion.Euler(0, y, 0) * input;
            var targetRotation = Quaternion.LookRotation(targetDir);
            float rate = 0;
            while (rate < 1)
            {
                rate += Time.deltaTime * 10;//10倍速旋转
                player.transform.rotation = Quaternion.Slerp(player.transform.rotation, targetRotation, rate);
                yield return null;
            }
            isRotate = false;

            curAnimationName = "Roll";
            player.PlayAnimation("Roll");
            player.Model.SetRootMotionAction(OnRootMotion);
        }
        private IEnumerator DoRotate2(Vector3 input)
        {
            float h = input.x;
            float v = input.z;


            //先将角色模型的旋转旋转到和摄像机一致
            isRotate = true;
            float y = Camera.main.transform.rotation.eulerAngles.y;
            var targetRotation = Quaternion.Euler(0, y, 0);
            float rate = 0;
            while (rate < 1)
            {
                rate += Time.deltaTime * 10;//10倍速旋转
                player.transform.rotation = Quaternion.Slerp(player.transform.rotation, targetRotation, rate);
                yield return null;
            }
            isRotate = false;

            //四个正方向的移动
            if (h == 0 && v == 1)
            {
                curAnimationName = "Roll";
                player.PlayAnimation("Roll");
            }
            else if (h == 0 && v == -1)
            {
                curAnimationName = "Roll_back";
                player.PlayAnimation("Roll_back");
            }
            else if (h == -1 && v == 0)
            {
                curAnimationName = "Roll_left";
                player.PlayAnimation("Roll_left");
            }
            else if (h == 1 && v == 0)
            {
                curAnimationName = "Roll_right";
                player.PlayAnimation("Roll_right");
            }

            player.Model.SetRootMotionAction(OnRootMotion);
        }

    }
}

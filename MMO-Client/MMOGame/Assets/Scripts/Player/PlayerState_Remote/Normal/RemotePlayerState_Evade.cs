using HS.Protobuf.SceneEntity;
using System.Collections;
using UnityEngine;
namespace Player
{
    public class RemotePlayerState_Evade : RemotePlayerState
    {
        private Coroutine CheckRollCorotine;

        public override void Enter()
        {
            CheckRollCorotine = MonoManager.Instance.StartCoroutine(CheckCanRoll());
        }
        public override void Exit()
        {
            remotePlayer.Model.ClearRootMotionAction();
            if (CheckRollCorotine != null)
            {
                MonoManager.Instance.StopCoroutine(CheckRollCorotine);
            }
        }
        private void OnRootMotion(Vector3 deltaPosition, Quaternion deltaRotation)
        {
            // deltaPosition *= Mathf.Clamp(moveStatePower, 1, 1.2f) * player.rollPower;
            deltaPosition.y = remotePlayer.gravity * Time.deltaTime;
            remotePlayer.CharacterController.Move(deltaPosition);
        }

        private IEnumerator CheckCanRoll()
        {
            while (remotePlayer.IsTransitioning)
            {
                yield return null;
            }

            if (StateMachineParameter.evadeStatePayload == NetAcotrEvadeStatePayload.NetActorEvadeStatePayloadFront)
            {
                remotePlayer.PlayAnimation("Roll");
            }
            else if (StateMachineParameter.evadeStatePayload == NetAcotrEvadeStatePayload.NetActorEvadeStatePayloadBack)
            {
                remotePlayer.PlayAnimation("Roll_back");
            }
            else if (StateMachineParameter.evadeStatePayload == NetAcotrEvadeStatePayload.NetActorEvadeStatePayloadLeft)
            {
                remotePlayer.PlayAnimation("Roll_left");
            }
            else if (StateMachineParameter.evadeStatePayload == NetAcotrEvadeStatePayload.NetActorEvadeStatePayloadRight)
            {
                remotePlayer.PlayAnimation("Roll_right");
            }
            remotePlayer.Model.SetRootMotionAction(OnRootMotion);
        }
    }
}

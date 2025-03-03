using HS.Protobuf.Scene;
using HS.Protobuf.SceneEntity;
using HSFramework.AI.StateMachine;
using UnityEngine;

namespace Player.PlayerState
{
    public class LocalPlayerState_Fly_ChangeHight : LocalPlayerState
    {
        private float changeHightSpeed = 0;
        private Vector3 deltaPos = new Vector3(0, 0, 0);

        private float timer = 0f;
        private const float SEND_INTERVAL = 0.2f; // 每100ms发送一次
        private ActorChangeTransformDataRequest actdReq;

        public override void Init(IStateMachineOwner owner)
        {
            base.Init(owner);
            actdReq = new ActorChangeTransformDataRequest();
            actdReq.PayLoad = new ActorChangeTransformDataPayLoad();
            actdReq.SessionId = NetManager.Instance.sessionId;
            actdReq.EntityId = player.Actor.EntityId;
            actdReq.OriginalTransform = new NetTransform();
            actdReq.OriginalTransform.Position = new();
            actdReq.OriginalTransform.Rotation = new();
            actdReq.OriginalTransform.Scale = new();
        }
        public override void Enter()
        {
            player.PlayAnimation("Fly_ChangeHight");
            // 发送状态改变请求
            player.NetworkActor.SendActorChangeStateRequest();
        } 
        public override void Update()
        {
            if (GameInputManager.Instance.SustainQ)
            {
                player.Model.Animator.SetFloat("Fly_Hight_Speed", 1, 0.1f, Time.deltaTime);
                changeHightSpeed = player.flyChangeHightSpeed;
            }
            else if (GameInputManager.Instance.SustainE)
            {
                player.Model.Animator.SetFloat("Fly_Hight_Speed", -1, 0.1f, Time.deltaTime);
                changeHightSpeed = -player.flyChangeHightSpeed;
            }
            else
            {
                player.ChangeState(NetActorState.Idle);
                goto End;
            }
            deltaPos.y = changeHightSpeed * Time.deltaTime;
            player.CharacterController.Move(deltaPos);

            SendActorChangeTransformDataRequest();
        End:
            return;
        }

        private bool SendActorChangeTransformDataRequest()
        {
            timer += Time.deltaTime;
            if (timer >= SEND_INTERVAL)
            {
                timer = 0.0f;
                player.NetworkActor.V3ToNV3(player.gameObject.transform.position, actdReq.OriginalTransform.Position);
                player.NetworkActor.V3ToNV3(player.gameObject.transform.eulerAngles, actdReq.OriginalTransform.Rotation);
                actdReq.PayLoad.HightSpeed = (int)(player.Model.Animator.GetFloat("Fly_Hight_Speed") * 1000);
                actdReq.Timestamp = NetworkTime.Instance.GetCurNetWorkTime();
                NetManager.Instance.Send(actdReq);
            }
            return true;
        }
    }
}

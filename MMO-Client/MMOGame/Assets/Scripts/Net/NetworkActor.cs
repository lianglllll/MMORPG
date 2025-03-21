using GameClient.Entities;
using HS.Protobuf.Common;
using HS.Protobuf.Scene;
using HS.Protobuf.SceneEntity;
using Player;
using UnityEngine;

public class NetworkActor : MonoBehaviour
{
    private bool m_isStart;
    private BaseController m_baseController;
    private Actor m_actor;
    private Transform m_transform;
    // temp
    ActorChangeModeRequest acmReq;
    private ActorChangeStateRequest acsReq;


    public bool Init(BaseController baseController)
    {
        m_baseController = baseController;
        m_actor = baseController.Actor;
        m_transform = baseController.CharacterController.gameObject.transform;

        //
        acmReq = new ActorChangeModeRequest();
        acmReq.EntityId = m_actor.EntityId;
        // 
        acsReq = new ActorChangeStateRequest();
        var netTransform = new NetTransform();
        var pos = new NetVector3();
        var rotation = new NetVector3();
        var scale = new NetVector3();
        var payLoad = new ActorStatePayLoad();
        netTransform.Position = pos;
        netTransform.Rotation = rotation;
        netTransform.Scale = scale;
        acsReq.OriginalTransform = netTransform;
        acsReq.EntityId = m_actor.EntityId;
        acsReq.SessionId = NetManager.Instance.sessionId;
        acsReq.PayLoad = payLoad;

        m_isStart = true;
        return true;
    }
    public bool SendActorChangeModeRequest()
    {
        bool result = false;
        if (!m_isStart)
        {
            goto End;
        }
        acmReq.Mode = m_baseController.CurMode;
        acmReq.Timestamp = NetworkTime.Instance.GetCurNetWorkTime();
        NetManager.Instance.Send(acmReq);
        result = true;
    End:
        return result;
    }
    public bool SendActorChangeStateRequest()
    {
        if(!m_isStart)
        {
            return false;
        }

        V3ToNV3(m_transform.position, acsReq.OriginalTransform.Position);
        V3ToNV3(m_transform.eulerAngles, acsReq.OriginalTransform.Rotation);
        V3ToNV3(m_transform.localScale, acsReq.OriginalTransform.Scale);    // todo
        acsReq.Timestamp = NetworkTime.Instance.GetCurNetWorkTime();
        acsReq.State = m_baseController.CurState;
        if(m_baseController.CurState == NetActorState.Motion)
        {
            acsReq.PayLoad.Speed = m_actor.Speed * 1000;
        }else if(m_baseController.CurState == NetActorState.Evade)
        {
            acsReq.PayLoad.EvadePayLoad = m_baseController.StateMachineParameter.evadeStatePayload;
            V3ToNV3(m_baseController.StateMachineParameter.evadeRotation, acsReq.OriginalTransform.Rotation);
        }else if(m_baseController.CurState == NetActorState.Jumpup)
        {
            acsReq.PayLoad.JumpVerticalVelocity = m_baseController.StateMachineParameter.jumpVelocity * 1000;
        }
        NetManager.Instance.Send(acsReq);
        return true;
    }

    public void V3ToNV3(Vector3 a, NetVector3 b)
    {
        b.X = (int)(a.x * 1000);
        b.Y = (int)(a.y * 1000);
        b.Z = (int)(a.z * 1000);
    }
    public void NV3ToV3(NetVector3 a, ref Vector3 b)
    {
        b.x = a.X * 0.001f;
        b.y = a.Y * 0.001f;
        b.z = a.Z * 0.001f;
    }
}

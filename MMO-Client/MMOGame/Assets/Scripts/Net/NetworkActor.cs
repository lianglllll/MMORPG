using GameClient.Entities;
using HS.Protobuf.Scene;
using HS.Protobuf.SceneEntity;
using HybridCLR.Editor.ABI;
using Player;
using System;
using UnityEngine;

public class NetworkActor : MonoBehaviour
{
    private bool m_isStart;
    private BaseController m_baseController;
    private Actor m_actor;
    private Transform m_transform;

    public bool Init(BaseController baseController)
    {
        m_baseController = baseController;
        m_actor = baseController.Actor;
        m_transform = baseController.CharacterController.gameObject.transform;
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
        var req = new ActorChangeModeRequest();
        req.EntityId = m_actor.EntityId;
        req.Mode = m_baseController.CurMode;
        req.Timestamp = NetworkTime.Instance.GetCurNetWorkTime();
        NetManager.Instance.Send(req);
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
        var req = new ActorChangeStateRequest();
        var netTransform = new NetTransform();
        var pos = new NetVector3();
        var rotation = new NetVector3();
        var scale = new NetVector3();
        V3ToNV3(m_transform.position, pos);
        V3ToNV3(m_transform.eulerAngles, rotation);
        V3ToNV3(m_transform.localScale, scale);
        netTransform.Position = pos;
        netTransform.Rotation = rotation;
        netTransform.Scale = scale;
        req.OriginalTransform = netTransform;
        req.EntityId = m_actor.EntityId;
        req.Timestamp = NetworkTime.Instance.GetCurNetWorkTime();
        req.State = m_baseController.CurState;
        req.SessionId = NetManager.Instance.sessionId;
        if(m_baseController.CurState == NetActorState.Motion)
        {
            req.Speed = m_actor.Speed * 1000;
        }
        NetManager.Instance.Send(req);
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

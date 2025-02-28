using Common.Summer.Core;
using Common.Summer.Net;
using GameClient;
using GameClient.Entities;
using HS.Protobuf.Scene;
using HS.Protobuf.SceneEntity;
using HSFramework.MySingleton;
public class SceneHandler : SingletonNonMono<SceneHandler>
{
    public void Init()
    {
        // 协议注册
        ProtoHelper.Instance.Register<OtherEntityEnterSceneResponse>((int)SceneProtocl.OtherEntityEnterSceneResp);
        ProtoHelper.Instance.Register<OtherEntityLeaveSceneResponse>((int)SceneProtocl.OtherEntityLeaveSceneResp);
        ProtoHelper.Instance.Register<ActorChangeModeRequest>((int)SceneProtocl.ActorChangeModeReq);
        ProtoHelper.Instance.Register<ActorChangeModeResponse>((int)SceneProtocl.ActorChangeModeResp);
        ProtoHelper.Instance.Register<ActorChangeStateRequest>((int)SceneProtocl.ActorChangeStateReq);
        ProtoHelper.Instance.Register<ActorChangeStateResponse>((int)SceneProtocl.ActorChangeStateResp);
        ProtoHelper.Instance.Register<ActorChangeMotionDataRequest>((int)SceneProtocl.ActorChangeMotionDataReq);
        ProtoHelper.Instance.Register<ActorChangeMotionDataResponse>((int)SceneProtocl.ActorChangeMotionDataResp);

        // 消息的订阅
        MessageRouter.Instance.Subscribe<OtherEntityEnterSceneResponse>(_HandleOtherEntityEnterSceneResponse);
        MessageRouter.Instance.Subscribe<OtherEntityLeaveSceneResponse>(_HandleOtherEntityLeaveSceneResponse);
        MessageRouter.Instance.Subscribe<ActorChangeModeResponse>(_HandleActorChangeModeResponse);
        MessageRouter.Instance.Subscribe<ActorChangeStateResponse>(_HandleActorChangeStateResponse);
        MessageRouter.Instance.Subscribe<ActorChangeMotionDataResponse>(_HandleActorChangeMotionDataResponse);
    }

    private void _HandleOtherEntityEnterSceneResponse(Connection sender, OtherEntityEnterSceneResponse message)
    {
        if(GameApp.SceneId != message.SceneId)
        {
            goto End;
        }
        if(message.EntityType == SceneEntityType.Actor)
        {
            EntityManager.Instance.OnActorEnterScene(message.ActorNode);
        }else if(message.EntityType == SceneEntityType.Item)
        {
            EntityManager.Instance.OnItemEnterScene(message.ItemNode);
        }else if(message.EntityType == SceneEntityType.Interactivo)
        {
            // ...
        }
    End:
        return;
    }
    private void _HandleOtherEntityLeaveSceneResponse(Connection sender, OtherEntityLeaveSceneResponse message)
    {
        if(message.SceneId != GameApp.SceneId)
        {
            goto End;
        }
        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            EntityManager.Instance.RemoveEntity(message.EntityId);
        });
    End:
        return;
    }

    private void _HandleActorChangeModeResponse(Connection sender, ActorChangeModeResponse message)
    {
        var acotr = EntityManager.Instance.GetEntity<Actor>(message.EntityId);
        if (acotr == null)
        {
            goto End;
        }
        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            acotr.HandleActorChangeModeResponse(message);
        });
    End:
        return;
    }
    private void _HandleActorChangeStateResponse(Connection sender, ActorChangeStateResponse message)
    {
        var acotr = EntityManager.Instance.GetEntity<Actor>(message.EntityId);
        if (acotr == null)
        {
            goto End;
        }
        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            acotr.HandleActorChangeStateResponse(message);
        });
    End:
        return;
    }
    private void _HandleActorChangeMotionDataResponse(Connection sender, ActorChangeMotionDataResponse message)
    {
        var acotr = EntityManager.Instance.GetEntity<Actor>(message.EntityId);
        if (acotr == null)
        {
            goto End;
        }
        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            acotr.HandleActorMotionChangeDate(message);
        });
    End:
        return;
    }
}

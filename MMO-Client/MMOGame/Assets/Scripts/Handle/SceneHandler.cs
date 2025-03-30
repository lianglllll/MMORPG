using Common.Summer.Core;
using Common.Summer.Net;
using GameClient;
using GameClient.Entities;
using HS.Protobuf.Scene;
using HS.Protobuf.SceneEntity;
using HSFramework.MySingleton;
using Serilog;
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
        ProtoHelper.Instance.Register<ActorChangeTransformDataRequest>((int)SceneProtocl.ActorChangeTransformDataReq);
        ProtoHelper.Instance.Register<ActorChangeTransformDataResponse>((int)SceneProtocl.ActorChangeTransformDataResp);

        // 消息的订阅
        MessageRouter.Instance.Subscribe<OtherEntityEnterSceneResponse>(_HandleOtherEntityEnterSceneResponse);
        MessageRouter.Instance.Subscribe<OtherEntityLeaveSceneResponse>(_HandleOtherEntityLeaveSceneResponse);
        MessageRouter.Instance.Subscribe<ActorChangeModeResponse>(_HandleActorChangeModeResponse);
        MessageRouter.Instance.Subscribe<ActorChangeStateResponse>(_HandleActorChangeStateResponse);
        MessageRouter.Instance.Subscribe<ActorChangeTransformDataResponse>(_HandleActorChangeTransformDataResponse);
    }

    private void _HandleOtherEntityEnterSceneResponse(Connection sender, OtherEntityEnterSceneResponse message)
    {
        if(GameApp.SceneId != message.SceneId)
        {
            Log.Warning("不是本场景的消息：_HandleOtherEntityEnterSceneResponse，curSceneId = {0} , msgSceneId = {1}", GameApp.SceneId, message.SceneId);
            goto End;
        }

        // 判断是否已经存在？
        var entity = EntityManager.Instance.GetEntity<Actor>(message.ActorNode.EntityId);
        if(entity != null)
        {
            Log.Warning("msg entityId = {0}, 错误重复加入", message.ActorNode.EntityId);
            goto End;
        }

        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            if (message.EntityType == SceneEntityType.Actor)
            {
                EntityManager.Instance.OnActorEnterScene(message.ActorNode);
            }
            else if (message.EntityType == SceneEntityType.Item)
            {
                EntityManager.Instance.OnItemEnterScene(message.ItemNode);
            }
            else if (message.EntityType == SceneEntityType.Interactivo)
            {
                // ...
            }
        });

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
        if(message.SceneId != GameApp.SceneId)
        {
            goto End;
        }
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
        if (message.SceneId != GameApp.SceneId)
        {
            goto End;
        }
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
    private void _HandleActorChangeTransformDataResponse(Connection sender, ActorChangeTransformDataResponse message)
    {
        if (message.SceneId != GameApp.SceneId)
        {
            goto End;
        }
        var acotr = EntityManager.Instance.GetEntity<Actor>(message.EntityId);
        if (acotr == null)
        {
            Log.Warning("SceneHandler:_HandleActorChangeTransformDataResponse不存在该actor");
            goto End;
        }
        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            acotr.HandleActorChangeTransformDate(message);
        });
    End:
        return;
    }
}

using BaseSystem.Tool.Singleton;
using Common.Summer.Core;
using Common.Summer.Net;
using GameClient;
using GameClient.Entities;
using HS.Protobuf.Login;
using HS.Protobuf.Scene;
using HS.Protobuf.SceneEntity;
using System;

public class SceneHandler : SingletonNonMono<SceneHandler>
{
    public void Init()
    {
        // 协议注册
        ProtoHelper.Instance.Register<OtherEntityEnterSceneResponse>((int)SceneProtocl.OtherEntityEnterSceneResp);

        // 消息的订阅
        MessageRouter.Instance.Subscribe<OtherEntityEnterSceneResponse>(_HandleOtherEntityEnterSceneResponse);

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
}

using HSFramework.MySingleton;
using Common.Summer.Core;
using Common.Summer.Net;
using HS.Protobuf.Chat;
using System;
using UnityEngine;
using HS.Protobuf.GameTask;
using GameClient;

public class TaskHandler : SingletonNonMono<TaskHandler>
{
    public bool Init()
    {
        // 协议注册
        ProtoHelper.Instance.Register<GetAllGameTasksRequest>((int)GameTaskProtocol.GetAllGameTasksReq);
        ProtoHelper.Instance.Register<GetAllGameTaskResponse>((int)GameTaskProtocol.GetAllGameTasksResp);
        // 消息的订阅
        MessageRouter.Instance.Subscribe<GetAllGameTaskResponse>(HandleGetAllGameTaskResponse);

        return true;
    }
    public bool UnInit()
    {
        MessageRouter.Instance.UnSubscribe<GetAllGameTaskResponse>(HandleGetAllGameTaskResponse);
        return true;
    }

    public bool SendGetAllGameTasksRequest()
    {
        var req = new GetAllGameTasksRequest();
        req.SessionId = NetManager.Instance.sessionId;
        req.CId = GameApp.chrId;
        NetManager.Instance.Send(req);
        return true;
    }
    private void HandleGetAllGameTaskResponse(Connection sender, GetAllGameTaskResponse message)
    {
        TaskDataManager.Instance.HandleGetAllGameTaskResponse(message.Tasks);
    }
}

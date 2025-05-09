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
        ProtoHelper.Instance.Register<GameTaskChangeOperationResponse>((int)GameTaskProtocol.GameTaskChangeOperationResp);
        ProtoHelper.Instance.Register<TakeGameTaskRequest>((int)GameTaskProtocol.TakeGameTasksReq);
        ProtoHelper.Instance.Register<ReTakeGameTaskRequest>((int)GameTaskProtocol.ReTakeGameTasksReq);
        ProtoHelper.Instance.Register<ClaimTaskRewardsRequest>((int)GameTaskProtocol.ClaimTaskRewardsReq);
        // 消息的订阅
        MessageRouter.Instance.Subscribe<GetAllGameTaskResponse>(HandleGetAllGameTaskResponse);
        MessageRouter.Instance.Subscribe<GameTaskChangeOperationResponse>(HandleGameTaskChangeOperationResponse);

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

    private void HandleGameTaskChangeOperationResponse(Connection sender, GameTaskChangeOperationResponse message)
    {
        UnityMainThreadDispatcher.Instance().Enqueue(() => {
            switch (message.Opration)
            {
                case GameTaskChangeOperationType.State:
                    TaskDataManager.Instance.ChangeGameTaskState(message.TaskId, message.NewState, message.NewConditions);
                    break;
                case GameTaskChangeOperationType.Condition:
                    TaskDataManager.Instance.ChangeGameTaskConditions(message.TaskId, message.NewConditions);
                    break;
                case GameTaskChangeOperationType.Add:
                    TaskDataManager.Instance.AddGameTask(message.NewNode);
                    break;
                case GameTaskChangeOperationType.Remove:
                    TaskDataManager.Instance.RemoveGameTask(message.TaskId);
                    break;
            }
        });
    }

    public bool SendTakeGameTaskRequest(int taskId)
    {
        var req = new TakeGameTaskRequest();
        req.TaskId = taskId;
        NetManager.Instance.Send(req);
        return true;
    }
    public bool SendReTakeGameTaskRequest(int taskId)
    {
        var req = new ReTakeGameTaskRequest();
        req.TaskId = taskId;
        NetManager.Instance.Send(req);
        return true;
    }
    public bool SendClaimTaskRewardsRequest(int taskId)
    {
        var req = new ClaimTaskRewardsRequest();
        req.TaskId = taskId;
        NetManager.Instance.Send(req);
        return true;
    }
}

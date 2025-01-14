using BaseSystem.Tool.Singleton;
using Common.Summer.Core;
using Common.Summer.Net;
using HS.Protobuf.Login;
using System;

public class EntryGameWorldService : SingletonNonMono<EntryGameWorldService>
{
    public void Init()
    {
        // 协议注册
        ProtoHelper.Instance.Register<GetAllWorldInfosRequest>((int)LoginProtocl.GetAllWorldInfoNodeReq);
        ProtoHelper.Instance.Register<GetAllWorldInfosResponse>((int)LoginProtocl.GetAllWorldInfoNodeResp);

        // 消息的订阅
        MessageRouter.Instance.Subscribe<GetAllWorldInfosResponse>(_HandleGetAllWorldInfosResponse);
    }
    public void UnInit()
    {
        MessageRouter.Instance.UnSubscribe<GetAllWorldInfosResponse>(_HandleGetAllWorldInfosResponse);
    }

    public void SendGetAllWorldInfosRequest()
    {
        GetAllWorldInfosRequest req = new();
        req.LoginGateToken = NetManager.Instance.m_loginGateToken;
        NetManager.Instance.SendToLoginGate(req);
    }
    private void _HandleGetAllWorldInfosResponse(Connection sender, GetAllWorldInfosResponse message)
    {
        var panel = UIManager.Instance.GetPanelByName("SelectWorldPanel");
        if (panel == null) return;
        UnityMainThreadDispatcher.Instance().Enqueue(() => {
            (panel as SelectWorldPanel).HandleGetAllWorldInfosResponse(message);
        });
    }




}

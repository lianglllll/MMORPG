using BaseSystem.Tool.Singleton;
using Common.Summer.Core;
using Common.Summer.Net;
using GameClient;
using HS.Protobuf.Game;
using HS.Protobuf.GameGate;
using HS.Protobuf.Login;
using HS.Protobuf.Scene;
using System;

public class EntryGameWorldService : SingletonNonMono<EntryGameWorldService>
{
    public void Init()
    {
        // 协议注册
        ProtoHelper.Instance.Register<GetAllWorldInfosRequest>((int)LoginProtocl.GetAllWorldInfoNodeReq);
        ProtoHelper.Instance.Register<GetAllWorldInfosResponse>((int)LoginProtocl.GetAllWorldInfoNodeResp);

        ProtoHelper.Instance.Register<GetCharacterListRequest>((int)GameProtocl.GetCharacterListReq);
        ProtoHelper.Instance.Register<GetCharacterListResponse>((int)GameProtocl.GetCharacterListResp);

        ProtoHelper.Instance.Register<CreateCharacterRequest>((int)GameProtocl.CreateCharacterReq);
        ProtoHelper.Instance.Register<CreateCharacterResponse>((int)GameProtocl.CreateCharacterResp);

        ProtoHelper.Instance.Register<DeleteCharacterRequest>((int)GameProtocl.DeleteCharacterReq);
        ProtoHelper.Instance.Register<DeleteCharacterResponse>((int)GameProtocl.DeleteCharacterResp);

        ProtoHelper.Instance.Register<EnterGameRequest>((int)GameProtocl.EnterGameReq);
        ProtoHelper.Instance.Register<EnterGameResponse>((int)GameProtocl.EnterGameResp);

        // 消息的订阅
        MessageRouter.Instance.Subscribe<GetAllWorldInfosResponse>(_HandleGetAllWorldInfosResponse);
        MessageRouter.Instance.Subscribe<GetCharacterListResponse>(_HandleGetCharacterListResponse);
        MessageRouter.Instance.Subscribe<CreateCharacterResponse>(_HandleCreateCharacterResponse);
        MessageRouter.Instance.Subscribe<DeleteCharacterResponse>(_HandleDeleteCharacterResponse);
        MessageRouter.Instance.Subscribe<EnterGameResponse>(_HandleEnterGameResponse);
    }
    public void UnInit()
    {
        MessageRouter.Instance.UnSubscribe<GetAllWorldInfosResponse>(_HandleGetAllWorldInfosResponse);
        MessageRouter.Instance.UnSubscribe<GetCharacterListResponse>(_HandleGetCharacterListResponse);
        MessageRouter.Instance.UnSubscribe<CreateCharacterResponse>(_HandleCreateCharacterResponse);
        MessageRouter.Instance.UnSubscribe<DeleteCharacterResponse>(_HandleDeleteCharacterResponse);
        MessageRouter.Instance.UnSubscribe<EnterGameResponse>(_HandleEnterGameResponse);
    }

    public void SendGetAllWorldInfosRequest()
    {
        GetAllWorldInfosRequest req = new();
        req.LoginGateToken = NetManager.Instance.m_loginGateToken;
        NetManager.Instance.Send(req);
    }
    private void _HandleGetAllWorldInfosResponse(Connection sender, GetAllWorldInfosResponse message)
    {
        var panel = UIManager.Instance.GetOpeningPanelByName("SelectWorldPanel");
        if (panel == null) return;
        UnityMainThreadDispatcher.Instance().Enqueue(() => {
            (panel as SelectWorldPanel).HandleGetAllWorldInfosResponse(message);
        });
    }

    public void SendGetCharacterListRequest()
    {
        //发起角色列表的请求
        GetCharacterListRequest req = new();
        NetManager.Instance.Send(req);
    }
    private void _HandleGetCharacterListResponse(Connection sender, GetCharacterListResponse msg)
    {
        var panel = UIManager.Instance.GetOpeningPanelByName("SelectRolePanel");
        if (panel == null) return;
        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            ((SelectRolePanelScript)panel).RefreshRoleListUI(msg);
        });
    }

    public void SendCreateCharacterRequest(string roleName, int jobId)
    {
        CreateCharacterRequest req = new();
        req.Name = roleName;
        req.VocationId = jobId;
        NetManager.Instance.Send(req);
    }
    private void _HandleCreateCharacterResponse(Connection sender, CreateCharacterResponse msg)
    {
        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            //显示消息内容
            if (msg.ResultMsg != null)
            {
                UIManager.Instance.ShowTopMessage(msg.ResultMsg);
            }

            //如果成功就进行跳转
            if (msg.ResultCode == 0)
            {
                //发起角色列表的请求，刷新角色列表
                SendGetCharacterListRequest();
                CreateRolePanelScript panel = (CreateRolePanelScript)UIManager.Instance.GetOpeningPanelByName("CreateRolePanel");
                panel.OnReturnBtn();
            }

        });
    }

    public void SendDeleteCharacterRequest(string chrId)
    {
        DeleteCharacterRequest req = new();
        //req.CharacterId = chrId;
        //NetManager.Instance.Send(req);
    }
    private void _HandleDeleteCharacterResponse(Connection sender, DeleteCharacterResponse msg)
    {
        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            //显示信息
            if (msg.ResultMsg != null)
            {
                UIManager.Instance.ShowTopMessage(msg.ResultMsg);
            }
            if (msg.ResultCode == 0)
            {
                //发起角色列表的请求
                GetCharacterListRequest req = new();
                NetManager.Instance.Send(req);
            }
        });



    }

    public void SendEnterGameRequest(string cId)
    {
        //安全校验
        if (GameApp.character != null) return;

        //发送请求 
        EnterGameRequest request = new();
        //request.CharacterId = roleId;
        //NetManager.Instance.Send(request);
    }
    private void _HandleEnterGameResponse(Connection sender, EnterGameResponse msg)
    {
        //这里处理一些其他事情，比如说ui关闭的清理工作
    }
}

using BaseSystem.Tool.Singleton;
using Common.Summer.Core;
using Common.Summer.Net;
using GameClient;
using HS.Protobuf.Game;
using HS.Protobuf.Login;
using HS.Protobuf.Scene;

public class UserService : SingletonNonMono<UserService>
{
    // 初始化，gamemanager中启用
    public void Init()
    {
        ProtoHelper.Instance.Register<UserLoginRequest>((int)LoginProtocl.UserLoginReq);
        ProtoHelper.Instance.Register<UserLoginResponse>((int)LoginProtocl.UserLoginResp);
        ProtoHelper.Instance.Register<UserRegisterRequest>((int)LoginProtocl.UserRegisterReq);
        ProtoHelper.Instance.Register<UserRegisterResponse>((int)LoginProtocl.UserRegisterResp);

        MessageRouter.Instance.Subscribe<UserLoginResponse>(_HandleUserLoginResponse);
        MessageRouter.Instance.Subscribe<UserRegisterResponse>(_HandleUserRegisterResponse);


        MessageRouter.Instance.Subscribe<GameEnterResponse>(_EnterGameResponse);
        MessageRouter.Instance.Subscribe<GetCharacterListResponse>(_GetCharacterListResponse);
        MessageRouter.Instance.Subscribe<DeleteCharacterResponse>(_CharacterDeleteResponse);
        MessageRouter.Instance.Subscribe<CreateCharacterResponse>(_CharacterCreateResponse);
        MessageRouter.Instance.Subscribe<ServerInfoResponse>(_GetServerInfoResponse);
    }
    public void UnInit()
    {
        MessageRouter.Instance.UnSubscribe<UserLoginResponse>(_HandleUserLoginResponse);
        MessageRouter.Instance.UnSubscribe<UserRegisterResponse>(_HandleUserRegisterResponse);

        MessageRouter.Instance.UnSubscribe<GameEnterResponse>(_EnterGameResponse);
        MessageRouter.Instance.UnSubscribe<GetCharacterListResponse>(_GetCharacterListResponse);
        MessageRouter.Instance.UnSubscribe<DeleteCharacterResponse>(_CharacterDeleteResponse);
        MessageRouter.Instance.UnSubscribe<CreateCharacterResponse>(_CharacterCreateResponse);
    }

    public void SendUserLoginRequest(string username,string password)
    {
        UserLoginRequest req = new UserLoginRequest();
        req.Username = NetManager.Instance.curNetClient.EncryptionManager.AesEncrypt(username);
        req.Password = NetManager.Instance.curNetClient.EncryptionManager.AesEncrypt(password);
        req.LoginGateToken = NetManager.Instance.m_loginGateToken;
        NetManager.Instance.Send(req);
    }
    private void _HandleUserLoginResponse(Connection sender, UserLoginResponse msg)
    {
        var panel = UIManager.Instance.GetOpeningPanelByName("LoginPanel");
        if (panel == null) return;
        UnityMainThreadDispatcher.Instance().Enqueue(() => {
            (panel as LoginPanelScript).HandleUserLoginResponse(msg);
        });
    }
    public void SendUserRegisterRequest(string username,string password)
    {
        UserRegisterRequest req = new UserRegisterRequest();
        req.Username = NetManager.Instance.curNetClient.EncryptionManager.AesEncrypt(username);
        req.Password = NetManager.Instance.curNetClient.EncryptionManager.AesEncrypt(password);
        req.LoginGateToken = NetManager.Instance.m_loginGateToken;
        NetManager.Instance.Send(req);
    }
    private void _HandleUserRegisterResponse(Connection sender, UserRegisterResponse msg)
    {
        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            UIManager.Instance.ShowTopMessage(msg.ResultMsg);

            switch (msg.ResultCode)
            {
                case 0://成功,跳转到选择角色页面

                    RegisterPanelScript panel = (RegisterPanelScript)UIManager.Instance.GetOpeningPanelByName("RegisterPanel");
                    panel.OnReturn();
                    break;
                case 1:

                    break;
            }
        });
    }

    public void GetCharacterListRequest()
    {
        //发起角色列表的请求
        GetCharacterListRequest req = new();
        NetManager.Instance.Send(req);
    }
    private void _GetCharacterListResponse(Connection sender, GetCharacterListResponse msg)
    {
        var panel = UIManager.Instance.GetOpeningPanelByName("SelectRolePanel");
        if (panel == null) return;
        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            ((SelectRolePanelScript)panel).RefreshRoleListUI(msg);
        });
    }

    public void CharacterCreateRequest(string roleName,int jobId)
    {
        CreateCharacterRequest req = new();
        req.Name = roleName;
        req.VocationId = jobId;
        NetManager.Instance.Send(req);
    }
    private void _CharacterCreateResponse(Connection sender, CreateCharacterResponse msg)
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
                UserService.Instance.GetCharacterListRequest();
                CreateRolePanelScript panel = (CreateRolePanelScript)UIManager.Instance.GetOpeningPanelByName("CreateRolePanel");
                panel.OnReturnBtn();
            }

        });


    }

    public void CharacterDeleteRequest(int chrId)
    {
        DeleteCharacterRequest req = new();
        req.CharacterId = chrId;
        NetManager.Instance.Send(req);
    }
    private void _CharacterDeleteResponse(Connection sender, DeleteCharacterResponse msg)
    {
        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            //显示信息
            if (msg.ResultMsg != null) {
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

    public void EnterGameRequest(int roleId)
    {
        //安全校验
        if (GameApp.character != null) return;

        //发送请求 
        GameEnterRequest request = new GameEnterRequest();
        request.CharacterId = roleId;
        NetManager.Instance.Send(request);
    }
    private void _EnterGameResponse(Connection sender, GameEnterResponse msg)
    {
        //这里处理一些其他事情，比如说ui关闭的清理工作
    }

    public void GetServerInfoRequest()
    {
        NetManager.Instance.Send(new ServerInfoRequest());
    }
    private void _GetServerInfoResponse(Connection sender, ServerInfoResponse message)
    {
        var panel = UIManager.Instance.GetOpeningPanelByName("LoginPanel");
        if (panel == null) return;
        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            ((LoginPanelScript)panel).OnServerInfoResponse(message);
        });


    }

}

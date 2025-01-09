using BaseSystem.Tool.Singleton;
using Common.Summer.Core;
using Common.Summer.Net;
using GameClient;
using HS.Protobuf.Login;
using HS.Protobuf.Scene;

public class UserService : SingletonNonMono<UserService>
{
    // 初始化，gamemanager中启用
    public void Init()
    {
        ProtoHelper.Instance.Register<UserLoginRequest>((int)LoginProtocl.UserLoginRequest);
        ProtoHelper.Instance.Register<UserLoginResponse>((int)LoginProtocl.UserLoginResponse);
        ProtoHelper.Instance.Register<UserRegisterRequest>((int)LoginProtocl.UserRegisterRequest);
        ProtoHelper.Instance.Register<UserRegisterResponse>((int)LoginProtocl.UserRegisterResponse);

        MessageRouter.Instance.Subscribe<UserLoginResponse>(_HandleUserLoginResponse);
        MessageRouter.Instance.Subscribe<UserRegisterResponse>(_HandleUserRegisterResponse);


        MessageRouter.Instance.Subscribe<GameEnterResponse>(_EnterGameResponse);
        MessageRouter.Instance.Subscribe<CharacterListResponse>(_GetCharacterListResponse);
        MessageRouter.Instance.Subscribe<CharacterDeleteResponse>(_CharacterDeleteResponse);
        MessageRouter.Instance.Subscribe<CharacterCreateResponse>(_CharacterCreateResponse);
        MessageRouter.Instance.Subscribe<ServerInfoResponse>(_GetServerInfoResponse);
    }
    public void UnInit()
    {
        MessageRouter.Instance.UnSubscribe<UserLoginResponse>(_HandleUserLoginResponse);
        MessageRouter.Instance.UnSubscribe<GameEnterResponse>(_EnterGameResponse);
        MessageRouter.Instance.UnSubscribe<CharacterListResponse>(_GetCharacterListResponse);
        MessageRouter.Instance.UnSubscribe<CharacterDeleteResponse>(_CharacterDeleteResponse);
        MessageRouter.Instance.UnSubscribe<UserRegisterResponse>(_HandleUserRegisterResponse);
        MessageRouter.Instance.UnSubscribe<CharacterCreateResponse>(_CharacterCreateResponse);
    }

    public void SendUserLoginRequest(string username,string password)
    {
        UserLoginRequest req = new UserLoginRequest();
        req.Username = NetManager.Instance.m_curNetClient.EncryptionManager.AesEncrypt(username);
        req.Password = NetManager.Instance.m_curNetClient.EncryptionManager.AesEncrypt(password);
        req.LoginGateToken = NetManager.Instance.m_loginGateToken;
        NetManager.Instance.m_curNetClient.Send(req);
    }
    private void _HandleUserLoginResponse(Connection sender, UserLoginResponse msg)
    {
        var panel = UIManager.Instance.GetPanelByName("LoginPanel");
        if (panel == null) return;
        UnityMainThreadDispatcher.Instance().Enqueue(() => {
            (panel as LoginPanelScript).HandleUserLoginResponse(msg);
        });
    }
    public void SendUserRegisterRequest(string username,string password)
    {
        UserRegisterRequest req = new UserRegisterRequest();
        req.Username = NetManager.Instance.m_curNetClient.EncryptionManager.AesEncrypt(username);
        req.Password = NetManager.Instance.m_curNetClient.EncryptionManager.AesEncrypt(password);
        req.LoginGateToken = NetManager.Instance.m_loginGateToken;
        NetManager.Instance.m_curNetClient.Send(req);
    }
    private void _HandleUserRegisterResponse(Connection sender, UserRegisterResponse msg)
    {
        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            UIManager.Instance.ShowTopMessage(msg.ResultMsg);

            switch (msg.ResultCode)
            {
                case 0://成功,跳转到选择角色页面

                    RegisterPanelScript panel = (RegisterPanelScript)UIManager.Instance.GetPanelByName("RegisterPanel");
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
        CharacterListRequest req = new CharacterListRequest();
        NetManager.Instance.m_curNetClient.Send(req);
    }
    private void _GetCharacterListResponse(Connection sender, CharacterListResponse msg)
    {
        var panel = UIManager.Instance.GetPanelByName("SelectRolePanel");
        if (panel == null) return;
        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            ((SelectRolePanelScript)panel).RefreshRoleListUI(msg);
        });
    }

    public void CharacterCreateRequest(string roleName,int jobId)
    {
        CharacterCreateRequest req = new CharacterCreateRequest();
        req.Name = roleName;
        req.JobType = jobId;
        NetManager.Instance.m_curNetClient.Send(req);
    }
    private void _CharacterCreateResponse(Connection sender, CharacterCreateResponse msg)
    {
        //显示消息内容
        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            UIManager.Instance.ShowTopMessage(msg.Message);
        });

        //如果成功就进行跳转
        if (msg.Success)
        {
            //发起角色列表的请求，刷新角色列表
            UserService.Instance.GetCharacterListRequest();
            UnityMainThreadDispatcher.Instance().Enqueue(() => {
                CreateRolePanelScript panel = (CreateRolePanelScript)UIManager.Instance.GetPanelByName("CreateRolePanel");
                panel.OnReturnBtn();
            });
        }
    }

    public void CharacterDeleteRequest(int chrId)
    {
        CharacterDeleteRequest req = new CharacterDeleteRequest();
        req.CharacterId = chrId;
        NetManager.Instance.m_curNetClient.Send(req);
    }
    private void _CharacterDeleteResponse(Connection sender, CharacterDeleteResponse msg)
    {
        //显示信息
        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            UIManager.Instance.ShowTopMessage(msg.Message);
        });

        if (msg.Success)
        {
            //发起角色列表的请求
            CharacterListRequest req = new CharacterListRequest();
            NetManager.Instance.m_curNetClient.Send(req);
        }

    }

    public void EnterGameRequest(int roleId)
    {
        //安全校验
        if (GameApp.character != null) return;

        //发送请求 
        GameEnterRequest request = new GameEnterRequest();
        request.CharacterId = roleId;
        NetManager.Instance.m_curNetClient.Send(request);
    }
    private void _EnterGameResponse(Connection sender, GameEnterResponse msg)
    {
        //这里处理一些其他事情，比如说ui关闭的清理工作
    }

    public void GetServerInfoRequest()
    {
        NetManager.Instance.m_curNetClient.Send(new ServerInfoRequest());
    }
    private void _GetServerInfoResponse(Connection sender, ServerInfoResponse message)
    {
        var panel = UIManager.Instance.GetPanelByName("LoginPanel");
        if (panel == null) return;
        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            ((LoginPanelScript)panel).OnServerInfoResponse(message);
        });


    }

}

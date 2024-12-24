using GameClient;
using Proto;
using Summer;
using Summer.Network;
using System;

public class UserService : Singleton<UserService>
{
    // 初始化，gamemanager中启用
    public void Init()
    {
        MessageRouter.Instance.Subscribe<UserLoginResponse>(_UserLoginResponse);
        MessageRouter.Instance.Subscribe<GameEnterResponse>(_EnterGameResponse);
        MessageRouter.Instance.Subscribe<CharacterListResponse>(_GetCharacterListResponse);
        MessageRouter.Instance.Subscribe<CharacterDeleteResponse>(_CharacterDeleteResponse);
        MessageRouter.Instance.Subscribe<UserRegisterResponse>(_UserRegisterResponse);
        MessageRouter.Instance.Subscribe<CharacterCreateResponse>(_CharacterCreateResponse);
        MessageRouter.Instance.Subscribe<ServerInfoResponse>(_GetServerInfoResponse);
    }
    public void UnInit()
    {
        MessageRouter.Instance.Off<UserLoginResponse>(_UserLoginResponse);
        MessageRouter.Instance.Off<GameEnterResponse>(_EnterGameResponse);
        MessageRouter.Instance.Off<CharacterListResponse>(_GetCharacterListResponse);
        MessageRouter.Instance.Off<CharacterDeleteResponse>(_CharacterDeleteResponse);
        MessageRouter.Instance.Off<UserRegisterResponse>(_UserRegisterResponse);
        MessageRouter.Instance.Off<CharacterCreateResponse>(_CharacterCreateResponse);
    }

    public void UserLoginRequest(string username,string password)
    {
        UserLoginRequest loginRequest = new UserLoginRequest();
        loginRequest.Username = username;
        loginRequest.Password = password;
        NetClient.Send(loginRequest);
    }
    private void _UserLoginResponse(Connection sender, UserLoginResponse msg)
    {
        var panel = UIManager.Instance.GetPanelByName("LoginPanel");
        if (panel == null) return;
        (panel as LoginPanelScript).OnLoginResponse(msg);
    }

    public void UserRegisterRequest(string username,string password)
    {
        UserRegisterRequest req = new UserRegisterRequest();
        req.Username = username;
        req.Password = password;
        NetClient.Send(req);
    }
    private void _UserRegisterResponse(Connection sender, UserRegisterResponse msg)
    {
        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            UIManager.Instance.ShowTopMessage(msg.Message);

            switch (msg.Code)
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
        NetClient.Send(req);
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
        NetClient.Send(req);
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
        NetClient.Send(req);
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
            NetClient.Send(req);
        }

    }

    public void EnterGameRequest(int roleId)
    {
        //安全校验
        if (GameApp.character != null) return;

        //发送请求 
        GameEnterRequest request = new GameEnterRequest();
        request.CharacterId = roleId;
        NetClient.Send(request);
    }
    private void _EnterGameResponse(Connection sender, GameEnterResponse msg)
    {
        //这里处理一些其他事情，比如说ui关闭的清理工作
    }

    public void GetServerInfoRequest()
    {
        NetClient.Send(new ServerInfoRequest());
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

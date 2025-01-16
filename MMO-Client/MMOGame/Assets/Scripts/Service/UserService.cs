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
        // 协议注册
        ProtoHelper.Instance.Register<UserLoginRequest>((int)LoginProtocl.UserLoginReq);
        ProtoHelper.Instance.Register<UserLoginResponse>((int)LoginProtocl.UserLoginResp);
        ProtoHelper.Instance.Register<UserRegisterRequest>((int)LoginProtocl.UserRegisterReq);
        ProtoHelper.Instance.Register<UserRegisterResponse>((int)LoginProtocl.UserRegisterResp);
        // 消息的订阅
        MessageRouter.Instance.Subscribe<UserLoginResponse>(_HandleUserLoginResponse);
        MessageRouter.Instance.Subscribe<UserRegisterResponse>(_HandleUserRegisterResponse);
        MessageRouter.Instance.Subscribe<ServerInfoResponse>(_HandleGetServerInfoResponse);
    }
    public void UnInit()
    {
        MessageRouter.Instance.UnSubscribe<UserLoginResponse>(_HandleUserLoginResponse);
        MessageRouter.Instance.UnSubscribe<UserRegisterResponse>(_HandleUserRegisterResponse);
        MessageRouter.Instance.UnSubscribe<ServerInfoResponse>(_HandleGetServerInfoResponse);
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

    public void GetServerInfoRequest()
    {
        NetManager.Instance.Send(new ServerInfoRequest());
    }
    private void _HandleGetServerInfoResponse(Connection sender, ServerInfoResponse message)
    {
        var panel = UIManager.Instance.GetOpeningPanelByName("LoginPanel");
        if (panel == null) return;
        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            ((LoginPanelScript)panel).OnServerInfoResponse(message);
        });


    }

}

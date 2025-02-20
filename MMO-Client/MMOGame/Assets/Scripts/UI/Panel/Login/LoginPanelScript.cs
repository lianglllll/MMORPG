using GameClient;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using HS.Protobuf.Login;
using GameClient.HSFramework;
public class LoginPanelScript : BasePanel
{
    private bool isOnClickLoginBtn;             //是否已经点击登录了，这里需要等响应回来

    private Transform loginBox;
    private Transform ServerInfoBox;
    private InputField usernameInputField;
    private InputField passwordInputField;
    private Button loginButton;
    private Button registerButton;
    private Button ExitButton;
    private Toggle recordUsernameAndPassword;

    //curServerinfo
    private Button RefreshServerInfoBtn;
    private Text OnlinePlayerCountText;
    private Text UserCountText;
    private Transform Content;
    private GameObject contentNode;
    protected override void Awake()
    {
        usernameInputField = transform.Find("Login-box/UsernameInputField").GetComponent<InputField>();
        passwordInputField = transform.Find("Login-box/PasswordInputField").GetComponent<InputField>();
        loginButton = transform.Find("Login-box/LoginButton").GetComponent<Button>();
        registerButton = transform.Find("Login-box/RegisterButton").GetComponent<Button>();
        recordUsernameAndPassword = transform.Find("Login-box/RecordToggle").GetComponent<Toggle>();
        loginBox = transform.Find("Login-box");
        ServerInfoBox = transform.Find("ServerInfoBox");
        ExitButton = transform.Find("ExitBtn").GetComponent<Button>();

        RefreshServerInfoBtn = transform.Find("ServerInfoBox/UpdateServerInfo").GetComponent<Button>();
        OnlinePlayerCountText = transform.Find("ServerInfoBox/OnlinePlayerCountText").GetComponent<Text>();
        UserCountText = transform.Find("ServerInfoBox/UserCountText").GetComponent<Text>();
        Content = transform.Find("ServerInfoBox/KillRank/Content");
        contentNode = transform.Find("ServerInfoBox/KillRank/ContentNode").gameObject;
        contentNode.SetActive(false);

    }
    protected override void Start()
    {
        passwordInputField.contentType = InputField.ContentType.Password;
        loginButton.onClick.AddListener(OnLoginBtn);
        registerButton.onClick.AddListener(OnRegisterBtn);
        ExitButton.onClick.AddListener(OnExitBtn);
        RefreshServerInfoBtn.onClick.AddListener(OnRefreshServerInfoBtn);
        isOnClickLoginBtn = false;
        OnRefreshServerInfoBtn();

        Init();

    }
    private void OnDestroy()
    {
        UnInit();
    }
    private void Update()
    {
        if (GameInputManager.Instance.Space)
        {
            OnLoginBtn();
        }
    }


    private void Init()
    {
        //加载上次缓存的用户名和密码
        string myUsername = PlayerPrefs.GetString("myUsername");
        string myPassword = PlayerPrefs.GetString("myPassword");
        if (!string.IsNullOrEmpty(myUsername))
        {
            usernameInputField.text = myUsername;
        }
        if (!string.IsNullOrEmpty(myPassword))
        {
            passwordInputField.text = myPassword;
        }

        //给登录框弄点移动效果
        loginBox.DOLocalMoveX(transform.localPosition.x + 2000f, 3f).From();
        ServerInfoBox.DOLocalMoveX(transform.localPosition.x - 4000f, 3f).From();
    }
    private void UnInit()
    {

    }

    private void OnLoginBtn()
    {
        GlobalAudioManager.Instance.PlayUIAudio(UIAudioClipType.ButtonClick);

        //与服务器没有建立连接时
        if (NetManager.Instance.curNetClient == null)
        {
            UIManager.Instance.MessagePanel.ShowTopMsg("正在帮您连接服务器...");
            goto End;
        }

        //防止多次连续点击
        if (isOnClickLoginBtn)
        {
            UIManager.Instance.MessagePanel.ShowTopMsg("您点击太快了...");
            goto End;
        }
        isOnClickLoginBtn = true;

        string username = usernameInputField.text;
        string password = passwordInputField.text;
        if (username.Equals("") ||password.Equals(""))
        {
            UIManager.Instance.MessagePanel.ShowTopMsg("登录名或密码不能为空！");
            goto End;
        }

        //向server发送登录请求
        UserService.Instance.SendUserLoginRequest(username, password);
    End:
        return;
    }
    public void HandleUserLoginResponse(UserLoginResponse msg)
    {
        //登录成功，切换到角色选择scene
        if (msg.ResultCode == 0)
        {
            //保存SessionId
            NetManager.Instance.sessionId = msg.SessionId;
            if (recordUsernameAndPassword.isOn)
            {
                //记录用户名和密码
                PlayerPrefs.SetString("myUsername", usernameInputField.text);
                PlayerPrefs.SetString("myPassword", passwordInputField.text);
                PlayerPrefs.Save();
            }
            UIManager.Instance.ShowTopMessage(msg.ResultMsg);
            //切换面板
            UIManager.Instance.ExchangePanelWithFade("LoginPanel", "SelectWorldPanel");
        }
        else
        {
            isOnClickLoginBtn = false;
            UIManager.Instance.ShowTopMessage(msg.ResultMsg);
        }

    }

    private void OnRegisterBtn()
    {
        GlobalAudioManager.Instance.PlayUIAudio(UIAudioClipType.ButtonClick);
        UIManager.Instance.OpenPanelWithFade("RegisterPanel");
    }

    private void OnRefreshServerInfoBtn()
    {
        UserService.Instance.GetServerInfoRequest();
    }
    public void OnServerInfoResponse(ServerInfoResponse message)
    {
        OnlinePlayerCountText.text = "当前服务器在线人数：" + message.OnlinePlayerCount;
        UserCountText.text = "当前服务器注册人数：" + message.UserCount;

        //清理
        foreach(Transform child in Content)
        {
            Destroy(child.gameObject);
        }

        //添加
        int index = 0;
        foreach(var item in message.KillRankingList)
        {
            index++;
            var obj = Instantiate(contentNode, Content);
            obj.GetComponent<Text>().text = $"{index}.{item.ChrName}-击杀数量：{item.KillCount}";
            obj.SetActive(true);
        }


    }

    private void OnExitBtn()
    {
        GlobalAudioManager.Instance.PlayUIAudio(UIAudioClipType.ButtonClick);

        //弹框提示
        UIManager.Instance.MessagePanel.ShowSelectionPanel("退出游戏", "是否退出游戏？", () =>
        {
#if (UNITY_EDITOR)
            {
                UnityEditor.EditorApplication.isPlaying = false;
            }
#else
            {
                Application.Quit();
            }
#endif
        });
    }
}

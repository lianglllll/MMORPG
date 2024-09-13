using GameClient;
using Proto;
using Summer.Network;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;


public class LoginPanelScript : BasePanel
{
    private Transform loginBox;
    private Transform ServerInfoBox;
    private InputField usernameInputField;
    private InputField passwordInputField;
    private Button loginButton;
    private Button registerButton;
    private Button ExitButton;
    private Toggle recordUsernameAndPassword;
    private bool isOnClickLoginBtn;             //是否已经点击登录了，这里需要等响应回来


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
    }

    protected override void Start()
    {
        passwordInputField.contentType = InputField.ContentType.Password;
        loginButton.onClick.AddListener(OnLogin);
        registerButton.onClick.AddListener(OnRegister);
        ExitButton.onClick.AddListener(OnExitBtn);
        isOnClickLoginBtn = false;
        Init();
    }

    private void Init()
    {
        //加载用户名和密码
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
        loginBox.DOLocalMoveX(transform.localPosition.x + 2000f, 2f).From();
        ServerInfoBox.DOLocalMoveX(transform.localPosition.x  -4000f, 2f).From();
    }


    /// <summary>
    /// 登录按钮回调
    /// </summary>
    private void OnLogin()
    {

        //防止多次连续点击
        if (isOnClickLoginBtn) return;

        //与服务器没有建立连接时
        if (!NetManager.Instance.isConnected)
        {
            UIManager.Instance.MessagePanel.ShowTopMsg("正在帮您连接服务器.....");
            NetManager.Instance.ConnectToServer();
            return;
        }

        string username = usernameInputField.text;
        string password = passwordInputField.text;
        if (username.Equals("") ||password.Equals(""))
        {
            UIManager.Instance.MessagePanel.ShowTopMsg("登录名或密码不能为空！");
            return;
        }

        isOnClickLoginBtn = true;

        //向server发送登录请求
        UserService.Instance._UserLoginRequest(username, password);
    }

    /// <summary>
    /// 登录事件触发的响应
    /// </summary>
    /// <param name="msg"></param>
    public void OnLoginResponse(UserLoginResponse msg)
    {

        //登录成功，切换到角色选择scene
        if (msg.Success)
        {

            //保存SessionId
            GameApp.SessionId = msg.SessionId;
            //切换面板
            UnityMainThreadDispatcher.Instance().Enqueue(()=> {

                if (recordUsernameAndPassword.isOn)
                {
                    //记录用户名和密码
                    PlayerPrefs.SetString("myUsername", usernameInputField.text);
                    PlayerPrefs.SetString("myPassword", passwordInputField.text);
                    PlayerPrefs.Save();
                }

                UnityMainThreadDispatcher.Instance().StartCoroutine(_OnLoginResponse());
            });
        } 
        else
        {
            isOnClickLoginBtn = false;
            UnityMainThreadDispatcher.Instance().Enqueue(() => {
                UIManager.Instance.ShowTopMessage("登录失败！！");
            });
        }

    }
    private IEnumerator _OnLoginResponse()
    {
        UIManager.Instance.ShowTopMessage("登录成功");

        yield return new WaitForSeconds(0.5f);

        yield return  ScenePoster.Instance.FadeIn();
        UIManager.Instance.OpenPanel("SelectRolePanel");
        UIManager.Instance.ClosePanel("LoginPanel");
        yield return ScenePoster.Instance.FadeOut();
    }

    /// <summary>
    /// 注册按钮触发
    /// </summary>
    private void OnRegister()
    {
        StartCoroutine(_OnRegister());
    }
    private IEnumerator _OnRegister()
    {
        yield return ScenePoster.Instance.FadeIn();

        //切换到registerpanel
        UIManager.Instance.OpenPanel("RegisterPanel");

        yield return ScenePoster.Instance.FadeOut();

    }


    private void OnExitBtn()
    {
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

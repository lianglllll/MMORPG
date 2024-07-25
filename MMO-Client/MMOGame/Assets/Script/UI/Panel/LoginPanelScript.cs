using Proto;
using Summer.Network;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class LoginPanelScript : BasePanel
{

    private InputField usernameInputField;
    private InputField passwordInputField;
    private Button loginButton;
    private Button registerButton;

    private bool isOnClickLoginBtn;             //是否已经点击登录了，这里需要等响应回来

    protected override void Awake()
    {
        usernameInputField = transform.Find("Login-box/UsernameInputField").GetComponent<InputField>();
        passwordInputField = transform.Find("Login-box/PasswordInputField").GetComponent<InputField>();
        loginButton = transform.Find("Login-box/LoginButton").GetComponent<Button>();
        registerButton = transform.Find("Login-box/RegisterButton").GetComponent<Button>();
    }

    protected override void Start()
    {
        passwordInputField.contentType = InputField.ContentType.Password;
        loginButton.onClick.AddListener(OnLogin);
        registerButton.onClick.AddListener(OnRegister);
        isOnClickLoginBtn = false;
    }

    /// <summary>
    /// 登录按钮回调
    /// </summary>
    private void OnLogin()
    {

        //防止多次连续点击
        if (isOnClickLoginBtn) return;

        //与服务器没有建立连接时
        if (!NetStart.Instance.isConnectServer)
        {
            UIManager.Instance.MessagePanel.ShowTopMsg("正在帮您连接服务器.....");
            NetStart.Instance.ConnectToServer();
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
            UnityMainThreadDispatcher.Instance().Enqueue(() =>
            {
                UIManager.Instance.ShowTopMessage("登录成功");
                UIManager.Instance.OpenPanel("SelectRolePanel");
                UIManager.Instance.ClosePanel("LoginPanel");
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

    /// <summary>
    /// 注册按钮触发
    /// </summary>
    private void OnRegister()
    {
        //切换到registerpanel
        UIManager.Instance.OpenPanel("RegisterPanel");
    }

}

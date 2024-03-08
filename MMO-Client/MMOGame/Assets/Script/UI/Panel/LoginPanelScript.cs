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

    public InputField usernameInputField;
    public InputField passwordInputField;
    public Button loginButton;
    public Button registerButton;

    private bool isOnClickLoginBtn;

    protected override void Start()
    {
        loginButton.onClick.AddListener(OnLogin);
        registerButton.onClick.AddListener(OnRegister);
        isOnClickLoginBtn = false;

        MessageRouter.Instance.Subscribe<UserLoginResponse>(OnLoginResponse);
    }

    private void OnDestroy()
    {
        MessageRouter.Instance.Off<UserLoginResponse>(OnLoginResponse);
    }

    //登录按钮触发
    private void OnLogin()
    {
        //与服务器没有建立连接时
        if (!NetStart.Instance.isConnectServer)
        {
            UIManager.Instance.MessagePanel.ShowTopMsg("正在帮您连接服务器.....");
            NetStart.Instance.ConnectToServer();
            return;
        }

        //防止多次连续点击
        if (isOnClickLoginBtn) return;

        string username = usernameInputField.text;
        string password = passwordInputField.text;
        if (username.Equals("") ||password.Equals(""))
        {
            UIManager.Instance.MessagePanel.ShowTopMsg("登录名或密码不能为空！");
            return;
        }

        isOnClickLoginBtn = true;

        //向server发送登录请求
        UserLoginRequest loginRequest = new UserLoginRequest();
        loginRequest.Username = username;
        loginRequest.Password = password;
        NetClient.Send(loginRequest);
    }




    //登录事件触发的响应
    private void OnLoginResponse(Connection conn, UserLoginResponse msg)
    {

        //登录成功，切换到角色选择scene
        if (msg.Success)
        {
            //保存SessionId
            GameApp.SessionId = msg.SessionId;
            //切换面板
            UnityMainThreadDispatcher.Instance().Enqueue(() =>
            {
                UIManager.Instance.ShowMessage("登录成功");
                UIManager.Instance.OpenPanel("SelectRolePanel");
                UIManager.Instance.ClosePanel("LoginPanel");
            });
        } 
        else
        {
            isOnClickLoginBtn = false;
            UIManager.Instance.AsyncShowMessage("登录失败！！");
        }

    }


    //注册按钮触发
    private void OnRegister()
    {
        //切换到registerpanel
        UIManager.Instance.OpenPanel("RegisterPanel");
    }


}

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


    void Start()
    {
        loginButton.onClick.AddListener(OnLogin);
        registerButton.onClick.AddListener(OnRegister);
        MessageRouter.Instance.Subscribe<UserLoginResponse>(OnLoginResponse);
    }

    private void OnDestroy()
    {
        MessageRouter.Instance.Off<UserLoginResponse>(OnLoginResponse);
    }

    //登录按钮触发
    private void OnLogin()
    {
        string username = usernameInputField.text;
        string password = passwordInputField.text;
        if (username.Equals("") ||password.Equals(""))
        {
            UIManager.Instance.MessagePanel.ShowMessage("登录名或密码不能为空！");
            return;
        }

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
            UnityMainThreadDispatcher.Instance().Enqueue(() =>
            {
                UIManager.Instance.ShowMessage("登录成功");
                UIManager.Instance.OpenPanel("SelectRolePanel");
                UIManager.Instance.ClosePanel("LoginPanel");
            });
        }
        else
        {
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

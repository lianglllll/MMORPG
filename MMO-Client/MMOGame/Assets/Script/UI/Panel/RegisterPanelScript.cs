using Proto;
using Summer.Network;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RegisterPanelScript : BasePanel
{
    public InputField usernameInputField;
    public InputField passwordInputField;
    public InputField confirmPasswordInputField;
    public Button returnButton;
    public Button registerButton;

    protected override void  Start()
    {
        returnButton.onClick.AddListener(OnReturn);
        registerButton.onClick.AddListener(OnRegister);
        MessageRouter.Instance.Subscribe<UserRegisterResponse>(OnRegisterResponse);
    }



    private void OnDestroy()
    {
        MessageRouter.Instance.Off<UserRegisterResponse>(OnRegisterResponse);
    }
    private void OnRegister()
    {
        string username = usernameInputField.text;
        string password = passwordInputField.text;
        string confirmPassword = confirmPasswordInputField.text;
        if (username.Equals("") || password.Equals("") || confirmPassword.Equals(""))
        {
            UIManager.Instance.ShowMessage("用户名或密码不能为空！");
            return;
        }

        if(password.Equals(confirmPassword) == false)
        {
            UIManager.Instance.ShowMessage("二次密码不一致");
            return;
        }

        UserRegisterRequest req = new UserRegisterRequest();
        req.Username = username;
        req.Password = password;
        NetClient.Send(req);
    }
    private void OnRegisterResponse(Connection sender, UserRegisterResponse msg)
    {
        UIManager.Instance.AsyncShowMessage(msg.Message);
    }

    private void OnReturn()
    {
        UIManager.Instance.ClosePanel("RegisterPanel");
    }


}

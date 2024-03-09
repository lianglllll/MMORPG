using Proto;
using Summer.Network;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RegisterPanelScript : BasePanel
{
    private InputField usernameInputField;
    private InputField passwordInputField;
    private InputField confirmPasswordInputField;
    private Button returnButton;
    private Button registerButton;

    protected override void Awake()
    {
        usernameInputField = transform.Find("Register-box/UsernameInputField").GetComponent<InputField>();
        passwordInputField = transform.Find("Register-box/PasswordInputField").GetComponent<InputField>();
        confirmPasswordInputField = transform.Find("Register-box/ConfirmPasswordInputField").GetComponent<InputField>();
        returnButton = transform.Find("ReturnBtn").GetComponent<Button>();
        registerButton = transform.Find("Register-box/RegisterButton").GetComponent<Button>();

    }

    protected override void  Start()
    {
        passwordInputField.contentType = InputField.ContentType.Password;
        confirmPasswordInputField.contentType = InputField.ContentType.Password;
        returnButton.onClick.AddListener(OnReturn);
        registerButton.onClick.AddListener(OnRegister);
    }

    /// <summary>
    /// 注册按钮回调
    /// </summary>
    private void OnRegister()
    {
        string username = usernameInputField.text;
        string password = passwordInputField.text;
        string confirmPassword = confirmPasswordInputField.text;
        if (username.Equals("") || password.Equals("") || confirmPassword.Equals(""))
        {
            UIManager.Instance.ShowTopMessage("用户名或密码不能为空！");
            return;
        }

        if(password.Equals(confirmPassword) == false)
        {
            UIManager.Instance.ShowTopMessage("二次密码不一致");
            return;
        }

        UserService.Instance._UserRegisterRequest(username, password);
    }

    /// <summary>
    /// 返回按钮回调
    /// </summary>
    private void OnReturn()
    {
        UIManager.Instance.ClosePanel("RegisterPanel");
    }

}

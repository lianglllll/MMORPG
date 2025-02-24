using HSFramework.Audio;
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
        GlobalAudioManager.Instance.PlayUIAudio(UIAudioClipType.ButtonClick);

        string username = usernameInputField.text;
        string password = passwordInputField.text;
        string confirmPassword = confirmPasswordInputField.text;
        if (username.Equals("") || password.Equals("") || confirmPassword.Equals(""))
        {
            UIManager.Instance.ShowTopMessage("用户名或密码不能为空！");
            goto End;
        }
        if(password.Equals(confirmPassword) == false)
        {
            UIManager.Instance.ShowTopMessage("二次密码不一致");
            goto End;
        }
        if (username.Length < 2 || username.Length > 24)
        {
            UIManager.Instance.ShowTopMessage("请限制用户名长度在2-16个字符");
            goto End;
        }
        if (password.Length < 6 || username.Length > 36)
        {
            UIManager.Instance.ShowTopMessage("请限制密码长度在6-36个字符");
            goto End;
        }

        UserService.Instance.SendUserRegisterRequest(username, password);

    End:
        return;
    }

    /// <summary>
    /// 返回按钮回调
    /// </summary>
    public void OnReturn()
    {
        GlobalAudioManager.Instance.PlayUIAudio(UIAudioClipType.ButtonClick);
        UIManager.Instance.ClosePanelWithFade("RegisterPanel");
    }
}

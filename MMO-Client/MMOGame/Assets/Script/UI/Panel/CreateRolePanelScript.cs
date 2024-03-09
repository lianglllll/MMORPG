using Proto;
using Summer.Network;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreateRolePanelScript : BasePanel
{
    //ui
    private Text SelectedJobInfo;
    private InputField usernameFileid;
    private Button returnBtn;
    private Button createBtn;

    private Button jobType0;
    private Button jobType1;
    private Button jobType2;
    private Button jobType3;
    private Button jobType4;

    private int jobid = -1;                 //选择的职业id

    protected override void Awake()
    {
        SelectedJobInfo = transform.Find("Canvas/SelectedJobInfo/Text").GetComponent<Text>();
        usernameFileid = transform.Find("Canvas/HeroNameInputField").GetComponent<InputField>();
        createBtn = transform.Find("Canvas/CreateBtn").GetComponent<Button>();
        returnBtn = transform.Find("Canvas/ReturnBtn").GetComponent<Button>();
        jobType0 = transform.Find("Canvas/HeroTypePanel/HereTypeItem0").GetComponent<Button>();
        jobType1 = transform.Find("Canvas/HeroTypePanel/HereTypeItem1").GetComponent<Button>();
        jobType2 = transform.Find("Canvas/HeroTypePanel/HereTypeItem2").GetComponent<Button>();
        jobType3 = transform.Find("Canvas/HeroTypePanel/HereTypeItem3").GetComponent<Button>();
        jobType4 = transform.Find("Canvas/HeroTypePanel/HereTypeItem4").GetComponent<Button>();

    }

    protected override void Start()
    {
        SelectedJobInfo.text = "未选中职业";

        //todo  写得有点蠢
        jobType0.onClick.AddListener(() => { jobid = 0; SelectedJobInfo.text = jobType0.transform.Find("Text").GetComponent<Text>().text; });
        jobType1.onClick.AddListener(() => { jobid = 1; SelectedJobInfo.text = jobType1.transform.Find("Text").GetComponent<Text>().text; });
        jobType2.onClick.AddListener(() => { jobid = 2; SelectedJobInfo.text = jobType2.transform.Find("Text").GetComponent<Text>().text; });
        jobType3.onClick.AddListener(() => { jobid = 3; SelectedJobInfo.text = jobType3.transform.Find("Text").GetComponent<Text>().text; });
        jobType4.onClick.AddListener(() => { jobid = 4; SelectedJobInfo.text = jobType4.transform.Find("Text").GetComponent<Text>().text; });

        createBtn.onClick.AddListener(OnCreateBtn);
        returnBtn.onClick.AddListener(OnReturnBtn);

    }


    /// <summary>
    /// 返回按钮回调
    /// </summary>
    public void OnReturnBtn()
    {
        UIManager.Instance.ClosePanel("CreateRolePanel");
    }

    /// <summary>
    /// 创建角色按钮回调
    /// </summary>
    public void OnCreateBtn()
    {
        //安全校验，姓名输入是否合理，有无选择角色
        if(jobid == -1)
        {
            UIManager.Instance.ShowTopMessage("请选择你的角色");
            return;
        }


        //发送网络请求
        UserService.Instance._CharacterCreateRequest(usernameFileid.text, jobid);

        //善后处理
        jobid = -1;
    }

}

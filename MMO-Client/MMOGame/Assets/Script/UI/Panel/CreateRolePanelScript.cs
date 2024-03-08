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

        createBtn.onClick.AddListener(OnCreateBtn);
        returnBtn.onClick.AddListener(OnReturnBtn);

        //todo  写得有点蠢
        jobType0.onClick.AddListener(() => { jobid = 0; SelectedJobInfo.text = jobType0.transform.Find("Text").GetComponent<Text>().text; });
        jobType1.onClick.AddListener(() => { jobid = 1; SelectedJobInfo.text = jobType1.transform.Find("Text").GetComponent<Text>().text; });
        jobType2.onClick.AddListener(() => { jobid = 2; SelectedJobInfo.text = jobType2.transform.Find("Text").GetComponent<Text>().text; });
        jobType3.onClick.AddListener(() => { jobid = 3; SelectedJobInfo.text = jobType3.transform.Find("Text").GetComponent<Text>().text; });

    }

    protected override void Start()
    {
        SelectedJobInfo.text = "未选中职业";

        //订阅创建角色响应消息
        MessageRouter.Instance.Subscribe<CharacterCreateResponse>(_CharacterCreateResponse);
    }


    private void OnDestroy()
    {
        MessageRouter.Instance.Off<CharacterCreateResponse>(_CharacterCreateResponse);
    }


    public void OnReturnBtn()
    {
        UIManager.Instance.ClosePanel("CreateRolePanel");
    }


    public void OnCreateBtn()
    {
        //安全校验，姓名输入是否合理，有无选择角色

        //发送网络请求
        CharacterCreateRequest req = new CharacterCreateRequest();
        req.Name = usernameFileid.text;
        req.JobType = jobid;
        NetClient.Send(req);

        //善后处理
        jobid = -1;

    }



    //=========================网络响应====================

    //创建角色响应
    private void _CharacterCreateResponse(Connection sender, CharacterCreateResponse msg)
    {

        //显示消息内容
        UIManager.Instance.AsyncShowMessage(msg.Message);

        //如果成功就进行跳转
        if (msg.Success)
        {
            //发起角色列表的请求
            CharacterListRequest req = new CharacterListRequest();
            NetClient.Send(req);
        }

    }


}

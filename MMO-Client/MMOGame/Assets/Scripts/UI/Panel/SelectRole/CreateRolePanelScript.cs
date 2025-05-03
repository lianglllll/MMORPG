using HSFramework.Audio;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CreateRolePanelScript : BasePanel
{
    //ui
    private Text SelectedJobInfo;
    private InputField usernameFileid;
    private Button createBtn;
    private Button returnBtn;

    private Transform ItemMountPoint;
    private VocationItem curItem;

    protected override void Awake()
    {
        SelectedJobInfo = transform.Find("Canvas/Below/SelectedJobInfo/Text").GetComponent<Text>();
        usernameFileid = transform.Find("Canvas/Below/HeroNameInputField").GetComponent<InputField>();
        createBtn = transform.Find("Canvas/Below/CreateBtn").GetComponent<Button>();
        returnBtn = transform.Find("Canvas/Below/ReturnBtn").GetComponent<Button>();
        ItemMountPoint = transform.Find("Canvas/Left/ItemMountPoint");
    }
    protected override void Start()
    {
        createBtn.onClick.AddListener(OnCreateBtn);
        returnBtn.onClick.AddListener(OnReturnBtn);
        Init();
    }
    public void Init()
    {
        GameObject panelPrefab = null;
        GameObject panelObject = null;
        panelPrefab = UIManager.Instance.GetPanelPrefab("VocationItem");
        var defines = LocalDataManager.Instance.m_unitDefineDict.Values;

        VocationItem first = null;
        foreach(var item in defines)
        {
            if (!item.Kind.Equals("Character")) return;
            panelObject = GameObject.Instantiate(panelPrefab, ItemMountPoint, false);
            VocationItem itemScript = panelObject.GetComponent<VocationItem>();
            itemScript.Init(this, item);

            if(first == null)
            {
                first = itemScript;
                //默认选中第一个
                StartCoroutine(SelectDefaultItem(first));
            }

        }

    }
    private IEnumerator SelectDefaultItem(VocationItem first)
    {
        yield return null;
        yield return null;
        first.OnBtn();
    }
    public void OnReturnBtn()
    {
        GlobalAudioManager.Instance.PlayUIAudio(UIAudioClipType.ButtonClick);
        UIManager.Instance.ClosePanelWithFade("CreateRolePanel");
    }
    public void OnCreateBtn()
    {
        GlobalAudioManager.Instance.PlayUIAudio(UIAudioClipType.ButtonClick);

        //安全校验，姓名输入是否合理，有无选择角色
        if (curItem == null)
        {
            UIManager.Instance.ShowTopMessage("请选择你的角色");
            return;
        }

        //发送网络请求
        EntryGameWorldHandler.Instance.SendCreateCharacterRequest(usernameFileid.text, curItem.JobId);

    }
    public void OnSelectBtn(VocationItem vocationItem)
    {
        if(curItem != null)
        {
            curItem.RestoreEffect();
        }

        curItem = vocationItem;
        curItem.SelectedEffect();

        SelectedJobInfo.text = curItem.Tname;
    }
}

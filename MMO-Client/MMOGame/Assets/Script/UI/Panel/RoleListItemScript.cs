using Proto;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoleListItemScript : MonoBehaviour
{
    private int index;                                      //ui列表中的序号
    private NetActor characterInfo;                         //角色信息的网络对象
    private SelectRolePanelScript selectRolePanelScript;
    private UnitDefine define;
    private RectTransform rectTransform;
    private Button btn;
    private Image image;
    private Text nameText;
    private Text levelText;
    private Text jobText;
    private float scaleSize = 1.1f;

    //角色名
    public string RoleName
    {
        get
        {
            return characterInfo.Name;
        }
    }
    //角色id
    public int ChrId
    {
        get
        {
            return characterInfo.Id;
        }
    }


    private void Awake()
    {
        btn = GetComponent<Button>();
        btn.onClick.AddListener(onBtn);
        image = transform.Find("Bg").GetComponent<Image>();
        nameText = transform.Find("RoleInfo/RoleName").GetComponent<Text>();
        levelText = transform.Find("RoleInfo/RoleLevel").GetComponent<Text>();
        jobText = transform.Find("RoleInfo/RoleJob").GetComponent<Text>();

    }

    // Start is called before the first frame update
    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }


    //注入信息
    public void InjectInfo(SelectRolePanelScript selectRole,NetActor nCharacter,int index)
    {
        this.selectRolePanelScript = selectRole;
        this.characterInfo = nCharacter;
        this.index = index;

        //通过jobid可以在unitdefine中找到响应的图片资源，然后进行加载
        int tid = nCharacter.Tid;
        define = DataManager.Instance.unitDict.GetValueOrDefault(tid, null);
        if(define == null)
        {
            Debug.LogError("define is null，tid is invalid");
            return;
        }

        //加载当前ui的背景图
        Sprite sprite = Res.LoadAssetSync<Sprite>(define.BgResource,FileType.Png);
        image.sprite = sprite;

        //设置item的info显示
        SetRoleInfo();
    }

    //选中当前ui
    public void onBtn()
    {
        //修改roleinfo里面的消息//给panel的脚本做
        //修改选择的id
        //当前ui高亮，也是给panle做
        Kaiyun.Event.FireIn("OnSelectedRoleItem",this);
    }


    //ui高亮：放大效果即可 scale
    public void SelectedEffect()
    {
        rectTransform.localScale = new Vector3(scaleSize, scaleSize, 1f);
    }
    //ui取消高亮：恢复正常
    public void RestoreEffect()
    {
        rectTransform.localScale = new Vector3(1f, 1f, 1f);
    }


    //设置当前item的roleinfo
    public void SetRoleInfo()
    {
        nameText.text = characterInfo.Name;
        levelText.text = "Lv. " + characterInfo.Level;
        jobText.text = define.Name;
    }


}

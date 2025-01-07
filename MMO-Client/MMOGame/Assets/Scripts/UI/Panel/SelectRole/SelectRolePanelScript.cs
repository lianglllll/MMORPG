using HS.Protobuf.Login;
using HS.Protobuf.SceneEntity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectRolePanelScript : BasePanel
{
    private Button createBtn;
    private Button startBtn;
    private Button deleteBtn;
    private Transform roleListItemMountPoint;                                           //item挂载点
    private RoleListItemScript curSelectedItem;                                         //rolelist中选中的item
    private List<RoleListItemScript> roleListItemList = new List<RoleListItemScript>(); //rolelistItem的脚本
    private List<NetActor> characterInfoList = new List<NetActor>();                    //从网络中接收到的Ncharacter列表(缓冲)

    private Text cname;
    private Text vocation;
    private Text level;


    protected override void Awake()
    {
        roleListItemMountPoint = transform.Find("RoleList");
        createBtn = transform.Find("CreateRoleBtn").GetComponent<Button>();
        startBtn = transform.Find("StartBtn").GetComponent<Button>();
        deleteBtn = transform.Find("DeleteBtn").GetComponent<Button>();
        cname = transform.Find("RoleInfo/Name").GetComponent<Text>();
        vocation = transform.Find("RoleInfo/Vocation").GetComponent<Text>();
        level = transform.Find("RoleInfo/Level").GetComponent<Text>();
    }
    protected override void Start()
    {
        createBtn.onClick.AddListener(OnCreateBtn);
        startBtn.onClick.AddListener(OnstartBtn);
        deleteBtn.onClick.AddListener(OnDeleteRoleBtn);

        //拉取角色列表
        UserService.Instance.GetCharacterListRequest();
    }
    public void RefreshRoleListUI(CharacterListResponse msg)
    {
        //1.存储全部角色信息
        characterInfoList.Clear();
        foreach (NetActor chr in msg.CharacterList)
        {
            characterInfoList.Add(chr);
        }

        //清理挂载点下的全部item实例
        if (roleListItemList.Count > 0)
        {
            foreach (RoleListItemScript item in roleListItemList)
            {
                Destroy(item.gameObject);
            }
        }
        roleListItemList.Clear();

        //实例化新的roleItem
        GameObject panelPrefab = null;
        GameObject panelObject = null;
        panelPrefab = UIManager.Instance.GetPanelPrefab("RoleListItem");
        for (int i = 0; i < characterInfoList.Count; i++)
        {
            //实例化item
            panelObject = GameObject.Instantiate(panelPrefab, roleListItemMountPoint, false);
            if (panelObject == null)
            {
                Debug.LogError("实例化item失败");
                return;
            }

            //注入信息
            RoleListItemScript itemScript = panelObject.GetComponent<RoleListItemScript>();
            itemScript.Init(this, characterInfoList[i]);

            //将item脚本交给当前脚本管理
            roleListItemList.Add(itemScript);
        }

        //选中第一个
        if(roleListItemList.Count > 0)
        {
            StartCoroutine(SelectDefaultItem());
        }


    }
    private IEnumerator SelectDefaultItem()
    {
        yield return null;
        roleListItemList[0].onBtn();
    }


    /// <summary>
    /// 点击创建角色按钮回调
    /// </summary>
    public void OnCreateBtn()
    {
        StartCoroutine(_OnCreateBtn());
    }
    private IEnumerator _OnCreateBtn()
    {
        yield return ScenePoster.Instance.FadeIn();

        //切换创建角色面板
        UIManager.Instance.OpenPanel("CreateRolePanel");

        yield return ScenePoster.Instance.FadeOut();
    }

    /// <summary>
    /// 点击开始按钮回调
    /// </summary>
    public void OnstartBtn()
    {
        if(curSelectedItem == null)
        {
            return;
        }

        curSelectedItem.Stop();

        //获取当前roleItemId对应的role信息，将角色id发送到服务端进行处理
        //发送网络请求
        UserService.Instance.EnterGameRequest(curSelectedItem.ChrId);

        StartCoroutine(_OnstartBtn());

    }
    private IEnumerator _OnstartBtn()
    {
        //关闭当前ui
        yield return ScenePoster.Instance.FadeIn();
        UIManager.Instance.ClosePanel("SelectRolePanel");
    }


    /// <summary>
    /// 点击删除按钮回调
    /// </summary>
    public void OnDeleteRoleBtn()
    {
        if(curSelectedItem == null)
        {
            return;
        }

        //todo 需要进行弹窗，确认删除

        //发送请求
        UserService.Instance.CharacterDeleteRequest(curSelectedItem.ChrId);

    }


    /// <summary>
    /// 选中roleitem回调
    /// </summary>
    /// <param name="roleListItemScript"></param>
    public void OnSelectedRoleItem(RoleListItemScript roleListItemScript)
    {
        if(curSelectedItem != null)
        {
            curSelectedItem.RestoreEffect();
        }

        //roleInfo
        cname.text = "姓名：" + roleListItemScript.Name;
        vocation.text = "职业：" + roleListItemScript.Vocation;
        level.text = "修为：Lv." + roleListItemScript.Level;

        //选中效果
        curSelectedItem = roleListItemScript;
        curSelectedItem.SelectedEffect();

    }




}

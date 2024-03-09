using Proto;
using Summer.Network;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectRolePanelScript : BasePanel
{
    //ui
    private Button createBtn;
    private Button startBtn;
    private Button deleteBtn;

    //item挂载点
    private Transform roleListItemMountPoint;                   

    //rolelist中选中的item
    private RoleListItemScript curSelectedItem;

    //rolelistItem的脚本
    private List<RoleListItemScript> roleListItemList = new List<RoleListItemScript>();

    //从网络中接收到的Ncharacter列表(缓冲)
    private List<NetActor> characterInfoList = new List<NetActor>();

    protected override void Awake()
    {
        roleListItemMountPoint = transform.Find("RoleList");
        createBtn = transform.Find("CreateRoleBtn").GetComponent<Button>();
        startBtn = transform.Find("StartBtn").GetComponent<Button>();
        deleteBtn = transform.Find("DeleteBtn").GetComponent<Button>();
    }

    protected override void Start()
    {

        createBtn.onClick.AddListener(OnCreateBtn);
        startBtn.onClick.AddListener(OnstartBtn);
        deleteBtn.onClick.AddListener(OnDeleteRoleBtn);

        //订阅选中角色item的事件
        Kaiyun.Event.RegisterIn("OnSelectedRoleItem",this, "OnSelectedRoleItem");

        //拉取角色列表
        UserService.Instance._CharacterListRequest();
    }


    private void OnDestroy()
    {
        Kaiyun.Event.UnregisterIn("OnSelectedRoleItem", this, "OnSelectedRoleItem");

    }

    /// <summary>
    /// 点击创建角色按钮回调
    /// </summary>
    public void OnCreateBtn()
    {
        //切换创建角色面板
        UIManager.Instance.OpenPanel("CreateRolePanel");
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

        //获取当前roleItemId对应的role信息，将角色id发送到服务端进行处理
        //发送网络请求
        UserService.Instance._GameEnterRequest(curSelectedItem.ChrId);

        //关闭当前ui
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
        UserService.Instance._CharacterDeleteRequest(curSelectedItem.ChrId);

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
        //选中效果
        curSelectedItem = roleListItemScript;
        curSelectedItem.SelectedEffect();
    }

    /// <summary>
    /// 加载rolelist 的 UI
    /// </summary>
    public void LoadRoleList()
    {
        //清理挂载点下的全部item
        ClearRoleListItem();

        GameObject panelPrefab = null;
        GameObject panelObject = null;
        for (int i = 0; i < characterInfoList.Count;i++)
        {
            //实例化item
            panelPrefab = UIManager.Instance.GetPanelPrefab("RoleListItem");
            panelObject = GameObject.Instantiate(panelPrefab, roleListItemMountPoint, false);
            if(panelObject == null)
            {
                Debug.LogError("实例化item失败");
                return;
            }
            RoleListItemScript itemScript = panelObject.GetComponent<RoleListItemScript>();
            itemScript.InjectInfo(this, characterInfoList[i], i);

            //将item脚本交给当前脚本管理
            roleListItemList.Add(itemScript);
        }


    }

    /// <summary>
    /// 清理全部itemUI
    /// </summary>
    public void ClearRoleListItem()
    {
        if (roleListItemList == null) return;
        foreach(RoleListItemScript item in roleListItemList)
        {
            Destroy(item.gameObject);
        }
        //此时roleListItemList里面记录的全是无效地址，所以需要clear
        roleListItemList.Clear();
    }

    /// <summary>
    /// 重新刷新rolelist UI
    /// </summary>
    /// <param name="msg"></param>
    public void RefreshRoleListUI(CharacterListResponse msg)
    {

        characterInfoList.Clear();
        //将得到的角色列表数据放入roleInfos
        foreach (NetActor chr in msg.CharacterList)
        {
            characterInfoList.Add(chr);
        }

        //调用加载rolelist
        LoadRoleList();
    }

}

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

    private void Start()
    {
        createBtn = transform.Find("CreateRoleBtn").GetComponent<Button>();
        startBtn = transform.Find("StartBtn").GetComponent<Button>();
        deleteBtn = transform.Find("DeleteBtn").GetComponent<Button>();
        createBtn.onClick.AddListener(OnCreateBtn);
        startBtn.onClick.AddListener(OnstartBtn);
        deleteBtn.onClick.AddListener(OnDeleteRoleBtn);

        roleListItemMountPoint = transform.Find("RoleList");


        //订阅各种网络消息
        //订阅角色列表响应消息
        MessageRouter.Instance.Subscribe<CharacterListResponse>(_CharacterListResponse);
        //订阅删除角色响应消息
        MessageRouter.Instance.Subscribe<CharacterDeleteResponse>(_CharacterDeleteResponse);

        //订阅选中角色item的事件
        Kaiyun.Event.RegisterIn("OnSelectedRoleItem",this, "OnSelectedRoleItem");

        //发包拉取rolelist信息
        CharacterListRequest req = new CharacterListRequest();
        NetClient.Send(req);
    }


    private void OnDestroy()
    {
        Kaiyun.Event.UnregisterIn("OnSelectedRoleItem", this, "OnSelectedRoleItem");
        MessageRouter.Instance.Off<CharacterListResponse>(_CharacterListResponse);
        MessageRouter.Instance.Off<CharacterDeleteResponse>(_CharacterDeleteResponse);
    }

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
        Debug.Log("开始游戏："+ curSelectedItem.name);

        //通过事件系统来执行
        //NetStart中实现了
        Kaiyun.Event.FireIn("GameEnter", curSelectedItem.ChrId);


        //关闭当前ui
        UIManager.Instance.ClosePanel("SelectRolePanel");
    }

    public void OnDeleteRoleBtn()
    {
        if(curSelectedItem == null)
        {
            return;
        }

        //todo 需要进行弹窗，确认删除

        //发送请求
        CharacterDeleteRequest req = new CharacterDeleteRequest();
        req.CharacterId = curSelectedItem.ChrId;
        NetClient.Send(req);

        /*
        //再次询问是否删除
        //ui设置按钮
        var ok = new Chibi.Free.Dialog.ActionButton("确定", () => {
            //发送请求
            CharacterDeleteRequest req = new CharacterDeleteRequest();
            req.CharacterId = roleInfos[roleItemId].roleId;
            NetClient.Send(req);
        }, new Color(0f, 0.9f, 0.9f));
        var cannel = new Chibi.Free.Dialog.ActionButton("取消", () => { }, new Color(0f, 0.9f, 0.9f));
        Chibi.Free.Dialog.ActionButton[] buttons = { ok, cannel };
        //调用工具类进行弹窗,工具类mydialog里面实现了异步
        MyDialog.Show("系统提升", "是否确定删除该角色", buttons);
        */
    }


    //选中roleitem回调
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

    //加载rolelist
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

    //清理全部item
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


    //====================================网络=====================================================
    //角色列表请求响应
    private void _CharacterListResponse(Connection sender, CharacterListResponse msg)
    {

        characterInfoList.Clear();
        //将得到的角色列表数据放入roleInfos
        foreach (NetActor chr in msg.CharacterList)
        {
            characterInfoList.Add(chr);
        }

        //调用加载rolelist
        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            LoadRoleList();
        });
    }

    //角色删除响应
    private void _CharacterDeleteResponse(Connection sender, CharacterDeleteResponse msg)
    {

        //发起角色列表的请求
        CharacterListRequest req = new CharacterListRequest();
        NetClient.Send(req);

    }



}

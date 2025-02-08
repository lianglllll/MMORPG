using HS.Protobuf.Game;
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
    private List<SimpleCharacterInfoNode> characterInfoList = new();                    //从网络中接收到的chr列表(缓冲)

    // role info
    private Text cname;
    private Text vocation;
    private Text level;

    //
    private bool isStart;

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
        isStart = false;

        createBtn.onClick.AddListener(OnCreateBtn);
        startBtn.onClick.AddListener(OnStartBtn);
        deleteBtn.onClick.AddListener(OnDeleteRoleBtn);

        //拉取角色列表
        EntryGameWorldService.Instance.SendGetCharacterListRequest();
    }
    private void Update()
    {
        if (GameInputManager.Instance.Space)
        {
            OnStartBtn();
        }
    }


    public void RefreshRoleListUI(GetCharacterListResponse msg)
    {
        //1.存储全部角色信息
        characterInfoList.Clear();
        foreach (var chr in msg.CharacterNodes)
        {
            characterInfoList.Add(chr);
        }

        //清理挂载点下的全部item实例
        if (roleListItemList.Count > 0)
        {
            foreach (RoleListItemScript item in roleListItemList)
            {
                item.Stop();
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
        else
        {
            ClenrRoleInfo();
        }
    }
    private IEnumerator SelectDefaultItem()
    {
        yield return null;
        roleListItemList[0].onBtn();
    }

    public void OnCreateBtn()
    {
        //切换创建角色面板
        UIManager.Instance.OpenPanelWithFade("CreateRolePanel");
    }
    public void OnDeleteRoleBtn()
    {
        if(curSelectedItem == null)
        {
            return;
        }

        //弹框提示
        UIManager.Instance.MessagePanel.ShowSelectionPanelWithInput("删除角色", "是否删除角色？若是，请输入登录密码。", (password) =>
        {
            //发送请求
            EntryGameWorldService.Instance.SendDeleteCharacterRequest(curSelectedItem.ChrId, password);
        });

    }
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
    private void ClenrRoleInfo()
    {
        cname.text = "";
        vocation.text = "";
        level.text = "";
    }

    /// <summary>
    /// 点击开始按钮回调
    /// </summary>
    public void OnStartBtn()
    {
        if (curSelectedItem == null || isStart == true)
        {
            goto End;
        }
        isStart = true;
        //获取当前roleItemId对应的role信息，将角色id发送到服务端进行处理
        //发送网络请求
        EntryGameWorldService.Instance.SendEnterGameRequest(curSelectedItem.ChrId);

    End:
        return;
    }
    public void HandleStartResponse(int reslutCode, string msg)
    {
        if(reslutCode == 0)
        {
            // 关闭动画
            curSelectedItem.Stop();
            // 关闭当前ui
            UIManager.Instance.ClosePanel("SelectRolePanel");
        }
        else
        {
            isStart = false;
            UIManager.Instance.ShowTopMessage(msg);
        }
    }
}

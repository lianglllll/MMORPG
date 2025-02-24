using DG.Tweening;
using GameClient;
using HS.Protobuf.Login;
using HSFramework.Audio;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SelectWorldPanel : BasePanel
{
    private bool isStart;
    private bool isCanStart;

    private Button startBtn;
    private Button openSelectWorldsBoxBtn;
    private Button exitSelectWorldsBoxBtn;
    private Button exitPanelBtn;

    private TextMeshProUGUI currentServerName;

    private GameObject selectWorldBox;
    private GameObject worldInfoNodes;
    public GameObject worldInfoNodePrefab;

    private bool isSelectWorldsBoxAnimating;
    private CanvasGroup SelectWorldsBoxCanvasGroup;

    private int curSelectWorldId;

    protected override void Awake()
    {
        base.Awake();
        startBtn = transform.Find("InfoBox/StartBtn").GetComponent<Button>();
        openSelectWorldsBoxBtn = transform.Find("InfoBox/SelectBtn").GetComponent<Button>();
        currentServerName = transform.Find("InfoBox/SelectBtn/Text").GetComponent<TextMeshProUGUI>();
        selectWorldBox = transform.Find("SelectWorldsBox").gameObject;
        worldInfoNodes = transform.Find("SelectWorldsBox/WorldInfoNodes").gameObject;
        exitSelectWorldsBoxBtn = transform.Find("SelectWorldsBox/ExitBtn").GetComponent<Button>();
        exitPanelBtn = transform.Find("ExitBtn").GetComponent<Button>();
        SelectWorldsBoxCanvasGroup = selectWorldBox.GetComponent<CanvasGroup>();
    }
    protected override void Start()
    {
        base.Start();

        isStart = false;
        isCanStart = false;
        isSelectWorldsBoxAnimating = false;
        curSelectWorldId = -1;

        selectWorldBox.SetActive(false);
        SelectWorldsBoxCanvasGroup.alpha = 0;

        startBtn.onClick.AddListener(OnStartBtn);
        openSelectWorldsBoxBtn.onClick.AddListener(OnOpenSelectWorldsBoxBtn);
        exitSelectWorldsBoxBtn.onClick.AddListener(OnExitSelectWorldsBoxBtn);
        exitPanelBtn.onClick.AddListener(OnExitPanelBtn);


        // 拉取世界信息
        EntryGameWorldService.Instance.SendGetAllWorldInfosRequest();

        //加载上次的服务器信息
        string myServerInfo = PlayerPrefs.GetString("myWorldInfoNode");

        //加载上次缓存的用户名和密码
        string myLastSelectWorldId = PlayerPrefs.GetString("myLastSelectWorldId");
        string myLastSelectWorldName = PlayerPrefs.GetString("myLastSelectWorldName");
        if (!string.IsNullOrEmpty(myLastSelectWorldId))
        {
            curSelectWorldId = int.Parse(myLastSelectWorldId);
            currentServerName.text = myLastSelectWorldName;
        }
        else
        {
            currentServerName.text = "请选择世界.";
        }
    }
    private void Update()
    {
        if(GameInputManager.Instance.Space)
        {
            OnStartBtn();
        }
    }

    private void OnOpenSelectWorldsBoxBtn()
    {
        if (isSelectWorldsBoxAnimating) return; // 防止动画过程中重复点击
        isSelectWorldsBoxAnimating = true;
        selectWorldBox.SetActive(true);
        SelectWorldsBoxCanvasGroup.DOFade(1f, 0.5f).OnComplete(() => {
            isSelectWorldsBoxAnimating = false;
        });  // 0.5秒内淡入至完全可见
    }
    private void OnExitSelectWorldsBoxBtn()
    {
        if (isSelectWorldsBoxAnimating) return; // 防止动画过程中重复点击
        isSelectWorldsBoxAnimating = true;
        SelectWorldsBoxCanvasGroup.DOFade(0f, 0.5f).OnComplete(() =>
        {
            selectWorldBox.SetActive(false);
            isSelectWorldsBoxAnimating = false;
        });  // 0.5秒内淡出并在完成后禁用对象
    }
    public void OnStartBtn()
    {
        GlobalAudioManager.Instance.PlayUIAudio(UIAudioClipType.ButtonClick);

        if (!isCanStart) return;
        if (isStart)
        {
            UIManager.Instance.ShowTopMessage("已开始");
            return;
        }
        isStart = true;
        if (curSelectWorldId == -1)
        {
            UIManager.Instance.ShowTopMessage("未选择世界，无法开始");
            isStart = false;
            return;
        }
        // 获取GameGate信息
        NetManager.Instance.SendGetGameGatesRequest(curSelectWorldId);
    }
    private void OnExitPanelBtn()
    {
        // todo
        GlobalAudioManager.Instance.PlayUIAudio(UIAudioClipType.ButtonClick);
        UIManager.Instance.ShowTopMessage("未开发...");
    }
    private void OnSelectWorldBtn(WorldInfoNode infoNode)
    {
        curSelectWorldId = infoNode.WorldId;
        currentServerName.text = infoNode.WorldName;
        GameApp.curWorldInfoNode = infoNode;
        OnExitSelectWorldsBoxBtn();
    }
    public void HandleGetAllWorldInfosResponse(GetAllWorldInfosResponse message)
    {
        var sortedNodes = message.WorldInfoNodes.OrderBy(node => node.WorldId);
        foreach (var node in sortedNodes)
        {
            // 创建tempNode实例
            // 这里注意需要将实例化和设置父物体分开写，
            // 原因是当前我们的父物体是出于关闭状态，如果我们写到一块，实例化的物体是不会进行生命周期的。
            GameObject obj = GameObject.Instantiate(worldInfoNodePrefab);
            obj.transform.SetParent(worldInfoNodes.transform, false);
            WorldInfoNodeUI worldInfoNode =  obj.GetComponent<WorldInfoNodeUI>();
            worldInfoNode.Init(node, OnSelectWorldBtn);
        }
        isCanStart = true;
    }
    public void HandleStartResponse(int reslutCode, string msg)
    {
        if (reslutCode == 0)
        {
            UIManager.Instance.MessagePanel.ShowTopMsg("giaogiao");
            // 记录进入世界成功时的id和name
            PlayerPrefs.SetString("myLastSelectWorldId",curSelectWorldId.ToString());
            PlayerPrefs.SetString("myLastSelectWorldName", currentServerName.text);
            PlayerPrefs.Save();

            // 切换面板
            UIManager.Instance.ExchangePanelWithFade("SelectWorldPanel", "SelectRolePanel");
        }
        else
        {
            isStart = false;
            if (msg != null)
            {
                UIManager.Instance.MessagePanel.ShowTopMsg(msg);
            }
        }
    }

}

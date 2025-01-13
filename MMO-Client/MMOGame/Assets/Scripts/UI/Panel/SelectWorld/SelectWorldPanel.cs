using DG.Tweening;
using GameClient;
using HS.Protobuf.Login;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;




public class SelectWorldPanel : BasePanel
{
    private bool isStart;

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
        isSelectWorldsBoxAnimating = false;

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
        if (!string.IsNullOrEmpty(myServerInfo))
        {
            GameApp.WorldInfoNode = JsonUtility.FromJson<HS.Protobuf.Login.WorldInfoNode>(myServerInfo);
            currentServerName.text = GameApp.WorldInfoNode.WorldName;
        }
        else
        {
            currentServerName.text = "请选择世界.";
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
        if (isStart) return;

        if (GameApp.WorldInfoNode == null)
        {
            UIManager.Instance.ShowTopMessage("未选择世界，无法开始");
            return;
        }
        // 获取GameGate信息
        NetManager.Instance.SendGetGameGatesRequest(curSelectWorldId);
    }
    private void OnExitPanelBtn()
    {
        // todo
    }
    private void OnSelectWorldBtn(int worldId, string worldName)
    {
        curSelectWorldId = worldId;
        currentServerName.text = worldName;
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
        isStart = true;
    }

}

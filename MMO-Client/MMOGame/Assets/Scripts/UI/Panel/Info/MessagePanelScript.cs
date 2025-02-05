using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;
using DG.Tweening;

public class MessagePanelScript : MonoBehaviour
{
    //网络延迟text面板
    private Transform NetworkInfoBox;
    private Text NDelayText;
    private Image NSignalImage;

    //消息提示面板
    private GameObject topMsgBox;
    private Text topMsgBoxText;
    private CanvasGroup topMsgCanvasGroup;
    private GameObject bottonMsgBox;
    private TextMeshProUGUI bottonMsgBoxText;
    private float showTime = 1f;
    private Sequence currentTopMsgSequence; 

    //确认面板
    private bool selectionPanelActive;
    private SelectionPanel selectionPanel;
    private CanvasGroup selectionPanelCanvasGroup;

    //loading面板
    private LoadingBox loadingBox;

    //item获取丢弃面板
    private ItemIOInfoBox itemIOInfoBox;

    //传送面板
    private DeliverPanel deliverPanel;

    private void Awake()
    {
        //获取自身身上的Text组件
        topMsgBoxText = transform.Find("TopMessageBox/MessageText").GetComponent<Text>();
        topMsgCanvasGroup = transform.Find("TopMessageBox").GetComponent<CanvasGroup>();
        bottonMsgBoxText = transform.Find("BottonMessageBox/MessageText").GetComponent<TextMeshProUGUI>();
        topMsgBox = transform.Find("TopMessageBox").gameObject;
        bottonMsgBox = transform.Find("BottonMessageBox").gameObject;
        NetworkInfoBox = transform.Find("NetworkInfoBox");
        NDelayText = transform.Find("NetworkInfoBox/NetworkDelay").GetComponent<Text>();
        NSignalImage = transform.Find("NetworkInfoBox/SignalImage").GetComponent<Image>();
        selectionPanel = transform.Find("SelectionPanel").GetComponent<SelectionPanel>();
        selectionPanelCanvasGroup = transform.Find("SelectionPanel").GetComponent<CanvasGroup>();
        loadingBox = transform.Find("LoadingBox").GetComponent<LoadingBox>();
        itemIOInfoBox = transform.Find("ItemIOInfoBox").GetComponent<ItemIOInfoBox>();
        deliverPanel = transform.Find("DeliverPanel").GetComponent<DeliverPanel>();
    }
    private void Start()
    {
        // 确保初始颜色为透明状态
        topMsgCanvasGroup.alpha = 0;

        topMsgBox.SetActive(false);
        bottonMsgBox.SetActive(false);

        //初始化确认面板
        selectionPanelActive = false;
        selectionPanel.Init(() =>
        {
            selectionPanelActive = false;
            selectionPanel.gameObject.SetActive(false);
        });
        selectionPanel.gameObject.SetActive(false);

        //初始化loading面板
        loadingBox.gameObject.SetActive(false);

        //初始化item获取丢弃面板
        itemIOInfoBox.gameObject.SetActive(true);

        //初始化传送面板
        deliverPanel.gameObject.SetActive(false);

        //初始化网络信息
        NetworkInfoBox.gameObject.SetActive(true);
        ShowNetworkDisconnect();
    }

    /// <summary>
    /// 外部调用需要显示提示信息
    /// </summary>
    /// <param name="msg"></param>
    public void ShowTopMsg(string msg)
    {
        // 如果已有动画正在运行，立即终止并重置
        if (currentTopMsgSequence != null && currentTopMsgSequence.IsActive())
        {
            currentTopMsgSequence.Kill();
            topMsgCanvasGroup.alpha = 0; // 立即重置透明度
            topMsgBox.SetActive(false);  // 确保对象处于关闭状态
        }

        // 设置提示信息
        topMsgBoxText.text = msg;
        topMsgBox.SetActive(true);

        // 创建新动画序列
        currentTopMsgSequence = DOTween.Sequence();
        currentTopMsgSequence.Append(topMsgCanvasGroup.DOFade(1, 0.5f));
        currentTopMsgSequence.AppendInterval(showTime);
        currentTopMsgSequence.Append(topMsgCanvasGroup.DOFade(0, 0.5f).OnComplete(() => {
            topMsgBox.SetActive(false);
            currentTopMsgSequence = null; // 动画完成后清除引用
        }));

        // 可选：设置自动回收（根据DOTween设置）
        currentTopMsgSequence.SetAutoKill(true);
    }

    public void ShowBottonMsg(string msg,Color? color = null)
    {
        if (!color.HasValue)
        {
            color = Color.red;
        }
        //设置提示信息并且启动text
        bottonMsgBoxText.color = (Color)color;
        bottonMsgBoxText.text = msg;
        bottonMsgBox.SetActive(true);

    }

    /// <summary>
    /// 显示网络延迟
    /// </summary>
    /// <param name="ms"></param>
    public void ShowNetworkDelay(int ms)
    {
        Color newColor = Color.green;
        newColor.a = 64 / 256f;
        NSignalImage.color = newColor;
        NDelayText.color = Color.green;
        NDelayText.text = "网络延迟：" + ms + "ms";
    }
    public void ShowNetworkDisconnect()
    {
        Color newColor = Color.red;
        newColor.a = 64 / 256f;
        NSignalImage.color = newColor; NDelayText.color = Color.red;
        NDelayText.text = "网络断开";
    }

    /// <summary>
    /// 确认面板
    /// </summary>
    /// <param name="spaceid"></param>
    /// <param name="desc"></param>
    public void ShowSelectionPanel(string simpleTipsText, string detailTipsText, Action comfirmAction)
    {
        if (selectionPanelActive == true) return;
        //显示面板、设置ui
        selectionPanelActive = true;
        selectionPanel.gameObject.SetActive(true);
        selectionPanelCanvasGroup.alpha = 0;
        selectionPanelCanvasGroup.DOFade(1, 1f);
        selectionPanel.OpenPanel(simpleTipsText, detailTipsText, comfirmAction);
    }
    public void ShowSelectionPanelWithInput(string simpleTipsText, string detailTipsText, Action<string> comfirmAction)
    {
        if (selectionPanelActive == true) return;
        selectionPanelActive = true;
        selectionPanel.gameObject.SetActive(true);
        selectionPanelCanvasGroup.alpha = 0;
        selectionPanelCanvasGroup.DOFade(1, 1f);
        selectionPanel.OpenPanelWithInput(simpleTipsText, detailTipsText, comfirmAction);
    }
    public void CloseSelectionPanel()
    {
        if (selectionPanelActive == false) return;
        selectionPanelActive = false;
        selectionPanel.gameObject.SetActive(false);
    }

    /// <summary>
    /// 展示loadingbox
    /// </summary>
    /// <param name="value"></param>
    public void  ShowLoadingBox(string value)
    {
        loadingBox.gameObject.SetActive(true);
        loadingBox.Show(value);
    }
    /// <summary>
    /// 隐藏loadingbox
    /// </summary>
    public void HideLoadingBox()
    {
        loadingBox.gameObject.SetActive(false);
    }

    /// <summary>
    /// 展示一些个item的信息
    /// </summary>
    /// <param name="msg"></param>
    public void ShowItemIOInfo(string msg)
    {
        itemIOInfoBox.ShowMsg(msg);
    }

    /// <summary>
    /// 展示传送面板
    /// </summary>
    public void ShowDeliverPanel()
    {
        deliverPanel.gameObject.SetActive(true);
        deliverPanel.Show();
    }
    public void CloseDeliverPanel()
    {
        deliverPanel.Hide(() =>
        {
            deliverPanel.gameObject.SetActive(false);
        });
    }

    /// <summary>
    /// 设置鼠标的显示
    /// </summary>
    /// <param name="enable"></param>
    public void SetMouseUI(bool enable)
    {
        if (enable)
        {
            // 显示鼠标光标
            Cursor.visible = true;
            // 解除鼠标锁定状态
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            // 隐藏鼠标光标
            Cursor.visible = false;
            // 锁定鼠标在屏幕中央
            Cursor.lockState = CursorLockMode.Locked;
        }

    }

}

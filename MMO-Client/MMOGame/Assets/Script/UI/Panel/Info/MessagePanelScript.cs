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
    private GameObject bottonMsgBox;
    private Text topMsgBoxText;
    private TextMeshProUGUI bottonMsgBoxText;
    private float showTime = 2f;
    private float topMsgBoxCountdown;
    private float bottonMsgBoxCountdown;

    //确认面板
    private SelectionPanel selectionPanel;
    private bool selectionPanelActive;
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
        //因为消息提示默认是不显示的

        topMsgBoxCountdown = 0f;
        bottonMsgBoxCountdown = 0f;

        // 确保初始颜色为透明状态
        Color originalColor1 = topMsgBoxText.color;
        originalColor1.a = 0;
        topMsgBoxText.color = originalColor1;

        topMsgBox.SetActive(false);
        bottonMsgBox.SetActive(false);

        //初始化确认面板
        selectionPanel.gameObject.SetActive(false);
        selectionPanelActive = false;
        selectionPanel.Init(() =>
        {
            selectionPanelActive = false;
        });

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

    private void Update()
    {

        if (bottonMsgBoxCountdown > 0)
        {
            bottonMsgBoxCountdown -= Time.deltaTime;
            if (bottonMsgBoxCountdown <= 0f)
            {
                bottonMsgBox.SetActive(false);
                bottonMsgBoxCountdown = 0f;
            }
        }
    }

    /// <summary>
    /// 外部调用需要显示提示信息
    /// </summary>
    /// <param name="msg"></param>
    public void ShowTopMsg(string msg)
    {
        //设置提示信息并且启动text
        topMsgBoxText.text = msg;
        topMsgBox.SetActive(true);

        Sequence sequence = DOTween.Sequence();
        sequence.Append(topMsgBoxText.DOFade(1, 1f));
        sequence.AppendInterval(showTime);
        sequence.Append(topMsgBoxText.DOFade(0, 1f).OnComplete(()=> {
            topMsgBox.SetActive(false);
        }));

        //停留showTime秒后调用隐藏
        topMsgBoxCountdown = showTime;
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
        //停留showTime秒后调用隐藏
        bottonMsgBoxCountdown = showTime;
    }

    /// <summary>
    /// 显示网络延迟
    /// </summary>
    /// <param name="ms"></param>
    public void ShowNetworkDelay(int ms)
    {
        NSignalImage.color = Color.green;
        NDelayText.color = Color.green;
        NDelayText.text = "网络延迟：" + ms + "ms";
    }
    public void ShowNetworkDisconnect()
    {
        NSignalImage.color = Color.red;
        NDelayText.color = Color.red;
        NDelayText.text = "网络断开";
    }

    /// <summary>
    /// 显示确认面板
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
    /// <summary>
    /// 主动关闭确认面板
    /// </summary>
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

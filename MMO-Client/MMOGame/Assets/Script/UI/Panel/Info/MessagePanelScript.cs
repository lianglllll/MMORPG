using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class MessagePanelScript : MonoBehaviour
{

    //文本组件
    private Text topMsgBoxText;
    private Text NDelayText;
    private GameObject topMsgBox;

    //提示信息的停留时间
    private float showTime = 3f;

    //传送面板相关的
    private Transform DeliverBox;
    private Button DeliverBtn;
    private Text DeliverText;

    //临时数据
    private int spaceId;


    private void Awake()
    {
        //获取自身身上的Text组件
        topMsgBoxText = transform.Find("MessageBoxTop/MessageText").GetComponent<Text>();
        topMsgBox = transform.Find("MessageBoxTop").gameObject;

        NDelayText = transform.Find("NetworkDelay").GetComponent<Text>();

        DeliverBox = transform.Find("DeliverBox");
        DeliverBtn = transform.Find("DeliverBox/DeliverBtn").GetComponent<Button>();
        DeliverText = transform.Find("DeliverBox/TipsBox/Text").GetComponent<Text>();

    }

    private void Start()
    {
        //因为消息提示默认是不显示的
        topMsgBox.SetActive(false);
        //设置按钮回调
        DeliverBtn.onClick.AddListener(OnDeliverBtn);
        //初始化数据
        spaceId = -1;

    }

    /// <summary>
    /// 外部调用需要显示提示信息
    /// </summary>
    /// <param name="msg"></param>
    public void ShowMessage(string msg)
    {

        //设置提示信息并且启动text
        topMsgBoxText.text = msg;
        topMsgBox.SetActive(true);
        //停留showTime秒后调用Hide方法
        Invoke("Hide", showTime);

    }

    /// <summary>
    /// 隐藏信息,延时
    /// </summary>
    private void Hide()
    {
        topMsgBox.SetActive(false);
    }

    /// <summary>
    /// 显示网络延迟
    /// </summary>
    /// <param name="ms"></param>
    public void ShowNetworkDelay(int ms)
    {
        NDelayText.text = "网络延迟：" + ms + "ms";
    }

    /// <summary>
    /// 显示传送面板
    /// </summary>
    /// <param name="spaceid"></param>
    /// <param name="desc"></param>
    public void ShowDeliverBox(int spaceid)
    {
        //设置数据
        var spaceDefine = DataManager.Instance.GetSpaceDefineById(spaceid);
        if(spaceDefine != null)
        {
            DeliverText.text = "目标：" + spaceDefine.Name;
            DeliverBtn.enabled = true;
            this.spaceId = spaceid;

        }
        else
        {
            DeliverText.text = "由于空间乱流，目标点暂时无法传送";
            DeliverBtn.enabled = false;
        }

        //显示面板
        DeliverBox.gameObject.SetActive(true);
    }

    /// <summary>
    /// 关闭传送面板
    /// </summary>
    public void CloseDeliverBox()
    {
        DeliverBox.gameObject.SetActive(false);
    }

    /// <summary>
    /// 点击传送按钮回调
    /// </summary>
    private void OnDeliverBtn()
    {
        if(spaceId < 0)
        {
            ShowMessage("传送失败,无法搜寻坐标点");
            return;
        }

        //给服务器发送请求
        GameApp.SpaceDeliver(spaceId);

        CloseDeliverBox();
    }

}

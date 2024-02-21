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

    //确认面板
    private ConfirmBox confirmBox;
    private bool confirmBoxActive;

    //loading面板
    private LoadingBox loadingBox;

    private void Awake()
    {
        //获取自身身上的Text组件
        topMsgBoxText = transform.Find("MessageBoxTop/MessageText").GetComponent<Text>();
        topMsgBox = transform.Find("MessageBoxTop").gameObject;
        NDelayText = transform.Find("NetworkDelay").GetComponent<Text>();
        confirmBox = transform.Find("ConfirmBox").GetComponent<ConfirmBox>();
        loadingBox = transform.Find("LoadingBox").GetComponent<LoadingBox>();
    }

    private void Start()
    {
        //因为消息提示默认是不显示的
        topMsgBox.SetActive(false);
        //初始化确认面板
        confirmBox.Init(CloseConfirmBox);
        confirmBox.gameObject.SetActive(false);
        confirmBoxActive = false;
        //初始化loading面板
        loadingBox.gameObject.SetActive(false);
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
    /// 显示确认面板
    /// </summary>
    /// <param name="spaceid"></param>
    /// <param name="desc"></param>
    public void ShowConfirmBox(string tipsContent, string btnContent,bool btnActive, Action onClickBtnCallback)
    {
        if (confirmBoxActive == true) return;
        //显示面板、设置ui
        SetConfirmBoxActive(true);
        confirmBox.ShowBox(tipsContent, btnContent, btnActive, onClickBtnCallback);
    }

    /// <summary>
    /// 关闭确认面板
    /// </summary>
    public void CloseConfirmBox()
    {
        if (confirmBoxActive == false) return;
        SetConfirmBoxActive(false);
        confirmBox.CloseBox();
    }

    /// <summary>
    /// 是否激活显示面板
    /// </summary>
    /// <param name="active"></param>
    private void SetConfirmBoxActive(bool active)
    {
        confirmBox.gameObject.SetActive(active);
        confirmBoxActive = active;
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

}

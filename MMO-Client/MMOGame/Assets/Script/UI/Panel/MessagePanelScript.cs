using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class MessagePanelScript : MonoBehaviour
{

    //文本组件
    private Text msgText;
    private Text NDelayText;

    //提示信息的停留时间
    private float showTime = 3f;

    private void Start()
    {
        //获取自身身上的Text组件
        msgText = transform.Find("MessageBox").GetComponent<Text>();
        NDelayText = transform.Find("NetworkDelay").GetComponent<Text>();

        //因为消息提示默认是不显示的
        msgText.enabled = false;

    }


    // 外部调用需要显示提示信息
    public void ShowMessage(string msg)
    {

        //设置提示信息并且启动text
        msgText.text = msg;
        msgText.enabled = true;
        //停留showTime秒后调用Hide方法
        Invoke("Hide", showTime);

    }

    /// <summary>
    /// 隐藏信息,延时
    /// </summary>
    private void Hide()
    {
        msgText.enabled = false;
    }

    public void ShowNetworkDelay(int ms)
    {
        NDelayText.text = "网络延迟：" + ms + "ms";
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Proto;
using Summer.Network;
using System;
using UnityEngine.SceneManagement;
using GameClient.Entities;
using Assets.Script.Entities;
using GameClient;
using Serilog;

public class NetStart : MonoBehaviour
{
    public static NetStart Instance;

    public bool isConnectServer;

    [Header("服务器信息")]
    public string host = "127.0.0.1";
    public int port = 32510;

    private WaitForSeconds waitForSeconds = new WaitForSeconds(2f);      //心跳包时间控制
    DateTime lastBeatTime = DateTime.MinValue;                                  //上一次发送心跳包的时间

    private void Awake()
    {
        Instance = this;
    }

    /// <summary>
    /// 初始化
    /// </summary>
    void Start()
    {
        //消息分发注册
        MessageRouter.Instance.Subscribe<HeartBeatResponse>(_HeartBeatResponse);
        //事件监听
        Kaiyun.Event.RegisterOut("SuccessConnectServer", this, "ConnectToServerCallback");
        //启动消息分发器，单线程即可
        MessageRouter.Instance.Start(1);
        isConnectServer = false;
        ConnectToServer();
    }

    private void OnDestroy()
    {
        MessageRouter.Instance.Off<HeartBeatResponse>(_HeartBeatResponse);
        Kaiyun.Event.UnregisterOut("SuccessConnectServer", this, "ConnectToServerCallback");
        StopCoroutine(SendHeartMessage());
    }

    /// <summary>
    /// 连接到服务器
    /// </summary>
    public void ConnectToServer()
    {
        if (isConnectServer) return;
        //连接服务器
        NetClient.ConnectToServer(host, port);
    }

    /// <summary>
    /// 成功连接服务器事件回调
    /// </summary>
    public void ConnectToServerCallback()
    {

        UIManager.Instance.MessagePanel.ShowMessage("成功连接到服务器.....");
        isConnectServer = true;
        //心跳包
        StartCoroutine(SendHeartMessage());
    }

    /// <summary>
    /// 发送心跳包协程
    /// </summary>
    /// <returns></returns>
    IEnumerator SendHeartMessage()
    {
        //优化,防止不断在堆中创建新对象
        HeartBeatRequest beatReq = new HeartBeatRequest();

        while (true)
        {
            yield return waitForSeconds;
            NetClient.Send(beatReq);
            lastBeatTime = DateTime.Now;
        }
    }

    /// <summary>
    /// 心跳包响应
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="msg"></param>
    public void _HeartBeatResponse(Connection sender, HeartBeatResponse msg)
    {
        //说明服务器和客户端之间连接是通畅的
        TimeSpan gap = DateTime.Now - lastBeatTime;
        int ms = (int)Math.Round(gap.TotalMilliseconds);

        UnityMainThreadDispatcher.Instance().Enqueue(() => {
            //ui处理
            UIManager.Instance.MessagePanel.ShowNetworkDelay(ms);
        });
    }

    /// <summary>
    /// 客户端退出调试调用
    /// </summary>
    private void OnApplicationQuit()
    {
        NetClient.Close();
    }

}

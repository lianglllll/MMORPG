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
using System.Threading.Tasks;

public class NetStart : MonoBehaviour
{
    public static NetStart Instance;

    [Header("服务器信息")]
    public string host = "127.0.0.1";
    public int port = 6666;
    public bool isConnectServer;

    //心跳机制
    private bool isEnableHeartBeat;
    private WaitForSeconds waitForSeconds = new WaitForSeconds(2f);      //心跳包时间控制
    private DateTime lastBeatTime = DateTime.MinValue;                          //上一次发送心跳包的时间

    //重连
    private bool isReconnecting;

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
        Kaiyun.Event.RegisterOut("SuccessConnectServer", this, "ConnectToServerSuccessfulCallback");
        Kaiyun.Event.RegisterOut("FailedConnectServer", this, "ConnectToServerFailedCallback");
        Kaiyun.Event.RegisterOut("Disconnected", this, "OnDisconnected");

        //启动消息分发器，单线程即可
        MessageRouter.Instance.Start(1);

        //连接服务器
        isConnectServer = false;
        isReconnecting = false;
        isEnableHeartBeat = false;
        ConnectToServer();
    }

    private void OnDestroy()
    {
        MessageRouter.Instance.Off<HeartBeatResponse>(_HeartBeatResponse);
        Kaiyun.Event.UnregisterOut("SuccessConnectServer", this, "ConnectToServerCallback");
        Kaiyun.Event.UnregisterOut("FailedConnectServer", this, "ConnectToServerFailedCallback");
        Kaiyun.Event.UnregisterOut("Disconnected", this, "OnDisconnected");

        if (isEnableHeartBeat)
        {
            StopCoroutine(SendHeartMessage());
            isEnableHeartBeat = false;
        }
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
    /// 连接服务器成功回调
    /// </summary>
    public void ConnectToServerSuccessfulCallback()
    {
        UIManager.Instance.MessagePanel.ShowMessage("成功连接到服务器.....");
        isConnectServer = true;
        //心跳包
        isEnableHeartBeat = true;
        StartCoroutine(SendHeartMessage());
    }

    /// <summary>
    /// 连接服务器失败回调
    /// </summary>
    public void ConnectToServerFailedCallback()
    {
        ReConnect();
    }

    /// <summary>
    /// 与服务器断开连接事件回调
    /// </summary>
    public void OnDisconnected()
    {
        isConnectServer = false;
        if (isEnableHeartBeat)
        {
            StopCoroutine(SendHeartMessage());
            isEnableHeartBeat = false;
        }
        ReConnect();
    }

    /// <summary>
    /// 重连
    /// </summary>
    private void ReConnect()
    {
        if (!isReconnecting)
        {
            isReconnecting = true;
            Log.Information("重新连接服务器");
            Task.Delay(5000).ContinueWith(t =>
            {
                ConnectToServer();
                isReconnecting = false;
            });
        }
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
    /// 关闭与服务器的连接
    /// </summary>
    public void CloseConnect()
    {
        isConnectServer = false;
        if (isEnableHeartBeat)
        {
            StopCoroutine(SendHeartMessage());
            isEnableHeartBeat = false;
        }
        NetClient.Close();
    }

    /// <summary>
    /// 客户端退出调试调用
    /// </summary>
    private void OnApplicationQuit()
    {
        CloseConnect();
    }

    /// <summary>
    /// 模拟异常断开网络
    /// </summary>
    public void SimulateAbnormalDisconnection()
    {
        NetClient.SimulateAbnormalDisconnection();
    }

}

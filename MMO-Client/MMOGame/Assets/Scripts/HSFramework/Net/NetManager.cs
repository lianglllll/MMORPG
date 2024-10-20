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
using BaseSystem.Singleton;

public class NetManager : Singleton<NetManager>
{
    public bool isConnected;

    //心跳机制
    private bool isEnableHeartBeat;
    private WaitForSeconds waitForSeconds = new WaitForSeconds(2f);      //心跳包时间控制
    private DateTime lastBeatTime = DateTime.MinValue;                          //上一次发送心跳包的时间

    //是否处于重连状态
    private bool isReconnecting;

    void Start()
    {
        //消息分发注册
        MessageRouter.Instance.Subscribe<HeartBeatResponse>(_HeartBeatResponse);
        MessageRouter.Instance.Subscribe<ReconnectResponse>(_ReconnectResponse);


        //事件监听
        Kaiyun.Event.RegisterOut("SuccessConnectServer", this, "ConnectToServerSuccessfulCallback");
        Kaiyun.Event.RegisterOut("FailedConnectServer", this, "ConnectToServerFailedCallback");
        Kaiyun.Event.RegisterOut("Disconnected", this, "OnDisconnected");

        //启动消息分发器，单线程即可
        MessageRouter.Instance.Start(1);

        //连接服务器
        isConnected = false;
        isReconnecting = false;
        isEnableHeartBeat = false;

    }

    private void OnDestroy()
    {
        MessageRouter.Instance.Off<HeartBeatResponse>(_HeartBeatResponse);
        MessageRouter.Instance.Off<ReconnectResponse>(_ReconnectResponse);
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
    /// 尝试连接到服务器
    /// </summary>
    private Action connectSuccessAction;
    public void ConnectToServer(Action action = null)
    {
        if (isConnected) return;
        //连接服务器
        connectSuccessAction = action;
        NetClient.ConnectToServer(GameApp.ServerInfo.host, GameApp.ServerInfo.port);
    }

    /// <summary>
    /// 连接服务器成功回调
    /// </summary>
    public void ConnectToServerSuccessfulCallback()
    {
        isConnected = true;
        //显示ui
        UIManager.Instance.MessagePanel.ShowTopMsg("成功连接到服务器.....");
        connectSuccessAction?.Invoke();

        //发送心跳包
        isEnableHeartBeat = true;
        StartCoroutine(SendHeartMessage());

        //判断是否是重连的。
        if (GameApp.SessionId != null)
        {
            //发送重新连接的清请求
            ReconnectRequest req = new ReconnectRequest
            {
                SessionId = GameApp.SessionId
            };
            NetClient.Send(req);
        }

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
        isConnected = false;
        if (isEnableHeartBeat)
        {
            StopCoroutine(SendHeartMessage());
            isEnableHeartBeat = false;
        }

        //重连
        ReConnect();
        UIManager.Instance.MessagePanel.ShowNetworkDisconnect();
        UIManager.Instance.MessagePanel.ShowLoadingBox("断线重连中...");
    }

    /// <summary>
    /// 重连
    /// </summary>
    private void ReConnect()
    {

        //我们可用弹窗给它
        if (!isReconnecting)
        {
            isReconnecting = true;
            Task.Delay(5000).ContinueWith(t =>
            {
                ConnectToServer();
                isReconnecting = false;
            });
        }
    }

    /// <summary>
    /// 断线重连响应
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="msg"></param>
    private void _ReconnectResponse(Connection sender, ReconnectResponse msg)
    {
        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            //关闭ui
            UIManager.Instance.MessagePanel.HideLoadingBox();

            if (!msg.Success)
            {
                //没有登录信息可能是session超时了，跳回到登录界面
                GameApp.ClearGameAppData();
                EntityManager.Instance.Clear();
                UIManager.Instance.ClearAllOpenPanel();
                UIManager.Instance.OpenPanel("LoginPanel");
                return;
            }

            if(msg.EntityId == 0)
            {
                //没有选择角色，我们跳回到选择角色的界面
                string sessionId = GameApp.SessionId;
                GameApp.ClearGameAppData();
                GameApp.SessionId = sessionId;
                EntityManager.Instance.Clear();
                UIManager.Instance.ClearAllOpenPanel();
                UIManager.Instance.OpenPanel("SelectRolePanel");
                return;
            }

            //重连成功
            //这里不需要做任何操作

        });
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
    /// 主动关闭与服务器的连接
    /// </summary>
    public void CloseConnect()
    {
        isConnected = false;
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

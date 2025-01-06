using System.Collections;
using UnityEngine;
using System;
using GameClient.Entities;
using GameClient;
using BaseSystem.Singleton;
using HS.Protobuf.Common;
using Common.Summer.Core;
using Common.Summer.Net;
using HS.Protobuf.Login;
using Serilog;

public class NetManager : Singleton<NetManager>
{
    public NetClient curNetClient;
    public bool loginGateisConnected;
    public bool loginGateConnecting;
    private bool isEnableHeartBeat;

    private Action connectLoginGateSuccessAction;

    private WaitForSeconds waitForSeconds = new WaitForSeconds(2f);             //心跳包时间控制
    private DateTime lastBeatTime = DateTime.MinValue;                          //上一次发送心跳包的时间

    void Start()
    {
        //消息分发注册
        MessageRouter.Instance.Subscribe<CSHeartBeatResponse>(_HeartBeatResponse);
        MessageRouter.Instance.Subscribe<ReconnectResponse>(_ReconnectResponse);

        //启动消息分发器，单线程即可
        MessageRouter.Instance.Start(1);

        //连接服务器
        loginGateisConnected = false;
        isEnableHeartBeat = false;

        curNetClient = new NetClient();
    }

    private void OnDestroy()
    {
        MessageRouter.Instance.UnSubscribe<CSHeartBeatResponse>(_HeartBeatResponse);
        MessageRouter.Instance.UnSubscribe<ReconnectResponse>(_ReconnectResponse);
        Kaiyun.Event.UnregisterOut("SuccessConnectServer", this, "ConnectToServerCallback");
        Kaiyun.Event.UnregisterOut("FailedConnectServer", this, "ConnectToServerFailedCallback");
        Kaiyun.Event.UnregisterOut("Disconnected", this, "OnDisconnected");

        if (isEnableHeartBeat)
        {
            StopCoroutine(SendHeartMessage());
            isEnableHeartBeat = false;
        }
    }

    public void ConnectToLoginGate(Action action = null)
    {
        if (loginGateisConnected) return;
        //连接服务器
        connectLoginGateSuccessAction = action;
        curNetClient?.CloseConnection();
        curNetClient.Init("127.0.0.1", 10700, 10, _LoginGateConnectedCallback, _LoginGateConnectedFailedCallback, _LoginGateDisconnectedCallback);
    }
    private void _LoginGateConnectedCallback(NetClient tcpClient)
    {
        loginGateisConnected = true;
        //显示ui
        UIManager.Instance.MessagePanel.ShowTopMsg("成功连接到服务器.....");
        connectLoginGateSuccessAction?.Invoke();

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
            curNetClient.Send(req);
        }
    }
    private void _LoginGateConnectedFailedCallback(NetClient tcpClient, bool isEnd)
    {
        if (isEnd)
        {
            Log.Error("Connect  failed, the server may not be turned on");
        }
        else
        {
            //做一下重新连接
            Log.Error("Connect failed, attempting to reconnect");
        }

    }
    private void _LoginGateDisconnectedCallback(NetClient tcpClient)
    {
        loginGateisConnected = false;
        if (isEnableHeartBeat)
        {
            StopCoroutine(SendHeartMessage());
            isEnableHeartBeat = false;
        }

        //重连
        curNetClient.ReConnectToServer();
        UIManager.Instance.MessagePanel.ShowNetworkDisconnect();
        UIManager.Instance.MessagePanel.ShowLoadingBox("断线重连中...");
    }
    public void CloseConnection()
    {
        loginGateisConnected = false;
        if (isEnableHeartBeat)
        {
            StopCoroutine(SendHeartMessage());
            isEnableHeartBeat = false;
        }
        curNetClient.CloseConnection();
    }


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

            if (msg.EntityId == 0)
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



    IEnumerator SendHeartMessage()
    {
        //优化,防止不断在堆中创建新对象
        CSHeartBeatRequest beatReq = new();

        while (true)
        {
            yield return waitForSeconds;
            curNetClient.Send(beatReq);
            lastBeatTime = DateTime.Now;
        }
    }
    public void _HeartBeatResponse(Connection sender, CSHeartBeatResponse msg)
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
    /// 模拟异常断开网络
    /// </summary>
    public void SimulateAbnormalDisconnection()
    {
        curNetClient.SimulateAbnormalDisconnection();
    }
    private void OnApplicationQuit()
    {
        CloseConnection();
    }

}

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
using Newtonsoft.Json;
using static SelectWorldPanel;
using UnityEngine.Networking;
using System.Collections.Generic;
using HS.Protobuf.LoginGate;

public class NetManager : Singleton<NetManager>
{
    public NetClient m_curNetClient = new NetClient();
    public bool m_loginGateisConnected;
    public bool m_loginGateConnecting;
    private bool m_isEnableHeartBeat;

    private WaitForSeconds m_waitForSeconds = new WaitForSeconds(2f);             //心跳包时间控制
    private DateTime m_lastBeatTime = DateTime.MinValue;                          //上一次发送心跳包的时间

    private List<ServerInfo> m_loginGateInfo = new();
    public  string m_loginGateToken;

    public void Init()
    {
        //启动消息分发器，单线程即可
        MessageRouter.Instance.Start(1);
        ProtoHelper.Instance.Init();
        // proto
        ProtoHelper.Instance.Register<CSHeartBeatRequest>((int)CommonProtocl.CsHeartbeatReq);
        ProtoHelper.Instance.Register<CSHeartBeatResponse>((int)CommonProtocl.CsHeartbeatResp);
        ProtoHelper.Instance.Register<GetLoginGateTokenRequest>((int)LoginGateProtocl.GetLogingateTokenReq);
        ProtoHelper.Instance.Register<GetLoginGateTokenResponse>((int)LoginGateProtocl.GetLogingateTokenResp);
        //消息分发注册
        MessageRouter.Instance.Subscribe<CSHeartBeatResponse>(_HandleCSHeartBeatResponse);
        MessageRouter.Instance.Subscribe<GetLoginGateTokenResponse>(_HandleGetLoginGateTokenResponse);

        m_loginGateisConnected = false;
        m_isEnableHeartBeat = false;

        StartCoroutine(SendLoginGateRequest("http://8.138.30.5:12345/mmo/game-config.json"));
    }
    public void UnInit()
    {
        MessageRouter.Instance.UnSubscribe<CSHeartBeatResponse>(_HandleCSHeartBeatResponse);
        MessageRouter.Instance.UnSubscribe<ReconnectResponse>(_ReconnectResponse);

        if (m_isEnableHeartBeat)
        {
            StopCoroutine(_SendHeartMessage());
            m_isEnableHeartBeat = false;
        }
    }

    public IEnumerator SendLoginGateRequest(string uri)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            // 发送请求并等待返回
            yield return webRequest.SendWebRequest();

            if (webRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"Error: {webRequest.error}");
            }
            else
            {
                // 成功获取数据
                string json = webRequest.downloadHandler.text;

                // 可以在这里将json字符串解析为对象，或者进行其他处理
                // 例如，解析为自定义的数据结构
                // MyDataObject data = JsonUtility.FromJson<MyDataObject>(json);

                // 解析JSON
                RootObject rootObject = JsonConvert.DeserializeObject<RootObject>(json);

                if (rootObject != null && rootObject.ServerList != null)
                {
                    m_loginGateInfo.AddRange(rootObject.ServerList);
                }
            }
        }
        ConnectToLoginGate();
    }

    public void ConnectToLoginGate()
    {
        if (m_loginGateisConnected) return;
        if (m_loginGateConnecting) return;
        if (m_loginGateInfo.Count <= 0) return;
        m_loginGateConnecting = true;
        //连接服务器
        m_curNetClient.Init(m_loginGateInfo[0].host, m_loginGateInfo[0].port, 10,
            _LoginGateConnectedCallback, _LoginGateConnectedFailedCallback, _LoginGateDisconnectedCallback);
    }
    private void _LoginGateConnectedCallback(NetClient tcpClient)
    {
        UnityMainThreadDispatcher.Instance().Enqueue(() => {
            m_loginGateisConnected = true;
            m_loginGateConnecting = false;
            UIManager.Instance.MessagePanel.ShowTopMsg("成功连接到服务器.....");

            //发送心跳包
            StartCoroutine(_SendHeartMessage());
            m_isEnableHeartBeat = true;

            //判断是否是重连的。
            if (GameApp.SessionId != null)
            {
                //发送重新连接的清请求
                ReconnectRequest req = new ReconnectRequest
                {
                    SessionId = GameApp.SessionId
                };
                m_curNetClient.Send(req);
            }

            // 获取通信密钥流程
            SecurityService.Instance.SendExchangePublicKeyRequest();
        });
    }
    private void _LoginGateConnectedFailedCallback(NetClient tcpClient, bool isEnd)
    {
        UnityMainThreadDispatcher.Instance().Enqueue(() =>
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

        });
    }
    private void _LoginGateDisconnectedCallback(NetClient tcpClient)
    {
        UnityMainThreadDispatcher.Instance().Enqueue(() => {
            m_loginGateisConnected = false;
            if (m_isEnableHeartBeat)
            {
                StopCoroutine(_SendHeartMessage());
                m_isEnableHeartBeat = false;
            }

            //重连
            m_curNetClient.ReConnectToServer();
            UIManager.Instance.MessagePanel.ShowNetworkDisconnect();
            UIManager.Instance.MessagePanel.ShowLoadingBox("断线重连中...");
        });
    }
    private void OnApplicationQuit()
    {
        CloseConnection();
    }
    public void CloseConnection()
    {
        m_loginGateisConnected = false;
        if (m_isEnableHeartBeat)
        {
            StopCoroutine(_SendHeartMessage());
            m_isEnableHeartBeat = false;
        }
        m_curNetClient.CloseConnection();
    }

    private IEnumerator _SendHeartMessage()
    {
        //优化,防止不断在堆中创建新对象
        CSHeartBeatRequest beatReq = new();

        while (true)
        {
            yield return m_waitForSeconds;
            m_curNetClient.Send(beatReq);
            m_lastBeatTime = DateTime.Now;
        }
    }
    public void _HandleCSHeartBeatResponse(Connection sender, CSHeartBeatResponse msg)
    {
        //说明服务器和客户端之间连接是通畅的
        TimeSpan gap = DateTime.Now - m_lastBeatTime;
        int ms = (int)Math.Round(gap.TotalMilliseconds);

        UnityMainThreadDispatcher.Instance().Enqueue(() => {
            //ui处理
            UIManager.Instance.MessagePanel.ShowNetworkDelay(ms);
        });
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
    /// <summary>
    /// 模拟异常断开网络
    /// </summary>
    public void SimulateAbnormalDisconnection()
    {
        m_curNetClient.SimulateAbnormalDisconnection();
    }

    private void _HandleGetLoginGateTokenResponse(Connection sender, GetLoginGateTokenResponse message)
    {
        m_loginGateToken = message.LoginGateToken;
        Log.Debug($"以获取到loginToken:{m_loginGateToken}");
    }

}

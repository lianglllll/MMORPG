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
using UnityEngine.Networking;
using System.Collections.Generic;
using HS.Protobuf.LoginGate;
using Google.Protobuf;
using HS.Protobuf.GameGate;

public class NetManager : Singleton<NetManager>
{
    private WaitForSeconds m_waitForSeconds = new WaitForSeconds(2f);             //心跳包时间控制
    private DateTime m_lastBeatTime = DateTime.MinValue;                          //上一次发送心跳包的时间

    private NetClient m_loginGateClient = new NetClient();
    private List<TempServerInfo> m_loginGateInfo = new();
    public string m_loginGateToken;

    private NetClient m_gameGateClient = new NetClient();
    private List<ServerInfoNode> m_gameGateInfo = new();
    public string sessionId;

    public NetClient curNetClient;

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
        ProtoHelper.Instance.Register<GetGameGateByWorldIdRequest>((int)LoginProtocl.GetGameGateByWorldidReq);
        ProtoHelper.Instance.Register<GetGameGateByWorldIdResponse>((int)LoginProtocl.GetGameGateByWorldidResp);
        ProtoHelper.Instance.Register<VerifySessionRequeest>((int)GameGateProtocl.VerifySessionReq);
        ProtoHelper.Instance.Register<VerifySessionResponse>((int)GameGateProtocl.VerifySessionResp);

        //消息分发注册
        MessageRouter.Instance.Subscribe<CSHeartBeatResponse>(_HandleCSHeartBeatResponse);
        MessageRouter.Instance.Subscribe<GetLoginGateTokenResponse>(_HandleGetLoginGateTokenResponse);
        MessageRouter.Instance.Subscribe<GetGameGateByWorldIdResponse>(_HandleGetGameGateByWorldIdResponse);
        MessageRouter.Instance.Subscribe<VerifySessionResponse>(_HandleVerifySessionResponse);


        StartCoroutine(_SendGetLoginGatesRequest("http://8.138.30.5:12345/mmo/game-config.json"));

    }
    public void UnInit()
    {
        MessageRouter.Instance.UnSubscribe<CSHeartBeatResponse>(_HandleCSHeartBeatResponse);
        MessageRouter.Instance.UnSubscribe<ReconnectResponse>(_ReconnectResponse);
    }

    // loginGate
    public class RootObject
    {
        public TempServerInfo[] ServerList;
    }
    private IEnumerator _SendGetLoginGatesRequest(string uri)
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
        _ConnectToLoginGate();
    }
    private void _ConnectToLoginGate()
    {
        if (m_loginGateClient.IsConnected || m_loginGateClient.IsConnecting || m_loginGateInfo.Count <= 0) return;
        //连接服务器
        m_loginGateClient.Init(m_loginGateInfo[0].host, m_loginGateInfo[0].port, 10,
            _LoginGateConnectedCallback, _LoginGateConnectedFailedCallback, _LoginGateDisconnectedCallback);
    }
    private void _LoginGateConnectedCallback(NetClient tcpClient)
    {
        curNetClient = tcpClient;

        UnityMainThreadDispatcher.Instance().Enqueue(() => {
            UIManager.Instance.MessagePanel.ShowTopMsg("成功连接到服务器.....");

            //发送心跳包
            StartCoroutine(_SendLoginGateHeartBeatMessage());
            m_loginGateClient.IsHeartBeat = true;

            ////判断是否是重连的。
            //if (m_loginGateToken != null)
            //{
            //    //发送重新连接的请求
            //    ReconnectRequest req = new ReconnectRequest
            //    {
            //        SessionId = GameApp.SessionId
            //    };
            //    m_loginGateClient.Send(req);
            //}

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
            if (m_loginGateClient.IsHeartBeat)
            {
                StopCoroutine(_SendLoginGateHeartBeatMessage());
                m_loginGateClient.IsHeartBeat = false;
            }

            //重连
            m_loginGateClient.ReConnectToServer();
            UIManager.Instance.MessagePanel.ShowNetworkDisconnect();
            UIManager.Instance.MessagePanel.ShowLoadingBox("断线重连中...");
        });
    }
    private void _CloseLoginGateConnection()
    {
        if (m_loginGateClient.IsHeartBeat)
        {
            StopCoroutine(_SendLoginGateHeartBeatMessage());
            m_loginGateClient.IsHeartBeat = false;
        }
        m_loginGateClient.CloseConnection();
        m_loginGateClient.UnInit();
    }
    private IEnumerator _SendLoginGateHeartBeatMessage()
    {
        //优化,防止不断在堆中创建新对象
        CSHeartBeatRequest beatReq = new();

        while (true)
        {
            yield return m_waitForSeconds;
            m_lastBeatTime = DateTime.Now;
            m_loginGateClient.Send(beatReq);
        }
    }
    private void _HandleGetLoginGateTokenResponse(Connection sender, GetLoginGateTokenResponse message)
    {
        m_loginGateToken = message.LoginGateToken;
        Log.Debug($"以获取到loginToken:{m_loginGateToken}");
    }

    // GameGate
    public void SendGetGameGatesRequest(int worldId)
    {
        GetGameGateByWorldIdRequest req = new();
        req.WorldId = worldId;
        req.LoginGateToken = m_loginGateToken;
        req.SessionId = sessionId;
        Send(req);
    }
    private void _HandleGetGameGateByWorldIdResponse(Connection sender, GetGameGateByWorldIdResponse message)
    {
        var panel = UIManager.Instance.GetOpeningPanelByName("SelectWorldPanel");
        if (message.ResultCode != 0)
        {
            UnityMainThreadDispatcher.Instance().Enqueue(() =>
            {
                if (panel != null)
                {
                    (panel as SelectWorldPanel).HandleStartResponse(message.ResultCode, message.ResultMsg);
                }
            });
        }
        else
        {
            // 记录网关信息
            foreach (var item in message.GameGateInfos) {
                m_gameGateInfo.Add(item);
            }
            // ui提示
            UnityMainThreadDispatcher.Instance().Enqueue(() => {
                UIManager.Instance.MessagePanel.ShowTopMsg("正在连接世界门户...");
            });
            // 尝试连接
            ConnectToGameGate();
        }
    }
    public void ConnectToGameGate()
    {
        if (m_gameGateClient.IsConnected || m_gameGateClient.IsConnecting || m_gameGateInfo.Count <= 0) return;
        //连接服务器
        m_gameGateClient.Init(m_gameGateInfo[0].Ip, m_gameGateInfo[0].Port, 10,
            _GameGateConnectedCallback, _GameGateConnectedFailedCallback, _GameGateDisconnectedCallback);
    }
    private void _GameGateConnectedCallback(NetClient tcpClient)
    {
        UnityMainThreadDispatcher.Instance().Enqueue(() => {
            UIManager.Instance.MessagePanel.ShowTopMsg("成功连接到世界门户.....");

            // 切换通信网关，断开与loginGate的通信
            curNetClient = tcpClient;
            _CloseLoginGateConnection();

            //发送心跳包
            StartCoroutine(_SendGameGateHeartBeatMessage());
            m_gameGateClient.IsHeartBeat = true;

            // 验证session
            VerifySessionRequeest verifySessionRequeest = new();
            verifySessionRequeest.SessionId = sessionId;
            Send(verifySessionRequeest);

            // 获取通信密钥流程
            SecurityService.Instance.SendExchangePublicKeyRequest();

            ////判断是否是重连的
            //if (GameApp.SessionId != null)
            //{
            //    //发送重新连接的清请求
            //    ReconnectRequest req = new ReconnectRequest
            //    {
            //        SessionId = GameApp.SessionId
            //    };
            //    m_loginGateClient.Send(req);
            //}


        });
    }
    private void _GameGateConnectedFailedCallback(NetClient tcpClient, bool isEnd)
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
    private void _GameGateDisconnectedCallback(NetClient tcpClient)
    {
        UnityMainThreadDispatcher.Instance().Enqueue(() => {
            if (m_gameGateClient.IsHeartBeat)
            {
                StopCoroutine(_SendGameGateHeartBeatMessage());
                m_gameGateClient.IsHeartBeat = false;
            }

            //重连
            m_loginGateClient.ReConnectToServer();
            UIManager.Instance.MessagePanel.ShowNetworkDisconnect();
            UIManager.Instance.MessagePanel.ShowLoadingBox("断线重连中...");
        });
    }
    private IEnumerator _SendGameGateHeartBeatMessage()
    {
        //优化,防止不断在堆中创建新对象
        CSHeartBeatRequest beatReq = new();

        while (true)
        {
            yield return m_waitForSeconds;
            m_lastBeatTime = DateTime.Now;
            m_gameGateClient.Send(beatReq);
        }
    }
    private void _CloseGameGateConnection()
    {
        if (m_gameGateClient.IsHeartBeat)
        {
            StopCoroutine(_SendGameGateHeartBeatMessage());
            m_gameGateClient.IsHeartBeat = false;
        }
        m_gameGateClient.CloseConnection();
        m_gameGateClient.UnInit();
    }
    private void _HandleVerifySessionResponse(Connection sender, VerifySessionResponse message)
    {
        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            if (message.ResultCode == 0)
            {
                // 响应相关ui
                var panel = UIManager.Instance.GetOpeningPanelByName("SelectWorldPanel");
                if (panel != null)
                {
                    (panel as SelectWorldPanel).HandleStartResponse(0, null);
                }
            }
            else
            {
                UIManager.Instance.MessagePanel.ShowTopMsg(message.ResultMsg);
                // UIManager.Instance.MessagePanel.ShowTopMsg("什么黑客小子");
#if (UNITY_EDITOR)
                {
                    UnityEditor.EditorApplication.isPlaying = false;
                }
#else
                {
                    Application.Quit();
                }
#endif
            }
        });


    }


    // 工具
    public bool Send(IMessage message)
    {
        if (curNetClient != null)
        {
            return curNetClient.Send(message);
        }
        return false;
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
            ////关闭ui
            //UIManager.Instance.MessagePanel.HideLoadingBox();

            //if (!msg.Success)
            //{
            //    //没有登录信息可能是session超时了，跳回到登录界面
            //    GameApp.ClearGameAppData();
            //    EntityManager.Instance.Clear();
            //    UIManager.Instance.ClearAllOpenPanel();
            //    UIManager.Instance.OpenPanel("LoginPanel");
            //    return;
            //}

            //if (msg.EntityId == 0)
            //{
            //    //没有选择角色，我们跳回到选择角色的界面
            //    string sessionId = GameApp.SessionId;
            //    GameApp.ClearGameAppData();
            //    GameApp.SessionId = sessionId;
            //    EntityManager.Instance.Clear();
            //    UIManager.Instance.ClearAllOpenPanel();
            //    UIManager.Instance.OpenPanel("SelectRolePanel");
            //    return;
            //}

            //重连成功
            //这里不需要做任何操作

        });
    }
    public void SimulateAbnormalDisconnection()
    {
        m_loginGateClient.SimulateAbnormalDisconnection();
    }
    private void OnApplicationQuit()
    {
        if(curNetClient == m_loginGateClient)
        {
            _CloseLoginGateConnection();
        }
        else
        {
            _CloseGameGateConnection();
        }
        curNetClient = null;
    }

}

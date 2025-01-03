﻿using Common.Summer.Core;
using Common.Summer.Net;
using Common.Summer.Proto;
using Common.Summer.Tools;
using HS.Protobuf.Common;
using HS.Protobuf.ControlCenter;
using LoginServer.Core;
using LoginServer.Utils;
using Serilog;

namespace LoginServer.Net
{
    class ServerEntry
    {
        public ServerInfoNode ServerInfoNode { get; set; }
        public NetClient NetClient { get; set; }
    }

    public class ServersMgr : Singleton<ServersMgr>
    {
        private ServerInfoNode m_curSin;
        private Dictionary<SERVER_TYPE, ServerEntry> m_outgoingServerConnection = new();

        public void Init()
        {
            // 本服务器的信息
            m_curSin = new ServerInfoNode();
            LoginServerInfoNode lNode = new LoginServerInfoNode();
            m_curSin.ServerType = SERVER_TYPE.Login;
            m_curSin.Ip = Config.Server.ip;
            m_curSin.Port = Config.Server.port;
            m_curSin.ServerId = 0;
            m_curSin.LoginServerInfo = lNode;
            m_curSin.EventBitmap = SetEventBitmap();

            // 网络服务开启
            NetService.Instance.Init();
            LoginServerHandler.Instance.Init();

            // 协议注册
            ProtoHelper.Register<ServerInfoRegisterRequest>((int)ControlCenterProtocl.ServerinfoRegisterReq);
            ProtoHelper.Register<ServerInfoRegisterResponse>((int)ControlCenterProtocl.ServerinfoRegisterResp);

            // 消息的订阅
            MessageRouter.Instance.Subscribe<ServerInfoRegisterResponse>(_RegisterServerInfo2ControlCenterResponse);

            _ExecutePhase0();

        }
        public void UnInit()
        {

        }
        private int SetEventBitmap()
        {
            int bitmap = 0;
            List<ClusterEventType> events = new List<ClusterEventType>
            {
                ClusterEventType.DbproxyEnter,
            };
            foreach (var e in events)
            {
                bitmap |= (1 << (int)e);
            }
            return bitmap;
        }

        private bool _ExecutePhase0()
        {
            // 连接到控制中心cc
            _CCConnectToControlCenter();
            return true;
        }
        private bool _ExecutePhase1(Google.Protobuf.Collections.RepeatedField<ClusterEventNode> clusterEventNodes)
        {
            foreach (var node in clusterEventNodes)
            {
                if (node.EventType == ClusterEventType.DbproxyEnter)
                {
                    AddDBServerInfo(node.ServerInfoNode);
                }
            }
            return true;
        }
        private bool _ExecutePhase2()
        {
            // 开始网络监听，预示着当前服务器的正式启动
            NetService.Instance.Init2();
            return true;
        }

        // cc
        private void _CCConnectToControlCenter()
        {
            NetService.Instance.ConnctToServer(Config.CCConfig.ip, Config.CCConfig.port, _CCConnectedCallback, _CCConnectedFailedCallback, _CCDisconnectedCallback);
        }
        private void _CCConnectedCallback(NetClient tcpClient)
        {
            m_outgoingServerConnection.Add(SERVER_TYPE.Controlcenter, new ServerEntry { NetClient = tcpClient});
            Log.Information("[Successfully connected to the control center server.]");
            //向cc注册自己
            ServerInfoRegisterRequest req = new();
            req.ServerInfoNode = m_curSin;
            tcpClient?.Send(req);
        }
        private void _CCConnectedFailedCallback(NetClient tcpClient, bool isEnd)
        {
            if (isEnd)
            {
                Log.Error("Connect to controlCenter failed, the server may not be turned on");
            }
            else
            {
                //做一下重新连接
                Log.Error("Connect to controlCenter failed, attempting to reconnect controlCenter");
            }

        }
        private void _CCDisconnectedCallback(NetClient tcpClient)
        {
            
        }
        private void _RegisterServerInfo2ControlCenterResponse(Connection conn, ServerInfoRegisterResponse message)
        {
            if (message.ResultCode == 0)
            {
                m_curSin.ServerId = message.ServerId;
                Log.Information("[Successfully registered this server information with the ControlCenter.]");
                Log.Information($"The server ID of this server is [{message.ServerId}]");
                _ExecutePhase1(message.ClusterEventNodes);
            }
            else
            {
                Log.Error(message.ResultMsg);
            }
        }

        // db
        public void AddDBServerInfo(ServerInfoNode sin)
        {
            if (!m_outgoingServerConnection.ContainsKey(SERVER_TYPE.Dbproxy))
            {
                var entry = new ServerEntry();
                entry.ServerInfoNode = sin;
                m_outgoingServerConnection[SERVER_TYPE.Dbproxy] = entry;
                _ConnectToDB();
            }
        }
        private void _ConnectToDB()
        {
            ServerInfoNode node = m_outgoingServerConnection[SERVER_TYPE.Dbproxy].ServerInfoNode;
            NetService.Instance.ConnctToServer(node.Ip, node.Port,
                _DBConnectedCallback, _DBConnectedFailedCallback, _DBDisconnectedCallback);
        }
        private void _DBConnectedCallback(NetClient tcpClient)
        {
            Log.Information("Successfully connected to the DBProxy server.");
            // 记录
            m_outgoingServerConnection[SERVER_TYPE.Dbproxy].NetClient = tcpClient;
            _ExecutePhase2();
        }
        private void _DBConnectedFailedCallback(NetClient tcpClient, bool isEnd)
        {
            if (isEnd)
            {
                Log.Error("Connect to DBProxy server failed, the server may not be turned on");
            }
            else
            {
                //做一下重新连接
                Log.Error("Connect to DBProxy server failed, attempting to reconnect DBProxy server");
                Log.Error("重连还没写");
            }

        }
        private void _DBDisconnectedCallback(NetClient tcpClient)
        {

        }

    }
}
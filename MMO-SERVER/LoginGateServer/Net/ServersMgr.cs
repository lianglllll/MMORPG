﻿using Common.Summer.Core;
using Common.Summer.Net;
using Common.Summer.Proto;
using Common.Summer.Tools;
using GameServer.Utils;
using Google.Protobuf;
using HS.Protobuf.Common;
using HS.Protobuf.ControlCenter;
using HS.Protobuf.LoginGateMgr;
using LoginGateServer.Core;
using Serilog;

namespace LoginGateServer.Net
{
    public class ServersMgr : Singleton<ServersMgr>
    {
        private ServerInfoNode? m_curSin;
        private ServerInfoNode? m_curLGMSin;
        private ServerInfoNode? m_curLSin;
        private NetClient? m_CCClient;
        private NetClient? m_LGMClient;
        private NetClient? m_LClient;
        
        public void Init()
        {
            // 本服务器的信息
            m_curSin = new ServerInfoNode();
            LoginGateServerInfoNode lgNode = new();
            m_curSin.ServerType = SERVER_TYPE.Logingate;
            m_curSin.Ip = Config.Server.ip;
            m_curSin.Port = Config.Server.serverPort;
            m_curSin.ServerId = 0;
            m_curSin.LoginGateServerInfo = lgNode;

            // 部分网络服务开启
            NetService.Instance.Init();
            // 协议注册
            ProtoHelper.Register<ServerInfoRegisterRequest>((int)ControlCenterProtocl.ServerinfoRegisterReq);
            ProtoHelper.Register<ServerInfoRegisterResponse>((int)ControlCenterProtocl.ServerinfoRegisterResp);
            ProtoHelper.Register<RegisterLoginGateInstanceRequest>((int)LoginGateMgrProtocl.RegisterLogingateInstanceReq);
            ProtoHelper.Register<RegisterLoginGateInstanceResponse>((int)LoginGateMgrProtocl.RegisterLogingateInstanceResp);
            ProtoHelper.Register<GetAllServerInfoRequest>((int)ControlCenterProtocl.GetAllserverinfoReq);
            ProtoHelper.Register<GetAllServerInfoResponse>((int)ControlCenterProtocl.GetAllserverinfoResp);
            ProtoHelper.Register<ExecuteLGCommandRequest>((int)LoginGateMgrProtocl.ExecuteLgCommandReq);
            ProtoHelper.Register<ExecuteLGCommandResponse>((int)LoginGateMgrProtocl.ExecuteLgCommandResp);
            // 消息的订阅
            MessageRouter.Instance.Subscribe<ServerInfoRegisterResponse>(_RegisterServerInfo2ControlCenterResponse);
            MessageRouter.Instance.Subscribe<RegisterLoginGateInstanceResponse>(_RegisterLoginGateInstanceResponse);
            MessageRouter.Instance.Subscribe<GetAllServerInfoResponse>(_HandleGetAllServerInfoResponse);
            MessageRouter.Instance.Subscribe<ExecuteLGCommandRequest>(_ExecuteLGCommandRequest);

            // 连接到控制中心cc
            _ConnectToCC();
        }
        public void UnInit()
        {

        }

        // 一阶段：cc注册完成
        // 二阶段：lgm注册完成
        // 三阶段：l注册完成
        private bool _ExecutePhase1()
        {
            // 获取LoginGateMgr的信息
            var req = new GetAllServerInfoRequest();
            req.ServerType = SERVER_TYPE.Logingatemgr;
            m_CCClient.Send(req);
            return true;
        }
        private bool _ExecutePhase2()
        {
            return true;
        }
        private bool _ExecutePhase3()
        {
            // 开始网络监听，预示着当前服务器的正式启动
            NetService.Instance.Init2();
            return true;
        }

        private void _ConnectToCC()
        {
            NetService.Instance.ConnctToServer(Config.CCConfig.ip, Config.CCConfig.port, _CCConnectedCallback, _CCConnectedFailedCallback, _CCDisconnectedCallback);
        }
        private void _CCConnectedCallback(NetClient tcpClient)
        {
            Log.Information("Successfully connected to the control center server.");
            // 记录
            m_CCClient = tcpClient;
            // 向cc注册自己
            ServerInfoRegisterRequest req = new();
            req.ServerInfoNode = m_curSin;
            tcpClient.Send(req);
        }
        private void _CCConnectedFailedCallback(NetClient tcpClient,bool isEnd)
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
                Log.Information("Successfully registered this server information with the ControlCenter.");
                Log.Information($"The server ID of this server is [{message.ServerId}]");
                _ExecutePhase1();
            }
            else
            {
                Log.Error(message.ResultMsg);
            }
        }
       
        private void _HandleGetAllServerInfoResponse(Connection conn, GetAllServerInfoResponse message)
        {
            var list = message.ServerInfoNodes;
            if (list.Count == 0)
            {
                Log.Error("No LoginGateMgr server information was obtained.");
                Log.Information("The server may not be start.");
                return;
            }
            m_curLGMSin = list[0];    // 集群只有一个 
            _ConnectToLGM();
        }
        private void _ConnectToLGM()
        {
            NetService.Instance.ConnctToServer(m_curLGMSin.Ip, m_curLGMSin.Port,
                _LoginGateMgrConnectedCallback, _LoginGateMgrConnectedFailedCallback, _LoginGateMgrDisconnectedCallback);
        }
        private void _LoginGateMgrConnectedCallback(NetClient tcpClient)
        {
            Log.Information("Successfully connected to the LoginGateMgr server.");
            
            // 记录
            m_LGMClient = tcpClient;

            // 向lgm注册自己
            RegisterLoginGateInstanceRequest req = new();
            req.ServerInfoNode = m_curSin;
            m_LGMClient.Send(req);
        }
        private void _LoginGateMgrConnectedFailedCallback(NetClient tcpClient, bool isEnd)
        {
            if (isEnd)
            {
                Log.Error("Connect to loginGateMgr failed, the server may not be turned on");
            }
            else
            {
                //做一下重新连接
                Log.Error("Connect to loginGateMgr failed, attempting to reconnect loginGateMgr");
                Log.Error("重连还没写");
            }

        }
        private void _LoginGateMgrDisconnectedCallback(NetClient tcpClient)
        {

        }
        private void _RegisterLoginGateInstanceResponse(Connection conn, RegisterLoginGateInstanceResponse message)
        {
            if (message.ResultCode == 0)
            {
                // 注册成功我们等待分配任务。
                Log.Information("Successfully registered this server information with the LoginGateMgr.");
                _ExecutePhase2();
            }
            else
            {
                Log.Error(message.ResultMsg);
            }
        }

        private void _ConnectToL()
        {
            NetService.Instance.ConnctToServer(m_curLSin.Ip, m_curLSin.Port,
                _LoginConnectedCallback, _LoginConnectedFailedCallback, _LoginDisconnectedCallback);
        }
        private void _LoginConnectedCallback(NetClient tcpClient)
        {
            Log.Information("Successfully connected to the Login server.");

            // 记录
            m_LClient = tcpClient;

            _ExecutePhase3();
        }
        private void _LoginConnectedFailedCallback(NetClient tcpClient, bool isEnd)
        {
            if (isEnd)
            {
                Log.Error("Connect to login failed, the server may not be turned on");
            }
            else
            {
                //做一下重新连接
                Log.Error("Connect to login failed, attempting to reconnect login");
                Log.Error("重连还没写");




            }

        }
        private void _LoginDisconnectedCallback(NetClient tcpClient)
        {

        }

        private void _ExecuteLGCommandRequest(Connection conn, ExecuteLGCommandRequest message)
        {
            switch (message.Command)
            {
                case LoginGateCommand.Start:
                    _ExecuteStart(message);
                    break;
                case LoginGateCommand.Stop:
                    break;
                case LoginGateCommand.Resume:
                    break;
                case LoginGateCommand.End:
                    break;
                default:
                    Log.Error("[ServersMgr._ExecuteLGCommandRequest]未识别的命令");
                    break;
            }
        }
        private bool _ExecuteStart(ExecuteLGCommandRequest message)
        {
            if(message.LoginServerInfoNode != null)
            {
                m_curLSin = message.LoginServerInfoNode;
                _ConnectToL();
                return true;
            }
            return false;
        }

        public void SentToLoginServer(ByteString data)
        {
            m_LClient.Send(data);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;//用于byte[]拼接的
using System.Text;//encoding
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using Summer;
using Summer.Network;
using Serilog;
using Google.Protobuf;
using GameServer.Model;
using Proto;
using  System.Threading;
using GameServer.Database;
using GameServer.Manager;
using GameServer.core;

namespace GameServer.Network
{

    /// <summary>
    /// 网络服务
    /// </summary>
    public class NetService
    {
        //负责监听TCP网络端口
        TcpServer tcpServer;

        //记录conn最后一次心跳包的时间
        private Dictionary<Connection, DateTime> heartBeatPairs = new Dictionary<Connection, DateTime>();
        //心跳超时时间
        private static int HEARTBEATTIMEOUT = 5;
        //服务器查询心跳字典的间隔时间
        private static int HEARTBEATQUERYTIME = 5;


        /*
         构造方法
         */
        public NetService()
        {
            tcpServer = new TcpServer("127.0.0.1", 6666);
            tcpServer.Connected += OnConnected;
            tcpServer.Disconnected += OnDisconnected;            
        }

        /*
         开始服务
         */
        public void Start()
        {
            //启动网络监听
            tcpServer.Start();

            //启动消息分发器
            MessageRouter.Instance.Start(8);

            //订阅心跳事件
            MessageRouter.Instance.Subscribe<HeartBeatRequest>(_HeartBeatRequest);

            //定时检查心跳包的情况
            Timer timer = new Timer(TimerCallback, null, TimeSpan.Zero, TimeSpan.FromSeconds(HEARTBEATQUERYTIME));

        }


        //心跳包处理
        public void _HeartBeatRequest(Connection conn, HeartBeatRequest message)
        {
            //更新心跳时间
            heartBeatPairs[conn] = DateTime.Now;
            
            //知道当前连接还活着
            //Log.Information("[消息]收到心跳包：" + conn);

            //响应
            HeartBeatResponse resp = new HeartBeatResponse();
            conn.Send(resp);
        }

        //定时检查心跳包的情况
        private void TimerCallback(object state)
        {
            //Log.Information("[心跳检查]执行心跳检查");
            DateTime nowTime = DateTime.Now;
            //这里规定心跳包超过30秒没用更新就将连接清理
            foreach(var kv in heartBeatPairs)
            {
                 TimeSpan gap =  nowTime - kv.Value;
                if(gap.TotalSeconds> HEARTBEATTIMEOUT)
                {
                    //关闭超时的客户端连接
                    Connection conn = kv.Key;
                    Log.Information("[心跳检查]心跳超时==>");
                    conn.Close();
                    heartBeatPairs.Remove(kv.Key);
                }
            }
        }

        /*
        客户端连接成功的回调
        */
        private  void OnConnected(Connection conn)
        {
            //接收到客户端的socket
           var ipe = conn.Socket.RemoteEndPoint as IPEndPoint;//向下转型
           Log.Information("[连接成功]" + ipe.Address + ":" + ipe.Port);

            //给conn添加心跳时间
            heartBeatPairs[conn] = DateTime.Now;
            //给conn添加一个session
            conn.Set<Session>(new Session());
        }

        /*
         客户端断开连接回调
         */
        private  void OnDisconnected(Connection conn)
        {

            //如果玩家在场景中就让其离开场景
            Character chr = conn.Get<Session>().character;
            Space space = chr?.currentSpace;
            if (space != null)
            {
                space.CharacterLeave(conn, chr);
                CharacterManager.Instance.RemoveCharacter(chr.Id);
            }



            //从心跳字典中删除，这里交给心跳超时统一管理
            heartBeatPairs.Remove(conn);


            //测试信息
            DbUser dbUser =  conn.Get<Session>().dbUser;
            if (dbUser != null)
            {
                Log.Information("[连接断开]用户名：" + dbUser.Username);
            }
            else
            {
                Log.Information("[连接断开]未知用户");
            }
        }
    }
}

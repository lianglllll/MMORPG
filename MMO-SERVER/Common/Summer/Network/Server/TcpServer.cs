using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using Serilog;
using Google.Protobuf;

namespace Summer.Network
{

    /// <summary>
    /// 负责监听TCP网络端口，异步接收Socket连接
    /// 三个委托
    /// ----Connected    有新的连接
    /// ----DataReceived 有新的消息
    /// ----Disconnected 有连接断开
    /// IsRunning   是否正在运行
    /// Stop()      关闭服务器
    /// Start()     启动服务器
    /// </summary>
    public class TcpServer
    {
        //网络连接的属性
        private IPEndPoint endPoint;    //网络终结点
        private Socket serverSocket;    //服务端监听对象
        private int backlog = 100;      //可以排队接收的传入连接数

        //委托
        public delegate void ConnectedCallback(Connection conn);                
        public delegate void DataReceivedCallback(Connection conn,IMessage data);
        public delegate void DisconnectedCallback(Connection conn);             
        //事件
        public event ConnectedCallback Connected;       //连接
        public event DataReceivedCallback DataReceived;//消息接收
        public event DisconnectedCallback Disconnected;//连接断开



        /*
         构造方法
         */
        public TcpServer(string host,int port)
        {
            endPoint = new IPEndPoint(IPAddress.Parse(host), port);
        }
        public TcpServer(string host, int port,int backlog)
        {
            endPoint = new IPEndPoint(IPAddress.Parse(host), port);
            this.backlog = backlog;
        }

        /*
         是否在运行
         */
        public bool IsRunning
        {
            get
            {
                return serverSocket != null;
            }
        }

        /*
         开始监听
         */
        public void Start()
        {
            if (!IsRunning)
            {
                serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                serverSocket.Bind(endPoint);
                serverSocket.Listen(backlog);//等待队列长度 
                //Log.Information("开始监听端口：" + endPoint.Port);

                SocketAsyncEventArgs args = new SocketAsyncEventArgs();//这玩意可以复用
                args.Completed += OnAccept;//当有用户的连接时触发回调函数
                serverSocket.AcceptAsync(args);//异步接收

            }
            else
            {
                Log.Information("TcpServer already running");
            }
        }


        /*
        关闭监听
        */
        public void Stop()
        {
            if (serverSocket == null)
            {
                return;
            }
            serverSocket.Close();
            serverSocket = null;
        }



        /*
         连接成功的回调
         */
        private void OnAccept(object sender, SocketAsyncEventArgs e)
        {
            //连入的客户端
            Socket client = e.AcceptSocket;
            SocketError flag = e.SocketError;

            //继续接收下一位(异步操作)
            e.AcceptSocket = null;
            serverSocket.AcceptAsync(e);


            //有人连接进来//todo?不清楚是否会被覆盖
            if (flag == SocketError.Success)
            {
                if (client != null)
                {
                    //通过委托连接和消息转发出去
                    //将收消息和断开连接的消息向上传递给NetService
                    Connection conn = new Connection(client);
                    conn.OnDataReceived += (conn, data) => DataReceived?.Invoke(conn, data);//这里使用匿名函数
                    conn.OnDisconnected += (conn) => Disconnected?.Invoke(conn);
                    //将连接成功也委托出去
                    Connected?.Invoke(conn);
                }    

            }
           
        }
    }
}

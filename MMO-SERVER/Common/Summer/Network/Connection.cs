using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using Google.Protobuf;
using Google.Protobuf.Reflection;
using Summer.Core;
using Serilog;
using Summer.core;
using Proto;

namespace Summer.Network
{


    /// <summary>
    /// 通用的网络连接，可以继承本类进行功能扩展
    /// 职责：1.接收、发送网络消息
    ///       2.处理网络断开的时候需要有一个事件通知(回调)
    ///       3.关闭连接(回调)
    /// </summary>
    public class Connection:TypeAttributeStore
    {
        
        private Socket _socket;//连接客户端的socket
        public Socket Socket
        {
            get
            {
                return _socket;
            }
        }

        public delegate void DataReceivedHandler(Connection sender,IMessage data);
        public delegate void DisconnectedHandler(Connection sender);
        public DataReceivedHandler OnDataReceived;//消息接收的委托
        public DisconnectedHandler OnDisconnected;//关闭连接的委托

        /*
         构造函数
         */
        public Connection(Socket socket)
        {
            this._socket = socket;

            //给这个客户端连接创建一个解码器
            var lfd = new LengthFieldDecoder(socket, 64 * 1024, 0, 4, 0, 4);
            lfd.Received += OnDataRecived;
            lfd.disconnected += ()=> OnDisconnected?.Invoke(this); //向上转发，让其删除本connection对象
            lfd.Start();//启动解码器，开始接收消息

        }

        //主动关闭连接
        public void Close()
        {
            _socket?.Close();//其实就是四次挥手的开始，发送fin信号
            _socket = null;
        }

        /*
            接收消息的回调
         */
        private void OnDataRecived(byte[] data)
        {

            ushort code = GetUShort(data, 0);
            var msg = ProtoHelper.ParseFrom((int)code, data, 2, data.Length-2);

            //忽略心跳检测
            if(msg is not HeartBeatRequest)
            Log.Information("[消息]收到请求类型：{0}-{1}", code,msg);

            if (MessageRouter.Instance.Running)
            {
                MessageRouter.Instance.AddMessage(this,msg);
            }

        }

        #region 发送网络数据包


        /*
         发送消息包，编码过程(通用)
         */
        public void Send(Google.Protobuf.IMessage message)
        {

            //获取imessage类型所对应的编号，网络传输我们只传输编号
            using (var ds = DataStream.Allocate())
            {
                int code = ProtoHelper.SeqCode(message.GetType());
                ds.WriteInt(message.CalculateSize()+2);
                ds.WriteUShort((ushort)code);
                message.WriteTo(ds);
                SocketSend(ds.ToArray());
                    
            }
        }

        /*
         通过socket发送，原生数据
         */
        private void SocketSend(byte[] data)
        {
            SocketSend(data, 0, data.Length);
        }


        /*
         开始异步发送消息,原生数据
         */
        private void SocketSend(byte[] data, int start, int len)
        {
            lock (this)//多线程问题，防止争夺send
            {
                if (_socket!=null && _socket.Connected)
                {
                    _socket.BeginSend(data, start, len, SocketFlags.None, new AsyncCallback(SendCallback), _socket);
                }
            }
        }

        /*
         异步发送消息回调
         */
        private void SendCallback(IAsyncResult ar)
        {
            // 发送的字节数
            int len = _socket.EndSend(ar);
        }


        #endregion

        //前提：data必须是大端字节序
        private ushort GetUShort(byte[] data, int offset)
        {
            if (BitConverter.IsLittleEndian)
                return (ushort)((data[offset] << 8) | data[offset + 1]);
            return (ushort)((data[offset + 1] << 8) | data[offset]);
        }


    }
}

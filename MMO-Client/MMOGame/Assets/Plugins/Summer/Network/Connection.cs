using Google.Protobuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Serilog;
using Google.Protobuf.Reflection;
using Summer.Core;

namespace Summer.Network
{
    /// <summary>
    /// 通用网络连接，可以继承此类实现功能拓展
    /// 职责：发送消息，关闭连接，断开回调，接收消息回调，
    /// </summary>
    public class Connection
    {

        private Socket _socket;
        private SocketReceiver lfd;

        public delegate void DataReceivedCallback(Connection sender, IMessage data);   
        public delegate void DisconnectedCallback(Connection sender);
        public DataReceivedCallback OnDataReceived;         //接收到数据的委托，现在没啥用
        public DisconnectedCallback OnDisconnected;         //连接断开的委托

        /// <summary>
        /// 构造函数
        /// 创建消息解码器
        /// </summary>
        /// <param name="socket"></param>
        public Connection(Socket socket)
        {
            this._socket = socket;
            //创建解码器
            lfd = new SocketReceiver(socket);
            lfd.DataReceived += _received;
            lfd.Disconnected += _OnDisconnected;
            //启动解码器
            lfd.Start();
        }

        /// <summary>
        /// 消息接收回调
        /// </summary>
        /// <param name="data"></param>
        private void _received(byte[] data)
        {
            //获取消息序列号
            ushort code = GetUShort(data, 0);
            var msg = ProtoHelper.ParseFrom(code, data, 2, data.Length - 2);
            //交付消息路由处理
            if (MessageRouter.Instance.Running)
            {
                MessageRouter.Instance.AddMessage(this,msg);
            }
            //通知上层
            OnDataReceived?.Invoke(this, msg);
        }

        /// <summary>
        /// 连接端口回调
        /// </summary>
        private void _OnDisconnected()
        {
            OnDisconnected?.Invoke(this);
        }

        /// <summary>
        /// 主动关闭连接
        /// </summary>
        public void _Close()
        {
            _socket = null;
            lfd._Close();
        }

        #region 发送网络数据包

        /// <summary>
        /// 发送proto包
        /// </summary>
        /// <param name="message"></param>
        public void Send(IMessage message)
        {
            using(var ds = DataStream.Allocate())
            {
                int code = ProtoHelper.SeqCode(message.GetType());
                ds.WriteInt(message.CalculateSize()+2);
                ds.WriteUShort((ushort)code);
                message.WriteTo(ds);
                this.SocketSend(ds.ToArray());
            }
            
        }

        /// <summary>
        /// 通过socket发送原生数据
        /// </summary>
        /// <param name="data"></param>
        private void SocketSend(byte[] data)
        {
            this.SocketSend(data,0, data.Length);
        }

        /// <summary>
        /// 通过socket异步发送原生数据
        /// </summary>
        /// <param name="data"></param>
        /// <param name="offset"></param>
        /// <param name="len"></param>
        private void SocketSend(byte[] data, int offset, int len)
        {
            lock (this)
            {
                if (_socket.Connected)
                {
                    _socket.BeginSend(data, offset, len, SocketFlags.None, new AsyncCallback(SendCallback), _socket);
                }
            }
        }

        /// <summary>
        /// 通过socket异步发送原生数据回调
        /// </summary>
        /// <param name="ar"></param>
        private void SendCallback(IAsyncResult ar)
        {
            // 发送的字节数
            int len = _socket.EndSend(ar);
        }

        #endregion

        /// <summary>
        /// 通过小端方式获取data的前两个字节, 前提是data必须是大端字节序
        /// </summary>
        /// <param name="data"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        private ushort GetUShort(byte[] data, int offset)
        {
            if (BitConverter.IsLittleEndian)
            {
                return (ushort)((data[offset] << 8) | data[offset + 1]);
            }
            else
            {
                return (ushort)((data[offset + 1] << 8) | data[offset]);
            }
        }

    }
}

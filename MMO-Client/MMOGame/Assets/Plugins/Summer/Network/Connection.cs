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

        public delegate void DataReceivedCallback(Connection sender, IMessage data);
        public delegate void DisconnectedCallback(Connection sender);

        private Socket _socket;
        
        public Socket Socket
        {
            get { return _socket; }
        }
        

        /// <summary>
        /// 接收到数据
        /// </summary>
        public DataReceivedCallback OnDataReceived;
        /// <summary>
        /// 连接断开
        /// </summary>
        public DisconnectedCallback OnDisconnected;

        public Connection(Socket socket)
        {
            this._socket = socket;
            //创建解码器
            var lfd = new SocketReceiver(socket);
            lfd.DataReceived += _received;
            lfd.Disconnected += ()=> OnDisconnected?.Invoke(this);
            lfd.Start();//启动解码器
        }


        private void _received(byte[] data)
        {
            //Log.Debug("收到消息：len={0}", data.Length);
            //获取消息序列号
            ushort code = GetUShort(data, 0);
            var msg = ProtoHelper.ParseFrom(code, data, 2, data.Length - 2);

            if (MessageRouter.Instance.Running)
            {
                MessageRouter.Instance.AddMessage(this,msg);
            }

            OnDataReceived?.Invoke(this, msg);

        }



        /// <summary>
        /// 主动关闭连接
        /// </summary>
        public void Close()
        {
            try { _socket.Shutdown(SocketShutdown.Both); } catch { }
            _socket.Close();
            _socket = null;
            OnDisconnected?.Invoke(this);
        }


        #region 发送网络数据包



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

        //通过socket发送原生数据
        private void SocketSend(byte[] data)
        {
            this.SocketSend(data,0, data.Length);
        }
        //通过socket发送原生数据
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
        //前提是data必须是大端字节序
        private ushort GetUShort(byte[] data,int offset)
        {
            if (BitConverter.IsLittleEndian)
            {
                return (ushort)((data[offset] << 8) | data[offset+1]);
            }
            else
            {
                return (ushort)((data[offset+1] << 8) | data[offset]);
            }
        }

        private void SendCallback(IAsyncResult ar)
        {
            // 发送的字节数
            int len = _socket.EndSend(ar);
        }

        #endregion
    }
}

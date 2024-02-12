using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Summer.Network
{

    /// <summary>
    /// 客户端的消息解码器
    /// 这个就是服务端的LengthFieldDecoder的简化版本
    /// </summary>
    public class SocketReceiver
    {
        private bool IsRunning;

        private Socket mSocket;
        private byte[] buffer;                                      //缓冲区
        private int startIndex = 0;                                 //每次读取的开始位置

        public delegate void DataReceivedEventHandler(byte[] data);
        public delegate void DisconnectedEventHandler();
        public event DataReceivedEventHandler DataReceived;        //成功收到消息的委托事件
        public event DisconnectedEventHandler Disconnected;        //连接断开的委托事件

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="socket"></param>
        public SocketReceiver(Socket socket):this(socket, 1024 * 64)
        {
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="socket"></param>
        /// <param name="bufferSize"></param>
        public SocketReceiver(Socket socket,int bufferSize)
        {
            this.mSocket = socket;
            buffer = new byte[bufferSize];
        }

        /// <summary>
        /// 开启解码器
        /// </summary>
        public void Start()
        {
            BeginReceive();
            IsRunning = true;
        }

        /// <summary>
        /// 异步接收数据
        /// </summary>
        private void BeginReceive(){
            mSocket.BeginReceive(buffer, startIndex, buffer.Length - startIndex, SocketFlags.None, new AsyncCallback(ReceiveCallback), null);
        }

        /// <summary>
        /// 异步接收数据回调
        /// </summary>
        /// <param name="ar"></param>
        private void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                int len = mSocket.EndReceive(ar);
                // len==0代表接收到fin信号了
                if (len == 0)
                {
                    _disconnected();
                    return;
                }
                //解析数据
                doReceive(len);
                //继续接收
                BeginReceive();
            }
            catch (SocketException)
            {
                _disconnected();
            }
            catch (Exception)
            {
                _disconnected();
            }


        }

        /// <summary>
        /// 解析数据
        /// </summary>
        /// <param name="len"></param>
        private void doReceive(int len)
        {
            //解析数据
            int remain = startIndex + len;
            int offset = 0;
            while (remain > 4)
            {
                int msgLen = GetInt32BE(buffer, offset);
                if (remain < msgLen + 4)
                {
                    break;
                }
                //解析消息
                byte[] data = new byte[msgLen];
                Array.Copy(buffer, offset + 4, data, 0, msgLen);
                //解析消息
                try { DataReceived?.Invoke(data); } catch { Log.Debug("消息解析异常"); }
                
                offset += msgLen + 4;
                remain -= msgLen + 4;
            }
            
            if (remain > 0)
            {
                Array.Copy(buffer, offset, buffer, 0, remain);
            }

            startIndex = remain;

        }

        /// <summary>
        /// 断开连接
        /// </summary>
        private void _disconnected()
        {
            try
            {
                if (IsRunning)
                {
                    Disconnected?.Invoke();
                }

                if (mSocket != null)
                {
                    mSocket.Shutdown(SocketShutdown.Both);
                    mSocket.Close();
                }
            }
            catch (SocketException ex)
            {
                // 可以根据需要处理特定的 SocketException 类型
                // 例如：特定错误码的处理、日志记录等
                Console.WriteLine($"SocketException: {ex.Message}");
            }
            catch (ObjectDisposedException)
            {
                // 当 mSocket 已被关闭时可能会引发 ObjectDisposedException
                // 可以在这里处理该异常，或选择忽略
            }
            finally
            {
                IsRunning = false;
                mSocket?.Dispose(); // 确保释放资源
                mSocket = null;
            }

        }

        /// <summary>
        /// 给conn主动调用断开连接用
        /// </summary>
        public void _Close()
        {
            IsRunning = false;
            _disconnected();
        }

        /// <summary>
        /// 在大端数据中以小端格式获取数据前4个字节
        /// </summary>
        /// <param name="data"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        private int GetInt32BE(byte[] data, int index)
        {
            return (data[index] << 0x18) | (data[index + 1] << 0x10) | (data[index + 2] << 8) | (data[index + 3]);
        }

    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Summer;
using System.Net;
using System.Net.Sockets;
using Summer.Network;
using System;
using System.Threading;
using Google.Protobuf;

/// <summary>
/// 网络客户端操作对象
/// 主要是发送数据用
/// 接收数据通过con对象交付消息路由转发了
/// </summary>
public class NetClient
{
    private static Socket clientSocket = null;
    private static Connection conn = null;
    private static ManualResetEvent connectDone = new ManualResetEvent(false);

    /// <summary>
    /// 连接到服务器
    /// </summary>
    /// <param name="host">ip</param>
    /// <param name="port">端口</param>
    public static void ConnectToServer(string serverIP, int serverPort)
    {
        if (clientSocket != null && clientSocket.Connected) return;

        //新的socket对象
        clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        // 创建SocketAsyncEventArgs对象
        SocketAsyncEventArgs connectArgs = new SocketAsyncEventArgs();
        connectArgs.RemoteEndPoint = new IPEndPoint(IPAddress.Parse(serverIP), serverPort);
        connectArgs.Completed += new EventHandler<SocketAsyncEventArgs>(ConnectCallback);

        // 异步连接
        clientSocket.ConnectAsync(connectArgs);

        // 等待连接完成的信号
        connectDone.WaitOne();

    }

    /// <summary>
    /// 连接的异步回调
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private static void ConnectCallback(object sender, SocketAsyncEventArgs e)
    {
        if (e.SocketError == SocketError.Success)
        {
            conn = new Connection(clientSocket);
            conn.OnDisconnected += OnDisconnected;
            Kaiyun.Event.FireOut("SuccessConnectServer");
        }
        else
        {
            Debug.Log("连接服务器失败==>" + e);
            Kaiyun.Event.FireOut("FailedConnectServer");
        }

        // 释放连接完成的信号
        connectDone.Set();
    }

    /// <summary>
    /// 与服务器连接断开回调
    /// </summary>
    /// <param name="sender"></param>
    private static void OnDisconnected(Connection sender)
    {
        conn = null;
        //网络连接断开
        Kaiyun.Event.FireOut("Disconnected");
    }

    /// <summary>
    /// 关闭网络客户端
    /// </summary>
    public static void Close()
    {
        conn?._Close();
        conn = null;
    }

    /// <summary>
    /// 发送poto协议包
    /// </summary>
    /// <param name="message"></param>
    public static void Send(IMessage message)
    {
        if (conn != null)
        {
            conn.Send(message);
        }
    }

    /// <summary>
    /// 模拟异常断开网络
    /// </summary>
    public static void SimulateAbnormalDisconnection()
    {
        clientSocket?.Close();
    }
}

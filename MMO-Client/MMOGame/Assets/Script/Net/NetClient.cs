using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Summer;
using System.Net;
using System.Net.Sockets;
using Summer.Network;
using System;
using Google.Protobuf;

/// <summary>
/// 网络客户端操作对象
/// 主要是发送数据用
/// 接收数据通过con对象交付消息路由转发了
/// </summary>
public class NetClient
{
    private static Connection conn = null;

    /// <summary>
    /// 连接到服务器
    /// </summary>
    /// <param name="host">ip</param>
    /// <param name="port">端口</param>
    public static void ConnectToServer(string host, int port)
    {

        try
        {
            //服务器终端
            IPEndPoint ipe = new IPEndPoint(IPAddress.Parse(host), port);
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Connect(ipe);
            Debug.Log("连接到服务端");
            conn = new Connection(socket);
            conn.OnDisconnected += OnDisconnected;
            //启动消息分发器，单线程即可
            MessageRouter.Instance.Start(1);
        }
        catch (SocketException e)
        {
            Debug.Log("连接服务器失败==>"+e);
        }

    }

    /// <summary>
    /// 与服务器连接断开回调
    /// </summary>
    /// <param name="sender"></param>
    private static void OnDisconnected(Connection sender)
    {
        Debug.Log("与服务器断开");
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

}

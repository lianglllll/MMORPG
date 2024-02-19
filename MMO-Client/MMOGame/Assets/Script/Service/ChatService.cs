using Proto;
using Summer;
using Summer.Network;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 主要负责聊天消息的发送与接收
/// </summary>
public class ChatService : Singleton<ChatService>,IDisposable
{
    public ChatService()
    {

    }

    /// <summary>
    /// 初始化
    /// </summary>
    public void Init()
    {
        MessageRouter.Instance.Subscribe<ChatResponse>(_ChatResponse);
        MessageRouter.Instance.Subscribe<ChatResponseOne>(_ChatResponseOne);
    }

    public void Dispose()
    {
        MessageRouter.Instance.Off<ChatResponse>(_ChatResponse);
        MessageRouter.Instance.Off<ChatResponseOne>(_ChatResponseOne);
    }



    private void _ChatResponseOne(Connection sender, ChatResponseOne msg)
    {
        if (msg.Result == Result.Success)
        {
            ChatManager.Instance.AddMessage(ChatChannel.Local, msg.Message);

        }
        else if (msg.Result == Result.Fault)
        {
            ChatManager.Instance.AddSystemMessage(msg.Errormsg);
        }
    }

    /// <summary>
    /// 处理服务器发送过来的信息包：有全部频道的信息
    /// </summary>
    /// <param name="conn"></param>
    /// <param name="msg"></param>
    private void _ChatResponse(Connection conn, ChatResponse msg)
    {

        if(msg.Result == Result.Success)
        {
            ChatManager.Instance.AddMessage(ChatChannel.Local, msg.LocalMessages);
            //ChatManager.Instance.AddMessage(ChatChannel.World, msg.WorldMessages);
            //ChatManager.Instance.AddMessage(ChatChannel.System, msg.SystemMessages);
            //ChatManager.Instance.AddMessage(ChatChannel.Private, msg.PrivateMessages);
            //ChatManager.Instance.AddMessage(ChatChannel.Team, msg.TeamMessages);
            //ChatManager.Instance.AddMessage(ChatChannel.Guild, msg.GuildMessages);
        }
        else if(msg.Result == Result.Fault)
        {
            ChatManager.Instance.AddSystemMessage(msg.Errormsg);
        }
    }


    /// <summary>
    /// 发送消息
    /// </summary>
    /// <param name="channel"></param>
    /// <param name="content"></param>
    /// <param name="to_id"></param>
    /// <param name="to_name"></param>
    public void SendChatMessage(ChatChannel channel,string content,int to_id,string to_name)
    {
        ChatRequest req = new ChatRequest();
        ChatMessage msg = new ChatMessage();
        msg.Channel = channel;
        if(channel == ChatChannel.Private)
        {
            msg.ToId = to_id;
            msg.ToName = to_name;
        }
        msg.Content = content;
        req.Message = msg;
        NetClient.Send(req);

        //测试用
        if(content == "close")
        {
            NetStart.Instance.SimulateAbnormalDisconnection();
        }
    }



}

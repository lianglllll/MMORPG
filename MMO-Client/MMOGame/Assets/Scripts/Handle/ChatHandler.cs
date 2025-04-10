using HSFramework.MySingleton;
using Common.Summer.Core;
using Common.Summer.Net;
using HS.Protobuf.Chat;
using System;
using UnityEngine;

public class ChatHandler : SingletonNonMono<ChatHandler>
{
    public void Init()
    {
        // 协议注册
        ProtoHelper.Instance.Register<SendChatMessageRequest>((int)ChatProtocl.SendChatMessageReq);
        ProtoHelper.Instance.Register<ChatMessageResponse>((int)ChatProtocl.ChatMessageResp);
        // 消息的订阅
        MessageRouter.Instance.Subscribe<ChatMessageResponse>(_HandleChatMessageResponse);
    }
    public void UnInit()
    {
        MessageRouter.Instance.UnSubscribe<ChatMessageResponse>(_HandleChatMessageResponse);
    }

    public void SendChatMessage(ChatMessageChannel channel, string content, string to_chrId = null, string to_name = null)
    {
        var req = new SendChatMessageRequest();
        var chatMsg = new ChatMessageV2();
        req.ChatMessage = chatMsg;
        chatMsg.Channel = channel;
        if(channel == ChatMessageChannel.Private)
        {
            chatMsg.ToChrId = to_chrId;
            chatMsg.ToChrName = to_name;
        }
        chatMsg.Content = content;
        var result = NetManager.Instance.Send(req);

        if(result == false)
        {
            Debug.LogError("消息发送失败");
        }

        //测试用
        if (content == "close")
        {
            NetManager.Instance.SimulateAbnormalDisconnection();
        }
    }
    private void _HandleChatMessageResponse(Connection sender, ChatMessageResponse message)
    {
        ChatManager.Instance.AddMessages(message.ChatMessages);
    }
}

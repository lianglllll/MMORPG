using Google.Protobuf.Collections;
using Proto;
using Summer;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChatManager : Singleton<ChatManager>
{
    //当前聊天框选中的频道
    private LocalChannel sendChannel;
    public Action<LocalChannel> OnChat { get; set; }                //事件，触发ui去更新当前的频道信息
    public int PrivateID { get; private set; }                      //如果在私聊频道的画，私聊对象的id
    public string PrivateName { get; private set; }                 //如果在私聊频道的画，私聊对象的name

    /// <summary>
    /// 本地的频道类型
    /// </summary>
    public enum LocalChannel
    {
        All = 0,
        Local = 1,
        World = 2,
        Team = 3,
        Guild = 4,
        Private = 5,
    }

    /// <summary>
    /// 用来转换本地的channel到网络传输中的channel
    /// </summary>
    public ChatChannel SendChannel(LocalChannel channel)
    {
      
        switch (channel)
        {
            case LocalChannel.Local:
                return ChatChannel.Local;
            case LocalChannel.World:
                return ChatChannel.World;
            case LocalChannel.Team:
                return ChatChannel.Team;
            case LocalChannel.Guild:
                return ChatChannel.Guild;
            case LocalChannel.Private:
                return ChatChannel.Private;
        }
        return ChatChannel.Local;
        
    }

    /// <summary>
    /// 用来网络传输中的channel转换本地的channel
    /// </summary>
    /// <param name="channel"></param>
    /// <returns></returns>
    public LocalChannel ReciveChannel(ChatChannel channel)
    {
        
        switch (channel)
        {
            case ChatChannel.Local:
                return LocalChannel.Local;
            case ChatChannel.World:
                return LocalChannel.World;
            case ChatChannel.Team:
                return LocalChannel.Team;
            case ChatChannel.Guild:
                return LocalChannel.Guild;
            case ChatChannel.Private:
                return LocalChannel.Private;
        }
        return LocalChannel.Local;
    }

    /// <summary>
    /// 记录频道信息的list
    /// </summary>
    public Dictionary<ChatChannel, List<ChatMessage>> reciveChannels = new Dictionary<ChatChannel, List<ChatMessage>>();
    public Dictionary<LocalChannel, int> reciveChannelsIndex = new Dictionary<LocalChannel, int>
    {
        { LocalChannel.All ,-1},
        { LocalChannel.Guild ,-1},
        { LocalChannel.Local ,-1},
        { LocalChannel.Private ,-1},
        { LocalChannel.Team,-1},
        { LocalChannel.World,-1},
    };                          //指向list最后一个


    /// <summary>
    /// 设置当前chatbox的channel
    /// </summary>
    /// <param name="localChannel"></param>
    public void SetSendChannel(LocalChannel localChannel)
    {
        sendChannel = localChannel;
    }

    /// <summary>
    /// 添加一堆信息
    /// </summary>
    /// <param name="channel"></param>
    /// <param name="messages"></param>
    public void AddMessage(ChatChannel channel, RepeatedField<ChatMessage> messages)
    {
        if (!reciveChannels.ContainsKey(channel))
        {
            reciveChannels[channel] = new List<ChatMessage>();
        }
        foreach(var e in messages)
        {
            reciveChannels[channel].Add(e);
        }
        //通知频道UI该更新
        UnityMainThreadDispatcher.Instance().Enqueue(() => {
            OnChat?.Invoke(ReciveChannel(channel));
        });
    }

    /// <summary>
    /// 添加一条信息
    /// </summary>
    /// <param name="channel"></param>
    /// <param name="messages"></param>
    public void AddMessage(ChatChannel channel, ChatMessage messages)
    {
        if (!reciveChannels.ContainsKey(channel))
        {
            reciveChannels[channel] = new List<ChatMessage>();
        }        
        reciveChannels[channel].Add(messages);
        //通知频道UI该更新
        UnityMainThreadDispatcher.Instance().Enqueue(() => {
            OnChat?.Invoke(ReciveChannel(channel));
        });
    }


    /// <summary>
    /// 添加系统信息
    /// </summary>
    /// <param name="message"></param>
    public void AddSystemMessage(string message)
    {
        //直接加到all_channel频道即可
        this.reciveChannels[ChatChannel.System].Add(new ChatMessage()
        {
            Channel = ChatChannel.System,
            Content = message,
            FromName = ""
        });
        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            OnChat?.Invoke(LocalChannel.All);
        });

    }

    /// <summary>
    /// 发送信息
    /// </summary>
    /// <param name="content"></param>
    public void SendChat(string content)
    {
        ChatService.Instance.SendChatMessage(SendChannel(sendChannel), content, this.PrivateID, this.PrivateName);
    }

    public List<ChatMessage> GetNewChatMsg(LocalChannel channel)
    {
        int num =  reciveChannels[SendChannel(channel)].Count - reciveChannelsIndex[channel] -1  ;
        if (num == 0) return null;
        var result =  reciveChannels[SendChannel(channel)].GetRange(reciveChannelsIndex[channel] + 1, num);
        reciveChannelsIndex[channel] += num;
        return result;
    }


}

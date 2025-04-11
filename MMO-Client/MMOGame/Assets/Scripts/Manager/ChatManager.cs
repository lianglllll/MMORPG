using Google.Protobuf.Collections;
using HS.Protobuf.Chat;
using System;
using System.Collections.Generic;
using HSFramework.MySingleton;
using GameClient;

public class ChatManager : SingletonNonMono<ChatManager>
{
    private ChatMessageChannel m_curChannel;                               
    public Action OnChat { get; set; }          // 事件，触发ui去更新当前的频道信息
    public Dictionary<ChatMessageChannel, List<ChatMessageV2>> m_reciveChannels = new();

    // 私聊用
    public string PrivateID { get; private set; }                   // 如果在私聊频道的话，私聊对象的id
    public string PrivateName { get; private set; }                 // 如果在私聊频道的话，私聊对象的name

    public bool SetCurChannel(ChatMessageChannel curChannel)
    {
        m_curChannel = curChannel;
        return true;
    }
    public void AddMessages(RepeatedField<ChatMessageV2> chatMessages)
    {
        bool isHaveCurShowChannelMsg = false;

        foreach (var msg in chatMessages)
        {
            var channel = msg.Channel;

            if (!m_reciveChannels.ContainsKey(channel))
            {
                m_reciveChannels.Add(channel, new List<ChatMessageV2>());
            }

            switch (channel)
            {
                case ChatMessageChannel.Local:
                    break;
                case ChatMessageChannel.Scene:
                    if (msg.SceneId != GameApp.SceneId) continue;
                    break;
                case ChatMessageChannel.World:
                    break;
                case ChatMessageChannel.System:
                    break;
                case ChatMessageChannel.Team:
                    break;
                case ChatMessageChannel.Guild:
                    break;
                case ChatMessageChannel.Private:
                    break;
            }
            m_reciveChannels[channel].Add(msg);
            if(m_curChannel == channel)
            {
                isHaveCurShowChannelMsg = true;
            }
        }

        if (isHaveCurShowChannelMsg)
        {
            // 通知频道UI该更新
            UnityMainThreadDispatcher.Instance().Enqueue(() => {
                OnChat?.Invoke();
            });
        }
    }
    public void SendChatMessage(string content)
    {
        ChatHandler.Instance.SendChatMessage(m_curChannel, content, PrivateID, PrivateName);
    }
    public List<ChatMessageV2> GetAllCurChannelMsg()
    {
        m_reciveChannels.TryGetValue(m_curChannel, out var msgs);
        if(msgs == null)
        {
            return new List<ChatMessageV2>();
        }
        else
        {
            return msgs;
        }
    }
}

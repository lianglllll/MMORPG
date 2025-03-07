﻿using GameServer.Net;
using GameServer.Model;
using GameServer.Manager;
using GameServer.InventorySystem;
using Common.Summer.Tools;
using Common.Summer.Core;
using Common.Summer.Net;
using HS.Protobuf.Chat;
using HS.Protobuf.Game.Backpack;

namespace GameServer.Service
{
    public class ChatService:Singleton<ChatService>
    {
        public void Start()
        {
            MessageRouter.Instance.Subscribe<ChatRequest>(_ChatRequest);
        }

        private void _ChatRequest(Connection conn, ChatRequest message)
        {
            //获取当前的sender的info，填充信息
            Character chr = conn.Get<Session>().character;
            message.Message.FromId = chr.AcotrId;
            message.Message.FromName = chr.Name;
            message.Message.Time = MyTime.time;     
            //Log.Information("_ChatRequest:: character:{0}-Channel:{1}-Message:{2}", chr.Id, message.Message.Channel, message.Message.Content);

            //私聊
            if(message.Message.Channel == ChatChannel.Private)
            {
                //如果是私聊，且对方不在线
                var targetChr = CharacterManager.Instance.GetCharacter(message.Message.ToId);
                if(targetChr == null)
                {
                    ChatResponse res = new ChatResponse();
                    res.ResultCode = 1;
                    res.ResultMsg = "对方不在线";
                    res.PrivateMessages.Add(message.Message);
                    conn.Send(res);
                }
                else
                {
                    ChatResponse res = new ChatResponse();
                    res.ResultCode = 0;
                    res.PrivateMessages.Add(message.Message);
                    //向target发送res

                    //然后给发送者发送一条发送成功的消息
                    conn.Send(res);
                }

            }
            //地图，all，工会，队伍
            else
            {
                //转交管理器处理
                ChatManager.Instance.AddMessage(chr, message.Message);
                //广播
                ChatResponseOne res = new ChatResponseOne();
                res.ResultCode = 0;
                res.Message = message.Message;
                chr.currentSpace.Broadcast(res);
            }

            if (message.Message.Content == "-wear")
            {
                var def = DataManager.Instance.ItemDefinedDict[1005];
                var item = new Equipment(def);
                chr.equipmentManager.Wear(item,true);
            }

            if (message.Message.Content == "-unload")
            {

                chr.equipmentManager.Unload(EquipsType.Boots,true);
            }
        }

    }
}

﻿using Common.Summer.Core;
using Common.Summer.Net;
using Common.Summer.Tools;
using DBProxyServer.Core;
using HS.Protobuf.DBProxy.DBCharacter;
using HS.Protobuf.DBProxy.DBUser;

namespace DBProxyServer.Handle
{
    public class CharacterHandler : Singleton<CharacterHandler>
    {
        public void Init()
        {
            // 协议注册
            ProtoHelper.Instance.Register<GetDBCharacterByCidRequest>((int)DBCharacterProtocl.GetDbcharacterByCidReq);
            ProtoHelper.Instance.Register<GetDBCharacterByCidReponse>((int)DBCharacterProtocl.GetDbcharacterByCidResp);
            ProtoHelper.Instance.Register<AddDBCharacterRequset>((int)DBCharacterProtocl.AddDbcharacterReq);
            ProtoHelper.Instance.Register<AddDBCharacterResponse>((int)DBCharacterProtocl.AddDbcharacterResp);
            ProtoHelper.Instance.Register<DeleteDBCharacterByCidRequest>((int)DBCharacterProtocl.DeleteDbcharacterByCidReq);
            ProtoHelper.Instance.Register<DeleteDBCharacterByCidResponse>((int)DBCharacterProtocl.DeleteDbcharacterByCidResp);
            ProtoHelper.Instance.Register<GetDBCharactersByUidRequest>((int)DBCharacterProtocl.GetDbcharactersByUidReq);
            ProtoHelper.Instance.Register<GetDBCharactersByUidResponse>((int)DBCharacterProtocl.GetDbcharactersByUidResp);
            ProtoHelper.Instance.Register<SaveDBCharacterRequest>((int)DBCharacterProtocl.SaveDbcharactersReq);

            // 消息的订阅
            MessageRouter.Instance.Subscribe<GetDBCharacterByCidRequest>(_HandleGetDBCharacterByCidRequest);
            MessageRouter.Instance.Subscribe<AddDBCharacterRequset>(_HandleAddDBCharacterRequset);
            MessageRouter.Instance.Subscribe<DeleteDBCharacterByCidRequest>(_HandleDeleteDBCharacterRequest);
            MessageRouter.Instance.Subscribe<GetDBCharactersByUidRequest>(_HandleGetDBCharactersByUidRequest);
            MessageRouter.Instance.Subscribe<SaveDBCharacterRequest>(_HandleSaveDBCharacterRequestAsync);
        }

        public async void _HandleGetDBCharacterByCidRequest(Connection sender, GetDBCharacterByCidRequest message)
        {
            GetDBCharacterByCidReponse resp = new();
            resp.TaskId = message.TaskId;
            DBCharacterNode cNode = await CharacterOperations.Instance.GetCharacterByCidAsync(message.CId, message.ReadMask);
            if (cNode == null)
            {
                resp.ResultCode = 1;
                goto End;
            }
            resp.ChrNode = cNode;
            resp.ResultCode = 0;
        End:
            sender.Send(resp);
        }
        private async void _HandleGetDBCharactersByUidRequest(Connection conn, GetDBCharactersByUidRequest message)
        {
            GetDBCharactersByUidResponse resp = new();
            resp.TaskId = message.TaskId;
            var list = await CharacterOperations.Instance.GetCharactersByUWidAsync(message.UId, message.WorldId, message.ReadMask);
            if (list != null)
            {
                resp.CNodes.AddRange(list);
            }
            resp.ResultCode = 0;
            conn.Send(resp);
        }
        public async void _HandleAddDBCharacterRequset(Connection sender, AddDBCharacterRequset message)
        {
            AddDBCharacterResponse resp = new();
            resp.TaskId = message.TaskId;

            // chr数量上限
            var existIds = await UserOperations.Instance.GetCharacterIdsAsync(message.ChrNode.UId, message.ChrNode.WorldId);
            if (existIds.Count >= 4)
            {
                resp.ResultCode = 1;
                goto End;
            }

            // 名字是否重复
            var isExist = await CharacterOperations.Instance.CheckCharacterNameExistenceAsync(message.ChrNode.ChrName);
            if (isExist)
            {
                resp.ResultCode = 2;
                goto End;
            }

            var Node = message.ChrNode;
            Node.Level = 1;
            Node.CreationTimestamp = Scheduler.UnixTime;

            string cId = await CharacterOperations.Instance.AddCharacterAsync(Node);
            if (cId != null)
            {
                resp.CId = cId;
                resp.ResultCode = 0;
            }
            else
            {
                resp.ResultCode = 3;
            }
        End:
            sender.Send(resp);
        }
        private async void _HandleDeleteDBCharacterRequest(Connection sender, DeleteDBCharacterByCidRequest message)
        {
            DeleteDBCharacterByCidResponse resp = new();
            resp.TaskId = message.TaskId;

            bool successs = await CharacterOperations.Instance.RemoveCharacterByCidAsync(message.CId);
            if (successs)
            {
                resp.ResultCode = 0;
            }
            else
            {
                resp.ResultCode = 1;
            }

            sender.Send(resp);
        }
        private async void _HandleSaveDBCharacterRequestAsync(Connection conn, SaveDBCharacterRequest message)
        {
            await CharacterOperations.Instance.SaveCharacterAsync(message.CNode);
        }
    }
}

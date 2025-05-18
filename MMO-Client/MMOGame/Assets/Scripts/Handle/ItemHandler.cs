using Common.Summer.Core;
using Common.Summer.Net;
using HS.Protobuf.Backpack;
using HSFramework.MySingleton;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemHandler : SingletonNonMono<ItemHandler>
{
    public void Init()
    {
        // 协议注册
        ProtoHelper.Instance.Register<GetItemInventoryDataRequest>((int)BackpackProtocol.GetItemInventoryDataReq);
        ProtoHelper.Instance.Register<GetItemInventoryDataResponse>((int)BackpackProtocol.GetItemInventoryDataResp);
        ProtoHelper.Instance.Register<ChangeItemPositionRequest>((int)BackpackProtocol.ChangeItemPositionReq);
        ProtoHelper.Instance.Register<ChangeItemPositionResponse>((int)BackpackProtocol.ChangeItemPositionResp);
        ProtoHelper.Instance.Register<UseItemRequest>((int)BackpackProtocol.UseItemReq);
        ProtoHelper.Instance.Register<UseItemResponse>((int)BackpackProtocol.UseItemResp);
        ProtoHelper.Instance.Register<DiscardItemRequest>((int)BackpackProtocol.DiscardItemReq);
        ProtoHelper.Instance.Register<DiscardItemResponse>((int)BackpackProtocol.DiscardItemResp);
        ProtoHelper.Instance.Register<WearEquipRequest>((int)BackpackProtocol.WearEquipReq);
        ProtoHelper.Instance.Register<WearEquipResponse>((int)BackpackProtocol.WearEquipResp);
        ProtoHelper.Instance.Register<UnloadEquipRequest>((int)BackpackProtocol.UnloadEquipReq);
        ProtoHelper.Instance.Register<UnloadEquipResponse>((int)BackpackProtocol.UnloadEquipResp);

        // 消息的订阅
        MessageRouter.Instance.Subscribe<GetItemInventoryDataResponse>(HandleGetItemInventoryDataResponse);
        MessageRouter.Instance.Subscribe<ChangeItemPositionResponse>(HandleChangeItemPositionResponse);
        MessageRouter.Instance.Subscribe<UseItemResponse>(HandleUseItemResponse);
        MessageRouter.Instance.Subscribe<DiscardItemResponse>(HandleDiscardItemResponse);
        MessageRouter.Instance.Subscribe<WearEquipResponse>(HandleWearEquipResponse);
        MessageRouter.Instance.Subscribe<UnloadEquipResponse>(HandleUnloadEquipResponse);
    }

    public void SendGetItemInventoryDataRequest(ItemInventoryType type)
    {
        var req = new GetItemInventoryDataRequest();
        req.Type = type;
        NetManager.Instance.Send(req);
    }
    private void HandleGetItemInventoryDataResponse(Connection sender, GetItemInventoryDataResponse message)
    {
        ItemDataManager.Instance.ReloadInventoryData(message.Node);
    }


    private void HandleChangeItemPositionResponse(Connection sender, ChangeItemPositionResponse message)
    {
        throw new NotImplementedException();
    }
    private void HandleUseItemResponse(Connection sender, UseItemResponse message)
    {
        throw new NotImplementedException();
    }
    private void HandleDiscardItemResponse(Connection sender, DiscardItemResponse message)
    {
        throw new NotImplementedException();
    }
    private void HandleWearEquipResponse(Connection sender, WearEquipResponse message)
    {
        throw new NotImplementedException();
    }
    private void HandleUnloadEquipResponse(Connection sender, UnloadEquipResponse message)
    {
        throw new NotImplementedException();
    }
}

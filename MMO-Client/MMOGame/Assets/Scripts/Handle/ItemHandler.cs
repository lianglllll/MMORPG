using Common.Summer.Core;
using Common.Summer.Net;
using GameClient;
using HS.Protobuf.Backpack;
using HS.Protobuf.Scene;
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
        ProtoHelper.Instance.Register<PickUpSceneItemRequest>((int)SceneProtocl.PickupSceneItemReq);
        ProtoHelper.Instance.Register<PickupSceneItemResponse>((int)SceneProtocl.PickupSceneItemResp);
        ProtoHelper.Instance.Register<WearEquipRequest>((int)BackpackProtocol.WearEquipReq);
        ProtoHelper.Instance.Register<WearEquipResponse>((int)BackpackProtocol.WearEquipResp);
        ProtoHelper.Instance.Register<UnloadEquipRequest>((int)BackpackProtocol.UnloadEquipReq);
        ProtoHelper.Instance.Register<UnloadEquipResponse>((int)BackpackProtocol.UnloadEquipResp);

        // 消息的订阅
        MessageRouter.Instance.Subscribe<GetItemInventoryDataResponse>(HandleGetItemInventoryDataResponse);
        MessageRouter.Instance.Subscribe<ChangeItemPositionResponse>(HandleChangeItemPositionResponse);
        MessageRouter.Instance.Subscribe<UseItemResponse>(HandleUseItemResponse);
        MessageRouter.Instance.Subscribe<DiscardItemResponse>(HandleDiscardItemResponse);
        MessageRouter.Instance.Subscribe<PickupSceneItemResponse>(HandlePickupSceneItemResponse);
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

    public void SendChangeItemPositionRequest(ItemInventoryType originInventoryType, ItemInventoryType targetInventoryType, int originIndex, int targetIndex)
    {
        ChangeItemPositionRequest req = new();
        req.OriginInventory = originInventoryType;
        req.OriginIndex = originIndex;
        req.TargetInventory = targetInventoryType;
        req.TargetIndex = targetIndex;
        NetManager.Instance.Send(req);
    }
    private void HandleChangeItemPositionResponse(Connection sender, ChangeItemPositionResponse message)
    {
        // temp
        SendGetItemInventoryDataRequest(ItemInventoryType.Backpack);
    }

    public void SendUseItemRequest(int slotIndex, int count)
    {
        var req = new UseItemRequest();
        req.GridIndex = slotIndex;
        req.Count = count;
        NetManager.Instance.Send(req);
    }
    private void HandleUseItemResponse(Connection sender, UseItemResponse message)
    {
        SendGetItemInventoryDataRequest(ItemInventoryType.Backpack);
    }

    public void SendDiscardItemRequest(int slotIndex, int count, ItemInventoryType type)
    {
        var req = new DiscardItemRequest();
        req.Type = type;
        req.GridIndex = slotIndex;
        req.Count = count;
        //req.Seq
        NetManager.Instance.Send(req);
    }
    private void HandleDiscardItemResponse(Connection sender, DiscardItemResponse message)
    {
        // 刷一下ui
        if (message.ResultCode == 0)
        {
            Kaiyun.Event.FireOut("SceneItemChange");

            var item = LocalDataManager.Instance.m_itemDefineDict[message.ItemId];
            UnityMainThreadDispatcher.Instance().Enqueue(() =>
            {
                UIManager.Instance.MessagePanel.ShowItemIOInfo($"丢弃物品:{item.Name}X{message.Count}");
            });

            SendGetItemInventoryDataRequest(ItemInventoryType.Backpack);
        }

    }

    public void SendPickupSceneItemRequest(int itemEntityId)
    {
        var req = new PickUpSceneItemRequest();
        req.ItemEntityId = itemEntityId;
        req.EntityId = GameApp.entityId;
        NetManager.Instance.Send(req);
    }
    private void HandlePickupSceneItemResponse(Connection sender, PickupSceneItemResponse message)
    {
        // 刷一下ui
        if (message.ResultCode == 0)
        {
            SendGetItemInventoryDataRequest(ItemInventoryType.Backpack);

            Kaiyun.Event.FireOut("SceneItemChange");

            var item = LocalDataManager.Instance.m_itemDefineDict[message.ItemId];
            UnityMainThreadDispatcher.Instance().Enqueue(() =>
            {
                UIManager.Instance.MessagePanel.ShowItemIOInfo($"拾取物品:{item.Name}X{message.Count}");
            });
        }
    }

    public void SendWearEquipRequest(int knapsackSlotIndex, EquipSlotType type)
    {
        var req = new WearEquipRequest();
        req.GridIndex = knapsackSlotIndex;
        req.EquipSlotType = type;
        NetManager.Instance.Send(req);
    }
    private void HandleWearEquipResponse(Connection sender, WearEquipResponse message)
    {
        SendGetItemInventoryDataRequest(ItemInventoryType.Equipments);
        SendGetItemInventoryDataRequest(ItemInventoryType.Backpack);
    }

    public void SendUnloadEquipRequest(EquipSlotType type)
    {
        var req = new UnloadEquipRequest();
        req.Type = type;
        NetManager.Instance.Send(req);
    }
    private void HandleUnloadEquipResponse(Connection sender, UnloadEquipResponse message)
    {
        SendGetItemInventoryDataRequest(ItemInventoryType.Equipments);
        SendGetItemInventoryDataRequest(ItemInventoryType.Backpack);
    }
}

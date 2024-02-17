using GameClient.Entities;
using Proto;
using Summer;
using Summer.Network;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemService : Singleton<ItemService>, IDisposable
{
    /// <summary>
    /// 初始化，gamemanager中启用
    /// </summary>
    public void Init()
    {
        MessageRouter.Instance.Subscribe<InventoryInfoResponse>(_InventoryInfoResponse);
        MessageRouter.Instance.Subscribe<ItemPlacementResponse>(_ItemPlacementResponse); 
        MessageRouter.Instance.Subscribe<NetItemEntitySync>(_NetItemEntitySync); 
        MessageRouter.Instance.Subscribe<ItemUseResponse>(_ItemUseResponse); 
    }

    public void Dispose()
    {
        MessageRouter.Instance.Off<InventoryInfoResponse>(_InventoryInfoResponse);
        MessageRouter.Instance.Off<ItemPlacementResponse>(_ItemPlacementResponse);
        MessageRouter.Instance.Off<NetItemEntitySync>(_NetItemEntitySync);
        MessageRouter.Instance.Off<ItemUseResponse>(_ItemUseResponse);

    }


    /// <summary>
    /// 获取背包信息
    /// </summary>
    /// <returns></returns>
    public void _InventoryInfoRequest()
    {
        //发送查询请求，查询背包信息
        InventoryInfoRequest req = new InventoryInfoRequest();
        req.EntityId = GameApp.character.EntityId;

        req.QueryKnapsack = true;
        NetClient.Send(req);
    }

    /// <summary>
    /// 获取inventory信息的响应,这里暂时用作本机角色使用的
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="msg"></param>
    private void _InventoryInfoResponse(Connection conn, InventoryInfoResponse msg)
    {
        var chr = GameApp.character;
        if (chr == null || chr.EntityId != msg.EntityId) return;

        if (msg.KnapsackInfo != null)
        {
            //缓存背包信息
            RemoteDataManager.Instance.LoadLocalCharacterKnapsack(msg.KnapsackInfo);
        }

    }

    /// <summary>
    /// 物品放置请求
    /// </summary>
    /// <param name="originType"></param>
    /// <param name="targetType"></param>
    /// <param name="originSlot"></param>
    /// <param name="targetSlot"></param>
    public void ItemPlacementRequeset(ItemPlacementRequest req)
    {
        NetClient.Send(req);
    }

    /// <summary>
    /// 物品放置请求的响应
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="msg"></param>
    private void _ItemPlacementResponse(Connection conn, ItemPlacementResponse msg)
    {
        var action = RemoteDataManager.Instance.GetItemUIActionById(msg.ActionId);
        if (action == null) return;
        action.isReceiveAResponse = true;
        action.actionResult = msg.Result;
        action.AuxiliarySpace = msg.AuxiliarySpace;

        //符合顺序
        if(action == RemoteDataManager.Instance.ItemUIAtionsQueue.Peek())
        {
            RemoteDataManager.Instance.ProcessItemUIAction();
        }
    }

    /// <summary>
    /// 场景中的物品信息同步响应
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="msg"></param>
    private void _NetItemEntitySync(Connection sender, NetItemEntitySync msg)
    {
        EntityManager.Instance.OnItemEntitySync(msg.NetItemEntity);
    }

    /// <summary>
    /// 发送使用物品请求
    /// </summary>
    /// <param name="slotIndex"></param>
    public void ItemUseRequest(int slotIndex,int count)
    {
        ItemUseRequest req = new ItemUseRequest();
        req.EntityId = GameApp.character.EntityId;
        req.SlotIndex = slotIndex;
        req.Count = count;
        NetClient.Send(req);
    }


    /// <summary>
    /// 使用物品请求响应
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="msg"></param>
    private void _ItemUseResponse(Connection conn, ItemUseResponse msg)
    {
        //server下发的数据一定是正确的，如果有问题，我们就请求拉去背包数据
        if(msg.Result == true)
        {
            //更新背包数据
            RemoteDataManager.Instance.UpdateKnapsackItemAmount(msg);
        }
    }
}

using GameClient.Entities;
using GameServer.Model;
using Proto;
using Summer;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//缓存itemUI的操作
public class ItemUIAction
{
    public ItemPlacementRequest req;
    public bool isReceiveAResponse;
    public bool actionResult;
    public int AuxiliarySpace;
}

/// <summary>
/// 用于缓存远程服务器的一些数据
/// </summary>
public class RemoteDataManager : Singleton<RemoteDataManager>
{

    private int ItemUIActionId;
    public Queue<ItemUIAction> ItemUIAtionsQueue = new Queue<ItemUIAction>();
    public Inventory localCharacterKnapsack;

    /// <summary>
    /// 初始化
    /// </summary>
    public void Init()
    {
        localCharacterKnapsack = null;
        ItemUIActionId = 0;
    }

    //势必让这个类比较混乱，是不是应该使用一个knapsackmanager之类的来管理一下呢？
    /// <summary>
    ///获取本地角色的背包信息
    /// </summary>
    public void GetLocalChasracterKnapsack()
    {
        ItemService.Instance._InventoryInfoRequest();
    }

    /// <summary>
    /// 获取本地角色背包信息的响应
    /// </summary>
    /// <param name="knapsackInfo"></param>
    public void LoadLocalCharacterKnapsack(InventoryInfo knapsackInfo)
    {
        localCharacterKnapsack = new Inventory(GameApp.character);
        localCharacterKnapsack.Init(knapsackInfo);
        //发出背包更新的事件，如果背包面板存在就会处理这个事件
        Kaiyun.Event.FireOut("UpdateCharacterKnapsackData");
    }

    /// <summary>
    /// 物品拾起操作
    /// </summary>
    /// <param name="msg"></param>
    public void ItemPickup(int entityId,int itemId)
    {
        ItemPlacement(InventoryType.CurrentScene, InventoryType.Knapsack, entityId, itemId);
    }

    /// <summary>
    /// 物品的放置操作
    /// </summary>
    /// <param name="originType"></param>
    /// <param name="targetType"></param>
    /// <param name="originIndex"></param>
    /// <param name="targetIndex"></param>
    public void ItemPlacement(InventoryType originType, InventoryType targetType, int originIndex, int targetIndex)
    {
        ItemPlacementRequest req = new ItemPlacementRequest();
        req.EntityId = GameApp.character.EntityId;
        req.OriginInventoryTpey = originType;
        req.TargetInventoryTpey = targetType;
        req.OriginIndex = originIndex;
        req.TargetIndex = targetIndex;
        ItemUIActionId++;
        ItemUIActionId %= 10;
        req.ActionId = ItemUIActionId;

        var action = new ItemUIAction();
        action.req = req;
        action.isReceiveAResponse = false;
        action.actionResult = false;
        lock (ItemUIAtionsQueue)
        {
            ItemUIAtionsQueue.Enqueue(action);
        }

        ItemService.Instance.ItemPlacementRequeset(req);
    }

    /// <summary>
    /// 物品使用操作
    /// </summary>
    public void ItemUse(int slotIndex,int count)
    {
        ItemService.Instance.ItemUseRequest(slotIndex, count);
    }

    /// <summary>
    /// 更新背包ui中某个itemui的数量显示
    /// </summary>
    /// <param name="resp"></param>
    public void UpdateKnapsackItemAmount(ItemUseResponse resp)
    {
        var item = localCharacterKnapsack.GetItemByIndex(resp.SlotIndex);
        if (item != null)
        {
            item.Amount -= resp.Count;
            if(item.Amount <= 0)
            {
                localCharacterKnapsack.removeSlot(resp.SlotIndex);
                Kaiyun.Event.FireOut("UpdateCharacterKnapsackData");

            }
            else
            {
                //通知更新，小范围刷新即可
                Kaiyun.Event.FireOut("UpdateCharacterKnapsackSingletonItemAmount", resp.SlotIndex);
            }

        }
        else
        {
            //说明客户端的数据可能有问题。我们重新拉取一下背包数据
            GetLocalChasracterKnapsack();
        }
    }

    /// <summary>
    /// 通过actionId来获取队列中的acton
    /// </summary>
    /// <param name="actionId"></param>
    /// <returns></returns>
    public ItemUIAction GetItemUIActionById(int actionId)
    {
        foreach(var action in ItemUIAtionsQueue)
        {
            if(action.req.ActionId == actionId)
            {
                return action;
            }
        }
        return null;
    }

    /// <summary>
    /// 处理itemui的数据变化
    /// </summary>
    public void ProcessItemUIAction()
    {
        while (ItemUIAtionsQueue.Count > 0 && ItemUIAtionsQueue.Peek().isReceiveAResponse)
        {
            ItemUIAction action;
            lock (ItemUIAtionsQueue)
            {
                action = ItemUIAtionsQueue.Peek();
            }

            //如果成功就需要刷新数据了
            if (action.actionResult)
            {
                //成功就将数据刷新到指定位置即可，这里分好几种情况的
                //分情况处理
                if (action.req.OriginInventoryTpey == InventoryType.Knapsack)
                {
                    ProcessKnapsackTo(action.req);
                }
                else if (action.req.OriginInventoryTpey == InventoryType.Warehouse)
                {

                }
                else if (action.req.OriginInventoryTpey == InventoryType.EquipmentColumn)
                {

                }else if(action.req.OriginInventoryTpey == InventoryType.CurrentScene)
                {
                    ProcessCurrentSceneTo(action);
                }

            }

            lock (ItemUIAtionsQueue)
            {
                ItemUIAtionsQueue.Dequeue();
            }

        }

        //刷新一下ui
        Kaiyun.Event.FireOut("UpdateCharacterKnapsackData");
    }

    /// <summary>
    /// 处理背包到其他inventory的放置
    /// </summary>
    /// <param name="req"></param>
    /// <returns></returns>
    private void ProcessKnapsackTo(ItemPlacementRequest req)
    {
        if(req.TargetInventoryTpey == InventoryType.Knapsack)
        {
            localCharacterKnapsack.Exchange(req.OriginIndex, req.TargetIndex);
        }
        else if(req.TargetInventoryTpey == InventoryType.Warehouse)
        {
            
        }else if(req.TargetInventoryTpey == InventoryType.EquipmentColumn)
        {

        }else if(req.TargetInventoryTpey == InventoryType.CurrentScene)
        {
            //丢弃了count个物品，对应的item数据要减去count个
            localCharacterKnapsack.SetItemAmount(req.OriginIndex,-(req.TargetIndex));
            Kaiyun.Event.FireOut("UpdateCharacterKnapsackPickupItemBox");
        }
    }

    /// <summary>
    /// 处理场景到其他inventory的放置
    /// </summary>
    /// <param name="req"></param>
    private void ProcessCurrentSceneTo(ItemUIAction action)
    {
        if (action.req.TargetInventoryTpey == InventoryType.Knapsack)
        {
            //其实就是拾起操作，这里我们将数据刷新到背包中
            localCharacterKnapsack.AddItem(action.req.TargetIndex,action.AuxiliarySpace);
            Kaiyun.Event.FireOut("UpdateCharacterKnapsackPickupItemBox");
        }
        else if (action.req.TargetInventoryTpey == InventoryType.Warehouse)
        {

        }
        else if (action.req.TargetInventoryTpey == InventoryType.EquipmentColumn)
        {

        }
        else if (action.req.TargetInventoryTpey == InventoryType.CurrentScene)
        {

        }
    }
}

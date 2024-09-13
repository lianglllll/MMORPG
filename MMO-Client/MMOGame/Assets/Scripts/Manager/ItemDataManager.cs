using GameClient;
using GameClient.Entities;
using GameClient.InventorySystem;
using GameServer.Model;
using Google.Protobuf.Collections;
using Proto;
using Summer;
using System;
using System.Collections;
using System.Collections.Concurrent;
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
/// 缓存一些数据，并且给ui提供获取数据的接口
/// </summary>
public class ItemDataManager : Singleton<ItemDataManager>
{
    //缓存的背包数据
    public Inventory localCharacterKnapsack;

    /// <summary>
    ///获取本地角色的背包信息
    /// </summary>
    public Inventory GetLocalCharacterKnapsack()
    {
        if (localCharacterKnapsack != null)
        {
            return localCharacterKnapsack;
        }
        else
        {
            ItemService.Instance._InventoryInfoRequest();
            return null;
        }
    }

    /// <summary>
    /// 获取本地角色的装备数据
    /// </summary>
    /// <returns></returns>
    public ConcurrentDictionary<EquipsType, Equipment> GetEquipmentDict()
    {
        return GameApp.character.equipsDict;
    }

    /// <summary>
    /// 重新缓存缓存背包数据
    /// </summary>
    /// <param name="knapsackInfo"></param>
    public void ReloadKnapsackData(InventoryInfo knapsackInfo)
    {
        localCharacterKnapsack = new Inventory(GameApp.character);
        localCharacterKnapsack.Init(knapsackInfo);

        //刷ui
        Kaiyun.Event.FireOut("UpdateCharacterKnapsackData");
    }

    /// <summary>
    /// 重新加载装备数据
    /// </summary>
    /// <param name="equipmentInfo"></param>
    public void ReloadEquipData(Actor actor, RepeatedField<ItemInfo> equipsList)
    {
        actor.LoadEquips(equipsList);

        //刷ui
        if (GameApp.character != null && GameApp.character.EntityId == actor.EntityId)
        {
            Kaiyun.Event.FireOut("UpdateCharacterEquipmentData");
        }
    }


    /// <summary>
    /// 物品拾起请求
    /// </summary>
    /// <param name="msg"></param>
    public void ItemPickup(int entityId)
    {
        ItemService.Instance.ItemPickupRequest(entityId);
    }

    public void ItemDiscard(int slotIndex, int number, InventoryType type)
    {
        ItemService.Instance.ItemDiscardRequest(slotIndex, number, type);
    }


    /// <summary>
    /// 物品使用请求
    /// </summary>
    public void ItemUse(int slotIndex,int count)
    {
        ItemService.Instance.ItemUseRequest(slotIndex, count);
    }

    /// <summary>
    /// 穿戴装备请求
    /// </summary>
    public void WearEquipment(int knapsackSlotIndex)
    {
        ItemService.Instance._WearEquipmentRequest(knapsackSlotIndex);
    }

    /// <summary>
    /// 装备卸载请求
    /// </summary>
    /// <param name="type"></param>
    public void UnloadEquipment(EquipsType type)
    {
    }

    /// <summary>
    /// 背包物品的位置变换请求
    /// </summary>
    /// <param name="originType"></param>
    /// <param name="targetType"></param>
    /// <param name="originIndex"></param>
    /// <param name="targetIndex"></param>
    public void ItemPlacement(InventoryType originType, int originIndex, int targetIndex)
    {
        ItemPlacementRequest req = new ItemPlacementRequest();
        req.EntityId = GameApp.character.EntityId;
        req.OriginInventoryTpey = originType;
        req.OriginIndex = originIndex;
        req.TargetIndex = targetIndex;

        ItemService.Instance.ItemPlacementRequeset(req);
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
            GetLocalCharacterKnapsack();
        }
    }


}

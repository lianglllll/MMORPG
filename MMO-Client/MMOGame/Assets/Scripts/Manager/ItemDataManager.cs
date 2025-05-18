using GameClient;
using GameClient.Entities;
using GameClient.InventorySystem;
using Google.Protobuf.Collections;
using HS.Protobuf.Backpack;
using System.Collections.Concurrent;
using HSFramework.MySingleton;
using GameClient.Combat;
using System;

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
public class ItemDataManager : SingletonNonMono<ItemDataManager>
{
    private Inventory m_localCharacterKnapsack;
    private EquipManager m_equipManager;

    #region GetSet
    public Inventory Backpack => m_localCharacterKnapsack;
    public EquipManager EquipManager => m_equipManager;
    public ConcurrentDictionary<EquipsType, Equipment> EquipmentDict => m_equipManager.EquipsDict;
    #endregion


    #region 生命周期
    public void Init()
    {
        // 获取背包和装备信息
        m_localCharacterKnapsack = new Inventory();
        m_localCharacterKnapsack.Init(GameApp.character);
        m_equipManager = new EquipManager();
        m_equipManager.Init(GameApp.character);
        ItemHandler.Instance.SendGetItemInventoryDataRequest(ItemInventoryType.Backpack);
        ItemHandler.Instance.SendGetItemInventoryDataRequest(ItemInventoryType.Equipments);
    }
    public void ReloadInventoryData(NetItemInventoryDataNode node)
    {
        if(node.InventoryType == ItemInventoryType.Backpack)
        {
            m_localCharacterKnapsack.ReloadInventoryData(node);
            // 刷ui
            Kaiyun.Event.FireOut("UpdateCharacterKnapsackData");
        }
        else if(node.InventoryType == ItemInventoryType.Equipments)
        {
            m_equipManager.ReloadInventoryData(node);
            // 刷ui
            Kaiyun.Event.FireOut("UpdateCharacterEquipmentData");
        }
        else if(node.InventoryType == ItemInventoryType.Warehouse)
        {

        }

    }
    #endregion

    public void ItemPlacement(InventoryType originType, int originIndex, int targetIndex)
    {
        ItemPlacementRequest req = new ItemPlacementRequest();
        req.EntityId = GameApp.character.EntityId;
        req.OriginInventoryTpey = originType;
        req.OriginIndex = originIndex;
        req.TargetIndex = targetIndex;

        ItemService.Instance.ItemPlacementRequeset(req);
    }
    public void ItemPickup(int entityId)
    {
        ItemService.Instance.ItemPickupRequest(entityId);
    }
    public void ItemDiscard(int slotIndex, int number, InventoryType type)
    {
        ItemService.Instance.ItemDiscardRequest(slotIndex, number, type);
    }
    public void ItemUse(int slotIndex,int count)
    {
        ItemService.Instance.ItemUseRequest(slotIndex, count);
    }
    public void WearEquipment(int knapsackSlotIndex)
    {
        ItemService.Instance._WearEquipmentRequest(knapsackSlotIndex);
    }
    public void UnloadEquipment(EquipsType type)
    {
    }
    public void UpdateKnapsackItemAmount(ItemUseResponse resp)
    {
        var item = m_localCharacterKnapsack.GetItemBySlotId(resp.SlotIndex);
        if (item != null)
        {
            item.Amount -= resp.Count;
            if(item.Amount <= 0)
            {
                m_localCharacterKnapsack.removeSlotBySlotId(resp.SlotIndex);
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
            ItemHandler.Instance.SendGetItemInventoryDataRequest(ItemInventoryType.Backpack);
        }
    }
}

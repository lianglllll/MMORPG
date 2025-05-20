using GameClient.Entities;
using GameClient.InventorySystem;
using HS.Protobuf.Backpack;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// 库存对象，可以用作仓库，可以用做背包
/// </summary>
public class Inventory
{
    protected Actor m_owner;                                        
    protected int m_capacity;                                       // 背包/仓库的大小
    public ConcurrentDictionary<int, Item> itemDict = new();        // <插槽索引，物品对象> 

    #region GetSet
    public int Capacity => m_capacity;
    #endregion
    #region 生命周期
    public void Init(Actor actor)
    {
        this.m_owner = actor;
        itemDict.Clear();
        this.m_capacity = 0;
    }
    public void ReloadInventoryData(NetItemInventoryDataNode node)
    {
        m_capacity = node.Capacity;
        itemDict.Clear();
        foreach (var itemNode in node.ItemDataNodes)
        {
            var def = LocalDataManager.Instance.m_itemDefineDict[itemNode.ItemId];
            Item item = null;
            switch (def.ItemType)
            {
                case "消耗品":
                    item = new Consumable(itemNode);
                    break;
                case "道具":
                    item = new MaterialItem(itemNode);
                    break;
                case "装备":
                    item = new Equipment(itemNode);
                    break;
                default:
                    throw new Exception("物品初始化失败");
            }
            itemDict.TryAdd(item.SlotId, item);
        }
    }
    #endregion
    #region 工具类
    public Item GetItemBySlotId(int slotId)
    {
        return itemDict.GetValueOrDefault(slotId, null);
    }
    public bool SetItemToSlot(int slotIndex, Item item)
    {
        if (slotIndex < 0 || slotIndex >= m_capacity)
        {
            return false;
        }
        if (item == null) return false;


        //设置插槽
        itemDict[slotIndex] = item;
        item.SlotId = slotIndex;
        return true;
    }
    public Item removeSlotBySlotId(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= m_capacity)
        {
            return null;
        }
        itemDict.TryRemove(slotIndex, out var _value);
        return _value;
    }
    #endregion
    #region 暂未使用
    /*
    /// <summary>
    /// 交换两个格子的数据
    /// </summary>
    /// <param name="originIndex"></param>
    /// <param name="targetIndex"></param>
    public void Exchange(int originIndex, int targetIndex)
    {
        //客户端就不做太多校验了，因为服务器已经校验过了
        //1.查找原始插槽物品
        if (!itemDict.TryGetValue(originIndex, out var originItem))
        {
            return;
        }

        //2.查找目标插槽物品，如果为空直接放置
        if (!itemDict.TryGetValue(targetIndex, out var targetItem))
        {
            SetItemToSlot(targetIndex, originItem);
            removeSlotBySlotId(originIndex);
        }

        //3.查找目标物品不为空
        else
        {
            //如果物品类型相同
            if (originItem.ItemId == targetItem.ItemId)
            {
                int processAmount = Math.Min(targetItem.StackingUpperLimit - targetItem.Amount, originItem.Amount);
                //原始物品的数量小于等于可操纵数量，将原始物品全部移动到目标物品当中
                if (originItem.Amount <= processAmount)
                {
                    targetItem.Amount += originItem.Amount;
                    removeSlotBySlotId(originIndex);
                }
                else
                {
                    //修改数量即可
                    originItem.Amount -= processAmount;
                    targetItem.Amount += processAmount;
                }
            }
            //物品类型不同，那就交换
            else
            {
                SetItemToSlot(originIndex, targetItem);
                SetItemToSlot(targetIndex, originItem);
            }
        }
    }
    /// <summary>
    /// 修改某个格子中item的数量,changeAmount是变化量有正负
    /// </summary>
    /// <param name="slot"></param>
    /// <param name="changeAmount"></param>
    public void SetItemAmount(int slot,int changeAmount)
    {
        var item = GetItemBySlotId(slot);
        item.Amount += changeAmount;
        if(item.Amount <= 0)
        {
            removeSlotBySlotId(slot);
        }
    }
    /// <summary>
    /// 添加物品
    /// </summary>
    /// <param name="itemId"></param>
    /// <returns></returns>
    public int AddItem(int itemId, int amount = 1)
    {
        //不做太多的判断，因为服务器已经做过判断了，在做一次就是多余
        var def = LocalDataManager.Instance.m_itemDefineDict[itemId];
        var counter = amount;

        //3.循环放置
        while (counter > 0)
        {
            //寻找背包种存放着相同物品的格子
            var sameItem = _FindFirstItemByItemIdAndNotFull(itemId);
            if (sameItem != null)
            {
                //本次可以处理的数量
                var currentProcessAmount = Math.Min(counter, sameItem.StackingUpperLimit - sameItem.Amount);
                sameItem.Amount += currentProcessAmount;
                counter -= currentProcessAmount;
            }
            else
            {
                //找个空格
                var emptyIndex = _FindEmptyIndex();
                if (emptyIndex != -1)
                {
                    //本次可以处理的数量
                    var currentProcessAmount = Math.Min(counter, def.Capicity);
                    var newItem = new Item(def, currentProcessAmount, emptyIndex);
                    SetItemToSlot(emptyIndex, newItem);
                    counter -= currentProcessAmount;
                }
                else
                {
                    //没有空格了
                    return amount - counter;
                }
            }
        }

        //返回成功添加的个数
        return amount - counter;
    }
    /// <summary>
    /// 寻找一个空的背包索引
    /// </summary>
    /// <returns></returns>
    private int _FindEmptyIndex()
    {
        for (int i = 0; i < m_capacity; ++i)
        {
            if (!itemDict.TryGetValue(i, out var item))
            {
                return i;
            }

        }
        return -1;
    }
    /// <summary>
    /// 寻找相同的item并且没有达到堆叠上限
    /// </summary>
    /// <param name="itemId"></param>
    /// <returns></returns>
    private Item _FindFirstItemByItemIdAndNotFull(int itemId)
    {
        return itemDict.Values.FirstOrDefault(item => item.ItemDefine.ID == itemId && item.Amount < item.StackingUpperLimit);
    }
    */
    #endregion
}


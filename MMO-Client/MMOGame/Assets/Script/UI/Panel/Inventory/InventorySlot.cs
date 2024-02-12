using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class InventorySlot : MonoBehaviour
{
    private int _index;
    private ItemUI _itemUI;
    private KnapsackPanel _knapsackPanel;

    public ItemUI ItemUI
    {
        get
        {
            return transform.GetComponentInChildren<ItemUI>();
        }
    }
    public int SlotIndex
    {
        get
        {
            return _index;
        }
    }

    /// <summary>
    /// 插槽初始化
    /// </summary>
    /// <param name="knapsackPanel"></param>
    public void Init(KnapsackPanel knapsackPanel,int index)
    {
        this._knapsackPanel = knapsackPanel;
        this._index = index;
    }

    /// <summary>
    /// 在插槽中创建一个Itemui
    /// </summary>
    /// <param name="item"></param>
    public void CreateItemUI(Item item)
    {
        //清空slot中的itemui
        if(_itemUI != null && !_itemUI.gameObject.IsDestroyed())
        {
            Destroy(_itemUI.gameObject);
            _itemUI = null;
        }

        if(item != null)
        {
            var itemUIObj = GameObject.Instantiate(_knapsackPanel.itemUIPrefab, transform);
            var itemUI = itemUIObj.GetComponent<ItemUI>();
            itemUI.Init(item, _knapsackPanel.ItemUITmpParent, _knapsackPanel.numberInputBox);
            _itemUI = itemUI;
        }

    }

    /// <summary>
    /// 设置itemui，将传进来的设置为自己的儿子
    /// </summary>
    /// <param name="itemUI"></param>
    public void SetItemUI(ItemUI itemUI)
    {
        itemUI.transform.SetParent(transform);
        itemUI.transform.position = transform.position;
    }

    /// <summary>
    /// 删除当前的itemui
    /// </summary>
    public void DeleteItemUI()
    {
        //清空slot中的itemui
        if (_itemUI != null && !_itemUI.gameObject.IsDestroyed())
        {
            Destroy(_itemUI.gameObject);
            _itemUI = null;
        }
    }

    public void UpdateItemUIAmount()
    {
        _itemUI?.UpdateAmountUI(_itemUI.item.Amount);
    }
}

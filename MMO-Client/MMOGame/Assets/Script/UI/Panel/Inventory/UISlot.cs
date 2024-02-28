using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class UISlot : MonoBehaviour
{
    protected int _index;                                                           //在背包中第几个格子
    protected ItemUI _itemUI;
    protected KnapsackPanel _knapsackPanel;
    private Transform _itemUITmpParent;

    /// <summary>
    /// 拿itemui
    /// </summary>
    public ItemUI ItemUI
    {
        get
        {
            return transform.GetComponentInChildren<ItemUI>();
            //return _itemUI;
        }
    }

    /// <summary>
    /// slot索引
    /// </summary>
    public int SlotIndex
    {
        get
        {
            return _index;
        }
    }

    /// <summary>
    /// 初始化slot
    /// </summary>
    /// <param name="knapsackPanel"></param>
    /// <param name="index"></param>
    public virtual void Init(KnapsackPanel knapsackPanel, int index)
    {
        this._index = index;
        this._knapsackPanel = knapsackPanel;
        _itemUITmpParent = knapsackPanel.ItemUITmpParent;
    }

    /// <summary>
    /// 设置itemui，将传进来的设置为自己的儿子
    /// </summary>
    /// <param name="itemUI"></param>
    public virtual void SetItemUI(ItemUI itemUI)
    {
        _itemUI = itemUI;
        itemUI.transform.SetParent(transform);
        itemUI.transform.position = transform.position;
    }

    /// <summary>
    /// 移除当前slot下的itemui
    /// </summary>
    public virtual void RemoveItemUI()
    {
        if (_itemUI == null) return;
        _itemUI.transform.SetParent(_itemUITmpParent);
        _itemUI = null;
    }

    /// <summary>
    /// 删除当前slot下的itemui
    /// </summary>
    public virtual void DeleteItemUI()
    {
        //清空slot中的itemui
        if (_itemUI != null && !_itemUI.gameObject.IsDestroyed())
        {
            Destroy(_itemUI.gameObject);
            _itemUI = null;
        }
    }

    /// <summary>
    /// 在当前slot中创建一个itemui
    /// </summary>
    /// <param name="item"></param>
    public virtual void CreateItemUI(Item item,GameObject prefab)
    {
        //清空slot中的itemui
        DeleteItemUI();

        //创建一个itmeui实例
        if (item != null)
        {
            var itemUI = GameObject.Instantiate(prefab, transform).GetComponent<ItemUI>();
            itemUI.Init(item);
            SetItemUI(itemUI);
        }
    }

    /// <summary>
    /// 更新插槽中的itemui的amount
    /// </summary>
    public virtual void UpdateItemUIAmount(int amount = 1)
    {
        if(amount != -1)
        {
            _itemUI?.UpdateAmountUI(_itemUI.item.Amount);
        }
        else
        {
            _itemUI?.UpdateAmountUI(-1);
        }
    }
}

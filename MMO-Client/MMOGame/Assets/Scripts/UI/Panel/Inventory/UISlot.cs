using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class UISlot : MonoBehaviour
{
    protected int           m_index;                                       //在背包中第几个格子
    protected ItemUI        m_itemUI;
    protected KnapsackPanel m_knapsackPanel;
    private Transform       m_itemUITmpParent;

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
            return m_index;
        }
    }

    /// <summary>
    /// 初始化slot
    /// </summary>
    /// <param name="knapsackPanel"></param>
    /// <param name="index"></param>
    public virtual void Init(KnapsackPanel knapsackPanel, int index)
    {
        this.m_index = index;
        this.m_knapsackPanel = knapsackPanel;
        m_itemUITmpParent = knapsackPanel.ItemUITmpParent;
    }

    /// <summary>
    /// 设置itemui，将传进来的设置为自己的儿子
    /// </summary>
    /// <param name="itemUI"></param>
    public virtual void SetItemUI(ItemUI itemUI)
    {
        m_itemUI = itemUI;
        itemUI.transform.SetParent(transform);
        itemUI.transform.position = transform.position;
    }

    /// <summary>
    /// 移除当前slot下的itemui
    /// </summary>
    public virtual void RemoveItemUI()
    {
        if (m_itemUI == null) return;
        m_itemUI.transform.SetParent(m_itemUITmpParent);
        m_itemUI = null;
    }

    /// <summary>
    /// 删除当前slot下的itemui
    /// </summary>
    public virtual void DeleteItemUI()
    {
        //清空slot中的itemui
        if (m_itemUI != null && !m_itemUI.gameObject.IsDestroyed())
        {
            Destroy(m_itemUI.gameObject);
            m_itemUI = null;
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
            m_itemUI?.UpdateAmountUI(m_itemUI.item.Amount);
        }
        else
        {
            m_itemUI?.UpdateAmountUI(-1);
        }
    }
}

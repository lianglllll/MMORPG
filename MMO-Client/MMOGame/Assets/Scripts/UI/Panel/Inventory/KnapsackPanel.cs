using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using GameClient;
using HS.Protobuf.Backpack;
using GameClient.Entities;

public class KnapsackPanel:BasePanel
{
    private Transform           m_gridTransform;        // 插槽的父物体
    private GameObject          m_inventoryCellPrefab;  // 插槽的预制体
    private GameObject          m_itemUIPrefab;         // itemui的预制体，给插槽调用的
    private List<InventorySlot> m_slotList;             // 背包的slot
    private Dictionary<EquipSlotType, EquipSlot> m_equipSlots;

    private CommonOption        m_closeOption;
    private PickUpItemListBox   m_pickUpItemListBox;    // 拾取面板
    private NumberInputBox      m_numberInputBox;       // 丢弃物品时的输入数量框
    private Transform           m_itemUITmpParent;      // 移动itemui时，itemui临时存放的父对象

    #region GetSet
    public Transform ItemUITmpParent => m_itemUITmpParent;
    public NumberInputBox NumberInputBox => m_numberInputBox;
    #endregion

    #region 生命周期
    protected override void Awake()
    {
        base.Awake();
        m_gridTransform       = transform.Find("Right/ItemColumn/Scroll View/Viewport/Content").transform;
        m_closeOption         = transform.Find("Right/TopPart/ExitBtn").GetComponent<CommonOption>();
        m_itemUITmpParent   = transform.Find("ItemUITmpParent").transform;
        m_numberInputBox    = transform.Find("NumberInputBox").GetComponent<NumberInputBox>();
        m_pickUpItemListBox   = transform.Find("PickUpItemListBox").GetComponent<PickUpItemListBox>();

        m_inventoryCellPrefab = Res.LoadAssetSync<GameObject>("UI/Prefabs/Inventory/InventoryCell.prefab");
        m_itemUIPrefab        = Res.LoadAssetSync<GameObject>("UI/Prefabs/Inventory/ItemUI.prefab");

        // 拿到装备slot的对象，因为我们不是动态生成的
        m_equipSlots = new();
        var slots = transform.Find("CharacterInfoBox/EquipSlotAres").GetComponentsInChildren<EquipSlot>();
        foreach (var slot in slots)
        {
            m_equipSlots.Add(slot.equipsType, slot);
        }

        // 背包的slot
        m_slotList = new List<InventorySlot>();
    }
    protected override void Start()
    {
        Init();
    }
    private void OnEnable()
    {
        //监听一些个事件
        Kaiyun.Event.RegisterOut("UpdateCharacterKnapsackData", this, "RefreshKnapsackUI");
        Kaiyun.Event.RegisterOut("UpdateCharacterEquipmentData", this, "RefreshEquipsUI");
        Kaiyun.Event.RegisterOut("SceneItemChange", this, "RefreshPickUpBoxUI");
    }
    private void OnDisable()
    {
        Kaiyun.Event.UnRegisterOut("UpdateCharacterKnapsackData", this, "RefreshKnapsackUI");
        Kaiyun.Event.UnRegisterOut("UpdateCharacterEquipmentData", this, "RefreshEquipsUI");
        Kaiyun.Event.UnRegisterOut("SceneItemChange", this, "RefreshPickUpBoxUI");
    }
    public void Init()
    {
        m_closeOption.Init(OnCloseBtn);

        // 刷新背包ui
        RefreshKnapsackUI();
        // 刷新装备栏ui
        RefreshEquipsUI();
        RefreshPickUpBoxUI();
    }
    private void Update()
    {
        if (GameInputManager.Instance.UI_ESC)
        {
            OnCloseBtn();
        }
    }
    private void OnCloseBtn()
    {
        UIManager.Instance.ClosePanel("KnapsackPanel");
        Kaiyun.Event.FireIn("CloseKnapsackPanel");
    }
    #endregion

    #region tools
    public void RefreshKnapsackUI()
    {
        var knapsack = ItemDataManager.Instance.Backpack;
        if (knapsack == null) {
            _SetCellCount(20);
            _ReInitCurInventorySlots(ItemInventoryType.Backpack);
            goto End;
        }

        // 加载插槽
        _SetCellCount(knapsack.Capacity);

        // 清空一下插槽里面的东西
        _ReInitCurInventorySlots(ItemInventoryType.Backpack);

        // 将itemui加载进各个插槽
        for (int i = 0; i < m_slotList.Count; ++i)
        {
            var slot = m_slotList[i];
            var item = knapsack.GetItemBySlotId(i);
            if (item != null)
            {
                slot.CreateItemUI(item, m_itemUIPrefab);
            }
        }

    End:
        return;
    }
    public void RefreshEquipsUI()
    {
        _ReInitCurInventorySlots(ItemInventoryType.Equipments);

        // 获取数据
        var itemList = ItemDataManager.Instance.EquipmentDict;
        if (itemList == null) return;

        // 生成itemui
        foreach (var kv in itemList)
        {
            EquipSlot slot = m_equipSlots[kv.Key];
            slot.CreateItemUI(kv.Value, m_itemUIPrefab);
            slot.UpdateItemUIAmount(-1);            // 隐藏
        }
    }
    private void _SetCellCount(int count)
    {
        // 获取现有的cell数量
        int currentCellCount = m_slotList.Count;
        // 如果小于补齐
        if (currentCellCount < count)
        {
            m_slotList.Clear();

            int gapCount = count - currentCellCount;
            for (int i = 0; i < gapCount; i++)
            {
                GameObject newObj = Instantiate(m_inventoryCellPrefab, m_gridTransform);
                var iSlot = newObj.transform.GetComponent<InventorySlot>();
                iSlot.Init(this, currentCellCount + i);
                m_slotList.Add(iSlot);
            }
        }
        // 如果大于删除
        else if (currentCellCount > count)
        {
            int gapCount = currentCellCount - count;
            for (int i = currentCellCount - 1; i >= count; i--)
            {
                var removeObj = m_slotList[i].gameObject;
                m_slotList.RemoveAt(i);
                Destroy(removeObj);
            }
        }
        else
        {
            // 不干活
        }
    }
    private void _ReInitCurInventorySlots(ItemInventoryType type)
    {
        // 清空一下插槽里面的东西
        if (type == ItemInventoryType.Backpack)
        {
            foreach (var slot in m_slotList)
            {
                var itemUi = slot.GetComponentInChildren<ItemUI>();
                if (itemUi != null)
                {
                    Destroy(itemUi.gameObject);
                }
            }
        }else if(type == ItemInventoryType.Equipments)
        {
            // 删除slot下的ui
            foreach (var s in m_equipSlots.Values)
            {
                s.DeleteItemUI();
            }
        }
        else if(type == ItemInventoryType.Warehouse)
        {

        }
    }

    public void RefreshPickUpBoxUI()
    {
        var list = EntityManager.Instance.FindEntitiesWithinRadius<ClientItem>(GameApp.character.RenderObj.transform.position, 2);
        if (list != null)
        {
            m_pickUpItemListBox.ResetUI(list);
        }
    }
    #endregion
}

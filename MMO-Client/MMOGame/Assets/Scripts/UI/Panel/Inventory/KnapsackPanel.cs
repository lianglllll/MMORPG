using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using GameClient;
using HS.Protobuf.Backpack;

public class KnapsackPanel:BasePanel
{
    private Transform gridTransform;                // 插槽的父物体
    private GameObject inventoryCellPrefab;         // 插槽的预制体
    private GameObject itemUIPrefab;                // itemui的预制体，给插槽调用的
    private List<InventorySlot> slotList;           // 背包的slot

    private CommonOption closeOption;

    private PickUpItemListBox pickUpItemListBox;    // 拾取面板
    private NumberInputBox m_numberInputBox;        // 丢弃物品时的输入数量框
    private Transform m_itemUITmpParent;            // 移动itemui时，itemui临时存放的父对象

    // equips slot
    private Dictionary<EquipsType, EquipSlot> equipSlots = new();

    #region GetSet
    public Transform ItemUITmpParent => m_itemUITmpParent;
    public NumberInputBox NumberInputBox => m_numberInputBox;
    #endregion

    #region 生命周期
    protected override void Awake()
    {
        base.Awake();
        gridTransform       = transform.Find("Right/ItemColumn/Scroll View/Viewport/Content").transform;
        closeOption         = transform.Find("Right/TopPart/ExitBtn").GetComponent<CommonOption>();
        m_itemUITmpParent   = transform.Find("ItemUITmpParent").transform;
        m_numberInputBox    = transform.Find("NumberInputBox").GetComponent<NumberInputBox>();
        pickUpItemListBox   = transform.Find("PickUpItemListBox").GetComponent<PickUpItemListBox>();

        inventoryCellPrefab = Res.LoadAssetSync<GameObject>("UI/Prefabs/Inventory/InventoryCell.prefab");
        itemUIPrefab        = Res.LoadAssetSync<GameObject>("UI/Prefabs/Inventory/ItemUI.prefab");

        // 拿到装备slot的对象，因为我们不是动态生成的
        var slots = transform.Find("CharacterInfoBox/EquipSlotAres").GetComponentsInChildren<EquipSlot>();
        foreach (var slot in slots)
        {
            equipSlots.Add(slot.equipsType, slot);
        }
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
        Kaiyun.Event.RegisterOut("UpdateCharacterKnapsackPickupItemBox", this, "RefreshPickUpBox");
        Kaiyun.Event.RegisterOut("GoldChange", this, "UpdateCurrency");
    }
    private void OnDisable()
    {
        Kaiyun.Event.UnRegisterOut("UpdateCharacterKnapsackData", this, "RefreshKnapsackUI");
        Kaiyun.Event.UnRegisterOut("UpdateCharacterEquipmentData", this, "RefreshEquipsUI");
        Kaiyun.Event.UnRegisterOut("UpdateCharacterKnapsackPickupItemBox", this, "RefreshPickUpBox");
        Kaiyun.Event.UnRegisterOut("GoldChange", this, "UpdateCurrency");
    }
    public void Init()
    {
        //背包的slot
        slotList = new List<InventorySlot>();
        closeOption.Init(OnCloseBtn);

        // 刷新背包ui
        RefreshKnapsackUI();

        /*        
                // 刷新装备栏ui
                RefreshEquipsUI();
                // 刷新拾取栏ui
                RefreshPickUpBoxUI();
                // 刷新货币ui
                UpdateCurrency();*/


    }
    private void FixedUpdate()
    {
/*        if (GameApp.character.RenderObj.transform.hasChanged)
        {
            RefreshPickUpBox();
        }*/
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
            _ReInitCurSlots();
            goto End;
        }

        // 加载插槽
        _SetCellCount(knapsack.Capacity);

        // 清空一下插槽里面的东西
        _ReInitCurSlots();

        // 将itemui加载进各个插槽
        for (int i = 0; i < slotList.Count; ++i)
        {
            var slot = slotList[i];
            var item = knapsack.GetItemBySlotId(i);
            if (item != null)
            {
                slot.CreateItemUI(item, itemUIPrefab);
            }
        }

    End:
        return;
    }
    private void _SetCellCount(int count)
    {

        //获取现有的cell数量
        int currentCellCount = gridTransform.childCount;
        //如果小于补齐
        if (currentCellCount < count)
        {
            slotList.Clear();

            int gapCount = count - currentCellCount;
            for (int i = 0; i < gapCount; i++)
            {
                GameObject newObj = Instantiate(inventoryCellPrefab, gridTransform);
                var iSlot = newObj.transform.GetComponent<InventorySlot>();
                iSlot.Init(this, currentCellCount + i);
                slotList.Add(iSlot);
            }
        }
        //如果大于删除
        else if (currentCellCount > count)
        {

            int gapCount = currentCellCount - count;
            for (int i = currentCellCount - 1; i >= count; i--)
            {
                slotList.RemoveAt(i);
                GameObject removeObj = gridTransform.GetChild(i).gameObject;
                Destroy(removeObj);
            }
        }
        else
        {
            //不干活
        }
    }
    private void _ReInitCurSlots()
    {
        // 清空一下插槽里面的东西
        foreach (var slot in slotList)
        {
            var itemUi = slot.GetComponentInChildren<ItemUI>();
            if (itemUi != null)
            {
                Destroy(itemUi.gameObject);
            }
        }
    }


    public void RefreshEquipsUI()
    {
        //删除slot下的ui
        foreach(var s in equipSlots.Values)
        {
            s.DeleteItemUI();
        }

        //获取数据
        var itemList = ItemDataManager.Instance.EquipmentDict;
        if (itemList == null) return;

        //生成itemui
        foreach(var item in itemList.Values)
        {
            EquipSlot slot = equipSlots[item.EquipsType];
            slot.CreateItemUI(item, itemUIPrefab);
            slot.UpdateItemUIAmount(-1);
        }

    }
    public void UpdateCurrency()
    {
        if (GameApp.character == null) return;
        // goldText.text = GameApp.character.m_netActorNode.Gold + "";
    }
    public void RefreshPickUpBoxUI()
    {
        //我们控制的角色的entity数据是垃圾数据，因为我们根本就不更新它

        var list = GameTools.RangeItem(GameApp.character.RenderObj.transform.position * 1000, 2 * 1000);
        if (list != null)
        {
            pickUpItemListBox.Reset(list);
        }
    }
    #endregion
}

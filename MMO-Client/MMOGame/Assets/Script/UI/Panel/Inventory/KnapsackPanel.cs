using GameServer.Model;
using GGG.Tool.Singleton;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using Proto;
using System;
using GameClient.Entities;
using Serilog;
using GameClient;

public class KnapsackPanel:BasePanel
{
    private Transform gridTransform;                //插槽的父物体
    private Button closeBtn;
    private GameObject inventoryCellPrefab;         //插槽的预制体
    public GameObject itemUIPrefab;                 //itemui的预制体，给插槽调用的
    public Transform ItemUITmpParent;               //移动itemui时，itemui临时存放的父对象
    public NumberInputBox numberInputBox;           //丢弃物品时的输入数量框
    public PickUpItemListBox pickUpItemListBox;
    public List<InventorySlot> slotList;            //背包的slot

    //currency
    private Text goldText;

    //equips slot
    public Dictionary<EquipsType, EquipSlot> equipSlots = new Dictionary<EquipsType, EquipSlot>();


    protected override void Awake()
    {
        base.Awake();
        gridTransform = transform.Find("Right/ItemColumn/Grid").transform;
        inventoryCellPrefab = Resources.Load<GameObject>("UI/Prefabs/Inventory/InventoryCell");
        itemUIPrefab = Resources.Load<GameObject>("UI/Prefabs/Inventory/ItemUI");
        closeBtn = transform.Find("Right/ItemColumn/CloseBtn").GetComponent<Button>();
        ItemUITmpParent = transform.Find("ItemUITmpParent").transform;
        numberInputBox = transform.Find("NumberInputBox").GetComponent<NumberInputBox>();
        pickUpItemListBox = transform.Find("PickUpItemListBox").GetComponent<PickUpItemListBox>();

        goldText = transform.Find("Right/ItemColumn/Currency/Gold/IconBar/GoldNumberText").GetComponent<Text>();

    }

    protected  override void Start()
    {
        base.Start();

        //监听一些个事件
        Kaiyun.Event.RegisterOut("UpdateCharacterKnapsackData", this, "RefreshKnapsackUI");
        Kaiyun.Event.RegisterOut("UpdateCharacterKnapsackPickupItemBox", this, "RefreshPickUpBox");
        Kaiyun.Event.RegisterOut("GoldChange", this, "UpdateCurrency");
        Kaiyun.Event.RegisterOut("UpdateCharacterEquipmentData", this, "RefreshEquipsUI");

        Init();

    }

    private void FixedUpdate()
    {
        if (GameApp.character.renderObj.transform.hasChanged)
        {
            RefreshPickUpBox();
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            OnCloseBtn();
        }
    }

    private void OnDestroy()
    {
        Kaiyun.Event.UnregisterOut("UpdateCharacterKnapsackData", this, "RefreshKnapsackUI");
        Kaiyun.Event.UnregisterOut("UpdateCharacterKnapsackPickupItemBox", this, "RefreshPickUpBox");
        Kaiyun.Event.UnregisterOut("GoldChange", this, "UpdateCurrency");
        Kaiyun.Event.UnregisterOut("UpdateCharacterEquipmentData", this, "RefreshEquipsUI");
    }

    /// <summary>
    /// 初始化当前面板
    /// </summary>
    /// <param name="itemEntity"></param>
    public void Init()
    {
        //拿到装备slot的对象，因为我们不是动态生成的
        var slots = transform.Find("CharacterInfoBox/EquipSlotAres").GetComponentsInChildren<EquipSlot>();
        foreach (var slot in slots)
        {
            equipSlots.Add(slot.equipsType, slot);
        }

        //背包的slot
        slotList = new List<InventorySlot>();
        closeBtn.onClick.AddListener(OnCloseBtn);

        //刷新背包ui
        RefreshKnapsackUI();
        //刷新装备栏ui
        RefreshEquipsUI();
        //刷新拾取栏ui
        RefreshPickUpBox();
        //刷新货币ui
        UpdateCurrency();
    }

    /// <summary>
    /// 关闭背包面板
    /// </summary>
    private void OnCloseBtn()
    {
        UIManager.Instance.ClosePanel("KnapsackPanel");
        Kaiyun.Event.FireOut("CloseKnaspack");
    }

    /// <summary>
    /// 重新刷新背包ui
    /// </summary>
    public void RefreshKnapsackUI()
    {
        var knapsack = ItemDataManager.Instance.GetLocalCharacterKnapsack();
        if (knapsack == null) {
            SetCellCount(10);
            return;
        }

        //加载插槽
        SetCellCount(knapsack.Capacity);

        //清空一下插槽里面的东西
        foreach (var slot in slotList)
        {
            var itemUi = slot.GetComponentInChildren<ItemUI>();
            if (itemUi != null)
            {
                Destroy(itemUi.gameObject);
            }
        }

        //将itemui加载进各个插槽
        for (int i = 0; i < slotList.Count; ++i)
        {
            var slot = slotList[i];
            var item = knapsack.GetItemByIndex(i);
            if (item != null)
            {
                slot.CreateItemUI(item, itemUIPrefab);
            }
        }


    }

    /// <summary>
    /// 重新刷新拾取面板的UI
    /// </summary>
    public void RefreshPickUpBox()
    {
        //我们控制的角色的entity数据是垃圾数据，因为我们根本就不更新它

        var list = GameTools.RangeItem(GameApp.character.renderObj.transform.position * 1000, 2 * 1000);
        if (list != null)
        {
            pickUpItemListBox.Reset(list);
        }
    }

    /// <summary>
    /// 重新刷新装备栏的ui
    /// </summary>
    public void RefreshEquipsUI()
    {
        //删除slot下的ui
        foreach(var s in equipSlots.Values)
        {
            s.DeleteItemUI();
        }

        //获取数据
        var itemList = ItemDataManager.Instance.GetEquipmentDict();
        if (itemList == null) return;

        //生成itemui
        foreach(var item in itemList.Values)
        {
            EquipSlot slot = equipSlots[item.EquipsType];
            slot.CreateItemUI(item, itemUIPrefab);
            slot.UpdateItemUIAmount(-1);
        }

    }

    /// <summary>
    /// 刷新货币ui
    /// </summary>
    public void UpdateCurrency()
    {
        if (GameApp.character == null) return;
        goldText.text = GameApp.character.info.Gold + "";
    }

    /// <summary>
    /// 设置背包插槽数量
    /// </summary>
    /// <param name="count"></param>
    public void SetCellCount(int count)
    {

        //获取现有的cell数量
        int currentCellCount = gridTransform.childCount;
        //如果小于补齐
        if(currentCellCount < count)
        {
            slotList.Clear();

            int gapCount = count - currentCellCount;
            for(int i = 0; i < gapCount; i++)
            {
                GameObject newObj = Instantiate(inventoryCellPrefab, gridTransform);
                var iSlot = newObj.transform.GetComponent<InventorySlot>();
                iSlot.Init(this, currentCellCount+i);
                slotList.Add(iSlot);
            }
        }
        //如果大于删除
        else if (currentCellCount > count)
        {

            int gapCount = currentCellCount - count;
            for (int i = currentCellCount - 1; i >= count ; i--)
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

}

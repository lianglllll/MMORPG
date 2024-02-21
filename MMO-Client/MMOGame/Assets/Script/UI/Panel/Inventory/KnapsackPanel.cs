using GameServer.Model;
using GGG.Tool.Singleton;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using Proto;
using System;
using GameClient.Entities;

public class KnapsackPanel:BasePanel
{
    private Transform gridTransform;                //插槽的父物体
    private Button closeBtn;
    private GameObject inventoryCellPrefab;         //插槽的预制体
    public GameObject itemUIPrefab;                 //itemui的预制体，给插槽调用的
    public Transform ItemUITmpParent;               //移动itemui时，itemui临时存放的父对象
    public NumberInputBox numberInputBox;          //丢弃物品时的输入数量框
    public PickUpItemListBox pickUpItemListBox;
    public List<InventorySlot> slotList;

    //currency
    private Text goldText;


    protected override void Awake()
    {
        base.Awake();
        gridTransform = transform.Find("Right/Grid").transform;
        inventoryCellPrefab = Resources.Load<GameObject>("Prefabs/UI/Inventory/InventoryCell");
        itemUIPrefab = Resources.Load<GameObject>("Prefabs/UI/Inventory/ItemUI");
        closeBtn = transform.Find("Right/CloseBtn").GetComponent<Button>();
        ItemUITmpParent = transform.Find("ItemUITmpParent").transform;
        numberInputBox = transform.Find("NumberInputBox").GetComponent<NumberInputBox>();
        pickUpItemListBox = transform.Find("PickUpItemListBox").GetComponent<PickUpItemListBox>();

        goldText = transform.Find("Right/Currency/Gold/IconBar/GoldNumberText").GetComponent<Text>();
    }

    private void Start()
    {
        Init();

        //监听一些个事件
        Kaiyun.Event.RegisterOut("UpdateCharacterKnapsackData", this, "UpdateKnapsackUI");
        Kaiyun.Event.RegisterOut("UpdateCharacterKnapsackSingletonItemAmount", this, "UpdateKnapsackSingletonItemAmount");
        Kaiyun.Event.RegisterOut("UpdateCharacterKnapsackPickupItemBox", this, "UpdatePickUpBox");
        Kaiyun.Event.RegisterOut("GoldChange", this, "UpdateCurrency");

    }

    private void FixedUpdate()
    {
        if (GameApp.character.renderObj.transform.hasChanged)
        {
            UpdatePickUpBox();
        }
    }

    private void OnDestroy()
    {
        Kaiyun.Event.UnregisterOut("UpdateCharacterKnapsackData", this, "UpdateKnapsackUI");
        Kaiyun.Event.UnregisterOut("UpdateCharacterKnapsackSingletonItemAmount", this, "UpdateKnapsackSingletonItemAmount");
        Kaiyun.Event.UnregisterOut("UpdateCharacterKnapsackPickupItemBox", this, "UpdatePickUpBox");
        Kaiyun.Event.UnregisterOut("GoldChange", this, "UpdateCurrency");
    }

    /// <summary>
    /// 初始化当前面板
    /// </summary>
    /// <param name="itemEntity"></param>
    public void Init()
    {
        slotList = new List<InventorySlot>();
        closeBtn.onClick.AddListener(OnCloseBtn);

        UpdatePickUpBox();

        UpdateCurrency();

        //先确定是否获取了背包信息，如果没有就向服务器拿
        var chr = GameApp.character;
        if (chr == null) return;
        var knapsack = RemoteDataManager.Instance.localCharacterKnapsack;
        if (knapsack == null)
        {
            RemoteDataManager.Instance.GetLocalChasracterKnapsack();
            SetCellCount(10);
            return;
        }

        UpdateKnapsackUI();
    }

    /// <summary>
    /// 加载背包数据
    /// </summary>
    public void UpdateKnapsackUI()
    {
        //加载插槽
        var knapsack = RemoteDataManager.Instance.localCharacterKnapsack;
        SetCellCount(knapsack.Capacity);
        //清空一下插槽里面的东西
        var slotList = transform.GetComponentsInChildren<InventorySlot>();
        foreach(var slot in slotList)
        {
            var itemUi =  slot.GetComponentInChildren<ItemUI>();
            if(itemUi != null)
            {
                Destroy(itemUi.gameObject);
            }
        }
        //将itemui加载进各个插槽
        for (int i = 0; i < slotList.Length; ++i)
        {
            var slot = slotList[i];
            var item = knapsack.GetItemByIndex(i);
            if (item != null)
            {
                slot.CreateItemUI(item);
            }
        }

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

    /// <summary>
    /// 关闭背包面板
    /// </summary>
    private void OnCloseBtn()
    {
        UIManager.Instance.ClosePanel("KnapsackPanel");
    }

    /// <summary>
    /// 返回指定范围内的itementity
    /// </summary>
    /// <param name="spaceId"></param>
    /// <param name="pos"></param>
    /// <param name="range"></param>
    /// <returns></returns>
    public List<ItemEntity> RangeItem(Vector3 pos, int range)
    {
        Predicate<ItemEntity> match = (e) =>
        {
            return Vector3.Distance(pos, e.Position) <= range;
        };
        return EntityManager.Instance.GetEntityList(match);
    }

    /// <summary>
    /// 更新pickupbox
    /// </summary>
    public void UpdatePickUpBox()
    {
        //我们控制的角色的entity数据是垃圾数据，因为我们根本就不更新它
        var list = RangeItem(GameApp.character.renderObj.transform.position*1000, 2*1000);
        if (list != null)
        {
            pickUpItemListBox.Reset(list);
        }
    }

    /// <summary>
    /// 更加某个slot中物品的数量ui
    /// </summary>
    /// <param name="slotIndex"></param>
    public void UpdateKnapsackSingletonItemAmount(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= slotList.Count) return;
        var slot = slotList[slotIndex];
        slot.UpdateItemUIAmount();
    }

    /// <summary>
    /// 更新金币数量
    /// </summary>
    public void UpdateCurrency()
    {
        if (GameApp.character == null) return;
        goldText.text = GameApp.character.info.Gold + "";
    }


}

using GameClient.InventorySystem;
using HS.Protobuf.Game.Backpack;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// 物品的ui内容，需要放到插槽里面
/// </summary>
public class ItemUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler,IPointerEnterHandler,IPointerExitHandler
    ,IPointerClickHandler
{
    public Item item;
    private Image icon;
    private Text AmountText;

    private Vector3 offset;
    private UISlot originSlot;
    private Vector3 originPosition;
    private bool isDragging;

    private void Awake()
    {
        icon = transform.Find("Icon").GetComponent<Image>();
        AmountText = transform.Find("AmountText").GetComponent<Text>();
    }

    private void Start()
    {
        AmountText.raycastTarget = false;
    }

    private void OnDisable()
    {
        //防止提示框没关
        ToolTip.Instance?.Hide();
    }

    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="item"></param>
    public void Init(Item item)
    {
        this.item = item;
        UpdateItemUI();
    }

    /// <summary>
    /// 根据item更新当前的UI
    /// </summary>
    public void UpdateItemUI()
    {
        AmountText.text = ""+item.Amount;
        icon.gameObject.SetActive(true);
        icon.sprite = Res.LoadAssetSync<Sprite>(item.Define.Icon);
    }

    /// <summary>
    /// 更新物品数量
    /// </summary>
    public void UpdateAmountUI(int count)
    {
        if(count == -1)
        {
            AmountText.text = "";

        }
        else
        {
            AmountText.text = "" + count;
        }
    }

    /// <summary>
    /// 背包内放置物品
    /// </summary>
    /// <param name="originIndex"></param>
    /// <param name="targetIndex"></param>
    private void ItemPlacement(int originIndex, int targetIndex)
    {
        ItemDataManager.Instance.ItemPlacement(InventoryType.Knapsack,originIndex, targetIndex);
    }

    /// <summary>
    /// 下面这三个函数是鼠标拖拽ui的事件
    /// </summary>
    /// <param name="eventData"></param>
    public void OnBeginDrag(PointerEventData eventData)
    {
        // 记录初始位置和偏移量
        offset = transform.position - Input.mousePosition;
        originSlot = transform.parent.GetComponent<UISlot>();
        if (originSlot.gameObject.CompareTag("EquipSlot")) return;
        originPosition = transform.position;

        // 将物品UI从原来的格子中移除
        originSlot.RemoveItemUI();

        // 标记为正在拖拽中
        isDragging = true;

        // 隐藏物品的RaycastTarget，避免干扰鼠标事件
        icon.raycastTarget = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isDragging) return;
        // 更新物品位置
        transform.position = Input.mousePosition + offset;
    }

    public void OnEndDrag(PointerEventData eventData)
    {

        if (!isDragging) return;

        //是否指向UI组件
        if (EventSystem.current.IsPointerOverGameObject()) 
        {
            //获取鼠标位置的游戏对象
            InventorySlot targetSlot = eventData.pointerEnter.gameObject.GetComponent<InventorySlot>();
            //可能碰到了icon
            if(targetSlot == null)
            {
                targetSlot = eventData.pointerEnter.GetComponentInParent<InventorySlot>();
            }

            if (targetSlot != null && targetSlot.tag.Equals("InventorySlot"))
            {
                var targetItemUI = targetSlot.ItemUI;
                if (targetSlot.ItemUI == null)
                {
                    // 将物品放置到目标格子中
                    targetSlot.SetItemUI(this);
                }
                else
                {
                    //两个格子之间交换
                    originSlot.SetItemUI(targetItemUI);
                    targetSlot.SetItemUI(this);
                }
                ItemPlacement(originSlot.SlotIndex, targetSlot.SlotIndex);
            }
            else
            {
                // 还原物品位置和父级格子
                originSlot.SetItemUI(this);
            }
        }
        else
        {
            //指向了一个非ui物体上，丢弃
            originSlot.SetItemUI(this);            // 还原物品位置和父级格子,因为可能没有一次性丢完
            ItemDiscard(this.item.Position, 1);
        }

        // 取消拖拽标记
        isDragging = false;

        // 恢复物品的RaycastTarget
        icon.raycastTarget = true;

    }



    /// <summary>
    /// 鼠标掠过的事件
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerEnter(PointerEventData eventData)
    {
        var content = "<color=#ffffff>物品信息为空</color>";
        if (item != null)
        {
            content = this.item.GetDescText();
        }
        ToolTip.Instance.Show(content);
    } 

    public void OnPointerExit(PointerEventData eventData)
    {
        ToolTip.Instance?.Hide();
    }

    /// <summary>
    /// 鼠标点击事件
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerClick(PointerEventData eventData)
    {
        string[] options1 = { "使用","丢弃" };
        string[] options2 = { "穿戴", "丢弃" };
        string[] options3 = { "卸下" };
        string[] options4 = { "丢弃" };


        string[] opt = { };
        
        var slot = transform.parent.GetComponent<UISlot>();
        if(slot is InventorySlot)
        {
            if (item.GetItemType() == ItemType.Equipment)
            {
                opt = options2;
            }
            else if(item.GetItemType() == ItemType.Material)
            {
                opt = options4;
            }
            else
            {
                opt = options1;
            }
        }
        else if(slot is EquipSlot)
        {
            opt = options3;
        }

        if (opt.Length == 0) return;
        ItemMenu.Show(Input.mousePosition, opt, OnClickAction);
    }

    /// <summary>
    /// 行为
    /// </summary>
    /// <param name="value"></param>
    public void OnClickAction(string value)
    {
        switch (value)
        {
            case "使用":
                ItemUse();
                break;
            case "穿戴":
                WearEquipment(item.Position);
                break;
            case "卸下":
                UnloadEquipment((item as Equipment).EquipsType);
                break;
            case "丢弃":
                ItemDiscard(this.item.Position, 1);
                break;

        }
    }

    /// <summary>
    /// 物品使用
    /// </summary>
    private void ItemUse()
    {
        var slot = transform.parent.GetComponent<UISlot>();
        if (slot != null)
        {
            ItemDataManager.Instance.ItemUse(slot.SlotIndex, 1);
        }
    }

    /// <summary>
    /// 丢弃物品
    /// </summary>
    /// <param name="slotIndex"></param>
    /// <param name="count"></param>
    private void ItemDiscard(int slotIndex, int count)
    {
        //弹出提示框，询问扔多少个
        (UIManager.Instance.GetOpeningPanelByName("KnapsackPanel") as KnapsackPanel).numberInputBox.Show(transform.position,item.Define.Name, item.Amount,
            (targetAmount) => {
                ItemDataManager.Instance.ItemDiscard(item.Position, targetAmount, InventoryType.Knapsack);
            });
    }

    /// <summary>
    /// 武器穿戴
    /// </summary>
    /// <param name="slotIndex"></param>
    /// <param name="type"></param>
    private void WearEquipment(int knapsackSlotIndex)
    {
        ItemDataManager.Instance.WearEquipment(knapsackSlotIndex);
    }

    /// <summary>
    /// 装备卸载
    /// </summary>
    /// <param name="type"></param>
    private void UnloadEquipment(EquipsType type)
    {
        ItemService.Instance._UnloadEquipmentRequest(type);
    }

}

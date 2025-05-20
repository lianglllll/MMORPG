using GameClient.InventorySystem;
using HS.Protobuf.Backpack;
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

    #region 生命周期
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
        // 防止提示框没关
        ToolTip.Instance?.Hide();
    }
    public void Init(Item item)
    {
        this.item = item;
        UpdateItemUI();
    }
    #endregion

    #region 鼠标事件
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

        // 是否指向UI组件
        if (EventSystem.current.IsPointerOverGameObject()) 
        {
            // 获取鼠标位置的游戏对象
            InventorySlot targetSlot = eventData.pointerEnter.gameObject.GetComponent<InventorySlot>();

            // 可能碰到了icon, 找它父亲
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
            // 指向了一个非ui物体上，丢弃
            originSlot.SetItemUI(this);            // 还原物品位置和父级格子,因为可能没有一次性丢完
            ItemDiscard(this.item.SlotId, 1);
        }

        // 取消拖拽标记
        isDragging = false;

        // 恢复物品的RaycastTarget
        icon.raycastTarget = true;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        var content = "<color=#ffffff>物品信息为空</color>";
        if (item != null)
        {
            content = this.item.GetItemDescText();
        }
        ToolTip.Instance.Show(content);
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        ToolTip.Instance?.Hide();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        string[] options1 = { "使用", "丢弃" };
        string[] options2 = { "穿戴", "丢弃" };
        string[] options3 = { "卸下" };
        string[] options4 = { "丢弃" };

        string[] opt = { };

        var slot = transform.parent.GetComponent<UISlot>();
        if (slot is InventorySlot)
        {
            if (item.GetItemType() == ItemType.Equipment)
            {
                opt = options2;
            }
            else if (item.GetItemType() == ItemType.Material)
            {
                opt = options4;
            }
            else
            {
                opt = options1;
            }
        }
        else if (slot is EquipSlot)
        {
            opt = options3;
        }

        if (opt.Length == 0) return;
        ItemMenu.Show(Input.mousePosition, opt, OnClickAction);
    }
    public void OnClickAction(string value)
    {
        switch (value)
        {
            case "使用":
                ItemUse();
                break;
            case "穿戴":
                WearEquipment(item.SlotId);
                break;
            case "卸下":
                UnloadEquipment((item as Equipment).EquipsType);
                break;
            case "丢弃":
                ItemDiscard(this.item.SlotId, 1);
                break;
        }
    }
    #endregion

    #region UI更新
    public void UpdateItemUI()
    {
        AmountText.text = "" + item.Amount;
        icon.gameObject.SetActive(true);
        icon.sprite = Res.LoadAssetSync<Sprite>(item.ItemDefine.Icon);
    }
    public void UpdateAmountUI(int count)
    {
        if (count == -1)
        {
            AmountText.text = "";

        }
        else
        {
            AmountText.text = "" + count;
        }
    }
    #endregion

    #region 行为
    private void ItemPlacement(int originIndex, int targetIndex)
    {
        ItemDataManager.Instance.ItemPlacement(ItemInventoryType.Backpack, ItemInventoryType.Backpack, originIndex, targetIndex);
    }
    private void ItemUse()
    {
        var slot = transform.parent.GetComponent<UISlot>();
        if (slot != null)
        {
            ItemDataManager.Instance.ItemUse(slot.SlotIndex, 1);
        }
    }
    private void ItemDiscard(int slotIndex, int count)
    {
        //弹出提示框，询问扔多少个
        var panel = UIManager.Instance.GetOpeningPanelByName("KnapsackPanel") as KnapsackPanel;
        panel.NumberInputBox.Show(transform.position,item.ItemDefine.Name, item.Amount,
            (targetAmount) => {
                ItemDataManager.Instance.ItemDiscard(item.SlotId, targetAmount, ItemInventoryType.Backpack);
            });
    }
    private void WearEquipment(int knapsackSlotIndex)
    {
        ItemDataManager.Instance.WearEquipment(knapsackSlotIndex);
    }
    private void UnloadEquipment(EquipsType type)
    {
        ItemService.Instance._UnloadEquipmentRequest(type);
    }
    #endregion
}

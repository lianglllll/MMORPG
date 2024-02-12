using Proto;
using System.Collections;
using System.Collections.Generic;
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

    private Transform ItemUITmpParent;      //itemui在移动时放置在这个物体下
    private NumberInputBox numberInputBox;  

    private Vector3 offset;
    private InventorySlot originSlot;
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

    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="item"></param>
    public void Init(Item item,Transform ItemUITmpParent, NumberInputBox numberInputBox)
    {
        this.item = item;
        this.ItemUITmpParent = ItemUITmpParent;
        this.numberInputBox = numberInputBox;
        UpdateItemUI();
    }

    /// <summary>
    /// 根据item更新当前的UI
    /// </summary>
    public void UpdateItemUI()
    {
        AmountText.text = ""+item.Amount;
        icon.gameObject.SetActive(true);
        icon.sprite = Resources.Load<Sprite>(item.Define.Icon);
    }

    /// <summary>
    /// 更新物品数量
    /// </summary>
    public void UpdateAmountUI(int count)
    {
        AmountText.text = "" + count;
    }

    /// <summary>
    /// 背包内放置物品
    /// </summary>
    /// <param name="originIndex"></param>
    /// <param name="targetIndex"></param>
    private void ItemPlacement(int originIndex, int targetIndex)
    {
        RemoteDataManager.Instance.ItemPlacement(InventoryType.Knapsack, InventoryType.Knapsack, originIndex, targetIndex);
    }

    /// <summary>
    /// 丢弃物品到当前场景
    /// </summary>
    /// <param name="slotIndex"></param>
    /// <param name="count"></param>
    private void ItemDiscard(int slotIndex,int count)
    {
        // 还原物品位置和父级格子,因为可能没有一次性丢完
        originSlot.SetItemUI(this);

        //弹出提示框，询问扔多少个
        numberInputBox.Show(item.Define.Name,item.Amount,
            (targetAmount) => {
                //丢弃其实也是一个放置的操作
                RemoteDataManager.Instance.ItemPlacement(InventoryType.Knapsack, InventoryType.CurrentScene, slotIndex, targetAmount);

                //刷新一下当前ui
                int currentCount = item.Amount - targetAmount;
                if (currentCount > 0)
                {
                    UpdateAmountUI(currentCount);
                }
                else
                {
                    Destroy(this.gameObject);
                }

            });
    }


    /// <summary>
    /// 下面这三个函数是鼠标拖拽ui的事件
    /// </summary>
    /// <param name="eventData"></param>
    public void OnBeginDrag(PointerEventData eventData)
    {
        // 记录初始位置和偏移量
        offset = transform.position - Input.mousePosition;
        originSlot = transform.parent.GetComponent<InventorySlot>();
        originPosition = transform.position;

        // 将物品UI从原来的格子中移除
        transform.SetParent(ItemUITmpParent);

        // 标记为正在拖拽中
        isDragging = true;

        // 隐藏物品的RaycastTarget，避免干扰鼠标事件
        icon.raycastTarget = false;

    }

    public void OnDrag(PointerEventData eventData)
    {
        // 更新物品位置
        transform.position = Input.mousePosition + offset;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
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
                if(targetSlot.ItemUI == null)
                {
                    // 将物品放置到目标格子中
                    targetSlot.SetItemUI(this);
                }
                else
                {
                    //两个格子之间交换
                    originSlot.SetItemUI(targetSlot.ItemUI);
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
        ToolTip.Instance.Hide();
    }

    /// <summary>
    /// 鼠标点击事件
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerClick(PointerEventData eventData)
    {
        //双击使用物品
        if(eventData.clickCount == 2)
        {
            var slot = transform.parent.GetComponent<InventorySlot>();
            if(slot != null)
            {
                RemoteDataManager.Instance.ItemUse(slot.SlotIndex,1);
            }
        }

    }
}

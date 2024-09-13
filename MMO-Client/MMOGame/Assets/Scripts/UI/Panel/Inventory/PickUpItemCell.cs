using GameServer.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PickUpItemCell : MonoBehaviour
{
    private ItemEntity itemEntity;

    private Button pickupBtn;
    private Image icon;
    private Text itemName;
    private Text itemAmount;

    private void Awake()
    {
        pickupBtn = transform.GetComponent<Button>();
        icon = transform.Find("Icon").GetComponent<Image>();
        itemName = transform.Find("Name").GetComponent<Text>();
        itemAmount = transform.Find("Amount").GetComponent<Text>();
    }

    private void Start()
    {
        pickupBtn.onClick.AddListener(OnPickupBtn);
    }

    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="itemEntity"></param>
    public void Init(ItemEntity itemEntity)
    {
        this.itemEntity = itemEntity;
        SetUI();
    }

    /// <summary>
    /// 根据现有的ItemEntity设置ui
    /// </summary>
    public void SetUI()
    {
        if (itemEntity == null) return;
        icon.sprite = Res.LoadAssetSync<Sprite>(itemEntity.Icon);
        itemName.text = itemEntity.itemName;
        itemAmount.text ="" + itemEntity.Amount;
    }

    /// <summary>
    /// 拾取当前的item
    /// </summary>
    private void OnPickupBtn()
    {
        //发包
        ItemDataManager.Instance.ItemPickup(itemEntity.EntityId);
    }

}

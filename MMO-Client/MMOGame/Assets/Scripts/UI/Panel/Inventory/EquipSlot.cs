using HS.Protobuf.Backpack;
using UnityEngine;

public class EquipSlot : UISlot
{
    public EquipSlotType equipsType;        // 装备格子类型、inspector界面设置
    private Transform defaultBg;

    private void Awake()
    {
        defaultBg = transform.Find("Bg");
        SetDefaultBg(true);
    }

    public override void SetItemUI(ItemUI itemUI)
    {
        base.SetItemUI(itemUI);
        SetDefaultBg(false);
    }

    public override void RemoveItemUI()
    {
        base.RemoveItemUI();
        SetDefaultBg(true);
    }


    public override void DeleteItemUI()
    {
        base.DeleteItemUI();
        SetDefaultBg(true);
    }

    private void SetDefaultBg(bool flag)
    {
        defaultBg.gameObject.SetActive(flag);
    }

}

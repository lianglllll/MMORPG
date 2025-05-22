using HS.Protobuf.Backpack;

public class Item 
{
    protected NetItemDataNode m_itmeDataNode;
    private ItemDefine m_itemDefine;

    #region GetSet
    public ItemDefine ItemDefine => m_itemDefine;
    public int ItemId => m_itemDefine.ID;
    public string ItemName => m_itemDefine.Name;
    public int StackingUpperLimit => m_itemDefine.Capicity;
    public int Amount
    {
        get { return m_itmeDataNode.Amount; }
        set { m_itmeDataNode.Amount = value; }
    }
    public int SlotId
    {
        get
        {
            return m_itmeDataNode.GridIdx;
        }
        set
        {
            m_itmeDataNode.GridIdx = value;
        }
    }
    public string IconPath => m_itemDefine.Icon;
    #endregion

    public Item(NetItemDataNode itemDataNode)
    {
        m_itmeDataNode = itemDataNode;
        m_itemDefine = LocalDataManager.Instance.m_itemDefineDict[itemDataNode.ItemId];
    }
    public Item(ItemDefine define, int amount = 1, int position = 0)
    {
        m_itemDefine = define;
        m_itmeDataNode = new NetItemDataNode() { ItemId = ItemDefine.ID, Amount = amount, GridIdx = position };
    }

    public ItemType GetItemType()
    {
        switch (ItemDefine.ItemType)
        {
            case "消耗品": return ItemType.Consumable;
            case "道具": return ItemType.Material;
            case "装备": return ItemType.Equipment;
        }
        return ItemType.Consumable;
    }
    public Quality GetItemQuality()
    {
        switch (ItemDefine.Quality)
        {
            case "普通": return Quality.Common;
            case "非凡": return Quality.Fine;
            case "稀有": return Quality.Rare;
            case "史诗": return Quality.Epic;
            case "传说": return Quality.Legendary;
            case "神器": return Quality.Artifact;
        }
        return Quality.Common;
    }
    public virtual string GetItemDescText()
    {
       var content = $"<color=#ffffff>{this.ItemDefine.Name}</color>\n" +
                     $"<color=yellow>{this.ItemDefine.Description}</color>\n\n" +
                     $"<color=bulue>堆叠上限：{this.ItemDefine.Capicity}</color>";
        return content;
    }

}



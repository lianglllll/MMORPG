using HS.Protobuf.Game.Backpack;

public class Item 
{
    private ItemInfo m_itmeInfo;
    private ItemDefine m_itemDefine;

    public ItemDefine ItemDefine => m_itemDefine;
    public int ItemId => m_itemDefine.ID;
    public string ItemName => m_itemDefine.Name;
    public int StackingUpperLimit => m_itemDefine.Capicity;
    public int Amount
    {
        get { return m_itmeInfo.Amount; }
        set { m_itmeInfo.Amount = value; }
    }
    public int Position
    {
        get
        {
            return m_itmeInfo.Position;
        }
        set
        {
            m_itmeInfo.Position = value;
        }
    }
    public string IconPath => m_itemDefine.Icon;

    public Item(ItemInfo itemInfo)
    {
        m_itmeInfo = itemInfo;
    }
    public Item(ItemDefine define, int amount = 1, int position = 0)
    {
        m_itemDefine = define;
        m_itmeInfo = new ItemInfo() { ItemId = ItemDefine.ID, Amount = amount, Position = position };
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



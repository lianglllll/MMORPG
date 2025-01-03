using HS.Protobuf.Game.Backpack;
using System;

/// <summary>
/// 物品基类
/// </summary>
[Serializable]
public class Item 
{
    public ItemDefine Define { get; set; } 
    private ItemInfo _itmeInfo;                             //网络对象

    public int ItemId
    {
        get
        {
            return Define.ID;
        }
    }
    public int Amount
    {
        get { return _itmeInfo.Amount; }
        set { _itmeInfo.Amount = value; }
    }
    public int Position
    {
        get
        {
            return _itmeInfo.Position;
        }
        set
        {
            _itmeInfo.Position = value;
        }
    }
    public int StackingUpperLimit
    {
        get
        {
            return Define.Capicity;
        }
    }
    public ItemInfo ItemInfo
    {
        get
        {
            return _itmeInfo;
        }
    }

    /// <summary>
    /// 无参构造
    /// </summary>
    public Item()
    {
    }

    /// <summary>
    /// 构造方法,用网络对象初始化
    /// </summary>
    /// <param name="itemInfo"></param>
    public Item(ItemInfo itemInfo)
    {
        Define = DataManager.Instance.itemDefineDict[itemInfo.ItemId];
        _itmeInfo = new ItemInfo() { ItemId = Define.ID };
        this._itmeInfo.Amount = itemInfo.Amount;
        this._itmeInfo.Position = itemInfo.Position;
    }

    /// <summary>
    /// 构造方法，添加用
    /// </summary>
    /// <param name="define"></param>
    public Item(ItemDefine define, int amount = 1, int position = 0)
    {
        Define = define;
        _itmeInfo = new ItemInfo() { ItemId = Define.ID };
        this._itmeInfo.Amount = amount;
        this._itmeInfo.Position = position;
    }

    /// <summary>
    /// 获取item的类型
    /// </summary>
    /// <returns></returns>
    public ItemType GetItemType()
    {
        switch (Define.ItemType)
        {
            case "消耗品": return ItemType.Consumable;
            case "道具": return ItemType.Material;
            case "装备": return ItemType.Equipment;
        }
        return ItemType.Consumable;
    }

    /// <summary>
    /// 获取item的品质
    /// </summary>
    /// <returns></returns>
    public Quality GetQuality()
    {
        switch (Define.Quality)
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

    /// <summary>
    /// 获取描述文本
    /// </summary>
    /// <returns></returns>
    public virtual string GetDescText()
    {
       var content = $"<color=#ffffff>{this.Define.Name}</color>\n" +
                     $"<color=yellow>{this.Define.Description}</color>\n\n" +
                     $"<color=bulue>堆叠上限：{this.Define.Capicity}</color>";
        return content;
    }

}



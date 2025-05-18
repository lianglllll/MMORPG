using GameClient.Combat;
using HS.Protobuf.Backpack;

namespace GameClient.InventorySystem
{
    /// <summary>
    /// 装备
    /// </summary>
    public class Equipment : Item
    {
        private EquipsType m_equipsType;
        private AttrubuteData m_attrubuteData;
        public EquipsType EquipsType => m_equipsType;

        public Equipment(NetItemDataNode itemInfo) : base(itemInfo)
        {
            m_equipsType = ParseEquipType(ItemDefine.EquipType);
            // LoadAttrubuteData(itemInfo);
        }
        public Equipment(ItemDefine define,int position = 0) : base(define, 1, position)
        {
            LoadAttrubuteData(null);
        }

        private void LoadAttrubuteData(ItemInfo itemInfo)
        {
            m_attrubuteData = new AttrubuteData
            {
                Speed = ItemDefine.Speed,
                HPMax = ItemDefine.HP,
                MPMax = ItemDefine.MP,
                AD = ItemDefine.AD,
                AP = ItemDefine.AP,
                DEF = ItemDefine.DEF,
                MDEF = ItemDefine.MDEF,
                CRI = ItemDefine.CRI,
                CRD = ItemDefine.CRD,
                STR = ItemDefine.STR,
                INT = ItemDefine.INT,
                AGI = ItemDefine.AGI,
                HitRate = ItemDefine.HitRate,
                DodgeRate = ItemDefine.DodgeRate,
                HpRegen = ItemDefine.HpRegen,
                HpSteal = ItemDefine.HpSteal,
            };

            //可能需要处理一些额外的数据
            //打孔、镶嵌、强化
            //itemInfo.Equipdata;

        }
        public override string GetItemDescText()
        {
            var content = $"<color=#ffffff>{this.ItemDefine.Name}</color>\n" +
              $"<color=yellow>{this.ItemDefine.Description}</color>\n\n" +
              $"<color=bulue>堆叠上限：{this.ItemDefine.Capicity}</color>\n";
            var attr = m_attrubuteData;
            if (attr.Speed != 0)
                content += $"<color=green>速度: {attr.Speed}</color>\n";
            if (attr.HPMax != 0)
                content += $"<color=green>最大生命值: {attr.HPMax}</color>\n";
            if (attr.MPMax != 0)
                content += $"<color=green>最大法力值: {attr.MPMax}</color>\n";
            if (attr.AD != 0)
                content += $"<color=green>物理攻击: {attr.AD}</color>\n";
            if (attr.AP != 0)
                content += $"<color=green>法力强度: {attr.AP}</color>\n";
            if (attr.DEF != 0)
                content += $"<color=green>物理防御: {attr.DEF}</color>\n";
            if (attr.MDEF != 0)
                content += $"<color=green>魔法防御: {attr.MDEF}</color>\n";
            if (attr.CRI != 0)
                content += $"<color=green>暴击率: {attr.CRI}</color>\n";
            if (attr.CRD != 0)
                content += $"<color=green>暴击伤害: {attr.CRD}</color>\n";
            if (attr.HitRate != 0)
                content += $"<color=green>命中率: {attr.HitRate}</color>\n";
            if (attr.DodgeRate != 0)
                content += $"<color=green>闪避率: {attr.DodgeRate}</color>\n";
            if (attr.HpRegen != 0)
                content += $"<color=green>生命回复: {attr.HpRegen}</color>\n";
            if (attr.HpSteal != 0)
                content += $"<color=green>生命偷取: {attr.HpSteal}</color>\n";
            if (attr.STR != 0)
                content += $"<color=green>力量: {attr.STR}</color>\n";
            if (attr.INT != 0)
                content += $"<color=green>智力: {attr.INT}</color>\n";
            if (attr.AGI != 0)
                content += $"<color=green>敏捷: {attr.AGI}</color>\n";

            return content;
        }

        // tools
        private EquipsType ParseEquipType(string value)
        {
            switch (value)
            {
                case "无":
                    return EquipsType.Unset;
                case "武器":
                    return EquipsType.Weapon;
                case "胸甲":
                    return EquipsType.Chest;
                case "腰带":
                    return EquipsType.Belt;
                case "裤子":
                    return EquipsType.Legs;
                case "鞋子":
                    return EquipsType.Boots;
                case "戒指":
                    return EquipsType.Ring;
                case "项链":
                    return EquipsType.Neck;
                case "翅膀":
                    return EquipsType.Wings;
                default:
                    return EquipsType.Unset;
            }
        }
    }
}


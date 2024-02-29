using GameClient.Combat;
using Proto;
using System.Collections;
using System.Collections.Generic;

namespace GameClient.InventorySystem
{
    /// <summary>
    /// 装备
    /// </summary>
    public class Equipment : Item
    {
        public EquipsType EquipsType => ParseEquipType(Define.EquipType);
        public AttrubuteData attrubuteData;

        //network
        public Equipment(ItemInfo itemInfo) : base(itemInfo)
        {
            LoadAttrubuteData(itemInfo);
        }

        //define
        public Equipment(ItemDefine define,int position = 0) : base(define, 1, position)
        {
            LoadAttrubuteData(null);
        }

        /// <summary>
        /// str -> enum
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 加载装备数据
        /// </summary>
        /// <param name="itemInfo"></param>
        private void LoadAttrubuteData(ItemInfo itemInfo)
        {
            attrubuteData = new AttrubuteData
            {
                Speed = Define.Speed,
                HPMax = Define.HP,
                MPMax = Define.MP,
                AD = Define.AD,
                AP = Define.AP,
                DEF = Define.DEF,
                MDEF = Define.MDEF,
                CRI = Define.CRI,
                CRD = Define.CRD,
                STR = Define.STR,
                INT = Define.INT,
                AGI = Define.AGI,
                HitRate = Define.HitRate,
                DodgeRate = Define.DodgeRate,
                HpRegen = Define.HpRegen,
                HpSteal = Define.HpSteal,
            };

            //可能需要处理一些额外的数据
            //打孔、镶嵌、强化
            //itemInfo.Equipdata;

        }

        /// <summary>
        /// 获取描述文本
        /// </summary>
        /// <returns></returns>
        public override string GetDescText()
        {
            var content = $"<color=#ffffff>{this.Define.Name}</color>\n" +
              $"<color=yellow>{this.Define.Description}</color>\n\n" +
              $"<color=bulue>堆叠上限：{this.Define.Capicity}</color>\n";
            var attr = attrubuteData;
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



    }

}


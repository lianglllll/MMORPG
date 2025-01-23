using GameClient.Entities;
using GameClient.InventorySystem;
using Google.Protobuf.Collections;
using HS.Protobuf.Game.Backpack;
using System.Collections.Concurrent;

namespace GameClient.Combat
{
    public class EquipManager
    {
        public ConcurrentDictionary<EquipsType, Equipment> m_equipsDict = new();

        public bool Init(Actor owner, RepeatedField<ItemInfo> itemInfos)
        {
            m_equipsDict.Clear();
            foreach (var itemInfo in itemInfos)
            {
                var item = new Equipment(itemInfo);
                m_equipsDict[item.EquipsType] = item;
            }
            return true;
        }

        public ConcurrentDictionary<EquipsType, Equipment> EquipsDict => m_equipsDict;
    }
}

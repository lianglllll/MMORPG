using GameClient.Entities;
using GameClient.InventorySystem;
using Google.Protobuf.Collections;
using HS.Protobuf.Backpack;
using System;
using System.Collections.Concurrent;

namespace GameClient.Combat
{
    public class EquipManager
    {
        private ConcurrentDictionary<EquipsType, Equipment> m_equipsDict = new();

        public ConcurrentDictionary<EquipsType, Equipment> EquipsDict => m_equipsDict;

        public bool Init(Actor owner)
        {
            return true;
        }

        public void ReloadInventoryData(NetItemInventoryDataNode node)
        {
            m_equipsDict.Clear();
            foreach (var itemNode in node.ItemDataNodes)
            {
                var item = new Equipment(itemNode);
                m_equipsDict[item.EquipsType] = item;
            }
        }
    }
}

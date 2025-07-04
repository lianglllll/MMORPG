﻿using GameServer.core.Model.BaseItem;
using GameServer.core.Model.BaseItem.Sub;
using GameServer.Core.Model;
using HS.Protobuf.Backpack;
using HS.Protobuf.Common;
using HS.Protobuf.Scene;
using HS.Protobuf.SceneEntity;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace GameServer.InventorySystem
{
    /// <summary>
    /// 装备管理器，每个角色都有
    /// </summary>
    public class EquipmentManager
    {
        private GameCharacter m_owner;
        private ConcurrentDictionary<EquipSlotType, GameEquipment> equipDict = new();
        private NetItemInventoryDataNode m_netItemInventoryDataNode;                    // 网络对象,主要是用作发送给客户端，和保存数据库
        private bool m_netDataIsNewly;
        private bool m_hasChange;

        #region GetSet
        public GameCharacter Owner => m_owner;
        public bool HasChanged
        {
            get => m_hasChange;
            set
            {
                m_hasChange = value;
                m_netDataIsNewly = false;
            }
        }
        public NetItemInventoryDataNode NetItemInventoryDataNode
        {
            get
            {
                if (!m_netDataIsNewly)
                {
                    m_netItemInventoryDataNode.ItemDataNodes.Clear();
                    foreach (var item in equipDict.Values)
                    {
                        m_netItemInventoryDataNode.ItemDataNodes.Add(item.NetItemDataNode);
                    }
                    m_netDataIsNewly = true;
                }
                return m_netItemInventoryDataNode;
            }
        }

        #endregion

        #region 生命周期
        public void Init(GameCharacter chr, byte[] bytes)
        {
            m_owner = chr;

            // 信息为空
            if (bytes == null || bytes.Count() == 0)
            {
                m_netItemInventoryDataNode = new();
                m_netItemInventoryDataNode.InventoryType = ItemInventoryType.Equipments;
                m_netItemInventoryDataNode.Capacity = 15;
                HasChanged = true;
                goto End;
            }

            // 数据还原 binary => Inventory
            m_netItemInventoryDataNode = NetItemInventoryDataNode.Parser.ParseFrom(bytes);

            // 创建物品
            foreach (var iteminfo in m_netItemInventoryDataNode.ItemDataNodes)
            {
                var item = new GameEquipment(iteminfo);
                equipDict.TryAdd(item.EquipSlotType, item);
            }

            // 更新chr装备信息
            // Owner.Info.EquipList.Clear();
            // Owner.Info.EquipList.AddRange(NetItemInventoryDataNode.List);
        End:
            return;
        }
        #endregion

        #region 操作函数
        public GameEquipment GetEquipment(EquipSlotType type)
        {
            return equipDict.GetValueOrDefault(type, null);
        }
        public bool WearEquipment(GameEquipment equipment, EquipSlotType equipSlotType)
        {
            // 穿戴之前，先卸下来身上的装备放回背包
            UnloadEquipment(equipSlotType, false);

            // 移除背包中的物品格子
            int gridIdx = equipment.GridIdx;
            Owner.BackPackManager.RemoveGrid(gridIdx);

            // 穿到身上
            equipDict[equipSlotType] = equipment;
            equipment.EquipSlotType = equipSlotType;

            HasChanged = true;

            // 通知scene装备属性的变更
            _SendChangeEquipmentToSceneRequest(equipSlotType, ChangeEquipmentOperationType.Wear, equipment.NetAttrubuteDataNode);

            return true;
        }
        public bool UnloadEquipment(EquipSlotType type, bool isSendToScene)
        {
            bool result = false;

            // 没有这个装备
            if (!equipDict.TryGetValue(type, out var equipment))
            {
                goto End;
            }

            bool isSuccess = Owner.BackPackManager.AddGameEquipItem(equipment);
            if (!isSuccess)
            {
                // 放回背包失败
                goto End;
            }

            // 放回背包成功,从字典中移除
            equipDict.TryRemove(type, out var equip);
            equip.EquipSlotType = EquipSlotType.None;

            HasChanged = true;

            // 需要通知scene中，关于chr的装备属性变化
            if (isSendToScene)
            {
                _SendChangeEquipmentToSceneRequest(type, ChangeEquipmentOperationType.Unload);
            }
        End:
            return result;
        }
        #endregion

        #region tools
        private void _SendChangeEquipmentToSceneRequest(EquipSlotType equipsType, ChangeEquipmentOperationType opType, NetAttrubuteDataNode attrubuteData = null)
        {
            var req = new ChangeEquipmentToSceneRequest();
            req.EntityId = Owner.EntityId;
            req.OperationType = opType;
            var equipNode = new NetEquipmentNode();
            req.EquipNode = equipNode;
            equipNode.EquipType = equipsType;
            if (opType == ChangeEquipmentOperationType.Wear)
            {
                equipNode.AttrubuteDataNode = attrubuteData;
            }
            Owner.SendToScene(req);
        }
        public List<NetEquipmentNode> GetNetEquipmentNodes()
        {
            var list = new List<NetEquipmentNode>();
            foreach(var kv in equipDict)
            {
                var node = new NetEquipmentNode();
                node.EquipType = kv.Key;
                node.AttrubuteDataNode = kv.Value.NetAttrubuteDataNode; 
                list.Add(node);
            }
            return list;
        }
        #endregion
    }
}

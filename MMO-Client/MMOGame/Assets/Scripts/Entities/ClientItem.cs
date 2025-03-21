using GameClient.Entities;
using HS.Protobuf.SceneEntity;
using UnityEngine;

namespace GameClient.Entities
{
    /// <summary>
    /// 物体的实体模型
    /// 场景中显示的模型
    /// </summary>
    public class ClientItem:Entity
    {
        private GameObject m_renderObj;        
        private NetItemNode m_netItemNode;
        private Item m_item;

        public GameObject RenderObj
        {
            get
            {
                return m_renderObj;
            }
            set
            {
                m_renderObj = value;
            }
        }
        public string IconPath => m_item.IconPath;
        public int Amount
        {
            get
            {
                return m_item.Amount;
            }
            set
            {
                m_item.Amount = value;
            }
        }
        public string ItemName => m_item.ItemName;
        public int CurSceneId => m_netItemNode.SceneId;
        public int ItemId => m_item.ItemId;


        public ClientItem(NetItemNode netItemNode) :base(netItemNode.EntityId, netItemNode.Transform)
        {
            m_netItemNode = netItemNode;
            m_item = new Item(netItemNode.NetItemDataNode);
        }
        public void UpdateInfo(NetItemNode netItemNode)
        {
            m_netItemNode = netItemNode;
            m_item = new Item(netItemNode.NetItemDataNode);
        }
    }
}

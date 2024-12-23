using GameClient.Entities;
using HS.Protobuf.SceneEntity;
using UnityEngine;

namespace GameServer.Model
{
    /// <summary>
    /// 物体的实体模型
    /// 场景中显示的模型
    /// </summary>
    public class ItemEntity:Entity
    {
        private Item item;                  //item信息
        public GameObject renderObj;        //对应的游戏对象

        public int itemId
        {
            get
            {
                return item.ItemId;
            }
        }

        public string itemName
        {
            get
            {
                return item.Define.Name;
            }
        }

        public string Icon
        {
            get
            {
                return item.Define.Icon;
            }
        }

        public int Amount
        {
            get
            {
                return item.Amount;
            }
            set
            {
                item.Amount = value;
            }
        }


        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="item"></param>
        /// <param name="pos"></param>
        /// <param name="dir"></param>
        public ItemEntity(NetEItem netEItem) :base(netEItem.Entity)
        {
            item = new Item(netEItem.ItemInfo);
        }

        public void UpdateInfo(NetEItem netItemEntity)
        {
            item = new Item(netItemEntity.ItemInfo);
        }

    }
}

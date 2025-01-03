﻿using GameServer.Model;
using Common.Summer.Tools;
using HS.Protobuf.Game.Backpack;

namespace GameServer.Manager
{
    public class ItemManager : Singleton<ItemManager>
    {
        /// <summary>
        /// 处理物品使用
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public bool ItemUse(ItemUseRequest message)
        {
            //安全校验
            Entity entity = EntityManager.Instance.GetEntityById(message.EntityId);
            if (entity == null) return false;
            if (!((EntityManager.Instance.GetEntityById(message.EntityId)) is Character chr)) return false;

            //让chr使用
            return chr.UseItem(message.SlotIndex,message.Count);
        }

    }
}

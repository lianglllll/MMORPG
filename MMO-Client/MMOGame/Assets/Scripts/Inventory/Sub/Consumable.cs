using HS.Protobuf.Backpack;

namespace GameClient.InventorySystem
{
    /// <summary>
    /// 消耗品
    /// </summary>
    public class Consumable : Item
    {
        public Consumable(NetItemDataNode itemNode) : base(itemNode)
        {
        }
        public Consumable(ItemDefine define, int amount = 1, int position = 0) : base(define, amount, position)
        {
        }
    }

}


using HS.Protobuf.Backpack;

namespace GameClient.InventorySystem
{
    public class MaterialItem : Item
    {
        public MaterialItem(NetItemDataNode itemInfo) : base(itemInfo)
        {
        }
        public MaterialItem(ItemDefine define, int amount = 1, int position = 0) : base(define, amount, position)
        {
        }
    }

}


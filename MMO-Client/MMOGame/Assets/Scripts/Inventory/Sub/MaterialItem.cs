using HS.Protobuf.Game.Backpack;

namespace GameClient.InventorySystem
{
    public class MaterialItem : Item
    {
        public MaterialItem(ItemInfo itemInfo) : base(itemInfo)
        {
        }
        public MaterialItem(ItemDefine define, int amount = 1, int position = 0) : base(define, amount, position)
        {
        }
    }

}


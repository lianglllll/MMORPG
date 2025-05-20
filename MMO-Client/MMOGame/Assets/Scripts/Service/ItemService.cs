using HSFramework.MySingleton;
using Common.Summer.Core;
using Common.Summer.Net;
using GameClient;
using GameClient.Entities;
using HS.Protobuf.Backpack;
using HS.Protobuf.Scene;

public class ItemService : SingletonNonMono<ItemService>
{
    /// <summary>
    /// 初始化，gamemanager中启用
    /// </summary>
    public void Init()
    {
        MessageRouter.Instance.Subscribe<NetEItemSync>(_NetEItemSync); 
        MessageRouter.Instance.Subscribe<ItemUseResponse>(_ItemUseResponse);
    }
    public void UnInit()
    {
        MessageRouter.Instance.UnSubscribe<NetEItemSync>(_NetEItemSync);
        MessageRouter.Instance.UnSubscribe<ItemUseResponse>(_ItemUseResponse);
    }



    /// <summary>
    /// 场景中的物品信息同步响应
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="msg"></param>
    private void _NetEItemSync(Connection sender, NetEItemSync msg)
    {
        EntityManager.Instance.OnItemEntitySync(msg.NetEItem);
    }

    /// <summary>
    /// 发送使用物品请求
    /// </summary>
    /// <param name="slotIndex"></param>
    public void ItemUseRequest(int slotIndex, int count)
    {
        ItemUseRequest req = new ItemUseRequest();
        req.EntityId = GameApp.character.EntityId;
        req.SlotIndex = slotIndex;
        req.Count = count;
        NetManager.Instance.Send(req);
    }

    /// <summary>
    /// 使用物品请求响应
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="msg"></param>
    private void _ItemUseResponse(Connection conn, ItemUseResponse msg)
    {
        //server下发的数据一定是正确的，如果有问题，我们就请求拉去背包数据
        if (msg.Result == true)
        {
            //更新背包数据
            ItemDataManager.Instance.UpdateKnapsackItemAmount(msg);
        }
    }

    /// <summary>
    /// 穿戴装备请求
    /// </summary>
    /// <param name="slotIndex"></param>
    public void _WearEquipmentRequest(int slotIndex)
    {
        var req = new WearEquipmentRequest();
        req.SlotIndex = slotIndex;
        NetManager.Instance.Send(req);
    }

    /// <summary>
    /// 卸载装备请求
    /// </summary>
    /// <param name="type"></param>
    public void _UnloadEquipmentRequest(EquipsType type)
    {
        var req = new UnloadEquipmentRequest();
        req.Type = type;
        NetManager.Instance.Send(req);
    }








}

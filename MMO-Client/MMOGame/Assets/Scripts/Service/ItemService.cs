using Common.Summer.Core;
using Common.Summer.Net;
using GameClient;
using GameClient.Entities;
using HS.Protobuf.Backpack;
using HS.Protobuf.Scene;
using Summer;

public class ItemService : Singleton<ItemService>
{
    /// <summary>
    /// 初始化，gamemanager中启用
    /// </summary>
    public void Init()
    {
        MessageRouter.Instance.Subscribe<InventoryInfoResponse>(_InventoryInfoResponse);
        MessageRouter.Instance.Subscribe<NetEItemSync>(_NetEItemSync); 
        MessageRouter.Instance.Subscribe<ItemUseResponse>(_ItemUseResponse);
        MessageRouter.Instance.Subscribe<EquipsUpdateResponse>(_EquipsUpdateResponse);
        MessageRouter.Instance.Subscribe<ItemDiscardResponse>(_ItemDiscardResponse);
        MessageRouter.Instance.Subscribe<ItemPickupResponse>(_ItemPickupResponse);
    }


    public void UnInit()
    {
        MessageRouter.Instance.UnSubscribe<InventoryInfoResponse>(_InventoryInfoResponse);
        MessageRouter.Instance.UnSubscribe<NetEItemSync>(_NetEItemSync);
        MessageRouter.Instance.UnSubscribe<ItemUseResponse>(_ItemUseResponse);
        MessageRouter.Instance.UnSubscribe<EquipsUpdateResponse>(_EquipsUpdateResponse);
        MessageRouter.Instance.UnSubscribe<ItemDiscardResponse>(_ItemDiscardResponse);
        MessageRouter.Instance.UnSubscribe<ItemPickupResponse>(_ItemPickupResponse);
    }

    /// <summary>
    /// 发送获取整个背包信息的请求
    /// </summary>
    /// <returns></returns>
    public void _InventoryInfoRequest()
    {
        //发送查询请求，查询背包信息
        InventoryInfoRequest req = new InventoryInfoRequest();
        req.EntityId = GameApp.character.EntityId;

        req.QueryKnapsack = true;
        NetClient.Send(req);
    }

    /// <summary>
    /// 获取inventory信息的响应
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="msg"></param>
    private void _InventoryInfoResponse(Connection conn, InventoryInfoResponse msg)
    {
        var chr = GameApp.character;
        if (chr == null || chr.EntityId != msg.EntityId) return;

        if (msg.KnapsackInfo != null)
        {
            //缓存背包信息
            ItemDataManager.Instance.ReloadKnapsackData(msg.KnapsackInfo);

        }else if(msg.EquipmentInfo != null)
        {
            ItemDataManager.Instance.ReloadEquipData(chr,msg.EquipmentInfo.List);
        }
        else if(msg.WarehouseInfo != null)
        {

        }

    }

    /// <summary>
    /// 装备信息更新的响应
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="msg"></param>
    private void _EquipsUpdateResponse(Connection sender, EquipsUpdateResponse msg)
    {
        var actor = GameTools.GetActorById(msg.EntityId);
        if (actor == null) return;
        ItemDataManager.Instance.ReloadEquipData(actor, msg.EquipsList);
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
        NetClient.Send(req);
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
        NetClient.Send(req);
    }

    /// <summary>
    /// 卸载装备请求
    /// </summary>
    /// <param name="type"></param>
    public void _UnloadEquipmentRequest(EquipsType type)
    {
        var req = new UnloadEquipmentRequest();
        req.Type = type;
        NetClient.Send(req);
    }

    /// <summary>
    /// 物品放置请求（目前只用在背包中）
    /// </summary>
    /// <param name="originType"></param>
    /// <param name="targetType"></param>
    /// <param name="originSlot"></param>
    /// <param name="targetSlot"></param>
    public void ItemPlacementRequeset(ItemPlacementRequest req)
    {
        NetClient.Send(req);
    }

    /// <summary>
    /// 物品拾取请求
    /// </summary>
    /// <param name="entityId"></param>
    public void ItemPickupRequest(int entityId)
    {
        var req = new ItemPickUpRequest();
        req.EntityId = entityId;
        NetClient.Send(req);
    }

    /// <summary>
    /// 拾起响应
    /// </summary>
    public void _ItemPickupResponse(Connection sender, ItemPickupResponse msg)
    {

        //刷一下ui
        if (msg.ResultCode == 0)
        {
            Kaiyun.Event.FireOut("UpdateCharacterKnapsackPickupItemBox");

            Kaiyun.Event.FireOut("UpdateCharacterKnapsackPickupItemBox");
            var item = DataManager.Instance.itemDefineDict[msg.ItemId];
            UnityMainThreadDispatcher.Instance().Enqueue(() =>
            {
                UIManager.Instance.MessagePanel.ShowItemIOInfo($"拾取物品:{item.Name}X{msg.Amount}");
            });
        }

    }

    /// <summary>
    /// 物品丢弃请求
    /// </summary>
    /// <param name="slotIndex"></param>
    /// <param name="number"></param>
    /// <param name="type"></param>
    public void ItemDiscardRequest(int slotIndex,int number, InventoryType type)
    {
        var req = new ItemDiscardRequest();
        req.SlotIndex = slotIndex;
        req.Number = number;
        req.Type = type;
        NetClient.Send(req);
    }

    /// <summary>
    /// 丢弃响应
    /// </summary>
    public void _ItemDiscardResponse(Connection sender, ItemDiscardResponse msg)
    {
        //刷一下ui
        if(msg.ResultCode == 0)
        {
            Kaiyun.Event.FireOut("UpdateCharacterKnapsackPickupItemBox");
            var item = DataManager.Instance.itemDefineDict[msg.ItemId];
            UnityMainThreadDispatcher.Instance().Enqueue(() =>
            {
                UIManager.Instance.MessagePanel.ShowItemIOInfo($"丢弃物品:{item.Name}X{msg.Amount}");
            });
        }

    }

}

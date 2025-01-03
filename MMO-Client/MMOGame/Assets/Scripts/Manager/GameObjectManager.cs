using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameClient.Entities;
using Unity.VisualScripting;
using GameServer.Model;
using Assets.Script.Entities;
using System.Collections.Concurrent;
using UnityEngine.SceneManagement;
using GameClient;
using Player;
using Player.Controller;
using HSFramework.Net;
using HS.Protobuf.SceneEntity;
using HS.Protobuf.Scene;

/// <summary>
/// 游戏对象管理器，管理当前场景中的Gameobject
/// </summary>
public class GameObjectManager:MonoBehaviour
{
    public static GameObjectManager Instance;
    private static ConcurrentDictionary<int, GameObject> currentGameObjectDict = new();                         //<entityid,gameobject>  entity和gameobject的映射
    private static ConcurrentQueue<NetActor> preparCrateActorObjQueue = new();                                  //创建actorObj的缓冲队列
    private static ConcurrentQueue<NetEItem> preparCrateItemObjQueue = new();                                   //创建itemObj的缓冲队列

    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        Kaiyun.Event.RegisterOut("CreateActorObject", this, "CreateActorObject");
        Kaiyun.Event.RegisterOut("CreateItemObject", this, "CreateItemObject");
        Kaiyun.Event.RegisterOut("EntityLeave", this, "EntityLeave");
        Kaiyun.Event.RegisterOut("EntitySync", this, "EntitySync");
        Kaiyun.Event.RegisterOut("CtlEntitySync", this, "CtlEntitySync");
    }
    private void Update()
    {
        if (SceneManager.GetActiveScene().isLoaded)
        {
            while(preparCrateActorObjQueue.TryDequeue(out var item))
            {
                if (GameApp.SpaceId != item.SpaceId) continue;
                _CreateActorObject(item);
            }

            while(preparCrateItemObjQueue.TryDequeue(out var item))
            {
                if (GameApp.SpaceId != item.SpaceId) continue;
                _CreateItemObject(item);
            }
        }
    }
    private void OnDestroy()
    {
        Kaiyun.Event.UnregisterOut("CreateActorObject", this, "CreateActorObject");
        Kaiyun.Event.UnregisterOut("CreateItemObject", this, "CreateItemObject");
        Kaiyun.Event.UnregisterOut("EntityLeave", this, "EntityLeave");
        Kaiyun.Event.UnregisterOut("EntitySync", this, "EntitySync");
        Kaiyun.Event.UnregisterOut("CtlEntitySync", this, "CtlEntitySync");

    }

    /// <summary>
    /// 事件驱动：异步向当前场景中创建ActorObj
    /// </summary>
    /// <param name="chr"></param>
    public void CreateActorObject(NetActor nActor)
    {
        //
        if (GameApp.SpaceId != nActor.SpaceId) return;

        //添加到缓冲队列当中
        preparCrateActorObjQueue.Enqueue(nActor);
    }
    private void _CreateActorObject(NetActor nActor)
    {
        Actor actor = EntityManager.Instance.GetEntity<Actor>(nActor.Entity.Id);
        if (actor == null) return;

        if(currentGameObjectDict.TryGetValue(actor.EntityId,out var item))
        {
            if (item == null || item.IsDestroyed())
            {
                currentGameObjectDict.TryRemove(actor.EntityId, out _);
                actor.renderObj = null;
            }
            else
            {
                actor.renderObj = item;
                return;
            }
        }

        StartCoroutine(LoadActor(nActor));
    }
    private IEnumerator LoadActor(NetActor nActor)
    {
        //1.判断合法性
        Actor actor = EntityManager.Instance.GetEntity<Actor>(nActor.Entity.Id);
        if (actor == null) yield break;
        bool isMine = (nActor.Entity.Id == GameApp.entityId);

        //2.异步加载资源
        UnitDefine unitDefine = actor.define;
        GameObject prefab = null;
        yield return Res.LoadAssetAsyncWithTimeout<GameObject>(unitDefine.Resource, (obj) => {
            prefab = obj;
        });

        //下一帧再执行接下去的
        yield return prefab;

        //3.获取坐标和方向,实例化obj并初始化,将实例化的角色放到gamemanager下面
        Vector3 initPosition = V3.Of(nActor.Entity.Position) / 1000;
        initPosition.y = GameTools.CaculateGroundPosition(initPosition, 1.5f, 7).y;//计算地面坐标
        GameObject actorObj = Instantiate(prefab, initPosition, Quaternion.identity, this.transform);
        if (actorObj == null) yield break;
        if (!currentGameObjectDict.TryAdd(nActor.Entity.Id, actorObj))
        {
            Destroy(actorObj);
            yield break;
        }

        //加入actor图层
        actorObj.layer = 6;
        actor.renderObj = actorObj; //actor 和 gameobj关联
        if (actor is Character chr)
        {
            actorObj.name = "Character_" + nActor.Entity.Id;

        }else if(actor is Monster mon)
        {
            actorObj.name = "Monster_" + nActor.Entity.Id;
            //如果怪物死亡，就不要显示了
            if (Mathf.Approximately(nActor.Hp, 0))
            {
                actorObj.SetActive(false);
                actor.actorMode = ActorMode.Dead;
            }
        }


        //7.给我们控制的角色添加一些控制脚本
        if (isMine)
        {
            actorObj.tag = "CtlPlayer";                                                             //打标签
            PlayerModel modelBase = actorObj.transform.Find("Model").gameObject.AddComponent<PlayerModel>();
            UnitUIController uuc = actorObj.AddComponent<UnitUIController>();
            CtrlController ctl = actorObj.AddComponent<CtrlController>();                           //给当前用户控制的角色添加控制脚本
            PlayerCombatController combat = actorObj.AddComponent<PlayerCombatController>();        //给当前用户控制的角色添加战斗脚本
            SyncEntitySend syncEntitySend = actorObj.AddComponent<SyncEntitySend>();
            modelBase.Init();
            ctl.Init(actor, syncEntitySend);
            actor.Init(ctl);
            combat.Init(ctl);
            syncEntitySend.Init(ctl, initPosition, Vector3.zero);

            //启用第三人称摄像机
            TP_CameraController.instance.OnStart(actorObj.transform.Find("CameraLookTarget").transform);

        }
        else
        {
            actorObj.tag = "SyncPlayer";                                                             //打标签
            var modelBase = actorObj.transform.Find("Model").gameObject.AddComponent<SyncModel>();
            UnitUIController uuc = actorObj.AddComponent<UnitUIController>();
            SyncController ctl = actorObj.AddComponent<SyncController>();                           //给当前用户控制的角色添加控制脚本
            SyncEntityRecive syncEntityRecive = actorObj.AddComponent<SyncEntityRecive>();
            modelBase.Init();
            ctl.Init(actor, syncEntityRecive);
            actor.Init(ctl);
            syncEntityRecive.Init(ctl, initPosition, Vector3.zero);
        }


    }

    /// <summary>
    /// 事件驱动：异步向当前场景中创建物品
    /// </summary>
    /// <param name="netEItem"></param>
    public void CreateItemObject(NetEItem netEItem)
    {
        //
        if (GameApp.SpaceId != netEItem.SpaceId) return;

        preparCrateItemObjQueue.Enqueue(netEItem);

    }
    public void _CreateItemObject(NetEItem netEItem)
    {
        int entityId = netEItem.Entity.Id;
        ItemEntity itemEntity = EntityManager.Instance.GetEntity<ItemEntity>(entityId);
        if (itemEntity == null) return;

        //1.先判断当前character是否还保存在dict中
        if(currentGameObjectDict.TryGetValue(entityId,out var item))
        {
            if(item == null || item.IsDestroyed())
            {
                currentGameObjectDict.TryRemove(entityId, out _);
                itemEntity.renderObj = null;
            }
            else
            {
                itemEntity.renderObj = item;
                return;
            }
        }

        StartCoroutine(LoadItem(netEItem));
    }
    private IEnumerator LoadItem(NetEItem netItemEntity)
    {
        int entityId = netItemEntity.Entity.Id;
        ItemEntity itemEntity = EntityManager.Instance.GetEntity<ItemEntity>(entityId);
        var define = DataManager.Instance.itemDefineDict[netItemEntity.ItemInfo.ItemId];

        GameObject prefab = null;
        yield return Res.LoadAssetAsyncWithTimeout<GameObject>(define.Model, (obj) => {
            prefab = obj;
        });
        yield return prefab;

        //下面的操作可能有问题，应该移动到LoadAsset的回调中吧。

        //3.获取坐标和方向
        Vector3 initPosition = V3.Of(netItemEntity.Entity.Position) / 1000;
        if (initPosition.y == 0)
        {
            initPosition.y = GameTools.CaculateGroundPosition(initPosition, 0.5f, 7).y;           //计算地面坐标
        }

        //4.实例化
        GameObject itemObj = Instantiate(prefab, initPosition, Quaternion.identity, this.transform);//将实例化的角色放到gamemanager下面

        //5.actor 和 gameobj关联
        itemEntity.renderObj = itemObj;

        //6.修改一下obj的设置
        itemObj.name = "Item_" + entityId;
        itemObj.layer = 8;                   //加入Item图层

        //7.放入dict管理
        if(!currentGameObjectDict.TryAdd(netItemEntity.Entity.Id, itemObj))
        {
            Destroy(itemObj);
            yield break;
        }

    }

    /// <summary>
    /// 事件驱动：entity离开场景
    /// 不同的场景可以创建不同的对象池来使用这个东西
    /// </summary>
    /// <param name="entityId"></param>
    public void EntityLeave(int entityId)
    {
        if (!currentGameObjectDict.ContainsKey(entityId)) return;
        var obj = currentGameObjectDict[entityId];
        if(obj != null && !obj.IsDestroyed())
        {
            Destroy(obj);
        }
        currentGameObjectDict.TryRemove(entityId,out _);
    }
    /// <summary>
    /// 事件驱动：其他entity位置+动画状态信息同步
    /// </summary>
    /// <param name="nEntitySync"></param>
    public void EntitySync(NEntitySync nEntitySync)
    {
        int entityId = nEntitySync.Entity.Id;
        GameObject obj = currentGameObjectDict.GetValueOrDefault(entityId, null);
        if (obj == null) return;
        //设置数据到entity中
        SyncEntityRecive syncEntityRecive = obj.GetComponent<SyncEntityRecive>();
        syncEntityRecive.SyncEntity(nEntitySync);
    }
    /// <summary>
    /// 事件驱动：本机角色位置+动画状态信息同步
    /// </summary>
    /// <param name="nEntitySync"></param>
    public void CtlEntitySync(NEntitySync nEntitySync)
    {
        GameObject obj = GameApp.character?.renderObj;
        if (obj == null) return;

        //这里通常由服务器来通知一些我们的特殊变化
        //比如：眩晕、击退、击飞、强制位移
        //如果是None,一律不作处理，将维持原来的动画状态
        if (nEntitySync.State != ActorState.Constant)
        {
            var ctrl = GameApp.character.baseController;
            ctrl.ChangeState(nEntitySync.State);
        }
        //设置数据到entity中,这里强制设置
        SyncEntitySend syncEntitySend = obj.GetComponent<SyncEntitySend>();
        syncEntitySend.SyncPosAndRotaion(nEntitySync.Entity);
    }
}

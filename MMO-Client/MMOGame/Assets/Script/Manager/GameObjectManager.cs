using Summer;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Proto;
using GameClient.Entities;
using Unity.VisualScripting;
using GameServer.Model;
using Assets.Script.Entities;
using Serilog;
using YooAsset;
using System.Collections.Concurrent;
using System;

/// <summary>
/// 游戏对象管理器，管理当前场景中的Gameobject
/// </summary>
public class GameObjectManager:MonoBehaviour
{
    public static GameObjectManager Instance;
    private static ConcurrentDictionary<int, GameObject> currentGameObjectDict = new();                         //<entityid,gameobject>  entity和gameobject的映射
    private static ConcurrentQueue<NetActor> preparCrateActorObjQueue = new();                                  //创建actorObj的缓冲队列
    private static ConcurrentQueue<NetItemEntity> preparCrateItemObjQueue = new();                              //创建itemObj的缓冲队列

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
        if (Mathf.Approximately(SceneLoader.Progress, 1))
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
    /// 事件驱动：向当前场景中创建ActorObj
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

    /// <summary>
    /// 协程加载方式角色Obj
    /// </summary>
    /// <param name="nActor"></param>
    /// <returns></returns>
    private IEnumerator LoadActor(NetActor nActor)
    {
        //1.判断合法性
        Actor actor = EntityManager.Instance.GetEntity<Actor>(nActor.Entity.Id);
        if (actor == null) yield break;
        bool isMine = (nActor.Entity.Id == GameApp.entityId);

        //2.获取热更资源包
        UnitDefine unitDefine = actor.define;
        var package = YooAssets.GetPackage("DefaultPackage");
        AssetHandle handle = package.LoadAssetAsync<GameObject>(unitDefine.Resource);
        yield return handle;
        var prefab = handle.AssetObject as GameObject;

        //3.获取坐标和方向
        Vector3 initPosition = V3.Of(nActor.Entity.Position) / 1000;
        if (initPosition.y == 0)
        {
            initPosition.y = GameTools.CaculateGroundPosition(initPosition, 1.5f, 7).y;//计算地面坐标
        }

        //4.实例化obj并初始化
        GameObject chrObj = Instantiate(prefab, initPosition, Quaternion.identity, this.transform);//将实例化的角色放到gamemanager下面
        if (chrObj == null) yield break;

        if (!currentGameObjectDict.TryAdd(nActor.Entity.Id, chrObj))
        {
            Destroy(chrObj);
            yield break;
        }

        if (nActor.EntityType == EntityType.Character)
        {
            chrObj.name = "Character_" + nActor.Entity.Id;
        }
        else if (nActor.EntityType == EntityType.Monster)
        {
            chrObj.name = "Monster_" + nActor.Entity.Id;
            //如果怪物死亡，就不要显示了
            if (Mathf.Approximately(nActor.Hp, 0))
            {
                chrObj.SetActive(false);
                actor.unitState = UnitState.Dead;
            }
        }
        chrObj.layer = 6;//加入actor图层


        //5.actor 和 gameobj关联
        actor.renderObj = chrObj;
        actor.StateMachine = chrObj.GetComponent<PlayerStateMachine>();
        actor.StateMachine.parameter.owner = actor;

        //obj身上的ui
        var unitui = chrObj.AddComponent<UnitUIController>();
        unitui.Init(actor);
        actor.unitUIController = unitui;



        //6.设置一下同步脚本gameentity
        GameEntity gameEntity = chrObj.GetComponent<GameEntity>();
        gameEntity._Start(actor, isMine, initPosition, Vector3.zero);


        //7.给我们控制的角色添加一些控制脚本
        if (isMine)
        {
            chrObj.tag = "CtlPlayer";                                                          //打标签
            PlayerMovementController ctl = chrObj.AddComponent<PlayerMovementController>();    //给当前用户控制的角色添加控制脚本
            ctl.Init(actor);
            PlayerCombatController combat = chrObj.AddComponent<PlayerCombatController>();     //给当前用户控制的角色添加战斗脚本
            combat.Init(actor);

            //启用第三人称摄像机
            TP_CameraController.instance.OnStart(chrObj.transform.Find("CameraLookTarget").transform);
        }


    }

    /// <summary>
    /// 事件驱动：向当前场景中创建物品
    /// </summary>
    /// <param name="netItemEntity"></param>
    public void CreateItemObject(NetItemEntity netItemEntity)
    {
        //
        if (GameApp.SpaceId != netItemEntity.SpaceId) return;

        preparCrateItemObjQueue.Enqueue(netItemEntity);

    }
    public void _CreateItemObject(NetItemEntity netItemEntity)
    {
        int entityId = netItemEntity.Entity.Id;
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

        StartCoroutine(LoadItem(netItemEntity));
    }

    /// <summary>
    /// 异步加载物品
    /// </summary>
    /// <param name="netItemEntity"></param>
    /// <returns></returns>
    private IEnumerator LoadItem(NetItemEntity netItemEntity)
    {
        int entityId = netItemEntity.Entity.Id;
        ItemEntity itemEntity = EntityManager.Instance.GetEntity<ItemEntity>(entityId);
        var define = DataManager.Instance.itemDefineDict[netItemEntity.ItemInfo.ItemId];

        // 2.获取热更资源包
        var package = YooAssets.GetPackage("DefaultPackage");
        AssetHandle handle = package.LoadAssetAsync<GameObject>(define.Model);
        yield return handle;
        GameObject prefab = handle.AssetObject as GameObject;


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
        //1.设置位置+方向+speed
        int entityId = nEntitySync.Entity.Id;
        GameObject obj = currentGameObjectDict.GetValueOrDefault(entityId, null);
        if (obj == null) return;
        GameEntity gameEntity = obj.GetComponent<GameEntity>();
        //设置数据到entity中
        gameEntity.SetData(nEntitySync.Entity,nEntitySync.Force);

        //2.设置动画状态
        //如果是None,一律不作处理，将维持原来的动画状态
        if (nEntitySync.State != EntityState.NoneState)
        {
            PlayerStateMachine stateMachine = obj.GetComponent<PlayerStateMachine>();
            stateMachine.parameter.owner.entityState = nEntitySync.State;//这里也缓存一份，用于状态机退出一些特殊的状态
            stateMachine.SwitchState(nEntitySync.State);
        }

        //3.安全校验
        //强制回到目标位置
        if (nEntitySync.Force)
        {
            Vector3 target = V3.Of(nEntitySync.Entity.Position) * 0.001f;
            //获取位移向量
            //不能直接使用trasforme的原因是，存在charactercontroller会默认覆盖transform
            UnityMainThreadDispatcher.Instance().Enqueue(() => {
                gameEntity.Move(target);
            });
        }
    }

    /// <summary>
    /// 事件驱动：本机角色位置+动画状态信息同步
    /// </summary>
    /// <param name="nEntitySync"></param>
    public void CtlEntitySync(NEntitySync nEntitySync)
    {
        GameObject obj = GameApp.character?.renderObj;
        if (obj == null) return;

        //设置动画状态
        //如果是None,一律不作处理，将维持原来的动画状态
        if (nEntitySync.State != EntityState.NoneState)
        {
            PlayerStateMachine stateMachine = obj.GetComponent<PlayerStateMachine>();
            stateMachine.parameter.owner.entityState = nEntitySync.State;//这里也缓存一份，用于状态机退出一些特殊的状态
            stateMachine.SwitchState(nEntitySync.State);
        }

        //3.安全校验
        //强制回到目标位置
        if (nEntitySync.Force)
        {
            GameEntity gameEntity = obj.GetComponent<GameEntity>();
            Vector3 target = V3.Of(nEntitySync.Entity.Position) * 0.001f;
            //获取位移向量
            //不能直接使用trasforme的原因是，存在charactercontroller会默认覆盖transform
            UnityMainThreadDispatcher.Instance().Enqueue(() => {
                gameEntity.Move(target);
            });
        }
    }

}

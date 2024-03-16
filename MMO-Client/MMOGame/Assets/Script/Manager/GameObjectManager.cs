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


/// <summary>
/// 游戏对象管理器，管理当前场景中的Gameobject
/// </summary>
public class GameObjectManager:MonoBehaviour
{
    public static GameObjectManager Instance;
    private static Dictionary<int, GameObject> currentGameObjectDict = new Dictionary<int, GameObject>();    //<entityid,gameobject>  entity和gameobject的映射

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
    }
    private void OnDestroy()
    {
        Kaiyun.Event.UnregisterOut("CreateActorObject", this, "CreateActorObject");
        Kaiyun.Event.UnregisterOut("CreateItemObject", this, "CreateItemObject");
        Kaiyun.Event.UnregisterOut("EntityLeave", this, "EntityLeave");
        Kaiyun.Event.UnregisterOut("EntitySync", this, "EntitySync");
    }

    /// <summary>
    /// 事件驱动：向当前场景中创建角色/怪物/npc/陷阱
    /// </summary>
    /// <param name="chr"></param>
    public void CreateActorObject(NetActor nActor)
    {

        //1.先判断当前character是否还保存在dict中
        if (currentGameObjectDict.ContainsKey(nActor.Entity.Id)) return;

        //2.判断entity类型，获取prefab
        UnitDefine unitDefine =  DataManager.Instance.unitDict[nActor.Tid];
        var prefab = Resources.Load<GameObject>(unitDefine.Resource);

        //3.获取坐标和方向
        Vector3 initPosition = V3.Of(nActor.Entity.Position) / 1000; 
        if (initPosition.y == 0)
        {
            initPosition.y = GameTools.CaculateGroundPosition(initPosition,1.5f,7).y;//计算地面坐标
        }

        //4.实例化
        GameObject chrObj = Instantiate(prefab,initPosition,Quaternion.identity,this.transform);//将实例化的角色放到gamemanager下面

        //5.actor 和 gameobj关联
        Actor actor = EntityManager.Instance.GetEntity<Actor>(nActor.Entity.Id);
        actor.renderObj = chrObj;
        actor.StateMachine = chrObj.GetComponent<PlayerStateMachine>();
        actor.StateMachine.parameter.owner = actor;
        //放入dict管理
        currentGameObjectDict[nActor.Entity.Id] = chrObj;

        //6.修改一下obj的设置
        if (nActor.EntityType == EntityType.Character)
        {
            chrObj.name = "Character_" + nActor.Entity.Id;  
        }
        else if(nActor.EntityType == EntityType.Monster)
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

        //7.设置一下同步脚本gameentity
        GameEntity gameEntity = chrObj.GetComponent<GameEntity>();
        bool isMine = (nActor.Entity.Id == GameApp.entityId);
        gameEntity._Start(actor, isMine,initPosition,Vector3.zero);
    
        //8.给我们控制的角色添加一些控制脚本
        if (isMine)
        {
            PlayerMovementController ctl = chrObj.AddComponent<PlayerMovementController>();     //给当前用户控制的角色添加控制脚本
            chrObj.tag = "CtlPlayer";                                                           //打标签
        }

    }

    /// <summary>
    /// 事件驱动：向当前场景中创建物品
    /// </summary>
    /// <param name="netItemEntity"></param>
    public void CreateItemObject(NetItemEntity netItemEntity)
    {
        int entityId = netItemEntity.Entity.Id;

        //1.先判断当前character是否还保存在dict中
        if (currentGameObjectDict.ContainsKey(entityId)) return;

        //2.判断entity类型，获取prefab
        var define = DataManager.Instance.itemDefineDict[netItemEntity.ItemInfo.ItemId];
        var prefab = Resources.Load<GameObject>(define.Model);

        //3.获取坐标和方向
        Vector3 initPosition = V3.Of(netItemEntity.Entity.Position) / 1000;
        if (initPosition.y == 0)
        {
            initPosition.y = GameTools.CaculateGroundPosition(initPosition, 0.5f, 7).y;           //计算地面坐标
        }

        //4.实例化
        GameObject chrObj = Instantiate(prefab, initPosition, Quaternion.identity, this.transform);//将实例化的角色放到gamemanager下面

        //5.actor 和 gameobj关联
        ItemEntity itemEntity = EntityManager.Instance.GetEntity<ItemEntity>(entityId);
        itemEntity.renderObj = chrObj;

        //放入dict管理
        currentGameObjectDict[netItemEntity.Entity.Id] = chrObj;

        //6.修改一下obj的设置
        chrObj.name = "Item_" + entityId;
        chrObj.layer = 8;                   //加入Item图层

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
        currentGameObjectDict.Remove(entityId);
    }

    /// <summary>
    /// 事件驱动：角色位置+动画状态信息同步
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

}

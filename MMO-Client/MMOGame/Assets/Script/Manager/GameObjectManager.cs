using Summer;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Proto;
using GameClient.Entities;
using Unity.VisualScripting;


/// <summary>
/// 游戏对象管理器，管理当前场景中的Gameobject
/// </summary>
public class GameObjectManager:MonoBehaviour
{

    public static GameObjectManager Instance;

    private static Dictionary<int, GameObject> currentGameObjectDict = new Dictionary<int, GameObject>();    //<entityid,gameobject>  entity和gameobject的映射

    private void Start()
    {
        Instance = this;
        Kaiyun.Event.RegisterOut("CreateActorObject", this, "CreateActorObject");
        Kaiyun.Event.RegisterOut("CharacterLeave", this, "CharacterLeave");
        Kaiyun.Event.RegisterOut("EntitySync", this, "EntitySync");
    }

    private void OnDestroy()
    {
        Kaiyun.Event.UnregisterOut("CreateActorObject", this, "CreateActorObject");
        Kaiyun.Event.UnregisterOut("CharacterLeave", this, "CharacterLeave");
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
            initPosition.y = GameTools.CaculateGroundPosition(initPosition,10,7).y;//计算地面坐标
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
        }
        chrObj.layer = 6;//加入actor图层

        //7.设置一下同步脚本
        GameEntity gameEntity = chrObj.GetComponent<GameEntity>();
        gameEntity.entityName = nActor.Name;
        gameEntity.SetData(nActor.Entity);

        //8.根据是否为本机操纵的角色添加控制脚本，开启同步协程
        bool isMine = (nActor.Entity.Id == GameApp.entityId);
        gameEntity.isMine = isMine;
        //如果是本机角色,给它添加控制脚本，并且开启信息同步协程
        if (isMine)
        {
            gameEntity.startSync();//协程，角色开始信息的同步
            chrObj.AddComponent<CameraManager>();                                           //给当前用户控制的视角
            PlayerMovementController ctl = chrObj.AddComponent<PlayerMovementController>();//给当前用户控制的角色添加控制脚本
            GameApp.myCharacter = chrObj;                   //给gameapp当前角色的引用，方便而已

            //打标签
            chrObj.tag = "CtlPlayer";
        }

    }

    /// <summary>
    /// 事件驱动：角色离开场景//可以改造成对象池
    /// </summary>
    /// <param name="entityId"></param>
    public void CharacterLeave(int entityId)
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
        gameEntity.SetData(nEntitySync.Entity);

        //2.设置动画状态,不是常规的动画一律不传
        if(nEntitySync.State != EntityState.None)
        {
            obj.GetComponent<PlayerStateMachine>().SwitchState(TranslateState(nEntitySync.State));
        }

        //3.安全校验
        //强制回到目标位置
        if (nEntitySync.Force)
        {
            Vector3 target = V3.Of(nEntitySync.Entity.Position)*0.001f;
            //获取位移向量
            //不能直接使用trasforme的原因是，存在charactercontroller会默认覆盖transform、
            gameEntity.Move(target);
        }
    }

    public ActorState TranslateState(EntityState state)
    {
        switch (state)
        {
            case EntityState.Idle:
                return ActorState.Idle;
            case EntityState.Walk:
                return ActorState.Walk;
            case EntityState.Run:
                return ActorState.Run;
            case EntityState.Jump:
                return ActorState.Jump;
            case EntityState.Swordflight:
                return ActorState.Swordflight;
            default:
                return ActorState.None;
        }
    }

}

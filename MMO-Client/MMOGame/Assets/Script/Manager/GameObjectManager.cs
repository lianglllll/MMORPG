using Summer;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Proto;
using GameClient.Entities;

/*
 管理游戏场景中的对象，我们使用entityid
 
 
 */

public class GameObjectManager:MonoBehaviour
{

    public static GameObjectManager Instance;

    //<entityid,gameobject>
    private static Dictionary<int, GameObject> currentGameObjectDict = new Dictionary<int, GameObject>();


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

    //向当前场景中创建角色/怪物/npc/陷阱
    public void CreateActorObject(NetActor chr)
    {

        //先判断当前character是否还保存在dict中
        if (currentGameObjectDict.ContainsKey(chr.Entity.Id)) return;


        //entity类型
        UnitDefine unitDefine =  DataManager.Instance.unitDict[chr.Tid];
        var prefab = Resources.Load<GameObject>(unitDefine.Resource);
        Vector3 initPosition = V3.Of(chr.Entity.Position) / 1000; //设置出生点
        if (initPosition.y == 0)
        {
            initPosition.y = GameTools.CaculateGroundPosition(initPosition).y+10;//计算地面坐标,//todo有点问题
        }
        GameObject chrObj = Instantiate(prefab,initPosition,Quaternion.identity,this.transform);//将实例化的角色放到gamemanager下面

        //actor 和 gameobj关联
        Actor actor = EntityManager.Instance.GetEntity<Actor>(chr.Entity.Id);
        actor.renderObj = chrObj;


        //gameobjectName
        if (chr.EntityType == EntityType.Character)
        {
            chrObj.name = "Character_" + chr.Id;  

        }
        else if(chr.EntityType == EntityType.Monster)
        {
            chrObj.name = "Monster_" + chr.Id;
        }
        chrObj.layer = 6;//加入actor图层

        bool isMine = (chr.Entity.Id == GameApp.entityId);                      //判断是否为本机角色
        GameEntity gameEntity = chrObj.GetComponent<GameEntity>();
        gameEntity.entityName = chr.Name;
        gameEntity.isMine = isMine;
        gameEntity.SetData(chr.Entity);

        //如果是本机角色,给它添加控制脚本，并且开启信息同步协程
        if (isMine)
        {
            gameEntity.startSync();//协程，角色开始信息的同步
            chrObj.AddComponent<HeroController>();//给当前用户控制的角色添加控制脚本

            GameApp.myCharacter = chrObj;                   //给gameapp当前角色的引用，方便而已
        }

        //放入dict管理
        currentGameObjectDict[chr.Entity.Id] = chrObj;

    }

    //角色离开场景
    public void CharacterLeave(int entityId)
    {
        if (!currentGameObjectDict.ContainsKey(entityId)) return;
        GameObject obj = currentGameObjectDict[entityId];
        Destroy(obj);
        currentGameObjectDict.Remove(entityId);

    }


    //角色信息同步
    public void EntitySync(NEntitySync nEntitySync)
    {
        int entityId = nEntitySync.Entity.Id;
        GameObject obj = currentGameObjectDict.GetValueOrDefault(entityId, null);
        if (obj == null) return;
        GameEntity gameEntity = obj.GetComponent<GameEntity>();
        gameEntity.SetData(nEntitySync.Entity);//设置数据到entity中

        //通过动画状态机设置动画状态
        if(nEntitySync.State == EntityState.None)
        {
            obj.GetComponent<HeroAnimations>().switchState(gameEntity.lastEntityState);
        }
        else
        {
            obj.GetComponent<HeroAnimations>().switchState(nEntitySync.State);

        }

        //通过事件驱动更新相对应的ui
        //Kaiyun.Event.FireOut("Local")


        //安全校验
        //强制回到目标位置
        if (nEntitySync.Force)
        {
            Vector3 target = V3.Of(nEntitySync.Entity.Position)*0.001f;
            //获取位移向量
            //不能直接使用trasforme的原因是，存在charactercontroller会默认覆盖transform、
            gameEntity.Move(target);
        }
    }
}

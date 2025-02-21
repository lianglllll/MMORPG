using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameClient.Entities;
using System.Collections.Concurrent;
using UnityEngine.SceneManagement;
using GameClient;
using Player;
using Player.Controller;
using HSFramework.Net;
using HS.Protobuf.SceneEntity;
using HS.Protobuf.Scene;
using Unity.VisualScripting;
using Serilog;

/// <summary>
/// 游戏对象管理器，管理当前场景中的Gameobject
/// </summary>
public class GameObjectManager : BaseSystem.Singleton.Singleton<GameObjectManager>
{
    private  ConcurrentDictionary<int, GameObject> currentGameObjectDict    = new();    //<entityid,gameobject>  entity和gameobject的映射
    private  ConcurrentQueue<Actor> preparCrateActorObjQueue                = new();    //创建actorObj的缓冲队列
    private  ConcurrentQueue<ClientItem> preparCrateItemObjQueue              = new();  //创建itemObj的缓冲队列

    private void Update()
    {
        if (SceneManager.GetActiveScene().isLoaded)
        {
            while(preparCrateActorObjQueue.TryDequeue(out var item))
            {
                if (GameApp.SceneId != item.CurSceneId) continue;
                _CreateActorObject(item);
            }

            while(preparCrateItemObjQueue.TryDequeue(out var item))
            {
                if (GameApp.SceneId != item.CurSceneId) continue;
                _CreateItemObject(item);
            }
        }
    }

    public void CreateActorObject(Actor actor)
    {
        Log.Debug("wgiaogiao");

        // 二次验证这玩意到底是不是在我们这个场景的
        if (GameApp.SceneId != actor.CurSceneId) return;

        //添加到缓冲队列当中
        preparCrateActorObjQueue.Enqueue(actor);
    }
    private void _CreateActorObject(Actor actor)
    {
        // 推迟来做
        if (actor == null) return;

        if(currentGameObjectDict.TryGetValue(actor.EntityId,out var item))
        {
            if (item == null || item.IsDestroyed())
            {
                currentGameObjectDict.TryRemove(actor.EntityId, out _);
                actor.RenderObj = null;
            }
            else
            {
                actor.RenderObj = item;
                goto End;
            }
        }

        StartCoroutine(LoadActor(actor));
    End:
        return;
    }
    private IEnumerator LoadActor(Actor actor)
    {
        // 1.判断合法性
        bool isMine = (actor.EntityId == GameApp.entityId);

        // 2.异步加载资源
        UnitDefine unitDefine = actor.UnitDefine;
        GameObject prefab = null;
        yield return Res.LoadAssetAsyncWithTimeout<GameObject>(unitDefine.Resource, (obj) => {
            prefab = obj;
        });
        // 下一帧再执行接下去的
        yield return prefab;

        // 3.获取坐标和方向,实例化obj并初始化,将实例化的角色放到gamemanager下面
        Vector3 initPosition = actor.Position;

        if(actor.NetActorMode != NetActorMode.FlyNormal)
        {
            // 如果不是flyMode的话，y轴是的重力来控制的。
            // 计算地面坐标,调整y轴
            initPosition.y = GameTools.CaculateGroundPosition(initPosition, 1.5f, 7).y;
        }

        GameObject actorObj = Instantiate(prefab, initPosition, Quaternion.identity, transform);
        if (actorObj == null)
        {
            Log.Error("actor 实例化失败");
            yield break;
        }
        // 调整旋转
        actorObj.transform.rotation = Quaternion.Euler(actor.Rotation);

        if (!currentGameObjectDict.TryAdd(actor.EntityId, actorObj))
        {
            Log.Error("actor 加入字典失败");
            Destroy(actorObj);
            yield break;
        }
        //actor 和 gameobj关联
        actor.RenderObj = actorObj; 

        // 4.设置实例的一些信息
        // 加入actor图层
        actorObj.layer = 6;
        // 名字
        if (actor is Character chr)
        {
            actorObj.name = "Character_" + actor.EntityId;

        }else if(actor is Monster mon)
        {
            actorObj.name = "Monster_" + actor.EntityId;
            //如果怪物死亡，就不要显示了
            if (Mathf.Approximately(actor.Hp, 0))
            {
                actorObj.SetActive(false);
                //actor.actorMode = ActorMode.Dead;
            }
        }else if(actor is Npc npc)
        {
            actorObj.name = "Npc_" + actor.EntityId;
        }


        //7.给我们控制的角色添加一些控制脚本
        if (isMine)
        {
            //打标签
            actorObj.tag = "CtlPlayer";
            // ui控制器脚本
            UnitUIController uuc = actorObj.AddComponent<UnitUIController>();
            // 模型层控制脚本
            PlayerModel modelBase = actorObj.transform.Find("Model").gameObject.AddComponent<PlayerModel>();
            // 角色控制脚本
            LocalPlayerController ctl = actorObj.AddComponent<LocalPlayerController>();                           
            // 战斗控制脚本
            PlayerCombatController combat = actorObj.AddComponent<PlayerCombatController>();
            // 同步脚本
            NetworkActor networkActor = actorObj.AddComponent<NetworkActor>();

            modelBase.Init(ctl);
            ctl.Init(actor, networkActor);
            actor.Init(ctl);
            combat.Init(ctl);
            networkActor.Init(ctl);

            //启用第三人称摄像机
            TP_CameraController.Instance.OnStart(actorObj.transform.Find("CameraLookTarget").transform);
        }
        else
        {
            actorObj.tag = "SyncPlayer";                                                             //打标签
            // ui控制器脚本
            UnitUIController uuc = actorObj.AddComponent<UnitUIController>();
            // 模型层控制脚本
            SyncModel modelBase = actorObj.transform.Find("Model").gameObject.AddComponent<SyncModel>();
            // 角色控制脚本
            RemotePlayerController ctl = actorObj.AddComponent<RemotePlayerController>();
            // 同步脚本
            NetworkActor networkActor = actorObj.AddComponent<NetworkActor>();

            modelBase.Init(ctl);
            ctl.Init(actor, networkActor);
            actor.Init(ctl);
            networkActor.Init(ctl);
        }
    }

    public void CreateItemObject(ClientItem clientItem)
    {
        if (GameApp.SceneId != clientItem.CurSceneId) return;

        preparCrateItemObjQueue.Enqueue(clientItem);

    }
    public void _CreateItemObject(ClientItem itemEntity)
    {
        if (itemEntity == null) return;

        //1.先判断当前character是否还保存在dict中
        if(currentGameObjectDict.TryGetValue(itemEntity.EntityId, out var item))
        {
            if(item == null || item.IsDestroyed())
            {
                currentGameObjectDict.TryRemove(itemEntity.EntityId, out _);
                itemEntity.RenderObj = null;
            }
            else
            {
                itemEntity.RenderObj = item;
                return;
            }
        }

        StartCoroutine(LoadItem(itemEntity));
    }
    private IEnumerator LoadItem(ClientItem clientItem)
    {
        var define = DataManager.Instance.itemDefineDict[clientItem.ItemId];

        GameObject prefab = null;
        yield return Res.LoadAssetAsyncWithTimeout<GameObject>(define.Model, (obj) => {
            prefab = obj;
        });
        yield return prefab;

        //下面的操作可能有问题，应该移动到LoadAsset的回调中吧。

        //3.获取坐标和方向
        Vector3 initPosition = clientItem.Position;
        if (initPosition.y == 0)
        {
            initPosition.y = GameTools.CaculateGroundPosition(initPosition, 0.5f, 7).y;           //计算地面坐标
        }

        //4.实例化
        GameObject itemObj = Instantiate(prefab, initPosition, Quaternion.identity, this.transform);//将实例化的角色放到gamemanager下面

        //5.actor 和 gameobj关联
        clientItem.RenderObj = itemObj;

        //6.修改一下obj的设置
        itemObj.name = "Item_" + clientItem.EntityId;
        itemObj.layer = 8;                   //加入Item图层

        //7.放入dict管理
        if(!currentGameObjectDict.TryAdd(clientItem.EntityId, itemObj))
        {
            Destroy(itemObj);
            yield break;
        }

    }

    public void EntityLeave(int entityId)
    {
        if (!currentGameObjectDict.ContainsKey(entityId)) return;
        currentGameObjectDict.TryRemove(entityId, out var obj);
        if(obj != null && !obj.IsDestroyed())
        {
            Destroy(obj);
        }
    }
    public void ActorLeave(Actor actor)
    {
        if (!currentGameObjectDict.ContainsKey(actor.EntityId)) return;
        actor.m_baseController.UnInit();
        currentGameObjectDict.TryRemove(actor.EntityId, out var obj);
        if (obj != null && !obj.IsDestroyed())
        {
            Destroy(obj);
        }
    }



    public void EntitySync(NEntitySync nEntitySync)
    {
        int entityId = nEntitySync.Entity.Id;
        GameObject obj = currentGameObjectDict.GetValueOrDefault(entityId, null);
        if (obj == null) return;
        //设置数据到entity中
        SyncEntityRecive syncEntityRecive = obj.GetComponent<SyncEntityRecive>();
        syncEntityRecive.SyncEntity(nEntitySync);
    }
    public void CtlEntitySync(NEntitySync nEntitySync)
    {
        GameObject obj = GameApp.character?.RenderObj;
        if (obj == null) return;

        //这里通常由服务器来通知一些我们的特殊变化
        //比如：眩晕、击退、击飞、强制位移
        //如果是None,一律不作处理，将维持原来的动画状态
        if (nEntitySync.State != ActorState.Constant)
        {
            var ctrl = GameApp.character.m_baseController;
            // ctrl.ChangeState(nEntitySync.State);
        }
        //设置数据到entity中,这里强制设置
        SyncEntitySend syncEntitySend = obj.GetComponent<SyncEntitySend>();
        syncEntitySend.SyncPosAndRotaion(nEntitySync.Entity);
    }
}

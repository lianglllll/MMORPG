using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Proto;
using Summer.Network;
using System;
using UnityEngine.SceneManagement;
using GameClient.Entities;
using Assets.Script.Entities;
using GameClient;
using Serilog;

public class NetStart : MonoBehaviour
{
    [Header("服务器信息")]
    public string host = "127.0.0.1";
    public int port = 32510;


    private WaitForSeconds waitForSeconds = new WaitForSeconds(1f); //心跳包时间控制
    DateTime lastBeatTime = DateTime.MinValue;                             //上一次发送心跳包的时间


    void Start()
    {
        NetClient.ConnectToServer(host, port);

        //消息分发注册
        MessageRouter.Instance.Subscribe<SpaceCharactersEnterResponse>(_SpaceCharactersEnterResponse);
        MessageRouter.Instance.Subscribe<GameEnterResponse>(_GameEnterResponse);
        MessageRouter.Instance.Subscribe<SpaceEntitySyncResponse>(_SpaceEntitySyncResponse);
        MessageRouter.Instance.Subscribe<HeartBeatResponse>(_HeartBeatResponse);
        MessageRouter.Instance.Subscribe<SpaceCharactersLeaveResponse>(_SpaceCharactersLeaveResponse);
        MessageRouter.Instance.Subscribe<SpellCastResponse>(_SpellCastResponse);

        //心跳包，每秒一次//todo 感觉这里会出问题哎，应该连接成功之后再开启心跳   
        StartCoroutine(SendHeartMessage());

        //注册特殊事件
        Kaiyun.Event.RegisterIn("GameEnter", this, "GameEnter");

    }

    private void OnDestroy()
    {
        //事件注销
        Kaiyun.Event.UnregisterIn("GameEnter",this, "GameEnter");

    }

    //发送心跳包
    IEnumerator SendHeartMessage()
    {
        //优化,防止不断在堆中创建新对象
        HeartBeatRequest beatReq = new HeartBeatRequest();

        while (true)
        {
            yield return waitForSeconds;
            NetClient.Send(beatReq);
            lastBeatTime = DateTime.Now;
            
        }
    }

    //心跳包响应
    public void _HeartBeatResponse(Connection sender, HeartBeatResponse msg)
    {
        //说明服务器和客户端之间连接是通畅的
        TimeSpan gap = DateTime.Now - lastBeatTime;
        int ms = (int)Math.Round(gap.TotalMilliseconds);

        UnityMainThreadDispatcher.Instance().Enqueue(() => {
            //ui处理
            UIManager.Instance.MessagePanel.ShowNetworkDelay(ms);
        });
    }

    //加入游戏请求
    public void GameEnter(int roleId)
    {
        //安全校验
        if (GameApp.myCharacter != null) return;
        //发送请求 
        GameEnterRequest request = new GameEnterRequest();
        request.CharacterId = roleId;
        NetClient.Send(request);
    }

    //加入游戏的响应结果（entity是自己）
    private void _GameEnterResponse(Connection sender, GameEnterResponse msg)
    {
        if (msg.Success)
        {
            NetActor character = msg.Character;

            //加载场景
            GameManager.LoadSpace(character.SpaceId);
            GameApp.entityId = character.Entity.Id;                 //记录本机user的entityid
            EntityManager.Instance.OnEntityEnterScene(msg.Character);
            GameApp.character = EntityManager.Instance.GetEntity<Character>(character.Entity.Id);//主控角色info获取

            //combatui推入
            UnityMainThreadDispatcher.Instance().Enqueue(() =>
            {
                UIManager.Instance.ShowMessage("进入游戏，开始你的冒险");
                GameApp.combatPanelScript = (CombatPanelScript)UIManager.Instance.OpenPanel("CombatPanel");
            });

        }
    }

    //新角色进入场景通知（entity不是自己）
    private void _SpaceCharactersEnterResponse(Connection conn,SpaceCharactersEnterResponse msg)
    {
        Debug.Log("[test1:_SpaceCharactersEnterResponse] = " + msg);
        
        foreach (var actorObj in msg.CharacterList)
        {
            //触发角色进入事件
            EntityManager.Instance.OnEntityEnterScene(actorObj);
        }
    }

    //同步信息接收，去找到这个entity对象，然后更新//todo 用字典<id,gameobject>来取
    private void _SpaceEntitySyncResponse(Connection sender, SpaceEntitySyncResponse msg)
    {
        //注意这个由网络线程中的任务，它是并发的
        //所以对游戏对象的获取和访问都需要在主线程中完成
        EntityManager.Instance.OnEntitySync(msg.EntitySync);
    }

    //由玩家离开地图
    private void _SpaceCharactersLeaveResponse(Connection sender, SpaceCharactersLeaveResponse msg)
    {
        //触发角色离开事件
        EntityManager.Instance.RemoveEntity(msg.EntityId);
    }

    //施法通知
    private void _SpellCastResponse(Connection conn, SpellCastResponse msg)
    {

        Log.Information("_SpellCastResponse:{0}", msg);

        foreach (CastInfo item in msg.List)
        {
            Log.Information("施法信息:{0}", item);
            var caster = EntityManager.Instance.GetEntity<Actor>(item.CasterId);
            var skill = caster.skillManager.GetSkill(item.SkillId);
            if (skill.IsUnitTarget) {
                var target = EntityManager.Instance.GetEntity<Actor>(item.TargetId);
                skill.Use(new SCEntity(target));
            }else if (skill.IsPointTarget)
            {

            }
        }
    }



    //客户端退出调试调用
    private void OnApplicationQuit()
    {
        NetClient.Close();
    }

}

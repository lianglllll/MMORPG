using Assets.Script.Service;
using HSFramework.PoolModule;
using HSFramework.MySingleton;
using GameClient.Entities;
using HSFramework.Audio;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 全局游戏管理器
/// </summary>
public class GameManager : Singleton<GameManager>
{
    public List<GameObject> keepAlive;          //切换场景时不销毁的对象
    protected override void Awake()
    {
        base.Awake();
    }
    void Start()
    {
        //设置初始优先窗口大小
        LocalDataManager.Instance.init();
        var videoSetting = LocalDataManager.Instance.gameSettings.videoSetting;
        Screen.SetResolution(videoSetting.resolutionWidth, videoSetting.resolutionHeight, videoSetting.isFull);

        //初始化服务
        SecurityService.Instance.Init();
        UserService.Instance.Init();
        EntryGameWorldService.Instance.Init();
        SceneHandler.Instance.Init();
        CombatService.Instance.Init();
        ChatService.Instance.Init();
        ItemService.Instance.Init();
        BuffService.Instance.Init();
        UIManager.Instance.Init();
        GlobalAudioManager.Instance.Init(LocalDataManager.Instance.gameSettings.audioSetting);

        //忽略图层之间的碰撞，6号图层layer无视碰撞，可以把角色 npc 怪物，全都放入6号图层
        Physics.IgnoreLayerCollision(6, 6, true);

        //设置游戏对象不被销毁
        foreach (GameObject obj in keepAlive)
        {
            DontDestroyOnLoad(obj);
        }

        // gameObj对象池初始化
        UnityObjectPoolFactory.Instance.LoadFuncDelegate = PoolAssetLoad.LoadAssetByYoo<UnityEngine.Object>;

        // 打开登录面板ui
        UIManager.Instance.OpenPanel("LoginPanel");
        GlobalAudioManager.Instance.PlayBackgroundAudioRandomly();

        // 连接流程
        NetManager.Instance.Init();
    }
    void Update()
    {
        //执行事件系统
        Kaiyun.Event.Tick();
    }
    private void FixedUpdate()
    {
        EntityManager.Instance.OnUpdate(Time.fixedDeltaTime);
    }
}

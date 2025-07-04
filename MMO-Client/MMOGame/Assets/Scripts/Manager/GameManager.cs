using HSFramework.PoolModule;
using HSFramework.MySingleton;
using GameClient.Entities;
using HSFramework.Audio;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// 全局游戏管理器
/// </summary>
public class GameManager : Singleton<GameManager>
{
    public List<GameObject> keepAlive;          // 切换场景时不销毁的对象
    protected override void Awake()
    {
        base.Awake();
    }
    void Start()
    {
        // 设置初始优先窗口大小
        LocalDataManager.Instance.init();
        var videoSetting = LocalDataManager.Instance.gameSettings.videoSetting;
        if(videoSetting.resolutionWidth == 0 || videoSetting.resolutionHeight == 0)
        {
            videoSetting.resolutionWidth = 1920;
            videoSetting.resolutionHeight = 1080;
            videoSetting.isFull = true;
            LocalDataManager.Instance.SaveSettings();
        }
        Screen.SetResolution(videoSetting.resolutionWidth, videoSetting.resolutionHeight, videoSetting.isFull);


        // 初始化服务
        SecurityHandler.Instance.Init();
        UserHandler.Instance.Init();
        EntryGameWorldHandler.Instance.Init();
        SceneHandler.Instance.Init();
        CombatHandler.Instance.Init();
        ChatHandler.Instance.Init();
        TaskHandler.Instance.Init();
        ItemHandler.Instance.Init();
        UIManager.Instance.Init();
        GlobalAudioManager.Instance.Init(LocalDataManager.Instance.gameSettings.audioSetting);

        // 忽略图层之间的碰撞，6号图层layer无视碰撞，可以把角色 npc 怪物，全都放入6号图层
        Physics.IgnoreLayerCollision(6, 6, true);

        // 设置游戏对象不被销毁
        foreach (GameObject obj in keepAlive)
        {
            DontDestroyOnLoad(obj);
        }

        // gameObj对象池初始化
        // UnityObjectPoolFactory.Instance.Init(PoolAssetLoad.LoadAssetByYoo<UnityEngine.Object>);
        UnityObjectPoolFactory.Instance.Init(Res.LoadAssetSync<UnityEngine.Object>);

        // 打开登录面板ui
        UIManager.Instance.OpenPanel("LoginPanel");
        GlobalAudioManager.Instance.PlayBackgroundAudioRandomly();

        // 连接流程
        NetManager.Instance.Init();
    }
    void Update()
    {
        // 执行事件系统
        Kaiyun.Event.Tick();
    }
    private void FixedUpdate()
    {
        EntityManager.Instance.OnUpdate(Time.fixedDeltaTime);
    }

    public void ExitGame()
    {
        Application.Quit();

        // 编辑器内停止播放
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}

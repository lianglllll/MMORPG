using Assets.Script.Service;
using BaseSystem.PoolModule;
using GameClient.Entities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;



/// <summary>
/// 全局游戏管理器
/// </summary>
public class GameManager : MonoBehaviour
{

    public List<GameObject> keepAlive;          //切换场景时不销毁的对象

    public Texture2D mouseIcon;

    void Start()
    {
        Init();
    }
    /// <summary>
    /// 初始化游戏内的一些设置
    /// </summary>
    private void Init()
    {
        //设置鼠标光标icon
        if(mouseIcon != null)
        {
            Cursor.SetCursor(mouseIcon, Vector2.zero, CursorMode.Auto);
        }

        //设置初始优先窗口大小
        Screen.SetResolution(1920, 1080, false);

        //设置游戏对象不被销毁
        foreach (GameObject obj in keepAlive)
        {
            DontDestroyOnLoad(obj);
        }

        //忽略图层之间的碰撞，6号图层layer无视碰撞，可以把角色 npc 怪物，全都放入6号图层
        Physics.IgnoreLayerCollision(6, 6, true);

        //初始化服务
        DataManager.Instance.init();
        UserService.Instance.Init();
        CombatService.Instance.Init();
        ChatService.Instance.Init();
        ItemService.Instance.Init();
        BuffService.Instance.Init();
        UIManager.Instance.Init();

        //gameObj对象池初始化
        UnityObjectPoolFactory.Instance.LoadFuncDelegate = PoolAssetLoad.LoadAssetByYoo<UnityEngine.Object>;

        //打开登录面板ui
        UIManager.Instance.OpenPanel("LoginPanel");
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

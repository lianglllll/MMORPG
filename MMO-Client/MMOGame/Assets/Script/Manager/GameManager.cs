using GameClient.Entities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class GameManager : MonoBehaviour
{

    //切换场景不销毁的对象
    public List<GameObject> keepAlive;

    void Start()
    {

        //设置初始优先窗口大小
        Screen.SetResolution(1920, 1080, false);

        //设置游戏对象不被销毁
        foreach(GameObject obj in keepAlive)
        {
            DontDestroyOnLoad(obj);
        }

        //忽略图层之间的碰撞，6号图层layer无视碰撞，可以把角色 npc 怪物，全都放入6号图层
        Physics.IgnoreLayerCollision(6, 6, true);


        //初始化服务
        DataManager.Instance.init();                //初始化datamanager,加载文件数据
        CombatService.Instance.Init();
        ChatService.Instance.Init();









        UIManager.Instance.OpenPanel("LoginPanel");
    }

    void Update()
    {
        //执行事件系统
        Kaiyun.Event.Tick();
    }


    //加载场景 //todo manager的工作到时候结合一下
    public static void LoadSpace(int spaceId)
    {
        UnityMainThreadDispatcher.Instance().Enqueue(() => {
            //切换到对于的场景
            SpaceDefine space = DataManager.Instance.spaceDict[spaceId];
            SceneManager.LoadScene(space.Resource);
        });
    }

    private void FixedUpdate()
    {
        EntityManager.Instance.OnUpdate(Time.fixedDeltaTime);
    }




}

using Summer;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSceneManager : Singleton<GameSceneManager>
{
    private TP_CameraController cameraController;

    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="camera"></param>
    public void Init(TP_CameraController camera)
    {
        cameraController = camera;
    }

    /// <summary>
    /// 加载场景
    /// </summary>
    /// <param name="spaceId"></param>
    public  void LoadSpace(int spaceId)
    {
        CloseTPCamera();
        //切换到对于的场景
        SpaceDefine space = DataManager.Instance.spaceDict[spaceId];
        SceneManager.LoadScene(space.Resource);
    }

    /// <summary>
    /// 使用当前场景的第三人称摄像机
    /// </summary>
    /// <param name="lookTarget"></param>
    public void UseTPCamera(Transform lookTarget)
    {
        //cameraController.transform.position = new Vector3(0, 0, -0.1f);
        cameraController.OnStart(lookTarget);
    }


    public void CloseTPCamera()
    {
        cameraController.OnStop();
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using YooAsset;

public class SceneLoader : MonoBehaviour
{

    public static float Progress;
    public static SceneHandle handle;
    private static SceneLoader instance;

    public delegate void OnSceneLoaded(Scene scene);
    
    void Start()
    {
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void FixedUpdate()
    {
        if(handle != null)
        {
            Progress = handle.Progress;
        }
    }


    /// <summary>
    /// 异步加载场景
    /// </summary>
    /// <param name="sceneName"></param>
    /// <param name="action"></param>
    public static void LoadSceneAsync(string sceneName, OnSceneLoaded action=null)
    {
        instance.StartCoroutine(RunLoad(sceneName, action));
    }

    static IEnumerator RunLoad(string sceneName, OnSceneLoaded action)
    {
        var package = YooAssets.GetPackage("DefaultPackage");
        string location = sceneName;
        var sceneMode = LoadSceneMode.Single;
        bool suspendLoad = false;
        Progress = 0;
        handle = package.LoadSceneAsync(location, sceneMode, suspendLoad);
        yield return handle;
        yield return WaitForSceneLoaded();
        Debug.Log($"Scene name is {handle.SceneObject.name}");
        action?.Invoke(handle.SceneObject);
        Kaiyun.Event.FireOut("SceneCompleted", handle.SceneObject);
    }

    static IEnumerator WaitForSceneLoaded()
    {
        while(!(handle != null && handle.IsDone && Mathf.Approximately(handle.Progress, 1)))
        {
            yield return null;
        }
        Debug.Log($"Scene Loaded: {handle.SceneObject.name}");
    }

}

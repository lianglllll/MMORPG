using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using YooAsset;

/// <summary>
/// 通过热更的方式加载场景
/// </summary>
public class SceneLoader : MonoBehaviour
{

    public static float Progress;
    public delegate void OnSceneLoaded(Scene scene);
    private static SceneLoader instance;


    void Start()
    {
        instance = this;
        DontDestroyOnLoad(gameObject);
    }



    /// <summary>
    /// 异步加载场景
    /// </summary>
    /// <param name="sceneName"></param>
    /// <param name="action"></param>
    public static void LoadSceneAsync(string sceneName, OnSceneLoaded action = null)
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
        var handle = package.LoadSceneAsync(location, sceneMode, suspendLoad);
        yield return handle;
        yield return WaitForSceneLoaded(handle);
        Debug.Log($"Scene name is {handle.SceneObject.name}");
        action?.Invoke(handle.SceneObject);
        Kaiyun.Event.FireOut("SceneCompleted", handle.SceneObject);
    }

    static IEnumerator WaitForSceneLoaded(SceneHandle han)
    {
        if (han == null) yield break;
        while (!Mathf.Approximately(han.Progress, 1))
        {
            Debug.Log("进度：" + han.Progress);
            Progress = han.Progress;
            yield return null;
        }
        Progress = 1;
        Debug.Log($"Scene Loaded: {han.SceneObject.name}");
    }

}

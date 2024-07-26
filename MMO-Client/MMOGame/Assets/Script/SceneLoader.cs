using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using YooAsset;

/// <summary>
/// 通过热更的方式加载场景
/// </summary>
public class SceneLoader
{

    public static float Progress;

    public delegate void OnSceneLoaded(Scene scene);
    

    /// <summary>
    /// 异步加载场景
    /// </summary>
    /// <param name="sceneName"></param>
    /// <param name="action"></param>
    public static void LoadSceneAsync(string sceneName, Action<Scene> action=null)
    {
        var handle = Res.LoadSceneAsync(sceneName);
        handle.OnProgress = (p)=> Progress = p;
        handle.OnLoaded = action;
        
    }


}

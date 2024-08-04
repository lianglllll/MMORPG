using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using YooAsset;

/// <summary>
/// 场景加载器
/// </summary>
public class SceneLoader
{
    //用于标识场景更新的进度。
    public static float Progress;


    /// <summary>
    /// 异步加载场景
    /// </summary>
    /// <param name="sceneName">场景名称</param>
    /// <param name="action">场景加载完成后的回调</param>
    public static void LoadSceneAsync(string sceneName, Action<Scene> action=null)
    {
        var handle = Res.LoadSceneAsync(sceneName);
        //设置一些场景加载完成后的回调
        handle.OnProgress = (p)=> Progress = p;
        handle.OnLoaded = action;
    }


}

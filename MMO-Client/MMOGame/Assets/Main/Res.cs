using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR;
using YooAsset;


/// <summary>
/// Assets/Res作为资源目录
/// 场景文件放在Assets/Res/Scenes目录
/// </summary>
public class Res
{
    public const string Prefix = "Assets/Res";

    public class ResHandle<T>
    {
        private float _progress;
        public float Progress
        {
            get { return _progress; }
            set
            {
                if (!Mathf.Approximately(_progress, value))
                {
                    _progress = value;
                    OnProgress?.Invoke(value);
                }
            }
        }
        public bool IsDone => Mathf.Approximately(_progress, 1);
        public Action<T> OnLoaded;
        public Action<float> OnProgress;
    }

    /// <summary>
    /// 异步加载场景，场景文件必须放在Assets/Res/Scenes目录
    /// </summary>
    /// <param name="path"></param>
    /// <param name="sceneMode"></param>
    /// <returns></returns>
    public static ResHandle<Scene> LoadSceneAsync(string path, LoadSceneMode sceneMode = LoadSceneMode.Single)
    {
        var handle = new ResHandle<Scene>();
        UnityMainThreadDispatcher.Instance().StartCoroutine(_loadSceneAsync(path, handle));
        return handle;
    }
    private static IEnumerator _loadSceneAsync(string sceneName, ResHandle<Scene> handle)
    {
        string location = $"{Prefix}/Scenes/{sceneName}.unity";
#if (UNITY_EDITOR)
        {
            var assetPath = location;
            handle.Progress = 0;
            LoadSceneParameters parameters = new LoadSceneParameters() { loadSceneMode = LoadSceneMode.Single, localPhysicsMode = LocalPhysicsMode.None };
            AsyncOperation asyncOperation = UnityEditor.SceneManagement.EditorSceneManager.LoadSceneAsyncInPlayMode(assetPath, parameters);
            // 等待加载完成
            while (!asyncOperation.isDone)
            {
                handle.Progress = asyncOperation.progress;
                yield return null;
            }
            Scene loadedScene = SceneManager.GetSceneByPath(assetPath);
            handle.Progress = 1;
            yield return null;
            handle.OnLoaded?.Invoke(loadedScene);
        }
#else
        {
            var package = YooAssets.GetPackage("DefaultPackage");
            var sceneMode = LoadSceneMode.Single;
            bool suspendLoad = false;
            var han = package.LoadSceneAsync(location, sceneMode, suspendLoad);

            while (!Mathf.Approximately(han.Progress, 1))
            {
                if (!han.IsValid)
                {
                    Debug.Log($"加载场景已失效:{location}");
                    yield break; // 退出循环
                }
                Debug.Log("han.IsValid:" + han.IsValid);
                handle.Progress = han.Progress;
                //Debug.Log($"==== 进度：{handle.Progress}");
                yield return null;
            }
            yield return han;
            handle.Progress = 1;
            yield return null;
            handle.OnLoaded?.Invoke(han.SceneObject);
        }
#endif
        yield break;
    }

    /// <summary>
    /// 异步加载游戏对象
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="location"></param>
    /// <param name="priority"></param>
    /// <returns></returns>
    public static IEnumerator LoadAssetAsyncWithTimeout<T>(string location, Action<T> loadedAction, float timeout = 5f) where T : UnityEngine.Object
    {
        T prefab = null;
        var handle = Res.LoadAssetAsync<T>(location);
        handle.OnLoaded = (obj) => {
            prefab = obj;
        };

        //同步等待资源加载完成或超时
        var startTime = Time.time;
        while (prefab == null)
        {
            if (Time.time - startTime >= timeout)
            {
                Debug.LogError($"Loading {location} timed out.");
                yield break;
            }
            yield return null;
        }

        //下一帧在执行load完成的回调方法
        yield return prefab;
        loadedAction?.Invoke(prefab);
        yield break;
    }
    public static ResHandle<T> LoadAssetAsync<T>(string location, uint priority = 0) where T : UnityEngine.Object
    {
        var handle = new ResHandle<T>();
        UnityMainThreadDispatcher.Instance()
            .StartCoroutine(_loadAssetAsync<T>(location, handle, priority));
        return handle;
    }
    private static IEnumerator _loadAssetAsync<T>(string location, ResHandle<T> handle, uint priority = 0, float timeout=5f) where T : UnityEngine.Object
    {
        var path = $"{Prefix}/{location}.prefab";
        T prefab;
        handle.Progress = 0;
#if (UNITY_EDITOR)
        {
            // 加载本地资源
            var assetPath = path;
            prefab = AssetDatabase.LoadAssetAtPath<T>(assetPath);
            yield return prefab;
        }
#else
        {
            // 获取热更资源包
            var package = YooAssets.GetPackage("DefaultPackage");
            AssetHandle han = package.LoadAssetAsync<T>(path, priority);

            // 等待资源加载完成或超时
            var startTime = Time.time;
            while (han.IsValid && !han.IsDone)
            {
                if (Time.time - startTime >= timeout)
                {
                    Debug.LogError($"Res.Loading {location} timed out.");
                    yield break;
                }
                yield return null;
            }

            yield return han;
            prefab = han.AssetObject as T;
        }
#endif
        yield return prefab;
        handle.Progress = 1;
        handle.OnLoaded?.Invoke(prefab);
        yield break;
    }

    /// <summary>
    /// 同步加载游戏对象
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="location"></param>
    /// <param name="priority"></param>
    /// <returns></returns>
    public static T LoadAssetSync<T>(string location, FileType fileType = FileType.Prefab,uint priority = 0)where T : UnityEngine.Object
    {
        if (string.IsNullOrEmpty(location)) return null;

        string suffix = "";
        if(fileType == FileType.Prefab)
        {
            suffix = ".prefab";
        }
        else if(fileType == FileType.Png)
        {
            suffix = ".png";
        }

        var path = $"{Prefix}/{location}{suffix}";
        Debug.Log("当前path=" + path);
#if (UNITY_EDITOR)
        {
            T prefab = AssetDatabase.LoadAssetAtPath<T>(path);
            return prefab;
        }
#else
        {
            var package = YooAssets.GetPackage("DefaultPackage");
            var handle = package.LoadAssetSync<T>(path);
            return handle.AssetObject as T;
        }
#endif

    }


}

public enum FileType
{
    Prefab,Png
}

using System.Collections;
using UnityEngine;
using YooAsset;

public class Main : MonoBehaviour
{
    [Header("UI组件")]
    public MainScenePanel mainScenePanel;
    private bool isStart;

    [Header("配置")]
    public EPlayMode PlayMode = EPlayMode.EditorSimulateMode;
    public string assetServer;

    [Header("临时变量")]
    private string packageVersion;                          //包的版本
    private int readyCount = 0;                             // 准备就绪的包数量（DefaultPackage，RawPackage）


    void Start()
    {
        new GameObject("MainThreadDispatcher")
            .gameObject.AddComponent<UnityMainThreadDispatcher>();

        BetterStreamingAssets.Initialize();

        isStart = false;

        // temp
        Screen.SetResolution(1920, 1080, true);

        StartCoroutine(Init());
    }

    private void Update()
    {
        if(isStart)
        {
            if (Input.anyKeyDown)
            {
                StartGame();
            }
        }
    }

    private IEnumerator Init()
    {
        yield return new WaitForSeconds(0.1f);
        StartCoroutine(DownLoadAssetsByYooAssets());
    }

    private IEnumerator DownLoadAssetsByYooAssets()
    {
        // 1.初始化资源系统
        YooAssets.Initialize();
        var package = YooAssets.CreatePackage("DefaultPackage");
        var rawPackage = YooAssets.CreatePackage("RawPackage");
        YooAssets.SetDefaultPackage(package);
        if (PlayMode == EPlayMode.EditorSimulateMode)
        {
            //编辑器模拟模式
            //等待异步方法完成。
            yield return package.InitializeAsync(new EditorSimulateModeParameters()
            {
                SimulateManifestFilePath = EditorSimulateModeHelper.SimulateBuild(
                    EDefaultBuildPipeline.BuiltinBuildPipeline, "DefaultPackage")
            });
            yield return rawPackage.InitializeAsync(new EditorSimulateModeParameters()
            {
                SimulateManifestFilePath = EditorSimulateModeHelper.SimulateBuild(
                    EDefaultBuildPipeline.RawFileBuildPipeline, "RawPackage")
            });
        }
        else if (PlayMode == EPlayMode.HostPlayMode)
        {
            //联机运行模式
            string defaultHostServer = GetHostServerURL();
            string fallbackHostServer = GetHostServerURL();
            var initParameters = new HostPlayModeParameters();
            initParameters.BuildinQueryServices = new GameQueryServices();      //太空战机DEMO的脚本类，详细见StreamingAssetsHelper
            //initParameters.DecryptionServices = new GameDecryptionServices(); //这里的代码和官网上的代码有差别，官网的代码可能是旧版本的代码会报错已这里的代码为主
            //initParameters.DeliveryQueryServices = new DefaultDeliveryQueryServices();
            initParameters.RemoteServices = new RemoteServices(defaultHostServer, fallbackHostServer);
            var initOperation = package.InitializeAsync(initParameters);
            yield return initOperation;                             //等待异步方法完成
            if (initOperation.Status == EOperationStatus.Succeed)
            {
                //Debug.Log("资源包初始化成功！");
                mainScenePanel.TipsText.text = "资源包初始化成功！";
            }
            else
            {
                //Debug.LogError($"资源包初始化失败：{initOperation.Error}");
                mainScenePanel.TipsText.text = $"资源包初始化失败：{initOperation.Error}";
            }

            initParameters.RemoteServices = new RemoteServices(defaultHostServer, fallbackHostServer);
            var initOperation2 = rawPackage.InitializeAsync(initParameters);
            yield return initOperation2;
            if (initOperation2.Status == EOperationStatus.Succeed)
            {
                //Debug.Log("资源包2初始化成功！");
                mainScenePanel.TipsText.text = "资源包2初始化成功！";
            }
            else
            {
                //Debug.LogError($"资源包2初始化失败：{initOperation.Error}");
                mainScenePanel.TipsText.text = $"资源包2初始化失败：{initOperation.Error}";
            }

        }
        else if (PlayMode == EPlayMode.OfflinePlayMode)
        {
            //单机模式
            var initParameters = new OfflinePlayModeParameters();
            yield return package.InitializeAsync(initParameters);
            //var createParameters = new OfflinePlayModeParameters();
            //createParameters.DecryptionServices = new GameDecryptionServices();
            //initializationOperation = package.InitializeAsync(createParameters);
        }
        else
        {
            //WebGL运行模式
            string defaultHostServer = $"{assetServer}/WebGL";
            string fallbackHostServer = $"{assetServer}/WebGL";
            var initParameters = new WebPlayModeParameters();
            initParameters.BuildinQueryServices = new GameQueryServices(); //太空战机DEMO的脚本类，详细见StreamingAssetsHelper
            initParameters.RemoteServices = new RemoteServices(defaultHostServer, fallbackHostServer);
            //默认资源包
            var initOperation = package.InitializeAsync(initParameters);
            yield return initOperation;
            if (initOperation.Status == EOperationStatus.Succeed)
            {
                Debug.Log("资源包初始化成功！");
            }
            else
            {
                Debug.LogError($"资源包初始化失败：{initOperation.Error}");
            }
            //原生资源包
            var initOperation2 = rawPackage.InitializeAsync(initParameters);
            yield return initOperation2;
            if (initOperation2.Status == EOperationStatus.Succeed)
            {
                Debug.Log("资源包2初始化成功！");
            }
            else
            {
                Debug.LogError($"资源包2初始化失败：{initOperation.Error}");
            }
        }

        yield return HandlePackage(package);
        yield return HandlePackage(rawPackage);

        if (readyCount < 2)
        {
            mainScenePanel.TipsText.text = $"资源加载失败，请重新启动";
        }
        else
        {
            //联机模式加载dll文件
            if (PlayMode == EPlayMode.HostPlayMode)
            {
                yield return LoadDlls.InitDlls();
            }
            ReadyToStart();
        }

    }
    private IEnumerator HandlePackage(ResourcePackage package)
    {
        //2.获取资源版本
        mainScenePanel.TipsText.text = "获取资源版本";
        var operation = package.UpdatePackageVersionAsync();
        yield return operation;

        if (operation.Status != EOperationStatus.Succeed)
        {
            //更新失败
            Debug.LogError(operation.Error);
            yield break;
        }
        string PackageVersion = operation.PackageVersion;

        //3.更新补丁清单
        mainScenePanel.TipsText.text = "更新补丁清单";
        var operation3 = package.UpdatePackageManifestAsync(PackageVersion);
        yield return operation3;

        if (operation3.Status != EOperationStatus.Succeed)
        {
            //更新失败
            mainScenePanel.TipsText.text = "更新失败";
            Debug.LogError(operation3.Error);
            yield break;
        }

        //4.下载补丁包
        yield return Download(package);
    }
    IEnumerator Download(ResourcePackage package)
    {
        int downloadingMaxNum = 10;
        int failedTryAgain = 3;
        var downloader = package.CreateResourceDownloader(downloadingMaxNum, failedTryAgain);

        if (downloader.TotalDownloadCount == 0)
        {
            mainScenePanel.TipsText.text = "没有需要下载的资源";
            Debug.Log("没有需要下载的资源");
            readyCount++;
            yield break;
        }

        //需要下载的文件总数和总大小
        int totalDownloadCount = downloader.TotalDownloadCount;
        long totalDownloadBytes = downloader.TotalDownloadBytes;

        //注册回调方法
        downloader.OnDownloadErrorCallback = OnDownloadErrorFunction;
        downloader.OnDownloadProgressCallback = OnDownloadProgressUpdateFunction;
        downloader.OnDownloadOverCallback = OnDownloadOverFunction;
        downloader.OnStartDownloadFileCallback = OnStartDownloadFileFunction;

        //弹框询问是否更新
        mainScenePanel.OpenSelectUpdatePanel("游戏更新",$"需要下载{FormatBytes(totalDownloadBytes)}是否继续?", () =>
        {
            //开启下载
            downloader.BeginDownload();
        },()=> {
            Application.Quit();
        });

        //等待下载结束
        yield return downloader;

        //检测下载结果
        if (downloader.Status == EOperationStatus.Succeed)
        {
            //下载成功
            Debug.Log("下载资源:成功");
            readyCount++;
        }
        else
        {
            //下载失败
            Debug.Log("下载资源:失败");
            yield break;
        }
    }
    public void OnStartDownloadFileFunction(string fileName, long sizeBytes)
    {
        Debug.Log($"开始下载");
        mainScenePanel.TipsText.text = "开始下载";
        mainScenePanel.UpdatePercentageText.enabled = true;
        mainScenePanel.UpdatePercentageText.text = "";
        mainScenePanel.LoadingSlider.gameObject.SetActive(true);

    }
    public void OnDownloadOverFunction(bool isSucceed)
    {
        Debug.Log($"下载结束");
        mainScenePanel.TipsText.text = "下载结束";
        mainScenePanel.UpdatePercentageText.enabled = false;

    }
    public void OnDownloadProgressUpdateFunction(int totalDownloadCount, int currentDownloadCount, long totalDownloadBytes, long currentDownloadBytes)
    {
        //Debug.LogError($"下载进度:{currentDownloadBytes}/{totalDownloadBytes}");
        mainScenePanel.LoadingSlider.maxValue = totalDownloadBytes;
        mainScenePanel.LoadingSlider.value = currentDownloadBytes;
        mainScenePanel.TipsText.text = $"下载更新 : {FormatBytes(currentDownloadBytes)} / {FormatBytes(totalDownloadBytes)}";
        mainScenePanel.UpdatePercentageText.text = $"{(int)((currentDownloadBytes * 1.0f / totalDownloadBytes ) * 100)}%";
    }
    public void OnDownloadErrorFunction(string fileName, string error)
    {
        Debug.LogError($"下载失败");
        mainScenePanel.TipsText.text = $"下载失败";
    }
    private string FormatBytes(long bytes)
    {
        string[] suffixes = { "B", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };

        int suffixIndex = 0;
        double byteCount = bytes;

        while (byteCount >= 1024 && suffixIndex < suffixes.Length - 1)
        {
            byteCount /= 1024;
            suffixIndex++;
        }

        return $"{byteCount:0.00} {suffixes[suffixIndex]}";
    }

    private void ReadyToStart()
    {
        mainScenePanel.ReadyToStart();
        isStart = true;
    }

    private void StartGame()
    {
        ScenePoster.Instance.LoadSpaceWithPoster("--", "Game", null);
    }

    private string GetHostServerURL()
    {
        var path = Application.platform switch
        {
            RuntimePlatform.Android => $"{assetServer}/Android",
            RuntimePlatform.IPhonePlayer => $"{assetServer}/IPhone",
            RuntimePlatform.WebGLPlayer => $"{assetServer}/WebGL",
            RuntimePlatform.OSXPlayer => $"{assetServer}/MacOS",
            RuntimePlatform.OSXEditor => $"{assetServer}/MacOS",
            _ => $"{assetServer}/PC"
        };
        //Debug.LogWarning("GetHostServerURL：" + path);
        return path;
    }
    private class RemoteServices : IRemoteServices
    {
        // 远端资源地址查询服务类

        private readonly string _defaultHostServer;
        private readonly string _fallbackHostServer;

        public RemoteServices(string defaultHostServer, string fallbackHostServer)
        {
            _defaultHostServer = defaultHostServer;
            _fallbackHostServer = fallbackHostServer;
        }
        string IRemoteServices.GetRemoteMainURL(string fileName)
        {
            return $"{_defaultHostServer}/{fileName}";
        }
        string IRemoteServices.GetRemoteFallbackURL(string fileName)
        {
            return $"{_fallbackHostServer}/{fileName}";
        }
    }
}

public class GameQueryServices : IBuildinQueryServices
{
    public bool Query(string packageName, string fileName, string fileCRC)
    {
        bool exists = BetterStreamingAssets.FileExists($"yoo/{packageName}/{fileName}");
        Debug.Log($"GameQueryServices.Query：{packageName},{fileName},{fileCRC}，结果={exists}");
        return exists;
    }
}




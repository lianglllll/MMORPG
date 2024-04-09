using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using YooAsset;

public class Main : MonoBehaviour
{
    //ui
    public TextMeshProUGUI textPro;
    public Slider slider;

    // 包的版本
    private string packageVersion;

    void Start()
    {
        // 初始化资源系统
        YooAssets.Initialize();

        // 创建默认的资源包
        var package = YooAssets.CreatePackage("DefaultPackage");

        // 设置该资源包为默认的资源包，可以使用YooAssets相关加载接口加载该资源包内容。
        YooAssets.SetDefaultPackage(package);

        // 资源初始化
        StartCoroutine(InitializeYooAsset());

    }

    // Update is called once per frame
    void Update()
    {
        
    }


    private IEnumerator InitializeYooAsset()
    {
        
        string defaultHostServer = "http://175.178.99.14:12345/mmo";
        string fallbackHostServer = "http://175.178.99.14:12345/mmo";
        var initParameters = new HostPlayModeParameters();
        initParameters.BuildinQueryServices = new GameQueryServices();
        initParameters.DecryptionServices = new FileOffsetDecryption();
        initParameters.RemoteServices = new RemoteServices(defaultHostServer, fallbackHostServer);

        var package = YooAssets.GetPackage("DefaultPackage");
        textPro.text = "初始化资源包";
        var initOperation = package.InitializeAsync(initParameters);
        yield return initOperation;

        Debug.Log("initOperation.PackageVersion=" + initOperation.PackageVersion);

        if (initOperation.Status == EOperationStatus.Succeed)
        {
            Debug.Log("资源包初始化成功！");
            textPro.text = "资源包初始化成功";
            StartCoroutine(UpdatePackageVersion());
        }
        else
        {
            Debug.LogError($"资源包初始化失败：{initOperation.Error}");
            textPro.text = "资源包初始化失败";
        }
    }


    // 1.获取资源版本
    private IEnumerator UpdatePackageVersion()
    {
        textPro.text = "检查资源版本";

        var package = YooAssets.GetPackage("DefaultPackage");
        var operation = package.UpdatePackageVersionAsync();
        yield return operation;

        if (operation.Status == EOperationStatus.Succeed)
        {
            //更新成功
            packageVersion = operation.PackageVersion;
            Debug.Log($"Updated package Version : {packageVersion}");

            StartCoroutine(UpdatePackageManifest());
        }
        else
        {
            //更新失败
            Debug.LogError(operation.Error);
        }
    }

    // 2.更新资源
    private IEnumerator UpdatePackageManifest()
    {
        textPro.text = "更新资源清单";

        // 更新成功后自动保存版本号，作为下次初始化的版本。
        // 也可以通过operation.SavePackageVersion()方法保存。
        bool savePackageVersion = true;
        var package = YooAssets.GetPackage("DefaultPackage");
        var operation = package.UpdatePackageManifestAsync(packageVersion, savePackageVersion);
        yield return operation;

        if (operation.Status == EOperationStatus.Succeed)
        {
            //更新成功
            Debug.Log("更新资源:成功");
            StartCoroutine (Download());
        }
        else
        {
            //更新失败
            Debug.LogError(operation.Error);
        }
    }

    // 3.下载资源
    IEnumerator Download()
    {
        int downloadingMaxNum = 10;
        int failedTryAgain = 3;
        var package = YooAssets.GetPackage("DefaultPackage");
        var downloader = package.CreateResourceDownloader(downloadingMaxNum, failedTryAgain);

        //没有需要下载的资源
        if (downloader.TotalDownloadCount == 0)
        {
            Debug.Log("没有需要下载的资源");
            //进入游戏场景
            GameStart();
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

        //开启下载
        downloader.BeginDownload();
        yield return downloader;

        //检测下载结果
        if (downloader.Status == EOperationStatus.Succeed)
        {
            //下载成功
            Debug.Log("下载资源:成功");
            //进入游戏场景
            GameStart();
        }
        else
        {
            //下载失败
            Debug.Log("下载资源:失败");
        }
    }


    private void OnStartDownloadFileFunction(string fileName, long sizeBytes)
    {
        Debug.LogError($"开始下载");
        textPro.text = "开始下载";

    }

    private void OnDownloadOverFunction(bool isSucceed)
    {
        Debug.LogError($"下载结束");
        textPro.text = "下载结束";
    }

    private void OnDownloadProgressUpdateFunction(int totalDownloadCount, int currentDownloadCount, long totalDownloadBytes, long currentDownloadBytes)
    {
        Debug.LogError($"下载进度:{currentDownloadBytes}/{totalDownloadBytes}");
        slider.maxValue = totalDownloadBytes;
        slider.value = currentDownloadBytes;
        textPro.text = $"下载进度:{FormatBytes(currentDownloadBytes)}/{FormatBytes(totalDownloadBytes)}";
    }

    private void OnDownloadErrorFunction(string fileName, string error)
    {
        Debug.LogError($"下载失败");
        textPro.text = "下载失败";
    }

    static string FormatBytes(long bytes)
    {
        string[] suffixes = { "B", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };

        int suffixIndex = 0;
        double byteCount = bytes;

        while(byteCount >= 1024 && suffixIndex < suffixes.Length - 1)
        {
            byteCount /= 1024;
            suffixIndex++;
        }

        return $"{byteCount:0.00} {suffixes[suffixIndex]}";
    }



    /// <summary>
    /// 进入游戏场景
    /// </summary>
    private void GameStart()
    {
        textPro.text = "进入游戏";
        SceneManager.LoadScene("Game");
    }

    /// <summary>
    /// 资源文件偏移加载解密类
    /// </summary>
    private class FileOffsetDecryption : IDecryptionServices
    {
        /// <summary>
        /// 同步方式获取解密的资源包对象
        /// 注意：加载流对象在资源包对象释放的时候会自动释放
        /// </summary>
        AssetBundle IDecryptionServices.LoadAssetBundle(DecryptFileInfo fileInfo, out Stream managedStream)
        {
            managedStream = null;
            return AssetBundle.LoadFromFile(fileInfo.FileLoadPath, fileInfo.ConentCRC, GetFileOffset());
        }

        /// <summary>
        /// 异步方式获取解密的资源包对象
        /// 注意：加载流对象在资源包对象释放的时候会自动释放
        /// </summary>
        AssetBundleCreateRequest IDecryptionServices.LoadAssetBundleAsync(DecryptFileInfo fileInfo, out Stream managedStream)
        {
            managedStream = null;
            return AssetBundle.LoadFromFileAsync(fileInfo.FileLoadPath, fileInfo.ConentCRC, GetFileOffset());
        }

        private static ulong GetFileOffset()
        {
            return 32;
        }

    }


    /// <summary>
    /// 远端资源地址查询服务类
    /// </summary>
    private class RemoteServices : IRemoteServices
    {
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
        Debug.Log($"GameQueryServices.Query：{packageName},{fileName},{fileCRC}");
        return false;
    }
}

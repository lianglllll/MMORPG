using HybridCLR;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using YooAsset;


/// <summary>
/// 负责加载用到的dll文件
/// </summary>
public class LoadDlls
{
    //原生文件
    private static Dictionary<string, byte[]> s_assetDatas = new Dictionary<string, byte[]>();

    private static Assembly _hotUpdateAss;
    public static Assembly HotAssembly;

    //AOT程序集
    private static List<string> AOTMetaAssemblyFiles { get; } = new List<string>()
    {
        "Assembly-CSharp-firstpass.dll",
        "Google.Protobuf.dll",
        "Newtonsoft.Json.dll",
        "Serilog.dll",
        "System.Core.dll",
        "System.dll",
        "Unity.InputSystem.dll",
        "UnityEngine.CoreModule.dll",
        "YooAsset.dll",
        "mscorlib.dll",
    };

    private static void Handle_Completed(AssetHandle obj)
    {
        GameObject go = obj.InstantiateSync();
        Debug.Log($"Prefab name is {go.name}");
    }

    /// <summary>
    /// 加载热更代码，Main中初始化时调用。
    /// </summary>
    /// <returns></returns>
    public static IEnumerator InitDlls()
    {
        var package = YooAssets.GetPackage("RawPackage");

        var assets = new List<string>
        {
            "HotUpdate.dll",
            "Assembly-CSharp.dll"
        }.Concat(AOTMetaAssemblyFiles);

        foreach (var asset in assets)
        {
            //加载原生文件
            Debug.Log($"加载原生文件:{asset}");
            var handle = package.LoadRawFileAsync(asset);
            yield return handle;
            byte[] fileData = handle.GetRawFileData();
            s_assetDatas[asset] = fileData;
            Debug.Log($"== dll:{asset} size:{fileData.Length}");
        }
        RunCode();
    }

    private static void RunCode()
    {
        LoadMetadataForAOTAssemblies();

#if !UNITY_EDITOR
        HotAssembly = Assembly.Load(s_assetDatas["Assembly-CSharp.dll"]);
#else
        HotAssembly = System.AppDomain.CurrentDomain.GetAssemblies()
            .First(a => a.GetName().Name == "Assembly-CSharp");
#endif
        //Type entryType = _hotUpdateAss.GetType("Entry");
        //entryType.GetMethod("Start").Invoke(null, null);

        Run_InstantiateComponentByAsset();
    }

    private static void LoadMetadataForAOTAssemblies()
    {
        /// 注意，补充元数据是给AOT dll补充元数据，而不是给热更新dll补充元数据。
        /// 热更新dll不缺元数据，不需要补充，如果调用LoadMetadataForAOTAssembly会返回错误
        /// 
        HomologousImageMode mode = HomologousImageMode.SuperSet;
        foreach (var aotDllName in AOTMetaAssemblyFiles)
        {
            Debug.Log("加载dll文件：" + aotDllName);
            byte[] dllBytes = ReadBytesFromStreamingAssets(aotDllName);
            // 加载assembly对应的dll，会自动为它hook。一旦aot泛型函数的native函数不存在，用解释器版本代码
            LoadImageErrorCode err = RuntimeApi.LoadMetadataForAOTAssembly(dllBytes, mode);
            Debug.Log($"LoadMetadataForAOTAssembly:{aotDllName}. mode:{mode} ret:{err}");
        }
    }

    private static byte[] ReadBytesFromStreamingAssets(string dllName)
    {
        return s_assetDatas[dllName];
    }

    private static void Run_InstantiateComponentByAsset()
    {
        // 通过实例化assetbundle中的资源，还原资源上的热更新脚本
        //AssetBundle ab = AssetBundle.LoadFromMemory(LoadDll.ReadBytesFromStreamingAssets("prefabs"));
        //GameObject cube = ab.LoadAsset<GameObject>("Cube");
        //GameObject.Instantiate(cube);
        /*var package = YooAssets.GetPackage("DefaultPackage");
        var handle = package.LoadAssetAsync<GameObject>("Cube");
        handle.Completed += Handle_Completed;*/

    }

}

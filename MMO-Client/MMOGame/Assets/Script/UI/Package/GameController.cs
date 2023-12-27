using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



/// <summary>
/// 当前脚本是背包系统测试用
/// </summary>
public class GameController : MonoBehaviour
{
    public static GameController _instance;

    private PackageTable packageTable;//静态数据


    private void Awake()
    {
        _instance = this;
        DontDestroyOnLoad(gameObject);
    }


    private void Start()
    {
        CreateLocalPackageData();
        UIManager.Instance.OpenPanel("PackagePanel");
    }

    /// <summary>
    /// test用的加载数据
    /// </summary>
    public static void CreateLocalPackageData()
    {
        //保存数据
        PackageLocalDate.Instance.items = new List<PackageLocalItem>();
        for (int i = 0; i < 3; i++)
        {
            PackageLocalItem packageLocalItem = new PackageLocalItem()
            {
                uid = Guid.NewGuid().ToString(),
                id = i,
                num = i,
                level = i,
                isNew = i % 2 == 1
            };
            PackageLocalDate.Instance.items.Add(packageLocalItem);
        }
        PackageLocalDate.Instance.SavePackage();


    }

    /// <summary>
    /// 获取静态数据
    /// </summary>
    /// <returns></returns>
    public PackageTable GetPackageTable()
    {
        if(packageTable == null)
        {
            packageTable = Resources.Load<PackageTable>("Prefabs/UI/Package/TableData/PackageTable");
        }
        return packageTable;
    }

    /// <summary>
    /// 获取动态数据
    /// </summary>
    /// <returns></returns>
    public List<PackageLocalItem> GetPackageLocalData()
    {
        return PackageLocalDate.Instance.LoadPackage();
    }

    /// <summary>
    /// 根据id获取单个静态数据
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public PackageTableItem GetPackageItemById(int id)
    {
        List<PackageTableItem> list = GetPackageTable().dataList;
        foreach(var e in list)
        {
            if(e.id == id)
            {
                return e;
            }
        }
        return null;
    }

    /// <summary>
    /// 根据id获取单个动态数据
    /// </summary>
    /// <param name="uid"></param>
    /// <returns></returns>
    public PackageLocalItem GetPackageLocalItemById(string uid)
    {
        List<PackageLocalItem> packageLocalItems = GetPackageLocalData();
        foreach(var e in packageLocalItems)
        {
            if(e.uid == uid)
            {
                return e;
            }
        }
        return null;
    }


    /// <summary>
    /// 对item进行排序
    /// </summary>
    /// <returns></returns>
    public List<PackageLocalItem> GetSortPackageLocalData()
    {
        List<PackageLocalItem> localItems = GetPackageLocalData();
        localItems.Sort(new PackageItemComparer());
        return localItems;
    }

    /// <summary>
    /// 删除多个物品
    /// </summary>
    /// <param name="uids"></param>
    public void DeletePackageItems(List<string> uids)
    {
        foreach(string uid in uids)
        {
            DeletePackageItem(uid, false);
        }
        PackageLocalDate.Instance.SavePackage();
    }

    /// <summary>
    /// 删除单个物品
    /// </summary>
    /// <param name="uid"></param>
    /// <param name="v"></param>
    private void DeletePackageItem(string uid, bool needSave = true)
    {
        PackageLocalItem packageLocalItem = GetPackageLocalItemById(uid);
        if(packageLocalItem == null)
        {
            return;
        }
        PackageLocalDate.Instance.items.Remove(packageLocalItem);
        if (needSave)
        {
            PackageLocalDate.Instance.SavePackage();
        }
    }
}

/// <summary>
/// 比较器
/// </summary>
public class PackageItemComparer : IComparer<PackageLocalItem>
{
    public int Compare(PackageLocalItem a, PackageLocalItem b)
    {
        PackageTableItem x = GameController._instance.GetPackageItemById(a.id);
        PackageTableItem y = GameController._instance.GetPackageItemById(b.id);
        //首先按住star大小排序
        int starComparison = y.star.CompareTo(x.star);
        //if start 相同，则按id从大到小排序
        if(starComparison == 0)
        {
            int idComparison = y.id.CompareTo(x.id);
            if(idComparison == 0)
            {
                //按照等级排序
                return b.level.CompareTo(a.level);
            }
            return idComparison;
        }
        return starComparison;
    }
}

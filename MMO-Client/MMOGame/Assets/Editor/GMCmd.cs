using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GMCmd 
{
    [MenuItem("CMCmd/读取表格(静态)")]
    public static void ReadTable()
    {
        PackageTable packageTable = Resources.Load<PackageTable>("Prefabs/UI/Package/TableData/PackageTable");
        foreach(var e in packageTable.dataList)
        {
            Debug.Log(string.Format("[id]:{0},[name]:{1}", e.id, e.name));
        }
    }

    [MenuItem("CMCmd/创建背包测试数据(动态)")]
    public static void CreateLocalPackageData()
    {
        //保存数据
        PackageLocalDate.Instance.items = new List<PackageLocalItem>();
        for(int i = 0; i < 3; i++)
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


    [MenuItem("CMCmd/读取背包测试数据(静态)")]
    public static void ReadLocalPackageData()
    {
        //读取数据
        List<PackageLocalItem> readItems = PackageLocalDate.Instance.LoadPackage();
        foreach (var e in readItems)
        {
            Debug.Log(e);
        }
    }

    [MenuItem("CMCmd/打开背包面板")]

    public static void OpenPackagePanle()
    {
        UIManager.Instance.OpenPanel("PackagePanel");
    }

}

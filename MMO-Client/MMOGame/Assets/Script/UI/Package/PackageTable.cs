using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



/// <summary>
/// 背包配置表
/// </summary>
[CreateAssetMenu(menuName ="xiaoliang/PackageTable",fileName ="PackageTable")]
public class PackageTable : ScriptableObject
{
    public List<PackageTableItem> dataList = new List<PackageTableItem>();
}


/// <summary>
/// 物品项
/// 这里配置的都是静态数据
/// todo 到时候改为execl配置
/// </summary>
[Serializable]
public class PackageTableItem
{

    public int id;
    public int type;        //武器？食物
    public int star;
    public string name;
    public string descriptions;
    public string detailDescriptions;
    public string imagePath;
}

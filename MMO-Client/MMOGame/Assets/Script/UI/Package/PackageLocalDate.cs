using Summer;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



/// <summary>
/// 将背包数据以json数据存放到本地
/// 使用时从本地读取
/// </summary>
public class PackageLocalDate:Singleton<PackageLocalDate>
{

    public List<PackageLocalItem> items;

    public void SavePackage()
    {
        string inventoryJson = JsonUtility.ToJson(this.items);
        PlayerPrefs.SetString("PackageLocalData", inventoryJson);
        PlayerPrefs.Save();
    }

    public List<PackageLocalItem> LoadPackage()
    {
        if (items != null)
        {
            return items;
        }
        if (PlayerPrefs.HasKey("PackageLocalData"))
        {
            string inventoryJson = PlayerPrefs.GetString("PackageLocalData");
            PackageLocalDate packageLocalDate = JsonUtility.FromJson<PackageLocalDate>(inventoryJson);
            items = packageLocalDate.items;
            return items;
        }
        else
        {
            items = new List<PackageLocalItem>();
            return items;
        }
    }

}

/// <summary>
/// 动态数据
/// </summary>
[System.Serializable]
public class PackageLocalItem
{
    public string uid;
    public int id;
    public int num;
    public int level;
    public bool isNew;

    public override string ToString()
    {
        return string.Format("[id]:{0} [num]:{1}", id, num);
    }

}

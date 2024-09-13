using Summer;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using YooAsset;

public class DataManager : Singleton<DataManager>
{

    //地图场景数据
    public Dictionary<int, SpaceDefine> spaceDict = null;
    //职业|野怪的数据
    public Dictionary<int, UnitDefine> unitDict = null;
    //panel路径映射
    public Dictionary<string, PanelDefine> panelDict = null;
    //技能信息
    public Dictionary<int, SkillDefine> skillDefineDict = null;
    //物品信息
    public Dictionary<int, ItemDefine> itemDefineDict = null;
    //等级经验信息
    public Dictionary<int, LevelDefine> levelDefindeDict = null;
    //buff信息
    public Dictionary<int, BuffDefine> buffDefindeDict = null;



    /// <summary>
    /// 构造函数
    /// </summary>
    public DataManager()
    {
    }

    /// <summary>
    /// 初始化，就是将文件中的数据读入
    /// </summary>
    public void init()
    {
        //获取SpaceDefine场景文件对象，
        spaceDict = Load<SpaceDefine>("SpaceDefine");
        unitDict = Load<UnitDefine>("UnitDefine");
        skillDefineDict = Load<SkillDefine>("SkillDefine");
        itemDefineDict = Load<ItemDefine>("ItemDefine");
        levelDefindeDict = Load<LevelDefine>("LevelDefine");
        buffDefindeDict = Load<BuffDefine>("BuffDefine");
        panelDict = Load2<PanelDefine>("PanelDefine");

    }

    /// <summary>
    /// json -> dictionary
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="path"></param>
    /// <returns></returns>
    private Dictionary<int,T> Load<T>(string path)
    {
        var package = YooAssets.GetPackage("RawPackage");
        var hanle = package.LoadRawFileSync(path);
        string fileText = hanle.GetRawFileText();



        return JsonConvert.DeserializeObject<Dictionary<int, T>>(fileText, settings);
    }
    private Dictionary<string,T> Load2<T>(string path)
    {
        var package = YooAssets.GetPackage("RawPackage");
        var hanle = package.LoadRawFileSync(path);
        string fileText = hanle.GetRawFileText();
        return JsonConvert.DeserializeObject<Dictionary<string, T>>(fileText);
    }

    JsonSerializerSettings settings = new JsonSerializerSettings
    {
        Converters = new JsonConverter[] {
            new FloatArrayConverter(),
            new IntArrayConverter(),
        }
    };


    /// <summary>
    /// 通过spaceid拿spaceDefine信息
    /// </summary>
    /// <param name="spaceId"></param>
    /// <returns></returns>
    public SpaceDefine GetSpaceDefineById(int spaceId)
    {
        return spaceDict.GetValueOrDefault(spaceId,null);
    }


}


//自定义的JsonConverter,用于解决普通的JsonConverter无法转换float[]的问题
public class FloatArrayConverter : JsonConverter<float[]>
{
    public override float[] ReadJson(JsonReader reader, Type objectType, float[] existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        JToken token = JToken.Load(reader);
        if (token.Type == JTokenType.String)
        {
            string[] values = token.ToString().Replace("[", "").Replace("]", "").Split(',');
            float[] result = new float[values.Length];
            for (int i = 0; i < values.Length; i++)
            {
                float.TryParse(values[i], out result[i]);
            }
            return result;
        }
        else
        {
            return null;
        }
    }

    public override void WriteJson(JsonWriter writer, float[] value, JsonSerializer serializer)
    {
        throw new NotImplementedException();
    }
}

public class IntArrayConverter : JsonConverter<int[]>
{
    public override int[] ReadJson(JsonReader reader, Type objectType, int[] existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        JToken token = JToken.Load(reader);
        if (token.Type == JTokenType.String)
        {
            string[] values = token.ToString().Replace("[", "").Replace("]", "").Split(',');
            int[] result = new int[values.Length];
            for (int i = 0; i < values.Length; i++)
            {
                int.TryParse(values[i], out result[i]);
            }
            return result;
        }
        else
        {
            return null;
        }
    }

    public override void WriteJson(JsonWriter writer, int[] value, JsonSerializer serializer)
    {
        throw new NotImplementedException();
    }
}





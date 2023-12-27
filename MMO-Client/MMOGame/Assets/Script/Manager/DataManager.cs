using Summer;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

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


    public DataManager()
    {
        init();
    }

    //初始化，就是将文件中的数据读入
    public void init()
    {
        //获取SpaceDefine场景文件对象，
        spaceDict = Load<SpaceDefine>("Data/SpaceDefine");
        unitDict = Load<UnitDefine>("Data/UnitDefine");
        panelDict = Load2<PanelDefine>("Data/PanelDefine");
        skillDefineDict = Load<SkillDefine>("Data/SkillDefine");
    }


    //json -> dictionary
    private Dictionary<int,T> Load<T>(string path)
    {
        string sceneJson = Resources.Load<TextAsset>(path).text;
        var settings = new JsonSerializerSettings
        {
            Converters = new JsonConverter[] {
                new FloatArrayConverter(),
                new IntArrayConverter(),
            }
        };
        return JsonConvert.DeserializeObject<Dictionary<int, T>>(sceneJson, settings);
    }


    private Dictionary<string,T> Load2<T>(string path)
    {
        string sceneJson = Resources.Load<TextAsset>(path).text;
        return JsonConvert.DeserializeObject<Dictionary<string, T>>(sceneJson);
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





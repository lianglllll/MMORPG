using Summer;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;


public class DataManager : Singleton<DataManager>
{
    private const string _prefix = "Files/Data";

    JsonSerializerSettings settings = new JsonSerializerSettings
    {
        Converters = new JsonConverter[] {
            new FloatArrayConverter(),
            new IntArrayConverter(),
        }
    };

    //地图场景数据
    public Dictionary<int, SpaceDefine> spaceDefineDict = null;
    //职业|野怪的数据
    public Dictionary<int, UnitDefine> unitDefineDict = null;
    //panel路径映射
    public Dictionary<string, PanelDefine> panelDefineDict = null;
    //技能信息
    public Dictionary<int, SkillDefine> skillDefineDict = null;
    //物品信息
    public Dictionary<int, ItemDefine> itemDefineDict = null;
    //等级经验信息
    public Dictionary<int, LevelDefine> levelDefineDict = null;
    //buff信息
    public Dictionary<int, BuffDefine> buffDefineDict = null;
    //对话映射
    private Dictionary<int,DialogDefine> _dialogDefineDict = null;

    public DataManager()
    {
    }

    public void init()
    {
        //todo 就是将文件中的数据读入,真的有必要全部读入吗？懒加载可以吗
        spaceDefineDict = _LoadJsonAnd2Dict<int,SpaceDefine>("SpaceDefine");
        unitDefineDict = _LoadJsonAnd2Dict<int, UnitDefine>("UnitDefine");
        skillDefineDict = _LoadJsonAnd2Dict<int, SkillDefine>("SkillDefine");
        itemDefineDict = _LoadJsonAnd2Dict<int, ItemDefine>("ItemDefine");
        levelDefineDict = _LoadJsonAnd2Dict<int, LevelDefine>("LevelDefine");
        buffDefineDict = _LoadJsonAnd2Dict<int, BuffDefine>("BuffDefine");
        panelDefineDict = _LoadJsonAnd2Dict<string, PanelDefine>("PanelDefine");
        _dialogDefineDict = _LoadJsonAnd2Dict<int, DialogDefine>("DialogDefine");
    }

    private Dictionary<K,T> _LoadJsonAnd2Dict<K,T>(string location)
    {
        string path = _prefix + "/" + location + ".json";
        string fileText =Res.LoadRawJsonFileSync(path);
        return JsonConvert.DeserializeObject<Dictionary<K, T>>(fileText, settings);
    }

    public SpaceDefine GetSpaceDefineById(int spaceId)
    {
        return spaceDefineDict.GetValueOrDefault(spaceId,null);
    }
    public string GetDialogConfigPathByDid(int dId)
    {
        DialogDefine def = _dialogDefineDict.GetValueOrDefault(dId, null);
        if(def == null)
        {
            return null;
        }
        return def.DialogFilePath;
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

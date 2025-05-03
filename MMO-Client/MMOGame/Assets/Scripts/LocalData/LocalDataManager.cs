using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using UnityEngine;
using System.IO;
using HSFramework.Setting;
using HSFramework.MySingleton;
using GameClient.Combat.LocalSkill.Config;

public class LocalDataManager : SingletonNonMono<LocalDataManager>
{
    private const string _prefix = "Files/Data";
    private const string skillConfigPrefix = "Combat/SkillConfig/Skill_";
    private string settingsFilePath;

    JsonSerializerSettings settings = new JsonSerializerSettings
    {
        Converters = new JsonConverter[] {
            new FloatArrayConverter(),
            new IntArrayConverter(),
        }
    };

    public Dictionary<int, SpaceDefine> m_sceneDefineDict = null;
    public Dictionary<int, UnitDefine> m_unitDefineDict = null;
    public Dictionary<string, PanelDefine> m_panelDefineDict = null;
    public Dictionary<int, WeaponSkillArsenalDefine> m_weaponSkillArsenalDefineDict = null;
    public Dictionary<int, SkillDefine> m_skillDefineDict = null;
    public Dictionary<int, LocalSkill_Config_SO> m_localSkillConfigSODict;
    public Dictionary<int, BuffDefine> m_buffDefineDict = null;
    public Dictionary<int, ItemDefine> m_itemDefineDict = null;
    public Dictionary<int, LevelDefine> m_levelDefineDict = null;
    public Dictionary<int,DialogDefine> m_dialogDefineDict = null;
    public Dictionary<int, TaskDefine> m_taskDefineDict = null;
    public GameSettingDatas gameSettings = null;


    public void init()
    {
        //todo 就是将文件中的数据读入,真的有必要全部读入吗？懒加载可以吗
        m_sceneDefineDict = _LoadJsonAnd2Dict<int,SpaceDefine>("SpaceDefine");
        m_unitDefineDict = _LoadJsonAnd2Dict<int, UnitDefine>("UnitDefine");
        m_skillDefineDict = _LoadJsonAnd2Dict<int, SkillDefine>("SkillDefine");
        m_weaponSkillArsenalDefineDict = _LoadJsonAnd2Dict<int, WeaponSkillArsenalDefine>("WeaponSkillArsenalDefine");
        m_localSkillConfigSODict = new();
        m_itemDefineDict = _LoadJsonAnd2Dict<int, ItemDefine>("ItemDefine");
        m_levelDefineDict = _LoadJsonAnd2Dict<int, LevelDefine>("LevelDefine");
        m_buffDefineDict = _LoadJsonAnd2Dict<int, BuffDefine>("BuffDefine");
        m_panelDefineDict = _LoadJsonAnd2Dict<string, PanelDefine>("PanelDefine");
        m_dialogDefineDict = _LoadJsonAnd2Dict<int, DialogDefine>("DialogDefine");
        m_taskDefineDict = _LoadJsonAnd2Dict<int, TaskDefine>("TaskDefine");

        settingsFilePath = Path.Combine(Application.persistentDataPath, "gamesettings.json");
        gameSettings = LoadSettings();
    }
    private Dictionary<K,T> _LoadJsonAnd2Dict<K,T>(string location)
    {
        string path = _prefix + "/" + location + ".json";
        string fileText =Res.LoadRawJsonFileSync(path);
        return JsonConvert.DeserializeObject<Dictionary<K, T>>(fileText, settings);
    }
    public SpaceDefine GetSpaceDefineById(int spaceId)
    {
        return m_sceneDefineDict.GetValueOrDefault(spaceId,null);
    }
    public string GetDialogConfigPathByDid(int dId)
    {
        DialogDefine def = m_dialogDefineDict.GetValueOrDefault(dId, null);
        if(def == null)
        {
            return null;
        }
        return def.DialogFilePath;
    }

    // 设置面板相关
    private GameSettingDatas LoadSettings()
    {
        if (File.Exists(settingsFilePath))
        {
            try
            {
                string json = File.ReadAllText(settingsFilePath);   // 从文件读取 JSON 字符串
                return JsonUtility.FromJson<GameSettingDatas>(json);    // 反序列化为对象
            }
            catch (Exception e)
            {
                Debug.LogError("Failed to load game settings: " + e.Message);
                return new GameSettingDatas(); // 返回默认设置
            }
        }
        else
        {
            Debug.LogWarning("Settings file not found. Using default settings.");
            return new GameSettingDatas(); // 如果文件不存在，返回默认设置
        }
    }
    public void SaveSettings()
    {
        try
        {
            string json = JsonUtility.ToJson(gameSettings, true); // 序列化为 JSON 格式
            File.WriteAllText(settingsFilePath, json); // 写入到文件
            Debug.Log("Game settings saved successfully.");
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to save game settings: " + e.Message);
        }
    }

    // 技能相关
    public LocalSkill_Config_SO GetLocalSkillConfigSOBySkillId(int skillId)
    {
        LocalSkill_Config_SO resultSo;
        if (m_localSkillConfigSODict.TryGetValue(skillId, out resultSo))
        {
            goto End;
        }

        string path = skillConfigPrefix + skillId + ".asset";
        resultSo = Res.LoadAssetSync<LocalSkill_Config_SO>(path);

        // 缓存一下
        m_localSkillConfigSODict.Add(skillId, resultSo);

    End:
        return resultSo;
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

using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

public static class AudioDefineGenTool
{
    private static string AudioDataSOPath = "Assets/Res/Audios/AudioDataSO.asset";
    private static string AudioDefinePath = "Assets/Scripts/BaseSystem/AudioManager/AudioDefine.cs";
    /// <summary>
    /// 将选定的文件夹下的AudioClip生成对应的AudioDefine。
    /// </summary>
    [MenuItem("Assets/生成音频配置和枚举")]
    public static void GenAudioDefine()
    {
        //读取选定的文件夹名字，作为AudioMixerGroup的名字
        if(Selection.objects.Length == 0)
            return;
        Debug.Log(Selection.objects[0].name);
        Dictionary<string, SimpleAudioDataConf> confDic = new Dictionary<string, SimpleAudioDataConf>();
        
        //判断选中的是文件还是文件夹
        if(AssetDatabase.IsValidFolder(AssetDatabase.GetAssetPath(Selection.objects[0])))
        {
            string rootPath = AssetDatabase.GetAssetPath(Selection.objects[0]);
            //获取这个文件夹中的所有音频文件，并以音频文件的父文件夹名字作为MixerGroup的名字
            string[] guids = AssetDatabase.FindAssets("t:AudioClip", new string[] {rootPath});
            foreach (var guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                AudioClip clip = AssetDatabase.LoadAssetAtPath<AudioClip>(path);
                if (clip == null)
                    continue;
                SimpleAudioDataConf conf = new SimpleAudioDataConf();
                if(confDic.ContainsKey(clip.name))
                {
                    Debug.LogError($"音频名字重复了！名字为{clip.name} 路径为{path}");
                    continue;
                }
                //检测名字是否满足变量命名规范
                if (!System.Text.RegularExpressions.Regex.IsMatch(clip.name, @"^[a-zA-Z_][a-zA-Z0-9_]*$"))
                {
                    Debug.LogError($"音频名字不符合变量命名规范！名字为{clip.name} 路径为{path}");
                    continue;
                }
                
                conf.key = clip.name;
                conf.path = path.Substring(path.LastIndexOf("Res/") + "Res/".Length);//.Replace(".wav", "").Replace(".mp3", "").Replace(".ogg", "");
                confDic.Add(conf.key, conf);
                conf.loop = clip.name.EndsWith("_Loop");
                //相对路径
                string relativePath = path.Replace(rootPath + "/", "");
                relativePath = relativePath.Substring(0, relativePath.LastIndexOf("/"));
                conf.mixerName = relativePath;
            }

            AudioDataSO dataSo = AssetDatabase.LoadAssetAtPath<AudioDataSO>(AudioDataSOPath);
            bool isNewCrate = false;
            if (dataSo == null)
            {
                dataSo = ScriptableObject.CreateInstance(typeof(AudioDataSO)) as AudioDataSO;
                isNewCrate = true;
            }
            dataSo.Conf = new AudioDataConfig();
            dataSo.Conf.ConfList = new List<SimpleAudioDataConf>();
            foreach (var conf in confDic)
            {
                dataSo.Conf.ConfList.Add(conf.Value);
            }
            if (isNewCrate)
            {
                AssetDatabase.CreateAsset(dataSo, AudioDataSOPath);
            }
            else
            {
                EditorUtility.SetDirty(dataSo);
            }
            
            //生成定义的内容
            StringBuilder contentStringBuilder = new StringBuilder();
            contentStringBuilder.Append("//该文件是自动生成的，请勿手动修改！\n");
            contentStringBuilder.Append("public static class AudioDefine\n");
            contentStringBuilder.Append("{\n");
            foreach (KeyValuePair<string,SimpleAudioDataConf> conf in confDic)
            {
                string name = conf.Key;
                contentStringBuilder.Append("    public static string " + name + " = \"" + name + "\";\n");
            }
            contentStringBuilder.Append("}\n");
            File.WriteAllText(AudioDefinePath, contentStringBuilder.ToString());
            AssetDatabase.Refresh();
        }
    }
    
    //Project
}

using BaseSystem.Tool.Singleton;
using GameClient.UI.Dialog;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace GameClient.UI.Dialog
{
    public class RawDialogStepConfig
    {
        public int ID;
        public string Name;
        public bool IsPlayer;
        public int Flag;
        public int Pos;
        public string Content;
        public int NextIndex;
        public string IconPath;
        public string SoundPath;
        public string StartEvent;
        public string EndEvent;
        public string Remarks;
    }

    public class DialogConfigImporter : SingletonNonMono<DialogConfigImporter>
    {
        private Dictionary<string, Type> allEventTypeDict;

        public void Init()
        {
            //获取所有事件类型
            allEventTypeDict = new Dictionary<string, Type>();
            Type interfaceType = typeof(IDialogEvent);
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (Assembly assembly in assemblies)
            {
                Type[] types = assembly.GetTypes().Where(t => interfaceType.IsAssignableFrom(t) && !t.IsAbstract).ToArray();
                foreach (Type type in types)
                {
                    allEventTypeDict.Add(type.Name, type);
                }
            }
        }

        public DialogConfig GetDialogConfigByDid(int dId)
        {
            string path = DataManager.Instance.GetDialogConfigPathByDid(dId);
            string fileText = Res.LoadRawJsonFileSync(path);
            // 反序列化为字典
            var dictionary = JsonConvert.DeserializeObject<Dictionary<string, RawDialogStepConfig>>(fileText);

            // 转换为列表
            List<RawDialogStepConfig> rawDialogStepConfigs = new List<RawDialogStepConfig>(dictionary.Values);

            //todo 对象池里面拿
            DialogConfig dialogConfig = new DialogConfig();
            dialogConfig.Init(rawDialogStepConfigs);
            return dialogConfig;
        }
        public Type GetEventTyepByName(string name)
        {
            return allEventTypeDict.GetValueOrDefault(name, null);
        }
    }

}


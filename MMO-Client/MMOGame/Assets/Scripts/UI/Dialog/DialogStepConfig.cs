using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameClient.UI.Dialog
{
    //对话中的一句话
    public class DialogStepConfig
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
        public string Remarks;
        public List<IDialogEvent> OnStartEventList;
        public List<IDialogEvent> OnEndEventList;

        public DialogStepConfig() { }

        public void Init(RawDialogStepConfig config)
        {
            this.ID = config.ID;
            this.Name = config.Name;
            this.IsPlayer = config.IsPlayer;
            this.Flag = config.Flag;
            this.Pos = config.Pos;
            this.Content = config.Content;
            this.NextIndex = config.NextIndex;
            this.IconPath = config.IconPath;
            this.SoundPath = config.SoundPath;
            this.Remarks = config.Remarks;
            //事件转换
            OnStartEventList = ConverDialogEvent(config.StartEvent);
            OnEndEventList = ConverDialogEvent(config.EndEvent);

        }

        private List<IDialogEvent> ConverDialogEvent(string eventString)
        {
            List<IDialogEvent> events = new List<IDialogEvent>();

            if (string.IsNullOrEmpty(eventString)) return events;
            string[] eventStrings = eventString.Split('\n');
            foreach(string str1 in eventStrings)
            {
                string[] eventStringSplit = str1.Split(":");
                if(eventStringSplit.Length !=  2)
                {
                    Debug.LogError($"对话事件格式不符合:{str1}");
                    return events;
                }
                string typeString = eventStringSplit[0];
                string valueString = eventStringSplit[1];
                Type type = DialogConfigImporter.Instance.GetEventTyepByName($"Dialog{typeString}Event");
                if (type == null)
                {
                    Debug.LogError($"不存在该事件类型:{typeString}");
                    return events;
                }
                IDialogEvent eventObj = (IDialogEvent)Activator.CreateInstance(type);
                eventObj.ConverString(valueString);
                events.Add(eventObj);
            }
            return events;
        }

    }
}


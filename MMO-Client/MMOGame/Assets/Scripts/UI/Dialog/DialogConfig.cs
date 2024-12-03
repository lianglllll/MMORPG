using System.Collections.Generic;

namespace GameClient.UI.Dialog
{
    //某个角色的对话配置
    public class DialogConfig
    {
        private List<DialogStepConfig> m_stepList = new List<DialogStepConfig>();

        public void Init(List<RawDialogStepConfig> configList)
        {
            //configList有可能不是按照id顺序的
            m_stepList.Capacity = configList.Count;
            foreach (var step in configList) {
                //todo 对象池获取
                DialogStepConfig dialogStepConfig = new DialogStepConfig();
                dialogStepConfig.Init(step);
                m_stepList.Insert(dialogStepConfig.ID,dialogStepConfig);
            }
        }

        public void UnInit()
        {
            m_stepList.Clear();
        }

        public int DialogStepCount()
        {
            if(m_stepList.Count <= 0)
            {
                return 0;
            }
            return m_stepList.Count;
        }

        public DialogStepConfig GetDialogStepConfigByIndex(int index) {
            if(m_stepList == null ||  m_stepList.Count == 0 || index >= m_stepList.Count)
            {
                return null;
            }
            return m_stepList[index];
        }
    }
}

using GameClient;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 管理技能栏
/// </summary>
public class AbilityManager : MonoBehaviour
{
    public List<AbilitySlotScript> bars = new List<AbilitySlotScript>();

    public void Init()
    {
        if (GameApp.character == null) return;

        // todo 按键从配置中获取
        // LocalDataManager.Instance.gameSettings;
        string tipKeys = "QEFZXC";
        var skillList = GameApp.character.m_skillManager.GetActiveSkills();
        for (int i = 0; i < bars.Count; i++)
        {
            var bar = bars[i];
            if (i < skillList.Count)
            {
                bar.gameObject.SetActive(true);
                var skill = skillList[i];
                bar.SetAbilityBarInfo(skill,tipKeys[i].ToString());
            }
        }
    }

}

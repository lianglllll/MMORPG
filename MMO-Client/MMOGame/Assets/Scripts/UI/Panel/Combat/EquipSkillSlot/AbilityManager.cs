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
        string tipKeys = "12345678";

        var fixedSkillDict = GameApp.character.m_skillManager.GetFixedSkills();
        for (int i = 0; i < bars.Count; i++)
        {
            var bar = bars[i];
            if(fixedSkillDict.TryGetValue(i + 1, out var skill))
            {
                bar.gameObject.SetActive(true);
                bar.SetAbilityBarInfo(skill, tipKeys[i].ToString());
            }
            else
            {
                bar.gameObject.SetActive(false);
            }
        }
    }

}

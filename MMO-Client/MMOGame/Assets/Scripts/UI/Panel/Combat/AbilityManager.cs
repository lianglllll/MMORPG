using GameClient;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 管理技能栏
/// </summary>
public class AbilityManager : MonoBehaviour
{
    public List<AbilityBarScript> bars = new List<AbilityBarScript>();

    public void Init()
    {
        if (GameApp.character == null) return;
        string tipKeys = "QEFZXC";
        var skillList = GameApp.character.m_skillManager.GetActiveSkills();
        for (int i = 0; i < bars.Count; i++)
        {
            var bar = bars[i];
            if (i < skillList.Count)
            {
                var skill = skillList[i];
                bar.SetAbilityBar(skill,tipKeys[i].ToString());
            }
            else
            {
                bar.SetAbilityBar(null);
            }
        }
    }

}

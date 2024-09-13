using GameClient;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 管理技能栏
/// </summary>
public class AbilityGroupScript : MonoBehaviour
{
    public List<AbilityBarScript> bars = new List<AbilityBarScript>();

    private void Start()
    {
        
    }

    private void OnDestroy()
    {
        
    }

    /// <summary>
    /// 初始化技能格子
    /// </summary>
    public void Init()
    {
        if (GameApp.character == null) return;
        string tipKeys = "QEFZXC";
        var skillList = GameApp.character.skillManager.GetActiveSkills();
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

    //添加一个

    //删除一个
}

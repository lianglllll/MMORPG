using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//管理技能栏

public class AbilityGroupScript : MonoBehaviour
{
    public List<AbilityBarScript> bars = new List<AbilityBarScript>();

    void Start()
    {
        Init();
    }


    //初始化技能栏
    public void Init()
    {
        if (GameApp.character == null) return;//todo 是否可以使用事件系统来进行延迟？
        var skillList = GameApp.character.skillManager.Skills;
        for (int i = 0; i < bars.Count; i++)
        {
            var bar = bars[i];
            if (i < skillList.Count)
            {
                var skill = skillList[i];
                bar.InitSetInfo(skill);
            }
            else
            {
                bar.InitSetInfo(null);
            }
        }
    }
}

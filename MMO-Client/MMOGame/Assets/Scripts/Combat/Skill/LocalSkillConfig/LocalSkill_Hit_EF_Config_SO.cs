using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalSkill_Hit_EF_Config_SO : ScriptableObject
{
    //成功命中时，产生的物体:粒子
    public Skill_SpawnObj successfullyHitSpawnObj;

    //失败命中时，产生的物体:粒子
    public Skill_SpawnObj failedHitSpawnObj;

    //通用音效，无论对方是否格挡
    public AudioClip UniversalAudioClip;
}

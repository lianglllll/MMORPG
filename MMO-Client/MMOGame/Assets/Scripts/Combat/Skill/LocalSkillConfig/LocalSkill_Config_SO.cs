using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameClient.Combat.LocalSkill.Config
{
    [CreateAssetMenu(menuName ="Custom/SkillConfig/SkillSO")]
    public class LocalSkill_Config_SO : ScriptableObject
    {
        public int SkillId;
        public LocalSkill_ReleaseData releaseData;  // 技能释放的时候涉及到的东西。
        public LocalSkill_AttackData[] attackData;  // 攻击时候的数据
    }

    [Serializable]
    public class LocalSkill_ReleaseData
    {
        public LocalSkill_SpawnObject SpawnObj;             // 一个技能释放，它可能不是一个攻击技能，也有可能是加buff、生成一个物体。
        public AudioClip ReleaseAudioClip;
        public bool CanRotate;
    }
    [Serializable]
    public class LocalSkill_AttackData
    {
        public LocalSkill_SpawnObject SpawnObj;             // 生成的物体：如刀光剑影
        public AudioClip attackAudioClip;                   // 技能音效：武器破空的声音
        public LocalSkill_Hit_EF_Config_SO hitEFConfigSo;   // 命中失败与否的粒子效果
        public LocalSkill_HitData hitData;                  // 命中成功后的打击数据
        public float ScreenImpulseValue;                    // 屏幕振动
        public float ChromaticAberrationValue;              // 色差效果
        public float FreezeFrameTime;                       // 卡肉效果
        public float FreezeGameTime;                        // 时停
    }

    [Serializable]
    public class LocalSkill_HitData
    {
        public float DamageValue;           // 伤害数值
        public float HardTime;              // 硬直时机
        public bool Down;                   // 是否击倒
        public Vector3 RepelVelocity;       // 击飞、击退的程度
        public float RepelTime;             // 击飞、击退的过渡时间
        public bool IsForcedDisplacement;   // 击飞、击退是否强制位移
        public bool IsBreak;                // 是否破防
    }

    [Serializable]
    public class LocalSkill_SpawnObject
    {
        //生成的预制体
        public GameObject prefab;
        //位置
        public Vector3 pos;
        //旋转
        public Vector3 rotation;
        //缩放
        public Vector3 Scale = Vector3.one;
        //音效
        public AudioClip audioClip;
        //延迟时间
        public float delayTime;
    }

}


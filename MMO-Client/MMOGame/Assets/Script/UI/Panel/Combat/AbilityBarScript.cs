using GameClient.Combat;
using Serilog;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Proto;



/// <summary>
/// 当skill使用的时候就来这里触发倒计时，然后就不管了
/// 让当前这个脚本自己管自己，skill只提供了触发和倒计时
/// 这里设置一个标记为flag来标记是否进入倒计时
/// </summary>
public class AbilityBarScript : MonoBehaviour
{
    public Sprite icon;
    public string aName;
    public string desc;
    public float coldDown;
    public float maxColdDown;

    private Image iconImag;                 //技能图标
    private Image coldDownImag;             //冷却图层
    private Text coldDownTimeText;          //冷却数字文字

    private Skill _skill;
    private bool isUpdate;

    private void Awake()
    {
        iconImag = transform.Find("Icon").GetComponent<Image>();
        coldDownImag = transform.Find("ColdDown").GetComponent<Image>();
        coldDownTimeText = transform.Find("ColdDownTime").GetComponent<Text>();
        //添加一个btn
        var btn = gameObject.AddComponent<Button>();
        btn.onClick.AddListener(OnClick);
    }

    private void Start()
    {
        isUpdate = false;
        coldDownImag.enabled = false;
        Kaiyun.Event.RegisterIn("SkillEnterColdDown", this, "_SkillEnterColdDown");
    }

    private void OnDestroy()
    {
        Kaiyun.Event.UnregisterIn("SkillEnterColdDown", this, "_SkillEnterColdDown");
    }


    private void Update()
    {
        if (isUpdate)
        {
            //Debug.Log($"isupdate={isUpdate},skill={_skill.ColddownTime}");
            UpdateAbilityBar();
        }
    }

    /// <summary>
    /// 设置技能格中的信息
    /// </summary>
    /// <param name="skillInfo"></param>
    public void SetAbilityBar(Skill skillInfo)
    {
        if(skillInfo != null)
        {
            _skill = skillInfo;
            icon = Resources.Load<Sprite>(skillInfo.Define.Icon);
            aName = skillInfo.Define.Name;
            maxColdDown = skillInfo.Define.CD;
            coldDown = skillInfo.ColddownTime;
            desc = skillInfo.Define.Description;
        }
        else
        {
            _skill = null;
            icon = null;
            aName = "";
            maxColdDown = 1;
            coldDown = 0;
            desc = "";
        }

        //初始化一下技能格子的ui
        iconImag.enabled = icon != null;                        //是否显示技能图标  
        iconImag.sprite = icon;
        isUpdate = false;

    }

    /// <summary>
    /// 技能进入冷却事件回调
    /// </summary>
    public void _SkillEnterColdDown()
    {

        if (_skill != null && _skill.ColddownTime != 0)
        {
            isUpdate = true;
        }

    }

    /// <summary>
    /// 更新技能格的信息，主要是冷却的信息
    /// 根据skill来进行更新ui
    /// </summary>
    void UpdateAbilityBar()
    {

        coldDown = _skill.ColddownTime;                         //设置当前的冷却时间
        if (coldDown <= 0)
        {
            isUpdate = false;
            coldDownImag.enabled = false;
            coldDownTimeText.enabled = false;
            return;
        }
        coldDownImag.enabled = true;
        coldDownTimeText.enabled = true;                        //是否显示冷却text
        coldDownImag.fillAmount = coldDown / maxColdDown;       //冷却图层,遮罩
        //冷却数字                                                        
        if (coldDown >= 1.0f)
        {
            coldDownTimeText.text = coldDown.ToString("F0");//大于等于1秒不显示小数
        }
        else
        {
            coldDownTimeText.text = coldDown.ToString("F1");//小于1秒显示小数
        }

    }

    /// <summary>
    /// 技能格被点击事件
    /// </summary>
    private void OnClick()
    {
        if (_skill == null) return;
        if (_skill.IsUnitTarget && GameApp.target == null)
        {
            Log.Information("当前没有选中目标");
            return;
        }
        if (isUpdate)
        {
            Log.Information("技能冷却中");
            return;
        }

        GameApp.Spell(_skill);
    }

}

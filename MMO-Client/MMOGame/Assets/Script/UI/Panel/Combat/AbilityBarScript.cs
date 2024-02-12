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

    private Image iconImag;
    private Image coldDownImag;          //冷却图层
    private Text coldDownTimeText;

    public Skill skill;


    void Start()
    {
        iconImag = transform.Find("Icon").GetComponent<Image>();
        coldDownImag = transform.Find("ColdDown").GetComponent<Image>();
        coldDownTimeText = transform.Find("ColdDownTime").GetComponent<Text>();

        //添加一个btn
        var btn = gameObject.AddComponent<Button>();
        btn.onClick.AddListener(OnClick);

    }

    void Update()//todo
    {
        UpdateInfo();
    }

    public void InitSetInfo(Skill skillInfo)
    {
        skill = skillInfo;
        if(skillInfo == null)
        {
            icon = null;
            aName = "";
            maxColdDown = 1;
            coldDown = 0;
            desc = "";
            return;
        }
        icon = Resources.Load<Sprite>(skillInfo.Define.Icon);
        aName = skillInfo.Define.Name;
        maxColdDown = skillInfo.Define.CD;
        coldDown = skillInfo.ColdDown;
        desc = skillInfo.Define.Description;
    }

    //todo 委托+3个状态函数即可使用事件来进行触发，而不是一直update
    void UpdateInfo()
    {
        //todo 进入倒计时的时候才update
        if (skill != null)
        {
            coldDown = skill.ColdDown;
        }
        else
        {
            coldDown = 0;
        }


        iconImag.enabled = icon != null;                        //是否显示技能图标  
        iconImag.sprite = icon;
        coldDownImag.fillAmount = coldDown / maxColdDown;       //冷却图层，为0就相当于不显示了

        coldDownTimeText.enabled = coldDown > 0;                //是否显示冷却text
        if (coldDownTimeText)
        {
            if (coldDown >= 1.0f)
            {
                coldDownTimeText.text = coldDown.ToString("F0");//大于等于1秒不显示小数
            }
            else
            {
                coldDownTimeText.text = coldDown.ToString("F1");//小于1秒显示小数
            }
        }
    }




    private void OnClick()
    {
        if (skill == null) return;
        Log.Information("技能点击:{0}",skill.Define.Name);
        GameApp.Spell(skill);
    }

}

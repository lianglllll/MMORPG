using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameClient.Entities;
using TMPro;

public class EliteScript : MonoBehaviour
{
    private Image Healthbar;
    private Image Manabar;
    private Text Name;
    private Text Level;
    private TextMeshProUGUI HpText;
    private TextMeshProUGUI MpText;
    private BuffGroupScript buffGroup;

    private Actor actor;                //UI显示要的目标

    private void Awake()
    {
        Healthbar = transform.Find("Bars/Healthbar").GetComponent<Image>();
        Manabar = transform.Find("Bars/Manabar").GetComponent<Image>();
        Name = transform.Find("Name").GetComponent<Text>();
        Level = transform.Find("Level/Text").GetComponent<Text>();
        HpText = transform.Find("Bars/HpTxext").GetComponent<TextMeshProUGUI>();
        MpText = transform.Find("Bars/MpText").GetComponent<TextMeshProUGUI>();
        buffGroup = transform.Find("BuffGroup").GetComponent<BuffGroupScript>();
    }

    /// <summary>
    /// 设置状态栏归属者
    /// </summary>
    /// <param name="actor"></param>
    public void SetOwner(Actor actor)
    {   
        if (actor == null) return;
        if (this.actor == actor) return;
        this.actor = actor;
        buffGroup.SetOwner(actor);
        RefreshUI();
    }

    /// <summary>
    /// 刷新UI
    /// </summary>
    public void RefreshUI()
    {
        if (actor == null) return;
        Level.text = actor.info.Level + "";
        float hp = actor.info.Hp / actor.info.HpMax;
        float mp = actor.info.Mp / actor.info.MpMax;
        Healthbar.fillAmount = hp;
        Manabar.fillAmount = mp;
        Name.text = actor.info.Name;
        HpText.text = (int)actor.info.Hp + "/" + actor.info.HpMax;
        MpText.text = (int)actor.info.Mp + "/" + actor.info.MpMax;
        
    }

}
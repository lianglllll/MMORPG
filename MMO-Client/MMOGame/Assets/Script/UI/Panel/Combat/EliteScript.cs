using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameClient.Entities;


public class EliteScript : MonoBehaviour
{
    private Image Healthbar;
    private Image Manabar;
    private Text Name;
    private Text Level;

    private Actor actor;                //UI显示要的目标

    private void Awake()
    {
        Healthbar = transform.Find("Bars/Healthbar").GetComponent<Image>();
        Manabar = transform.Find("Bars/Manabar").GetComponent<Image>();
        Name = transform.Find("Name").GetComponent<Text>();
        Level = transform.Find("Level/Text").GetComponent<Text>();
    }

    /// <summary>
    /// 设置状态栏归属者
    /// </summary>
    /// <param name="actor"></param>
    public void SetOwner(Actor actor)
    {   
        this.actor = actor;
        if (this.actor == null) return;
        RefreshUI();
    }

    /// <summary>
    /// 刷新UI
    /// </summary>
    public void RefreshUI()
    {
        if (actor == null) return;
        Level.text = actor.info.Level + "";
        float hp = actor.info.Hp / actor.define.HPMax;
        float mp = actor.info.Mp / actor.define.MPMax;
        Healthbar.fillAmount = hp;
        Manabar.fillAmount = mp;
        Name.text = actor.info.Name;
    }

}

using Assets.Script.Entities;
using GameClient.Combat;
using Proto;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CombatPanelScript : BasePanel
{
    
    private EliteScript myElite;
    private EliteScript targetElite;
    private Slider intonateSlider;

    private void Awake()
    {
        myElite = transform.Find("MyElite").GetComponent<EliteScript>();
        targetElite = transform.Find("TargetElite").GetComponent<EliteScript>();
        intonateSlider = transform.Find("IntonateSlider/Slider").GetComponent<Slider>();
    }

    private void Start()
    {

    }

    private void OnDestroy()
    {
        
    }


    private void Update()
    {
        //敌我头像UI
        myElite.gameObject.SetActive(GameApp.character != null);
        myElite.SetOwner(GameApp.character);
        targetElite.gameObject.SetActive(GameApp.target != null);
        targetElite.SetOwner(GameApp.target);


        //技能释放进度条UI
        var sk = GameApp.CurrSkill;
        if(sk != null && sk.State == Stage.Intonate)
        {
            intonateSlider.gameObject.SetActive(true);
            intonateSlider.value = sk.IntonateProgress;
        }
        else
        {
            intonateSlider.gameObject.SetActive(false);
        }


    }




}

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
    private Button reviveBtn;
    private Image DeathBox;
    private Button KnapsackBtn;
    public ChatBoxScript chatBoxScript;

    protected override void Awake()
    {
        base.Awake();
        myElite = transform.Find("MyElite").GetComponent<EliteScript>();
        targetElite = transform.Find("TargetElite").GetComponent<EliteScript>();
        intonateSlider = transform.Find("IntonateSlider/Slider").GetComponent<Slider>();
        DeathBox = transform.Find("DeathBox").GetComponent<Image>();
        reviveBtn = transform.Find("DeathBox/ReviveBtn").GetComponent<Button>();
        KnapsackBtn = transform.Find("KnapsackBtn").GetComponent<Button>();
        chatBoxScript = transform.Find("ChatBox").GetComponent<ChatBoxScript>();
    }

    private void Start()
    {
        DeathBox.gameObject.SetActive(false);
        reviveBtn.onClick.AddListener(OnReviveBtn);
        KnapsackBtn.onClick.AddListener(OnKnaspackBtn);
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
        if(sk != null && sk.Stage == SkillStage.Intonate && sk.Define.IntonateTime > 0.1f)
        {
            intonateSlider.gameObject.SetActive(true);
            intonateSlider.value = sk.IntonateProgress;
        }
        else
        {
            intonateSlider.gameObject.SetActive(false);
        }


    }

    public void ShowDeathBox()
    {
        DeathBox.gameObject.SetActive(true);
    }

    private void OnReviveBtn()
    {
        DeathBox.gameObject.SetActive(false);
        GameApp.character._Revive();
    }

    private void OnKnaspackBtn()
    {
        UIManager.Instance.OpenPanel("KnapsackPanel");
    }

}

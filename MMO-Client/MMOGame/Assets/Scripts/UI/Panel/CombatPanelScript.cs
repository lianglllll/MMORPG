using Assets.Script.Entities;
using GameClient;
using GameClient.Combat;
using GameClient.Entities;
using Proto;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CombatPanelScript : BasePanel
{
    
    private EliteScript myElite;                            //自己的角色的状态栏
    private EliteScript targetElite;                        //目标的角色的状态栏
    private Slider intonateSlider;
    private Button KnapsackBtn;
    public ChatBoxScript chatBoxScript;
    private ExpBoxScript expBoxScript;
    private AbilityGroupScript abilityGroupScript;

    protected override void Awake()
    {
        base.Awake();
        myElite = transform.Find("MyElite").GetComponent<EliteScript>();
        targetElite = transform.Find("TargetElite").GetComponent<EliteScript>();
        intonateSlider = transform.Find("IntonateSlider/Slider").GetComponent<Slider>();
        KnapsackBtn = transform.Find("KnapsackBtn").GetComponent<Button>();
        chatBoxScript = transform.Find("ChatBox").GetComponent<ChatBoxScript>();
        expBoxScript = transform.Find("ExpBox").GetComponent<ExpBoxScript>();
        abilityGroupScript = transform.Find("AbilityGroup").GetComponent<AbilityGroupScript>();
    }

    protected override void Start()
    {
        Init();

        //监听事件
        Kaiyun.Event.RegisterOut("SelectTarget", this, "_SelectTarget");
        Kaiyun.Event.RegisterOut("TargetDeath", this, "_CancelSelectTarget");
        Kaiyun.Event.RegisterOut("CancelSelectTarget", this, "_CancelSelectTarget");
        Kaiyun.Event.RegisterOut("SpecificAcotrPropertyUpdate", this, "EliteRefreshUI"); 
        Kaiyun.Event.RegisterOut("ExpChange", this, "ExpBoxREfreshUI");
        Kaiyun.Event.RegisterOut("CloseKnaspack", this, "CloseKnaspackCallback");


    }

    private void OnDestroy()
    {
        Kaiyun.Event.UnregisterOut("SelectTarget", this, "_SelectTarget");
        Kaiyun.Event.UnregisterOut("TargetDeath", this, "_CancelSelectTarget");
        Kaiyun.Event.UnregisterOut("CancelSelectTarget", this, "_CancelSelectTarget");
        Kaiyun.Event.UnregisterOut("SpecificAcotrPropertyUpdate", this, "EliteRefreshUI");
        Kaiyun.Event.UnregisterOut("ExpChange", this, "ExpBoxREfreshUI");
        Kaiyun.Event.UnregisterOut("CloseKnaspack", this, "CloseKnaspackCallback");

    }

    private void Update()
    {

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


        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if(UIManager.Instance.GetPanelByName("KnapsackPanel") == null)
            {
                OnKnaspackBtn();
            }
        }



    }

    /// <summary>
    /// 初始化
    /// </summary>
    private void Init()
    {
        //ui
        KnapsackBtn.onClick.AddListener(OnKnaspackBtn);
        targetElite.gameObject.SetActive(false);
        myElite.SetOwner(GameApp.character);                    //设置一下我们主角的状态栏
        expBoxScript.Init(GameApp.character);                   //初始化经验条
        abilityGroupScript.Init();

    }

    /// <summary>
    /// 死亡面板
    /// </summary>
    public void ShowDeathBox()
    {
        //设置数据
        string deathText = "你已死亡，是否选择最近复活点复活？";     //提示语句
        UIManager.Instance.MessagePanel.ShowSelectionPanel("复活", deathText, () =>
        {
            GameApp._Revive();
        });
    }

    /// <summary>
    /// 背包
    /// </summary>
    public void OnKnaspackBtn()
    {
        UIManager.Instance.OpenPanel("KnapsackPanel");

    }

    /// <summary>
    /// 关闭背包的回调
    /// </summary>
    public void CloseKnaspackCallback()
    {

    }

    /// <summary>
    /// 选中目标的事件回调
    /// </summary>
    public void _SelectTarget()
    {
        var actor = GameApp.target;
        if (actor == null) return;
        //设置状态栏UI
        targetElite.gameObject.SetActive(true);
        targetElite.SetOwner(actor);
    }

    /// <summary>
    /// 取消选中目标
    /// </summary>
    public void _CancelSelectTarget() {
        targetElite.SetOwner(null);
        targetElite.gameObject.SetActive(false);
    }

    /// <summary>
    /// SpecificAcotrPropertyUpdate事件回调,刷新EliteUI
    /// </summary>
    public void EliteRefreshUI(Actor actor)
    {
        if(actor == GameApp.character)
        {
            myElite.RefreshUI();
        }else if(actor == GameApp.target)
        {
            targetElite.RefreshUI();
        }
    }

    /// <summary>
    /// 刷新经验条
    /// </summary>
    public void ExpBoxREfreshUI()
    {
        expBoxScript.RefrashUI();
    }







}

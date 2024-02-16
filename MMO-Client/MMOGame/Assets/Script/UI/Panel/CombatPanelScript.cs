using Assets.Script.Entities;
using GameClient.Combat;
using GameClient.Entities;
using Proto;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CombatPanelScript : BasePanel
{
    
    private EliteScript myElite;                            //自己的角色的状态栏
    private EliteScript targetElite;                        //目标的角色的状态栏
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
        //监听事件
        Kaiyun.Event.RegisterOut("SelectTarget", this, "_SelectTarget");
        Kaiyun.Event.RegisterOut("TargetDeath", this, "_CancelSelectTarget");
        Kaiyun.Event.RegisterOut("CancelSelectTarget", this, "_CancelSelectTarget");
        Kaiyun.Event.RegisterOut("SpecificAcotrPropertyUpdate", this, "EliteRefreshUI");
        Init();

    }

    private void OnDestroy()
    {
        Kaiyun.Event.UnregisterOut("SelectTarget", this, "_SelectTarget");
        Kaiyun.Event.UnregisterOut("TargetDeath", this, "_CancelSelectTarget");
        Kaiyun.Event.UnregisterOut("CancelSelectTarget", this, "_CancelSelectTarget");
        Kaiyun.Event.UnregisterOut("SpecificAcotrPropertyUpdate", this, "EliteRefreshUI");
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


    }

    /// <summary>
    /// 初始化
    /// </summary>
    private void Init()
    {
        //ui
        reviveBtn.onClick.AddListener(OnReviveBtn);
        KnapsackBtn.onClick.AddListener(OnKnaspackBtn);
        DeathBox.gameObject.SetActive(false);
        targetElite.gameObject.SetActive(false);

        //设置一下我们主角的状态栏
        myElite.SetOwner(GameApp.character);
    }

    /// <summary>
    /// 死亡面板
    /// </summary>
    public void ShowDeathBox()
    {
        DeathBox.gameObject.SetActive(true);
    }

    /// <summary>
    /// 复活
    /// </summary>
    private void OnReviveBtn()
    {
        DeathBox.gameObject.SetActive(false);
        GameApp.character._Revive();
    }

    /// <summary>
    /// 背包
    /// </summary>
    private void OnKnaspackBtn()
    {
        UIManager.Instance.OpenPanel("KnapsackPanel");
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
    /// 刷新EliteUI
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

}

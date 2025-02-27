using GameClient;
using GameClient.Combat;
using GameClient.Entities;
using System;
using UnityEngine;
using UnityEngine.UI;

public class CombatPanelScript : BasePanel
{
    private bool m_isShowRelatedCombatUI;
    private float m_notCombatOperationTime = 0;
    private float m_maxNotCombatOperationTime = 10;

    private int m_haveOtherPanelFromThisOpen;

    private EliteScript m_elite;                            
    private AbilityManager m_ablityManager;
    private ExpBoxScript m_expBox;

    private Slider intonateSlider;
    private Button KnapsackBtn;
    private ChatBoxScript chatBoxScript;

    private bool m_isShowTopAndRightUI;
    private GameObject TopPart;
    private GameObject RightPart;


    protected override void Awake()
    {
        m_elite = transform.Find("Elite").GetComponent<EliteScript>();
        intonateSlider = transform.Find("IntonateSlider/Slider").GetComponent<Slider>();
        KnapsackBtn = transform.Find("KnapsackBtn").GetComponent<Button>();
        chatBoxScript = transform.Find("ChatBox").GetComponent<ChatBoxScript>();
        m_expBox = transform.Find("ExpBox").GetComponent<ExpBoxScript>();
        m_ablityManager = transform.Find("AbilityManager").GetComponent<AbilityManager>();

        TopPart = transform.Find("TopPart").gameObject;
        RightPart = transform.Find("RightPart").gameObject;
    }
    protected override void Start()
    {
        //监听事件
        Kaiyun.Event.RegisterOut("SelectTarget", this, "_SelectTarget");
        Kaiyun.Event.RegisterOut("TargetDeath", this, "_CancelSelectTarget");
        Kaiyun.Event.RegisterOut("CancelSelectTarget", this, "_CancelSelectTarget");
        Kaiyun.Event.RegisterOut("SpecificAcotrPropertyUpdate", this, "EliteRefreshUI"); 
        Kaiyun.Event.RegisterOut("ExpChange", this, "ExpBoxREfreshUI");
        Kaiyun.Event.RegisterOut("CloseKnaspack", this, "CloseKnaspackCallback");

        Kaiyun.Event.RegisterIn("CloseSettingPanel", this, "HanleCloseSettingPanelEvent");
        Init();
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
        // 鼠标显示隐藏
        if (GameInputManager.Instance.SustainLeftAlt)
        {
            SetMouseShowAndHide(true);
        }
        else if(GameInputManager.Instance.GameInputMode == GameInputMode.Game)
        {
            SetMouseShowAndHide(false);
        }

        // top right part的显隐
        if (GameInputManager.Instance.GI_ESC && !m_isShowTopAndRightUI)
        {
            ShowTopAndRightUI();
        }
        if (GameInputManager.Instance.UI_ESC && m_isShowTopAndRightUI && m_haveOtherPanelFromThisOpen == 0)
        {
            HideTopAndRightUI();
        }

        // 战斗相关的ui,脱战自动隐藏
        if (GameInputManager.Instance.LAttack)
        {
            if(!m_isShowRelatedCombatUI)
            {
                ShowRelatedCombatUI();
            }
            m_notCombatOperationTime = 0;
        }
        else
        {
            m_notCombatOperationTime += Time.deltaTime;
            if(m_notCombatOperationTime > m_maxNotCombatOperationTime)
            {
                if (m_isShowRelatedCombatUI)
                {
                    HideRelatedCombatUI();
                }
            }
        }

        //技能释放进度条UI
        //var sk = GameApp.CurrSkill;
        //if(sk != null && sk.Stage == SkillStage.Intonate && sk.Define.IntonateTime > 0.1f)
        //{
        //    intonateSlider.gameObject.SetActive(true);
        //    intonateSlider.value = sk.IntonateProgress;
        //}
        //else
        //{
        //    intonateSlider.gameObject.SetActive(false);
        //}

        //if (Input.GetKeyDown(KeyCode.Tab))
        //{
        //    if(UIManager.Instance.GetOpeningPanelByName("KnapsackPanel") == null)
        //    {
        //        OnKnaspackBtn();
        //    }
        //}
    }

    private void Init()
    {
        m_elite.Init(GameApp.character);                    //设置一下我们主角的状态栏
        m_expBox.Init(GameApp.character);                   //初始化经验条
        m_ablityManager.Init();

        KnapsackBtn.onClick.AddListener(OnKnaspackBtn);

        HideRelatedCombatUI();
        HideTopAndRightUI();

        m_haveOtherPanelFromThisOpen = 0;

        GameInputManager.Instance.SwitchGameInputMode(GameInputMode.Game);
    }
    private void ShowRelatedCombatUI()
    {
        m_isShowRelatedCombatUI = true;
        m_elite.gameObject.SetActive(true);
        m_ablityManager.gameObject.SetActive(true);
    }
    private void HideRelatedCombatUI()
    {
        m_isShowRelatedCombatUI = false;
        m_elite.gameObject.SetActive(false);
        m_ablityManager.gameObject.SetActive(false);
    }

    private void ShowTopAndRightUI()
    {
        m_isShowTopAndRightUI = true;
        if (m_isShowRelatedCombatUI)
        {
            HideRelatedCombatUI();
        }
        GameInputManager.Instance.SwitchGameInputMode(GameInputMode.UI);
        TopPart.SetActive(true);
        RightPart.SetActive(true);

        SetMouseShowAndHide(true);
    }
    public void HideTopAndRightUI()
    {
        m_isShowTopAndRightUI = false;
        GameInputManager.Instance.SwitchGameInputMode(GameInputMode.Game);
        TopPart.SetActive(false);
        RightPart.SetActive(false);

        SetMouseShowAndHide(false);
    }
    private void SetMouseShowAndHide(bool active)
    {
        if (active)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    // 设置面板
    public void ShowSettingPanel()
    {
        UIManager.Instance.OpenPanel("SettingPanel");
        m_haveOtherPanelFromThisOpen++;
    }
    public void HanleCloseSettingPanelEvent()
    {
        m_haveOtherPanelFromThisOpen--;
    }
    public void ShowMailPanel()
    {
        throw new NotImplementedException();
    }
    public void ShowBackpackPanel()
    {
        throw new NotImplementedException();
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
        //targetElite.gameObject.SetActive(true);
        //targetElite.SetOwner(actor);
    }

    /// <summary>
    /// 取消选中目标
    /// </summary>
    public void _CancelSelectTarget() {
        //targetElite.SetOwner(null);
        //targetElite.gameObject.SetActive(false);
    }

    /// <summary>
    /// SpecificAcotrPropertyUpdate事件回调,刷新EliteUI
    /// </summary>
    public void EliteRefreshUI(Actor actor)
    {
        if(actor == GameApp.character)
        {
            m_elite.RefreshUI();
        }else if(actor == GameApp.target)
        {
            //targetElite.RefreshUI();
        }
    }

    /// <summary>
    /// 刷新经验条
    /// </summary>
    public void ExpBoxREfreshUI()
    {
        m_expBox.RefrashUI();
    }


}

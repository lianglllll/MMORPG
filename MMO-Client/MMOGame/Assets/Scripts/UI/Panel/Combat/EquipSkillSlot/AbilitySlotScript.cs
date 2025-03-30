using GameClient.Combat;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using GameClient;


/// <summary>
/// 当skill使用的时候就来这里触发倒计时，然后就不管了
/// 让当前这个脚本自己管自己，skill只提供了触发和倒计时
/// 这里设置一个标记为flag来标记是否进入倒计时
/// </summary>
public class AbilitySlotScript : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Skill m_skill;
    public Sprite icon;

    public string aName;
    public string desc;
    public float coldDown;
    public float maxColdDown;
    private bool isUpdate;

    private Image iconImag;                     // 技能图标
    private Image coldDownImag;                 // 冷却图层
    private TextMeshProUGUI coldDownTimeText;   // 冷却数字文字
    private TextMeshProUGUI TipsKeyText;        // 按键提示

    private void Awake()
    {
        iconImag = transform.Find("Icon").GetComponent<Image>();
        coldDownImag = transform.Find("ColdDown").GetComponent<Image>();
        coldDownTimeText = transform.Find("ColdDownTime").GetComponent<TextMeshProUGUI>();
        TipsKeyText = transform.Find("TipsKey/TipsKeyText").GetComponent<TextMeshProUGUI>();
    }
    private void Start()
    {
        isUpdate = false;
        coldDownImag.enabled = false;
        coldDownTimeText.enabled = false;

        Kaiyun.Event.RegisterIn("SkillEnterColdDown", this, "SkillEnterColdDownEvent");
    }
    private void OnDestroy()
    {
        Kaiyun.Event.UnRegisterIn("SkillEnterColdDown", this, "SkillEnterColdDownEvent");
    }
    private void Update()
    {
        if (isUpdate)
        {
            UpdateAbilityBar();
        }
    }

    public void SetAbilityBarInfo(Skill skillInfo,string tipKey = "")
    {
        // 数据初始化
        if(skillInfo != null)
        {
            m_skill = skillInfo;
            icon = Res.LoadAssetSync<Sprite>(skillInfo.Define.Icon);
            aName = skillInfo.Define.Name;
            maxColdDown = skillInfo.Define.CD;
            coldDown = skillInfo.ColddownTime;
            desc = skillInfo.Define.Description;
            TipsKeyText.text = tipKey;
        }
        else
        {
            m_skill = null;
            icon = null;
            aName = "";
            maxColdDown = 1;
            coldDown = 0;
            desc = "";
        }

        // 显示
        if(icon == null)
        {
            iconImag.enabled = false;
        }
        else
        {
            iconImag.sprite = icon;
            iconImag.enabled = true;
        }

        isUpdate = false;
    }
    public void SkillEnterColdDownEvent()
    {
        if (m_skill != null && m_skill.ColddownTime != 0)
        {
            isUpdate = true;
        }
    }
    void UpdateAbilityBar()
    {
        // 更新技能格的信息，主要是冷却的信息,根据skill来进行更新ui

        coldDown = m_skill.ColddownTime;                         //设置当前的冷却时间
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


    private void OnClick()
    {
        if (m_skill == null) return;
        if (m_skill.IsUnitTarget && GameApp.target == null)
        {
            UIManager.Instance.MessagePanel.ShowBottonMsg("当前没有选中目标");
            return;
        }
        if (isUpdate)
        {
            UIManager.Instance.MessagePanel.ShowBottonMsg("技能冷却中");
            return;
        }

        CombatHandler.Instance.SendSpellCastReq(m_skill,GameApp.target);
        Kaiyun.Event.FireIn("EnterCombatEvent");

    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        var content = "<color=#ffffff>技能信息信息为空</color>";
        if(m_skill != null)
        {
            content = m_skill.GetDescText();
        }
        ToolTip.Instance.Show(content);
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        ToolTip.Instance?.Hide();
    }
}

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;
using HSFramework.Audio;


public enum CombatMenuOptionType
{
    Settings, ExitPanel, ExitGame
}

public class CombatMenuSelectOption : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler,IPointerClickHandler
{
    private Image Bg;
    private Text text;

    private bool isClicked;
    private bool isEntering;
    private bool isExiting;
    private float hoverA = 0.5f;
    private float hoverDuration = 0.5f;
    public CombatMenuOptionType type;

    private CombatPanelScript combatPanel;
    private CombatPanelScript CombatPanelScript
    {
        get
        {
            if (combatPanel == null)
            {
                combatPanel = UIManager.Instance.GetOpeningPanelByName("CombatPanel") as CombatPanelScript;
            }
            return combatPanel;
        }
    }
    private void Awake()
    {
        Bg = transform.Find("Bg").GetComponent<Image>();
        text = transform.Find("Text").GetComponent<Text>();
    }
    private void OnEnable()
    {
        isClicked = false;
        isEntering = false;
        isExiting = false;
        Color color = Bg.color;
        color.a = 0;
        Bg.color = color;
    }
    private void OnDisable()
    {
        // Kill all tweens associated with this object to ensure no animations are left running
        DOTween.Kill(Bg);
        isEntering = false;  // Reset entering state
        isExiting = false;   // Reset exiting state
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (isClicked) return;
        GlobalAudioManager.Instance.PlayUIAudio(UIAudioClipType.ButtonClick);

        switch(type)
        {
            case CombatMenuOptionType.Settings:
                OnSettingOption();
                break;
            case CombatMenuOptionType.ExitPanel:
                OnExitPanelOption();
                break;
            case CombatMenuOptionType.ExitGame:
                OnExitGameOption();
                break;
        }
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (isClicked) return;

        //bg透明度从0->0.5

        // 如果正在淡入或者当前已经完全不透明，不触发淡入动画
        if (isEntering || Bg.color.a == hoverA)
            return;

        // 如果正在进行淡出动画，立即停止它，开始淡入
        if (isExiting)
        {
            DOTween.Kill(Bg);  // 停止当前的淡出动画
            isExiting = false;
        }

        isEntering = true;  // 标记淡入动画开始

        Bg.DOFade(hoverA, hoverDuration).OnComplete(() =>
        {
            isEntering = false;  // 动画完成，重置标记
        });

    }
    public void OnPointerExit(PointerEventData eventData)
    {
        if (isClicked) return;

        //bg透明度变回0

        // 如果正在淡出或者当前已经完全透明，不触发淡出动画
        if (isExiting || Bg.color.a == 0)
            return;

        // 如果正在进行淡入动画，立即停止它，开始淡出
        if (isEntering)
        {
            DOTween.Kill(Bg);  // 停止当前的淡入动画
            isEntering = false;
        }

        isExiting = true;  // 标记淡入动画开始

        Bg.DOFade(0, hoverDuration).OnComplete(() =>
        {
            isExiting = false;  // 动画完成，重置标记
        });
    }


    private void OnSettingOption()
    {
        CombatPanelScript.ShowSettingPanel();
    }

    private void OnExitPanelOption()
    {
        CombatPanelScript.HideTopAndRightUI();
    }

    private void OnExitGameOption()
    {

    }
}

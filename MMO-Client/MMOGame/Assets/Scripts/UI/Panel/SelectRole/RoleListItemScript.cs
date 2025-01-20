using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;
using HS.Protobuf.SceneEntity;
using HS.Protobuf.Game;

public class RoleListItemScript : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler
{
    private SimpleCharacterInfoNode characterInfo;                         //角色信息的网络对象
    private SelectRolePanelScript selectRolePanelScript;
    private UnitDefine define;

    private Button btn;
    private Image Bg;
    private Image hover;
    private CanvasGroup click;
    private CanvasGroup select;
    private Text nameText;
    private Text levelText;
    private Text jobText;
    private float hoverDuration = 0.2f;
    private float clickDuration = 0.1f;
    private float selectDuration = 1f;
    private bool IsSelected ;
    private Tween breathingTween;
    private float minAlpha = 0.1f;      // 最小透明度
    private float maxAlpha = 1f;        // 最大透明度

    public string ChrId
    {
        get
        {
            return characterInfo.CId;
        }
    }
    public string Name => characterInfo.ChrName;
    public string Vocation => define.Name;
    public int Level => characterInfo.Level;

    private void Awake()
    {
        btn = GetComponent<Button>();
        btn.onClick.AddListener(onBtn);

        Bg = transform.Find("Bg").GetComponent<Image>();

        hover = transform.Find("Hover").GetComponent<Image>();
        click = transform.Find("Click").GetComponent<CanvasGroup>();
        select = transform.Find("Select").GetComponent<CanvasGroup>();

        nameText = transform.Find("RoleInfo/RoleName").GetComponent<Text>();
        levelText = transform.Find("RoleInfo/RoleLevel").GetComponent<Text>();
        jobText = transform.Find("RoleInfo/RoleJob").GetComponent<Text>();

    }

    private void Start()
    {
        hover.enabled = false;
        var hoverColor = hover.color;
        hoverColor.a = 0f;
        hover.color = hoverColor;

        select.gameObject.SetActive(false);
        select.alpha = 0f;
        click.gameObject.SetActive(false);
        click.alpha = 0f;
        IsSelected = false;
    }
    public void Init(SelectRolePanelScript selectRole,SimpleCharacterInfoNode nCharacter)
    {
        this.selectRolePanelScript = selectRole;
        this.characterInfo = nCharacter;

        //通过jobid可以在unitdefine中找到响应的图片资源，然后进行加载
        int tid = nCharacter.ProfessionId;
        define = DataManager.Instance.unitDefineDict.GetValueOrDefault(tid, null);
        if(define == null)
        {
            Debug.LogError("define is null，tid is invalid");
            return;
        }

        //加载当前ui的背景图
        Sprite sprite = Res.LoadAssetSync<Sprite>(define.BgResource);
        Bg.sprite = sprite;

        //设置item的info显示
        nameText.text = characterInfo.ChrName;
        levelText.text = "Lv. " + characterInfo.Level;
        jobText.text = define.Name;
    }
    public void Stop()
    {
        if (breathingTween != null && breathingTween.IsActive())
        {
            breathingTween.Kill(); // 中止动画
            breathingTween = null;
        }
        DOTween.Kill(hover);
        DOTween.Kill(select);
        DOTween.Kill(click);
    }

    public void onBtn()
    {
        //告知控制中心
        if (IsSelected) return;
        selectRolePanelScript.OnSelectedRoleItem(this);
    }

    //当前item选中的效果
    public void SelectedEffect()
    {
        IsSelected = true;
        Sequence sequence = DOTween.Sequence();
        click.gameObject.SetActive(true);
        select.gameObject.SetActive(true);
        sequence.Append(click.DOFade(1, clickDuration));
        sequence.Append(click.DOFade(0, clickDuration)).OnComplete(()=>{
            click.gameObject.SetActive(false);
        });
        StartBreathingEffect();
    }
    private void StartBreathingEffect()
    {
        // 呼吸效果：透明度从最大淡出到最小，再淡入回来
        breathingTween = select.DOFade(minAlpha, selectDuration*0.5f)
            .SetEase(Ease.InOutSine)
            .OnComplete(() =>
            {
                // 确保对象还存在
                if (select != null &&  select.isActiveAndEnabled)
                {
                    breathingTween = select.DOFade(maxAlpha, selectDuration)
                        .SetEase(Ease.InOutSine)
                        .OnComplete(StartBreathingEffect); // 循环
                }
            });
    }
    //当前item取消选中的效果
    public void RestoreEffect()
    {
        // 停止呼吸效果
        if (breathingTween != null && breathingTween.IsActive())
        {
            breathingTween.Kill(); // 中止动画
            breathingTween = null;
        }

        select.DOFade(0, selectDuration).OnComplete(() =>
        {
            select.gameObject.SetActive(false);
            IsSelected = false;
        });
    }


    private bool isFadingIn = false;
    private bool isFadingOut = false;
    //淡入：透明=>不透明
    public void OnPointerEnter(PointerEventData eventData)
    {
        // 如果正在淡入或者当前已经完全不透明，不触发淡入动画
        if (isFadingIn || hover.color.a == 1f)
            return;

        // 如果正在进行淡出动画，立即停止它，开始淡入
        if (isFadingOut)
        {
            DOTween.Kill(hover);  // 停止当前的淡出动画
            isFadingOut = false;
        }

        isFadingIn = true;  // 标记淡入动画开始
        hover.enabled = true;

        hover.DOFade(1, hoverDuration).OnComplete(() =>
        {
            isFadingIn = false;  // 动画完成，重置标记
        });

    }
    public void OnPointerExit(PointerEventData eventData)
    {
        // 如果正在淡出或者当前已经完全透明，不触发淡出动画
        if (isFadingOut || hover.color.a == 0f)
            return;

        // 如果正在进行淡入动画，立即停止它，开始淡出
        if (isFadingIn)
        {
            DOTween.Kill(hover);  // 停止当前的淡入动画
            isFadingIn = false;
        }

        isFadingOut = true;  // 标记淡出动画开始

        hover.DOFade(0, hoverDuration).OnComplete(() =>
        {
            hover.enabled = false;  // 动画完成后禁用 select
            isFadingOut = false;     // 动画完成，重置标记
        });
    }

}

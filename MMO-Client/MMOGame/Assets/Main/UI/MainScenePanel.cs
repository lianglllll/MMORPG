using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
public class MainScenePanel : MonoBehaviour
{
    private Text tipsText;
    private Slider loadingSlider;
    private SelectUpdatePanel selectUpdatePanel;
    private CanvasGroup selectUpdatePanelCanvasGroup;
    private Image fadeImage;

    public Text TipsText => tipsText;
    public Slider LoadingSlider => loadingSlider;


    private float fadeDuration = 1f;    // 淡入淡出的时间
    private float minAlpha = 0f;        // 最小透明度
    private float maxAlpha = 1f;        // 最大透明度
    private Tween breathingTween;

    private void Awake()
    {
        tipsText = transform.Find("TipsText").GetComponent<Text>();
        loadingSlider = transform.Find("LoadingSlider").GetComponent<Slider>();
        selectUpdatePanel = transform.Find("SelectUpdatePanel").GetComponent<SelectUpdatePanel>();
        selectUpdatePanelCanvasGroup = transform.Find("SelectUpdatePanel").GetComponent<CanvasGroup>();
        fadeImage = transform.Find("FadeImage").GetComponent<Image>();
    }

    private void Start()
    {
        // 确保初始颜色为不透明状态
        Color originalColor1 = tipsText.color;
        originalColor1.a = maxAlpha;
        tipsText.color = originalColor1;

        Color originalColor2 = fadeImage.color;
        originalColor2.a = maxAlpha;
        fadeImage.color = originalColor2;

        tipsText.text = "正在加载";
        selectUpdatePanel.gameObject.SetActive(false);

        LoadingSlider.gameObject.SetActive(false);
    }

    // 在对象销毁时停止动画
    private void OnDestroy()
    {
        // 确保在对象销毁时停止 Tween 动画，避免报错
        if (breathingTween != null)
        {
            breathingTween.Kill();  // 停止并销毁 Tween 动画
        }
    }


    public void Init(Action action)
    {
        // 淡入效果
        fadeImage.DOFade(0, fadeDuration).OnComplete(() =>
        {
            action?.Invoke();
            fadeImage.gameObject.SetActive(false); // 隐藏 Image 组件
        });
    }

    public void FadeIn(Action action)
    {
        fadeImage.gameObject.SetActive(true); // 隐藏 Image 组件
        fadeImage.DOFade(1, fadeDuration).OnComplete(() =>
        {
            action?.Invoke();
            //场景已经要销毁了。
        });
    }


    public void OpenSelectUpdatePanel(string simpleTipsText, string detailTipsText, Action comfirm,Action cancel)
    {
        selectUpdatePanel.gameObject.SetActive(true);
        selectUpdatePanelCanvasGroup.alpha = 0; // 设置为完全透明

        // 淡入效果
        selectUpdatePanelCanvasGroup.DOFade(1, 0.5f) 
            .OnComplete(() =>
            {
                // 调用面板的打开方法
                selectUpdatePanel.OpenPanel(simpleTipsText, detailTipsText, () => {
                    // 开启下载
                    comfirm?.Invoke();
                    // 执行淡出效果
                    CloseSelectUpdatePanel();
                }, () => {
                    cancel?.Invoke();
                    // 执行淡出效果
                    CloseSelectUpdatePanel();
                });
            });
    }

    private void CloseSelectUpdatePanel()
    {
        // 淡出效果
        selectUpdatePanelCanvasGroup.DOFade(0, 0.5f) // 在 0.5 秒内淡出
            .OnComplete(() =>
            {
                // 当淡出完成时，隐藏面板并重置透明度
                selectUpdatePanel.gameObject.SetActive(false);
                selectUpdatePanelCanvasGroup.alpha = 1; // 重置为完全不透明，准备下次显示
            });
    }

    public void ReadyToStart()
    {
        tipsText.text = $"点击任意键进入游戏";
        StartBreathingEffect();
    }
    private void StartBreathingEffect()
    {
        // 呼吸效果：透明度从最大淡出到最小，再淡入回来
        breathingTween = tipsText.DOFade(minAlpha, fadeDuration)
            .SetEase(Ease.InOutSine)
            .OnComplete(() =>
            {
                // 确保对象还存在
                if (tipsText != null && tipsText.isActiveAndEnabled)
                {
                    breathingTween = tipsText.DOFade(maxAlpha, fadeDuration)
                        .SetEase(Ease.InOutSine)
                        .OnComplete(StartBreathingEffect); // 循环
                }
            });
    }

}

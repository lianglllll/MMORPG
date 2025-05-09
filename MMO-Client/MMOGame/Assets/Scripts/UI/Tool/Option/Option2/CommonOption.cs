using DG.Tweening;
using HSFramework.Audio;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CommonOption : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    private Image Bg;

    private bool isEntering;
    private bool isExiting;
    private float hoverA = 0.5f;
    private float hoverDuration = 0.5f;

    private Action m_optionAction;

    private void Awake()
    {
        Bg = transform.Find("Bg").GetComponent<Image>();
    }
    private void Start()
    {
        isEntering = false;
        isExiting = false;
    }
    public void Init(Action optionAction)
    {
        m_optionAction = optionAction;
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        GlobalAudioManager.Instance.PlayUIAudio(UIAudioClipType.ButtonClick);
        m_optionAction.Invoke();
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
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
}

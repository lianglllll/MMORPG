using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 转场海报，场景切换时展示
/// </summary>
public class ScenePoster : MonoBehaviour
{
    public static ScenePoster Instance;

    //场景切换海报+进度
    public GameObject SecneChangePoster;
    public Slider progressBar;
    public Image bgImage;               //背景图
    public TextMeshProUGUI nameText; //场景名称
    private bool isUpdatingProgress = false;
    private float targetProgress = 0f;
    private float initialProgress = 0f;
    private float elapsedTime = 0f;
    private float interval = 0.2f;
    private bool flag;

    //淡入淡出效果
    private Image fadeImage;
    public float fadeDuration = 0.8f;

    private void Awake()
    {
        fadeImage = transform.Find("Canvas/FadeAffect").GetComponent<Image>();
    }

    void Start()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
        gameObject.SetActive(true);
        SecneChangePoster.SetActive(false);
        // 初始时，图像完全黑
        fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, 1);
        //淡出，透明
        StartCoroutine(FadeOut());
        flag = false;
    }

    void Update()
    {
        //过场海报
        if (isUpdatingProgress)
        {
            elapsedTime += Time.deltaTime;
            progressBar.value = Mathf.Lerp(initialProgress, targetProgress, elapsedTime / interval);

            if (elapsedTime >= interval)
            {
                progressBar.value = targetProgress; // 确保最终值是目标进度
                isUpdatingProgress = false;

                //如果进度完成则隐藏UI
                if (Mathf.Approximately(progressBar.value, 1))
                {
                    StartCoroutine(HidePosterAfterDelay(0.3f));
                }
            }
        }
    }

    // 调用该函数来平滑地更新进度条
    public void SetProgress(float targetProgress, float interval = 0.2f)
    {
        this.targetProgress = targetProgress;
        this.interval = interval;
        initialProgress = progressBar.value;
        elapsedTime = 0f;

        //只有开启的时候才会调用
        if(!flag)
        {
            flag = true;
            StartCoroutine(ShowPoster());
        }
    }

    private IEnumerator ShowPoster()
    {
        yield return null;

        yield return FadeIn();
        SecneChangePoster.SetActive(true);
        yield return FadeOut();

        isUpdatingProgress = true;
    }

    private IEnumerator HidePosterAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        yield return FadeIn();
        SecneChangePoster.SetActive(false);
        progressBar.value = 0f;
        yield return FadeOut();
        flag = false;
    }

    public IEnumerator  FadeIn()
    {
        fadeImage.gameObject.SetActive(true); // 确保 Image 组件被激活
        fadeImage.DOFade(1, fadeDuration);
        yield return new WaitForSeconds(fadeDuration);
    }

    public IEnumerator FadeOut()
    {

        // 淡入效果
        fadeImage.DOFade(0, fadeDuration).OnComplete(() =>
        {
            fadeImage.gameObject.SetActive(false); // 隐藏 Image 组件
        });
        yield return new WaitForSeconds(fadeDuration);
    }


}

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

    public Slider progressBar;
    public Image bgImage;   //背景图
    public TextMeshProUGUI nameText; //场景名称

    private bool isUpdatingProgress = false;
    private float targetProgress = 0f;
    private float initialProgress = 0f;
    private float elapsedTime = 0f;
    private float interval = 0.2f;

    // 调用该函数来平滑地更新进度条
    public void SetProgress(float targetProgress, float interval = 0.2f)
    {
        gameObject.SetActive(true);
        this.targetProgress = targetProgress;
        this.interval = interval;
        initialProgress = progressBar.value;
        elapsedTime = 0f;
        isUpdatingProgress = true;
    }

    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
        gameObject.SetActive(false);
    }

    void Start()
    {

    }

    void Update()
    {
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
                    StartCoroutine(HideProgressBarAfterDelay(0.3f));
                }
            }
        }
    }

    private IEnumerator HideProgressBarAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        gameObject.SetActive(false);
        progressBar.value = 0f;
    }
}

using BaseSystem.Singleton;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// 转场海报，场景切换时展示
/// </summary>
public class ScenePoster : Singleton<ScenePoster>
{
    //场景切换海报+进度
    private GameObject m_poster;
    private Slider m_progressBar;
    private Image m_bgImage;                    //背景图
    private TextMeshProUGUI m_sceneNameText;    //场景名称
    private TextMeshProUGUI m_randomContentText;    //场景名称

    private bool m_isUpdatingProgress;
    private bool m_isShowPoster;
    private float m_initialProgress = 0f;
    private float m_targetProgress = 0f;
    private float m_elapsedTime = 0f;
    private float m_costTime = 0.2f;

    //淡入淡出效果
    private Image fadeImage;
    public float fadeDuration = 0.8f;

    private List<string> bgImgResPath = new List<string>();
    private Dictionary<string, Sprite> bgImgRes = new();
    private List<string> contents = new List<string>();
    System.Random random = new System.Random();


    protected override void Awake()
    {
        base.Awake();
        fadeImage = transform.Find("Canvas/FadeAffect").GetComponent<Image>();
        m_poster = transform.Find("Canvas/Poster").gameObject;
        m_bgImage = transform.Find("Canvas/Poster/background").GetComponent<Image>();
        m_sceneNameText = transform.Find("Canvas/Poster/NameText").GetComponent<TextMeshProUGUI>();
        m_randomContentText = transform.Find("Canvas/Poster/ContentText").GetComponent<TextMeshProUGUI>();
        m_progressBar = transform.Find("Canvas/Poster/Slider").GetComponent<Slider>();
    }
    void Start() 
    {
        gameObject.SetActive(true);
        m_poster.SetActive(false);

        // 初始时，图像完全黑
        fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, 1);
        //淡出，透明
        StartCoroutine(FadeOut());

        m_isShowPoster = false;
        m_isUpdatingProgress = false;

        // todo
        bgImgResPath.Add("Texture/bg/Poster/Poster1.jpg");
        bgImgResPath.Add("Texture/bg/Poster/Poster2.png");
        bgImgResPath.Add("Texture/bg/Poster/Poster3.png");
        bgImgResPath.Add("Texture/bg/Poster/Poster4.png");
        bgImgResPath.Add("Texture/bg/Poster/Poster5.png");

        contents.Add("Hello World.");
        contents.Add("不过是操作系统上的应用程序，竟心高气傲问候世界。");
        contents.Add("被隐盖的原理和价值。");
        contents.Add("世界是怎么活起来的?");
        contents.Add("落魄谷中寒风吹，春秋蝉鸣少年归。");
        contents.Add("荡魂山处石人泪，定仙游走魔向北。");
        contents.Add("逆流河上万仙退，爱情不敌坚持泪。");
        contents.Add("宿命天成命中败，仙尊悔而我不悔。");
        contents.Add("问君能有几多愁，恰是一江春水向东流。");
        contents.Add("众里寻他千百度。慕然回首，那人却在，灯火阑珊处。");

    }
    void Update()
    {
        //过场海报
        if (m_isUpdatingProgress)
        {
            m_elapsedTime += Time.deltaTime;
            m_progressBar.value = Mathf.Lerp(m_initialProgress, m_targetProgress, m_elapsedTime / m_costTime);

            if (m_elapsedTime >= m_costTime)
            {
                m_progressBar.value = m_targetProgress; // 确保最终值是目标进度
                m_isUpdatingProgress = false;

                //如果进度完成则隐藏UI
                if (Mathf.Approximately(m_progressBar.value, 1))
                {
                    StartCoroutine(_HidePosterAfterDelay(0.3f));
                }
            }
        }
    }

    // 调用该函数来平滑地更新进度条
    public void SetProgress(float targetProgress, float costTime = 0.2f)
    {
        this.m_targetProgress = targetProgress;
        this.m_costTime = costTime;
        m_initialProgress = m_progressBar.value;
        m_elapsedTime = 0f;

        //只有开启的时候才会调用
        if(!m_isShowPoster)
        {
            m_isShowPoster = true;
            StartCoroutine(_ShowPoster());
        }
    }
    public void SetNameText(string name)
    {
        if (!string.IsNullOrEmpty(name))
        {
            m_sceneNameText.text = name;
        }
        else
        {
            m_sceneNameText.text = "--";
        }
    }

    private IEnumerator _ShowPoster()
    {
        yield return FadeIn();
        m_poster.SetActive(true);
        int randomNumber = random.Next(0, bgImgResPath.Count);
        string randomStr = bgImgResPath[randomNumber];
        Sprite sprite;
        if (!bgImgRes.TryGetValue(randomStr, out  sprite)) {
            sprite = Res.LoadAssetSync<Sprite>(bgImgResPath[randomNumber]);
        }
        m_bgImage.sprite = sprite;
        m_progressBar.value = 0f;
        randomNumber = random.Next(0, contents.Count);
        m_randomContentText.text = contents[randomNumber];

        yield return FadeOut();
        m_isUpdatingProgress = true;
    }
    private IEnumerator _HidePosterAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        yield return FadeIn();
        m_poster.SetActive(false);
        m_progressBar.value = 0f;
        yield return FadeOut();
        m_isShowPoster = false;
    }

    public IEnumerator FadeIn()
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


    /// <summary>
    /// 加载场景
    /// </summary>
    /// <param name="spaceId">场景id</param>
    /// <param name="endAction">场景加载完成后的回调</param>
    public  void LoadSpaceWithPoster(string name, string resPath, Action<Scene> endAction)
    {
        UnityMainThreadDispatcher.Instance().StartCoroutine(_LoadSpaceWithPosterStart(name, resPath, endAction));
    }
    private  IEnumerator _LoadSpaceWithPosterStart(string name, string resPath, Action<Scene> endAction)
    {
        //展示转场UI,这里需要在4秒内模拟到进度的百分之90，这个是模拟出来假的进度。
        ScenePoster.Instance.SetNameText(name);
        ScenePoster.Instance.SetProgress(0.9f, 4.0f);
        yield return new WaitForSeconds(ScenePoster.Instance.fadeDuration * 1.1f);

        var handle = Res.LoadSceneAsync(resPath);
        handle.OnLoaded = (s) =>
        {
            UnityMainThreadDispatcher.Instance().StartCoroutine(_LoadSpaceWithPosterEnd(s, endAction));
        };

    }
    private  IEnumerator _LoadSpaceWithPosterEnd(Scene s, Action<Scene> endAction)
    {
        yield return null;

        //逻辑
        endAction?.Invoke(s);

        //完成转场ui
        ScenePoster.Instance.SetProgress(1f, 0.3f);
    }

}

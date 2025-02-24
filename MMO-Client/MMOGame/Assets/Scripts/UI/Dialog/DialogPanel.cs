using GameClient.UI.Dialog;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Linq;
using System.Collections.Generic;
using Google.Protobuf.Collections;
using System;

public class DialogPanel : MonoBehaviour
{
    private Transform m_abovePart;
    private Transform m_belowPart;
    private Transform m_theEndPart;
    private Image m_leftIconImg;
    private Image m_rightIconImg;
    private Image m_theEndIconImg;
    private TextMeshProUGUI m_dialogText;
    private TextMeshProUGUI m_endText;

    private DialogConfig m_dialogConfig;
    private DialogStepConfig m_curDialogStepConfig;
    private bool m_IsCanNext;

    private float m_typeSpeed = 0.2f; // 每个字符出现的时间间隔
    private Tween m_typingTween;
    private bool m_IsTyping;
    private float m_imageFadeDuration = 1f;

    private int m_curShowOptionCount;
    private List<Transform> m_selectOptions = new List<Transform>();
    private List<Button> m_selectOptionsBtns = new List<Button>();
    private List<Text> m_selectOptionsTexts = new List<Text>();

    private AudioSource m_audioSource;

    private void Awake()
    {
        m_abovePart = transform.Find("Above").transform;
        m_belowPart = transform.Find("Below").transform;
        m_theEndPart = transform.Find("TheEnd").transform;
        m_leftIconImg = transform.Find("Below/LeftIconImg").GetComponent<Image>();
        m_rightIconImg = transform.Find("Below/RightIconImg").GetComponent<Image>();
        m_theEndIconImg = transform.Find("TheEnd").GetComponent<Image>();
        m_dialogText = transform.Find("Below/TextBg/Text").GetComponent<TextMeshProUGUI>();
        m_endText = transform.Find("TheEnd/TextBg/Text").GetComponent<TextMeshProUGUI>();
        m_selectOptions = m_abovePart.Find("SelectOptions").transform.Cast<Transform>().ToList();
        foreach (Transform t in m_selectOptions)
        {
            m_selectOptionsBtns.Add(t.GetComponent<Button>());
            m_selectOptionsTexts.Add(t.Find("Text").GetComponent<Text>());
        }
        m_audioSource = GetComponent<AudioSource>();
    }
    //临时模拟调用
    void Start()
    {
        LocalDataManager.Instance.init();
        DialogConfigImporter.Instance.Init();
        var item = DialogConfigImporter.Instance.GetDialogConfigByDid(0);
        Init(item);
    }
    void Update()
    {
        if(GameInputManager.Instance.AnyKey && m_IsCanNext && m_curDialogStepConfig != null)
        {
            if (m_IsTyping)
            {
                //停止打字，立刻显示全部内容
                if (m_typingTween != null)
                {
                    m_typingTween.Kill();
                    m_typingTween = null; // 可选：防止重复终止
                    m_IsTyping = false;
                }
                m_dialogText.text = m_curDialogStepConfig.Content;
            }
            else
            {
                _NextDialogStep(m_curDialogStepConfig.NextIndex);
            }
        }
    }
    public bool Init(DialogConfig dialogConfig)
    {
        if (dialogConfig == null) {
            return false;
        }

        m_dialogConfig = dialogConfig;
        m_curDialogStepConfig = m_dialogConfig.GetDialogStepConfigByIndex(0);
        if(m_curDialogStepConfig == null)
        {
            return false;
        }
        m_IsCanNext = true;

        m_theEndPart.gameObject.SetActive(false);
        m_abovePart.gameObject.SetActive(false);
        m_belowPart.gameObject.SetActive(true);
        foreach (Transform t in m_selectOptions)
        {
            t.gameObject.SetActive(false);
        }

        m_curShowOptionCount = 0;

        m_IsTyping = false;

        _ShowCurDialogStep();
        return true;
    }
    public bool UnInit()
    {
        m_theEndPart.gameObject.SetActive(false);
        m_belowPart.gameObject.SetActive(false);
        m_IsCanNext = false;
        m_dialogConfig = null;
        return true;
    }

    private bool _ShowCurDialogStep()
    {
        if(m_curDialogStepConfig == null)
        {
            return false;
        }

        //判断当前对话类型
        switch (m_curDialogStepConfig.Flag)
        {
            case 0:
                _CommonDialogStep();
                break;
            case 1:
                _SelectDialogStep();
                break;
            case 3:
                _EndDialogStep();
                break;
            case 4:
                _ExitDialgPanel();
                break;
            default:
                Debug.LogError("对话配置异常,没有这种对话类型");
                break;

        }

        return true;
    }
    private bool _NextDialogStep(int index)
    {
        if(m_curDialogStepConfig == null || m_curDialogStepConfig.NextIndex == -1)
        {
            _ExitDialgPanel();
            return false;
        }
        //如果是选项要隐藏
        if (m_curShowOptionCount > 0) {
            m_abovePart.gameObject.SetActive(false);
            for (int i = 0; i < m_curShowOptionCount; i++) {
                //清理事件
                m_selectOptionsBtns[i].onClick.RemoveAllListeners();
                m_selectOptions[i].gameObject.SetActive(false);
            }
        }

        //结束后的事件
        m_curDialogStepConfig.ExcuteEndEvents();
        m_curDialogStepConfig = m_dialogConfig.GetDialogStepConfigByIndex(index);

        if(m_curDialogStepConfig == null)
        {
            return false;
        }

        _ShowCurDialogStep();
        return true;
    }
    private bool _CommonDialogStep()
    {
        //开始前的事件
        m_curDialogStepConfig.ExcuteStartEvents();

        //显示头像
        //todo 用对象池拿
        Sprite iconImage = Res.LoadAssetSync<Sprite>(m_curDialogStepConfig.IconPath);
        if (iconImage != null) {
            if (m_curDialogStepConfig.Pos == 0)
            {
                m_leftIconImg.enabled = true;
                m_rightIconImg.enabled = false;
                m_leftIconImg.sprite = iconImage;
            }
            else
            {
                m_leftIconImg.enabled = false;
                m_rightIconImg.enabled = true;
                m_rightIconImg.sprite = iconImage;
            }
        }
        else
        {
            m_leftIconImg.enabled = false;
            m_rightIconImg.enabled = false;
        }

        //开始显示文本
        _StartTyping(m_dialogText);

        //声音播放
        _PlayDialogSound(m_curDialogStepConfig.SoundPath);

        return true;
    }
    private bool _SelectDialogStep()
    {
        //选项无法跳过
        m_IsCanNext = false;

        //开始前的事件
        m_curDialogStepConfig.ExcuteStartEvents();

        //icon显示
        //todo 用对象池拿
        Sprite iconImage = Res.LoadAssetSync<Sprite>(m_curDialogStepConfig.IconPath);
        if (iconImage != null)
        {
            m_leftIconImg.enabled = true;
            m_rightIconImg.enabled = false;
            m_leftIconImg.sprite = iconImage;
        }
        else
        {
            m_leftIconImg.enabled = false;
            m_rightIconImg.enabled = false;
        }

        //选项显示和事件绑定
        //获取选项信息
        List<DialogStepConfig> selectOptionsConfigList = new List<DialogStepConfig>();
        DialogStepConfig tmpDialogStepConfig = m_dialogConfig.GetDialogStepConfigByIndex(m_curDialogStepConfig.NextIndex);
        while (tmpDialogStepConfig.Flag == 2)
        {
            selectOptionsConfigList.Add(tmpDialogStepConfig);
            tmpDialogStepConfig = m_dialogConfig.GetDialogStepConfigByIndex(tmpDialogStepConfig.ID + 1);
        }

        //显示选项
        m_abovePart.gameObject.SetActive(true);
        m_curShowOptionCount = selectOptionsConfigList.Count <= 5 ? selectOptionsConfigList.Count : 5;
        for (int i = 0; i < m_curShowOptionCount; ++i){
            tmpDialogStepConfig = selectOptionsConfigList[i];
            m_selectOptions[i].gameObject.SetActive(true);
            string path = tmpDialogStepConfig.SoundPath;
            m_selectOptionsBtns[i].onClick.AddListener(() => {
                m_IsCanNext = true;
                _PlayDialogSound(path);
                _NextDialogStep(tmpDialogStepConfig.NextIndex);
            });
            m_selectOptionsTexts[i].text = tmpDialogStepConfig.Content;
        }

        //文本显示
        _StartTyping(m_dialogText);

        //声音播放
        _PlayDialogSound(m_curDialogStepConfig.SoundPath);

        return true;
    }
    private bool _EndDialogStep()
    {
        //开始前的事件
        m_curDialogStepConfig.ExcuteStartEvents();

        m_belowPart.gameObject.SetActive(false);
        m_theEndPart.gameObject.SetActive(true);
        Sprite iconImage = Res.LoadAssetSync<Sprite>(m_curDialogStepConfig.IconPath);
        if (iconImage != null)
        {
            m_theEndIconImg.enabled = true;
            m_theEndIconImg.sprite = iconImage;
            _StartFadeImg(m_theEndIconImg, () => {
                _StartTyping(m_endText);
                //声音播放
                _PlayDialogSound(m_curDialogStepConfig.SoundPath);
            });
        }
        else
        {
            m_theEndIconImg.enabled = false;
            _StartTyping(m_endText);
            //声音播放
            _PlayDialogSound(m_curDialogStepConfig.SoundPath);
        }

        return true;
    }
    private bool _ExitDialgPanel()
    {
        m_IsCanNext = false;
        return true;
    }
    private bool _StartTyping(TextMeshProUGUI text)
    {
        m_IsTyping = true;

        // 清空初始文本以准备打字效果
        text.text = "";
        // 使用DOText创建打字效果
        m_typingTween = text.DOText(m_curDialogStepConfig.Content, m_curDialogStepConfig.Content.Length * m_typeSpeed)
            .SetEase(Ease.Linear)
            .OnComplete(() => {
                m_IsTyping = false;
            });
        return true;
    }
    private bool _StartFadeImg(Image image,Action cb) {

        // 确保图片一开始是完全透明的
        Color color = image.color;
        color.a = 0;
        image.color = color;

        // 使用 DoTween 对 alpha 值进行动画
        image.DOFade(1.0f, m_imageFadeDuration).OnComplete(() => {
            cb.Invoke();
        });

        return true;
    }
    private bool _PlayDialogSound(string audioPath)
    {
        var audioClip =  Res.LoadAssetSync<AudioClip>(audioPath);
        if (audioClip == null) {
            return false;
        }

        m_audioSource.clip = audioClip;
        m_audioSource.Play();
        return true;
    }
}

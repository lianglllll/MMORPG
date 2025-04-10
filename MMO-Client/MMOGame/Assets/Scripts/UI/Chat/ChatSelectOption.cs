using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;
using HSFramework.Audio;
using HS.Protobuf.Chat;

public class ChatSelectOption : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler,IPointerClickHandler
{
    private Image Bg;
    private Text text;

    private bool isClicked;
    private bool isEntering;
    private bool isExiting;
    private float hoverA = 0.5f;
    private float hoverDuration = 0.5f;

    public ChatMessageChannel m_curChannel;
    private ChatBoxScript m_chatBox;

    private void Awake()
    {
        Bg = transform.Find("Bg").GetComponent<Image>();
        text = transform.Find("Text").GetComponent<Text>();
    }
    private void Start()
    {
        isClicked = false;
        isEntering = false;
        isExiting = false;
    }

    public void Init(ChatBoxScript chatBox, ChatMessageChannel type)
    {
        this.m_curChannel = type;
        this.m_chatBox = chatBox;

        string optionName = "";
        switch (m_curChannel)
        {
            case ChatMessageChannel.System:
                optionName = "系统";
                break;
            case ChatMessageChannel.Local:
                optionName = "附近";
                break;
            case ChatMessageChannel.Scene:
                optionName = "场景";
                break;
            case ChatMessageChannel.World:
                optionName = "世界";
                break;
            case ChatMessageChannel.Team:
                optionName = "队伍";
                break;
            case ChatMessageChannel.Guild:
                optionName = "天庭";
                break;
            case ChatMessageChannel.Private:
                optionName = "私聊";
                break;
        }
        text.text = optionName;
    }
    // 外部调用
    public void OnClick()
    {
        if (isClicked) return;
        isClicked = true;
        //bg透明度->1
        //字体变黑
        text.color = Color.black;
        Bg.DOFade(1, hoverDuration);
    }
    public void CancelClick()
    {
        if (!isClicked) return;
        text.color = Color.white;
        Bg.DOFade(0, hoverDuration).OnComplete(() => {
            isClicked = false;
        });

    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (isClicked) return;
        GlobalAudioManager.Instance.PlayUIAudio(UIAudioClipType.ButtonClick);
        m_chatBox.Selected(this);
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
}

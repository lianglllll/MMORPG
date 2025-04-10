using HS.Protobuf.Chat;
using HSFramework.MyDelayedTaskScheduler;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static ChatManager;

public enum ChatShowType
{
    None    = 0,
    Simple  = 1,
    Complex = 2,
}

public class ChatBoxScript : MonoBehaviour
{
    private ChatShowType m_curShowType;

    // Complex
    private Transform m_complexBox;
    public Scrollbar m_scrollbarVertical;                 // Scrollbar
    public InputField m_chatMsgInputField;
    private Button m_sendMsgBtn;
    private Transform m_content;

    private ChatMessageChannel curChannel;
    private ChatSelectOption curOption;
    private Dictionary<ChatMessageChannel, ChatSelectOption> ChatSelectOptions;
    public ScrollRect scrollRect;
    private RecyclingListView<ChatMessageV2, ChatMsgLine> complexrecyclingListView;
    private RecyclingListView<ChatMessageV2, ChatMsgLine> simplerecyclingListView;

    // Simple
    private Transform m_simpleBox;

    private void Awake()
    {
        m_complexBox =          transform.Find("ComplexBox").transform;
        m_chatMsgInputField =   transform.Find("ComplexBox/ChatInputField").GetComponent<InputField>();
        m_sendMsgBtn =          transform.Find("ComplexBox/SendMsgBtn").GetComponent<Button>();
        m_content =             transform.Find("ComplexBox/ScrollView/Viewport/Content").transform;
        m_simpleBox =           transform.Find("SimpleBox");
    }
    private void Start()
    {
        m_sendMsgBtn.onClick.AddListener(OnSendMsgBtn);

        // 建立映射
        ChatSelectOptions = new Dictionary<ChatMessageChannel, ChatSelectOption>();
        ChatMessageChannel[] values = (ChatMessageChannel[])Enum.GetValues(typeof(ChatMessageChannel));
        var options = transform.Find("ComplexBox/SelectOptions");
        for (int index = 0; index < options.childCount; ++index)
        {
            var chatselectOption = options.GetChild(index).GetComponent<ChatSelectOption>();
            var channel = values[index + 1];
            chatselectOption.Init(this, channel);
            ChatSelectOptions.Add(channel, chatselectOption);
        }
        // 默认选择第一个menu
        DelayedTaskScheduler.Instance.AddDelayedTask(0.1f, () => {
            Selected(ChatSelectOptions[ChatMessageChannel.Scene]);
        });

        complexrecyclingListView = new RecyclingListView<ChatMessageV2, ChatMsgLine>();
        complexrecyclingListView.Init(scrollRect, 630, 50, 1, "UI/Prefabs/Chat/ChatComplexBoxMsgLine.prefab",
            ChatManager.Instance.GetAllCurChanelMsg());

        simplerecyclingListView = new RecyclingListView<ChatMessageV2, ChatMsgLine>();
        simplerecyclingListView.Init(scrollRect, 450, 50, 1, "UI/Prefabs/Chat/ChatSimpleBoxMsgLine.prefab",
            ChatManager.Instance.GetAllCurChanelMsg());
    }
    public void Init()
    {
        ChangeChatShowMode(ChatShowType.Simple);
    }
    private void Update()
    {
        if(m_curShowType == ChatShowType.Complex && GameInputManager.Instance.UI_Enter)
        {
            OnSendMsgBtn();
        }
    }
    private void OnEnable()
    {
        ChatManager.Instance.OnChat += UpdateChatBox;
    }
    private void OnDisable()
    {
        ChatManager.Instance.OnChat -= UpdateChatBox;
    }

    public void Selected(ChatSelectOption option)
    {
        // temp
        var channel = option.m_curChannel;
        if(channel != ChatMessageChannel.Scene && channel != ChatMessageChannel.World)
        {
            UIManager.Instance.ShowTopMessage("未开发！！");
            return;
        }

        if (curOption != null)
        {
            curOption.CancelClick();
        }
        curOption = option;
        curOption.OnClick();
        curChannel = option.m_curChannel;

        ChatManager.Instance.SetCurChannel(curChannel);            
    }
    private void OnSendMsgBtn()
    {
        string msg = m_chatMsgInputField.text;
        if (msg.Equals(""))
        {
            UIManager.Instance.ShowTopMessage("发送的信息不能为空！");
            return;
        }
        ChatManager.Instance.SendChatMessage(msg);
        m_chatMsgInputField.text = "";
        // 输入框获取焦点
        m_chatMsgInputField.Select();
        m_chatMsgInputField.ActivateInputField();
    }
    public void UpdateChatBox()
    {
        complexrecyclingListView.UpdateItems(ChatManager.Instance.GetAllCurChanelMsg());
    }
    public void ChangeChatShowMode(ChatShowType chatShowType)
    {
        if (m_curShowType == chatShowType) return;
        if(chatShowType == ChatShowType.Simple)
        {
            m_curShowType = ChatShowType.Simple;
            m_simpleBox.gameObject.SetActive(true);
            m_complexBox.gameObject.SetActive(false);
        }else if(chatShowType == ChatShowType.Complex)
        {
            m_curShowType = ChatShowType.Complex;
            m_simpleBox.gameObject.SetActive(false);
            m_complexBox.gameObject.SetActive(true);
            // 输入框获取焦点
            m_chatMsgInputField.Select();
            m_chatMsgInputField.ActivateInputField();
        }
    }
}

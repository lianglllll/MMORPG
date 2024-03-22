using Proto;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static ChatManager;

enum ChatShowType
{
    Simple = 0,
    Complex=1,
}

public class ChatBoxScript : MonoBehaviour
{

    private ChatShowType curShowType;

    //Complex
    private Transform _complexBox;
    private Button SendMsgBtn;
    public Scrollbar scrollbarVertical;                 //Scrollbar
    public GameObject chatTextObj;                      //显示单条message的text
    public InputField chatMsgInputField;
    private Button changeSimpleBoxBtn;
    private Transform content;

    //Simple
    private Transform _simpleBox;
    private Button changeComplexBoxBtn;
    private Text SimpleMsgText;

    private void Awake()
    {
        _complexBox = transform.Find("ComplexBox").transform;
        chatMsgInputField = transform.Find("ComplexBox/ChatInputField").GetComponent<InputField>();
        SendMsgBtn = transform.Find("ComplexBox/SendMsgBtn").GetComponent<Button>();
        changeSimpleBoxBtn = transform.Find("ComplexBox/ChangeSimpleBoxBtn").GetComponent<Button>();
        content = transform.Find("ComplexBox/ScrollView/Viewport/Content").transform;
        _simpleBox = transform.Find("SimpleBox");
        changeComplexBoxBtn = transform.Find("SimpleBox/SingleMsgConent").GetComponent<Button>();
        SimpleMsgText = transform.Find("SimpleBox/SingleMsgConent/ChatText").GetComponent<Text>();
    }

    private void Start()
    {
        curShowType = ChatShowType.Simple;
        _simpleBox.gameObject.SetActive(true);
        _complexBox.gameObject.SetActive(false);

        SendMsgBtn.onClick.AddListener(OnSendMsgBtn);
        changeSimpleBoxBtn.onClick.AddListener(() =>
        {
            if (curShowType == ChatShowType.Simple) return;
            curShowType = ChatShowType.Simple;
            _simpleBox.gameObject.SetActive(true);
            _complexBox.gameObject.SetActive(false);
        });
        changeComplexBoxBtn.onClick.AddListener(() =>
        {
            if (curShowType == ChatShowType.Complex) return;
            curShowType = ChatShowType.Complex;
            _simpleBox.gameObject.SetActive(false);
            _complexBox.gameObject.SetActive(true);
        });

        ChatManager.Instance.SetSendChannel(LocalChannel.Local);            //设置默认发送频道为本地

        ChatManager.Instance.OnChat += UpdateChatBox;

    }


    private void OnDestroy()
    {
        ChatManager.Instance.OnChat -= UpdateChatBox;
    }

    /// <summary>
    /// 添加一条信息
    /// </summary>
    /// <param name="msg"></param>
    public void AddOneMessage(string msg)
    {
        //这里是刷complex面板的
        //1.实例化一个chatTextObj到content下面
        GameObject obj  = Instantiate(chatTextObj);
        obj.transform.SetParent(content, false);
        obj.GetComponent<Text>().text = msg;
        //3.chatbox显示位置更新到最新的位置
        scrollbarVertical.value = 0f;
        scrollbarVertical.value = 0f;

        //这里刷simple面板
        SimpleMsgText.text = msg;

    }

    /// <summary>
    /// 点击发送信息按钮
    /// </summary>
    private void OnSendMsgBtn()
    {
        string msg = chatMsgInputField.text;
        if (msg.Equals("")) return;
        ChatManager.Instance.SendChat(msg);
        chatMsgInputField.text = "";
    }

    /// <summary>
    /// 更新某个频道的信息
    /// </summary>
    /// <param name="channel"></param>
    public void UpdateChatBox(LocalChannel channel)
    {
        switch (channel)
        {
            case LocalChannel.Local:
                UpdateLocalChannel();
                break;
        }
    }

    /// <summary>
    /// 更新本地的面板
    /// </summary>
    public void UpdateLocalChannel()
    {
        var list = ChatManager.Instance.GetNewChatMsg(LocalChannel.Local);
        if (list == null) return;
        foreach (var e in list)
        {
            string str = $"[玩家]{e.FromName}:{e.Content}";
            AddOneMessage(str);
        }
    }

}

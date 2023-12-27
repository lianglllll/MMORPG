using Proto;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static ChatManager;

public class ChatBoxScript : MonoBehaviour
{
    private Button SendMsgBtn;
    private RectTransform content;
    private GridLayoutGroup contentGridLayoutGroup;
    public Scrollbar scrollbarVertical;                 //Scrollbar
    public GameObject chatTextObj;                      //显示单条message的text
    public InputField chatMsgInputField;

    private float InitMaskHeight;                       //初始的mask高度
    private float oneMaskHeight;                        //一条消息的mask高度
    private float currentMaskHeight;                    //初始的mask高度
    private int singleRawCharNum = 28;

    private void Awake()
    {
        chatMsgInputField = transform.Find("ChatInputField").GetComponent<InputField>();
        SendMsgBtn = transform.Find("SendMsgBtn").GetComponent<Button>();
        content = transform.Find("ScrollView/Viewport/Content").GetComponent<RectTransform>();
        contentGridLayoutGroup = content.GetComponent<GridLayoutGroup>();
        InitMaskHeight = content.rect.height;
        oneMaskHeight = contentGridLayoutGroup.cellSize.y;
        currentMaskHeight = 0f;
    }

    private void Start()
    {
        SendMsgBtn.onClick.AddListener(OnSendMsgBtn);
        ChatManager.Instance.SetSendChannel(LocalChannel.Local);            //设置默认发送频道为本地
        ChatManager.Instance.OnChat += UpdateChatBox;
    }

    /// <summary>
    /// 添加一条信息
    /// </summary>
    /// <param name="msg"></param>
    public void AddOneMessage(string msg)
    {
        //0.分解字符串//todo
        
        //1.实例化一个chatTextObj到content下面
        GameObject obj  = Instantiate(chatTextObj);
        obj.transform.SetParent(content, false);
        obj.GetComponent<Text>().text = msg;
        //2.扩张mask的高度
        ExtensMaskHight(1);
        //3.chatbox显示位置更新到最新的位置
        scrollbarVertical.value = 0;
    }

    /// <summary>
    /// 扩张遮罩高度
    /// </summary>
    /// <param name="n"></param>
    public void ExtensMaskHight(int n)
    {
        currentMaskHeight += n * oneMaskHeight;
        if (currentMaskHeight < InitMaskHeight) return;
        Vector2 sizeDelta = content.sizeDelta;
        Vector2 tmp = new Vector2(0, currentMaskHeight);
        sizeDelta.Set(sizeDelta.x, tmp.y);
        content.sizeDelta = sizeDelta;
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
            string str = $"[玩家]{e.FromName}: {e.Content}";
            AddOneMessage(str);
        }
    }

}

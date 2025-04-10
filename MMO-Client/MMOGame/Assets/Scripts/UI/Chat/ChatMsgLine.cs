using HS.Protobuf.Chat;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChatMsgLine : MonoBehaviour, IRecyclingListViewItem<ChatMessageV2>
{
    private Text m_text;

    private void Awake()
    {
        m_text = transform.GetComponent<Text>();
    }
    public void InitInfo(ChatMessageV2 info)
    {
        m_text.text = $"{info.FromChrName} : {info.Content}"; 
    }
}

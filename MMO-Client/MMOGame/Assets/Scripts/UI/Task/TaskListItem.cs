using HS.Protobuf.GameTask;
using HSFramework.Audio;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TaskListItem : MonoBehaviour, IPointerClickHandler
{
    private bool isClicked;

    private TaskPanel m_taskPanel;
    private NetGameTaskNode m_task;
    private TaskDefine m_def;

    private TextMeshProUGUI m_taskTitleText;
    private Image m_bg1;
    private Image m_bg2;

    private void Awake()
    {
        m_taskTitleText = transform.Find("TaskTitleText").GetComponent<TextMeshProUGUI>();
        m_bg1 = transform.Find("Bg1").GetComponent<Image>();
        m_bg2 = transform.Find("Bg2").GetComponent<Image>();
    }

    public bool Init(NetGameTaskNode node, TaskPanel taskPanel)
    {
        bool result = false;
        m_taskPanel = taskPanel;
        // 获取相关配置
        LocalDataManager.Instance.m_taskDefineDict.TryGetValue(node.TaskId, out m_def);
        if(m_def == null)
        {
            goto End;
        }

        // 进度解析

        // ui显示
        isClicked = false;
        Select(false);

    End:
        return result;
    }
    public bool UnInit()
    {
        m_task = null;
        m_def = null;
        return true;
    }

    // 外部调用
    public void OnPointerClick(PointerEventData eventData)
    {
        if (isClicked) return;
        GlobalAudioManager.Instance.PlayUIAudio(UIAudioClipType.ButtonClick);
        m_taskPanel.Selected(this);
    }
    public void OnClick()
    {
        if (isClicked) return;
        isClicked = true;
        Select(true);
    }
    public void CancelClick()
    {
        if (!isClicked) return;
        isClicked = false;
        Select(false);
    }

    private void Select(bool isSelect)
    {
        if (isSelect)
        {
            // 选中时灰底 + 白字
            m_taskTitleText.color = Color.white;
            m_bg1.enabled = false;
            m_bg2.enabled = true;
        }
        else
        {
            // 没选中时白透明背景 + 黑字
            m_taskTitleText.color = Color.black;
            m_bg1.enabled = true;
            m_bg2.enabled = false;
        }
    }

}

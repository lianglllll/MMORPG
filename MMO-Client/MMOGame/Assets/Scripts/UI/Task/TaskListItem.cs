using HS.Protobuf.GameTask;
using HSFramework.Audio;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum TaskListItemMode
{
    Normal, Tips 
}

[Serializable]
public class ConditionData
{
    public string condType;         // 条件类型：KillEnemy/ReachPosition等
    public string[] Parameters;     // 参数：[1001] 或 [200,1,100]
    public int TargetValue;         // 目标值
    public int CurValue;
}

public class TaskListItem : MonoBehaviour, IPointerClickHandler
{
    private bool isClicked;

    private TaskPanel m_taskPanel;
    private NetGameTaskNode m_task;
    private TaskDefine m_def;
    private List<ConditionData> m_progress;

    // mode1
    private Transform mode1;
    private TextMeshProUGUI m_taskTitleText;
    private Image m_bg1;
    private Image m_bg2;

    // mode2
    private Transform mode2;
    private TextMeshProUGUI m_tipsTitleText;

    #region GetSet
    public List<ConditionData> Progress => m_progress;
    public string Title => m_def.Title;
    public string Desc => m_def.Desc;
    public string Condition
    {
        get
        {
            if(m_task.TaskState == GameTaskState.Locked)
            {
                return m_def.Pre_conditions;
            }else if(m_task.TaskState == GameTaskState.InProgress)
            {
                return m_def.Target_conditions;
            }
            else
            {
                return "";
            }
        }
    }
    public string Reward => m_def.Reward_items;
    #endregion

    private void Awake()
    {
        mode1 = transform.Find("Mode1").transform;
        m_taskTitleText = transform.Find("Mode1/TaskTitleText").GetComponent<TextMeshProUGUI>();
        m_bg1 = transform.Find("Mode1/Bg1").GetComponent<Image>();
        m_bg2 = transform.Find("Mode1/Bg2").GetComponent<Image>();

        mode2 = transform.Find("Mode2").transform;
        m_tipsTitleText = transform.Find("Mode2/TaskTitleText").GetComponent<TextMeshProUGUI>();
    }
    public bool Init(TaskListItemMode mode, string mode1TipsStr, NetGameTaskNode node, TaskPanel taskPanel)
    {
        bool result = false;

        if(mode == TaskListItemMode.Tips)
        {
            mode1.gameObject.SetActive(false);
            mode2.gameObject.SetActive(true);

            m_tipsTitleText.text = mode1TipsStr;
        }
        else if(mode == TaskListItemMode.Normal)
        {
            mode1.gameObject.SetActive(true);
            mode2.gameObject.SetActive(false);

            m_taskPanel = taskPanel;
            m_task = node;

            // 获取相关配置
            LocalDataManager.Instance.m_taskDefineDict.TryGetValue(node.TaskId, out m_def);
            if (m_def == null)
            {
                Log.Warning("TaskListItem init fail, taskDef is null");
                goto End;
            }
            // 任务状态
            if(node.TaskState == GameTaskState.Locked)
            {

            }else if(node.TaskState == GameTaskState.Unlocked)
            {

            }else if(node.TaskState == GameTaskState.InProgress)
            {

            }else if(node.TaskState == GameTaskState.Completed)
            {

            }else if(node.TaskState == GameTaskState.Rewarded)
            {

            }

            // 进度解析
            m_progress = JsonConvert.DeserializeObject<List<ConditionData>>(node.TaskProgress);

            // ui显示
            m_taskTitleText.text = m_def.Title;
            isClicked = false;
            _Select(false);
        }

        result = true;
    End:
        return result;
    }
    public bool UnInit()
    {
        m_taskPanel = null;
        m_task = null;
        m_def = null;
        m_progress = null;
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
        _Select(true);
    }
    public void CancelClick()
    {
        if (!isClicked) return;
        isClicked = false;
        _Select(false);
    }
    private void _Select(bool isSelect)
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

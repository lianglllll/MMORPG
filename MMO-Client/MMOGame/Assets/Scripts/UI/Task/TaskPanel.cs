using HS.Protobuf.GameTask;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TaskPanel : MonoBehaviour, ICommonSelectOpionMgr
{
    [Header("任务类型选项")]
    [SerializeField]private List<CommonSelectOption> optionObjs;
    private CommonSelectOption curOption;

    [Header("任务列表")]
    [SerializeField] private Transform content;
    private string taskListItemPrefabPath;
    private string taskListItemDefaultPrefabPath;
    private TaskListItem curTaskListItem;

    [Header("任务详情")]
    [SerializeField] private TextMeshProUGUI TaskTitleText;
    [SerializeField] private TextMeshProUGUI TaskDetailText;

    private void Awake()
    {
        
    }
    private void Start()
    {
        List<string> optionNames = new()
        {
            "主线",
            "支线",
            "奇遇"
        };
        List<int> flags = new()
        {
            (int)GameTaskType.MainStory,
            (int)GameTaskType.SideStory,
            (int)GameTaskType.Common,
        };
        InitOptions(optionObjs, optionNames, flags);
    }

    // 任务分类的
    public void InitOptions(List<CommonSelectOption> options, List<string> optionNames, List<int> flags)
    {
        for (int i = 0; i < options.Count; ++i)
        {
            options[i].Init(this, optionNames[i], flags[i]);
        }
    }
    public void Selected(CommonSelectOption commonSelectOption)
    {
        if (curOption != null)
        {
            curOption.CancelClick();
        }
        curOption = commonSelectOption;
        curOption.OnClick();

        // 刷新任务列表
        RefreshTaskList();
    }

    // 任务列表的
    private void RefreshTaskList()
    {
        NetGameTaskNode tasks = null;
    }
    public void Selected(TaskListItem taskListItem)
    {
        if (curTaskListItem != null)
        {
            curTaskListItem.CancelClick();
        }
        curTaskListItem = taskListItem;
        curTaskListItem.OnClick();

        RefreshTaskDetail();
    }
    private void RefreshTaskDetail()
    {
        TaskTitleText.text = "default";
        TaskDetailText.text = "default";
    }
}

using HS.Protobuf.GameTask;
using HSFramework.PoolModule;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TaskPanel : BasePanel, ICommonListSelectOpionMgr
{
    [Header("任务类型选项")]
    [SerializeField] private List<CommonListSelectOption> optionObjs;
    private CommonListSelectOption curTaskTypeOption;

    [Header("任务列表")]
    private string taskListItemPrefabPath = "UI/Prefabs/Task/TaskListItem.prefab";
    [SerializeField] private Transform content;
    private TaskListItem curTaskListItem;
    private List<TaskListItem> m_taskListItems;

    [Header("任务详情")]
    [SerializeField] private Transform TaskDetailBox;
    [SerializeField] private TextMeshProUGUI TaskTitleText;
    [SerializeField] private TextMeshProUGUI TaskDetailText;
    [SerializeField] private TextMeshProUGUI TaskCompelteText;
    [SerializeField] private TextMeshProUGUI TaskRewardText;

    [Header("其他的按钮")]
    [SerializeField] private CommonOption ExitBtn;
    [SerializeField] private CommonOption TakeTaskBtn;
    [SerializeField] private CommonOption TrackTaskBtn;
    [SerializeField] private CommonOption ReTakeTaskBtn;
    [SerializeField] private CommonOption ClaimRewardBtn;
    [SerializeField] private List<Transform> gameStatePart;
    private Dictionary<GameTaskState, Transform> gameStateBtnPartMap;
    private Transform curGameStateBtnPart;
    
    protected override void Start()
    {
        gameStateBtnPartMap = new();
        gameStateBtnPartMap.Add(GameTaskState.Locked,       gameStatePart[0]);
        gameStateBtnPartMap.Add(GameTaskState.Unlocked,     gameStatePart[1]);
        gameStateBtnPartMap.Add(GameTaskState.InProgress,   gameStatePart[2]);
        gameStateBtnPartMap.Add(GameTaskState.Completed,    gameStatePart[3]);
        gameStateBtnPartMap.Add(GameTaskState.Rewarded,     gameStatePart[4]);
        foreach (var item in gameStatePart)
        {
            item.gameObject.SetActive(false);
        }

        m_taskListItems = new();
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

        ExitBtn.Init(Close);
        TakeTaskBtn.Init(OnTakeTaskBtn);
        TrackTaskBtn.Init(OnTrackTaskBtn);
        ReTakeTaskBtn.Init(OnReTakeTaskBtn);
        ClaimRewardBtn.Init(OnClaimRewardBtn);
    }
    private void Update()
    {
        if (GameInputManager.Instance.UI_ESC)
        {
            Close();
        }
    }

    private void OnEnable()
    {
        Kaiyun.Event.RegisterIn("OneGameTaskInfoUpdate", this, "HandleOneGameTaskInfoUpdateEvent");
        Kaiyun.Event.RegisterIn("OneGameTaskInfoUpdate2", this, "HandleOneGameTaskInfoUpdate2Event");
    }
    private void OnDisable()
    {
        Kaiyun.Event.UnRegisterIn("OneGameTaskInfoUpdate", this, "HandleOneGameTaskInfoUpdateEvent");
        Kaiyun.Event.UnRegisterIn("OneGameTaskInfoUpdate2", this, "HandleOneGameTaskInfoUpdate2Event");
    }

    // 任务分类的
    public void InitOptions(List<CommonListSelectOption> options, List<string> optionNames, List<int> flags)
    {
        for (int i = 0; i < options.Count; ++i)
        {
            options[i].Init(this, optionNames[i], flags[i]);
        }

        // 默认选中第一个
        Selected(options[0]);
    }
    public void Selected(CommonListSelectOption commonSelectOption)
    {
        if (curTaskTypeOption != null)
        {
            curTaskTypeOption.CancelClick();
        }
        curTaskTypeOption = commonSelectOption;
        curTaskTypeOption.OnClick();

        // 刷新任务列表
        RefreshTaskList();
    }

    // 任务列表的
    private void RefreshTaskList()
    {
        var tasks = TaskDataManager.Instance.GetTaskByTaskType((GameTaskType)curTaskTypeOption.Flag);
        int curItemObjNum = m_taskListItems.Count;
        int needItemNum = 0;
        int defaultShowIdx = -1;

        // 进行中
        needItemNum++;
        tasks.TryGetValue(GameTaskState.InProgress, out var inProgress);
        tasks.TryGetValue(GameTaskState.Completed, out var completed);
        if (inProgress != null)
        {
            if(defaultShowIdx == -1)
            {
                defaultShowIdx = needItemNum;
            }
            needItemNum += inProgress.Count;
        }
        if(completed != null)
        {
            if (defaultShowIdx == -1)
            {
                defaultShowIdx = needItemNum;
            }
            needItemNum += completed.Count;
        }

        // 待接取
        needItemNum++;
        tasks.TryGetValue(GameTaskState.Unlocked, out var unLocked);
        if(unLocked != null)
        {
            if (defaultShowIdx == -1)
            {
                defaultShowIdx = needItemNum;
            }
            needItemNum += unLocked.Count;
        }

        // 未解锁
        needItemNum++;
        tasks.TryGetValue(GameTaskState.Locked, out var Locked);
        if (Locked != null)
        {
            if (defaultShowIdx == -1)
            {
                defaultShowIdx = needItemNum;
            }
            needItemNum += Locked.Count;
        }

        // 将itemObj增加或者删除
        if(needItemNum < curItemObjNum)
        {
            int returnNum = curItemObjNum - needItemNum;
            var returnList = m_taskListItems.GetRange(needItemNum, returnNum);
            m_taskListItems.RemoveRange(needItemNum, returnNum);
            foreach(var item in returnList)
            {
                UnityObjectPoolFactory.Instance.RecycleItem(taskListItemPrefabPath, item.gameObject);
            }
        }
        else if(needItemNum > curItemObjNum)
        {
            int addNum = needItemNum - curItemObjNum;
            for(int i = 0; i < addNum; ++i)
            {
                var obj = UnityObjectPoolFactory.Instance.GetItem<GameObject>(taskListItemPrefabPath);
                obj.transform.SetParent(content);
                var tli = obj.GetComponent<TaskListItem>();
                m_taskListItems.Add(tli);
            }
        }

        // 进行初始化
        int curIdx = 0;
        // 进行中
        m_taskListItems[curIdx].UnInit();
        m_taskListItems[curIdx].Init(TaskListItemMode.Tips, "进行中", null, null);
        curIdx++;
        if (inProgress != null)
        {
            foreach (var item in inProgress.Values)
            {
                m_taskListItems[curIdx].UnInit();
                m_taskListItems[curIdx].Init(TaskListItemMode.Normal, null, item, this);
                curIdx++;
            }
        }
        if (completed != null)
        {
            foreach (var item in completed.Values)
            {
                m_taskListItems[curIdx].UnInit();
                m_taskListItems[curIdx].Init(TaskListItemMode.Normal, null, item, this);
                curIdx++;
            }
        }

        // 待接取
        m_taskListItems[curIdx].UnInit();
        m_taskListItems[curIdx].Init(TaskListItemMode.Tips, "待接取", null, null);
        curIdx++;
        if (unLocked != null)
        {
            foreach (var item in unLocked.Values)
            {
                m_taskListItems[curIdx].UnInit();
                m_taskListItems[curIdx].Init(TaskListItemMode.Normal, null, item, this);
                curIdx++;
            }
        }

        // 未解锁
        m_taskListItems[curIdx].UnInit();
        m_taskListItems[curIdx].Init(TaskListItemMode.Tips, "未解锁", null, null);
        curIdx++;
        if (Locked != null)
        {
            foreach (var item in Locked.Values)
            {
                m_taskListItems[curIdx].UnInit();
                m_taskListItems[curIdx].Init(TaskListItemMode.Normal, null, item, this);
                curIdx++;
            }
        }

        curTaskListItem = null;
        if (needItemNum <= 3)
        {
            // 隐藏详情页即可
            RefreshTaskDetail();
        }
        else
        {
            // 默认选择第一个
            Selected(m_taskListItems[defaultShowIdx]);
        }
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
        if(curTaskListItem == null)
        {
            TaskDetailBox.gameObject.SetActive(false);
            curGameStateBtnPart?.gameObject.SetActive(false);
            curGameStateBtnPart = null;
        }
        else
        {
            TaskDetailBox.gameObject.SetActive(true);
            TaskTitleText.text = curTaskListItem.Title;
            TaskDetailText.text = curTaskListItem.Desc;
            // 任务解锁条件? 任务完成条件
            TaskCompelteText.text = curTaskListItem.Condition;
            // 任务奖励？
            TaskRewardText.text = curTaskListItem.Reward;
            // 相关的选项
            var btnPart = gameStateBtnPartMap[curTaskListItem.GameTaskState];
            curGameStateBtnPart?.gameObject.SetActive(false);
            btnPart.gameObject.SetActive(true);
            curGameStateBtnPart = btnPart;
        }
    }

    // tools
    public void Close()
    {
        UIManager.Instance.ClosePanel("TaskPanel");
        Kaiyun.Event.FireIn("CloseTaskPanel");
    }
    private void OnTakeTaskBtn()
    {
        if(curTaskListItem != null && curTaskListItem.GameTaskState == GameTaskState.Unlocked)
        {
            TaskHandler.Instance.SendTakeGameTaskRequest(curTaskListItem.TaskId);
        }
    }
    private void OnTrackTaskBtn()
    {
        UIManager.Instance.ShowTopMessage("杂鱼~，这个功能可没有完成哦，嚯嚯嚯！");
        if (curTaskListItem != null && curTaskListItem.GameTaskState == GameTaskState.InProgress)
        {
        }
    }
    private void OnReTakeTaskBtn()
    {
        if (curTaskListItem != null && curTaskListItem.GameTaskState == GameTaskState.InProgress)
        {
            TaskHandler.Instance.SendReTakeGameTaskRequest(curTaskListItem.TaskId);
        }
    }
    private void OnClaimRewardBtn()
    {
        if (curTaskListItem != null && curTaskListItem.GameTaskState == GameTaskState.Completed)
        {
            TaskHandler.Instance.SendClaimTaskRewardsRequest(curTaskListItem.TaskId);
        }
    }

    // event
    public void HandleOneGameTaskInfoUpdateEvent(int taskId)
    {
        if(curTaskListItem == null || curTaskListItem.TaskId != taskId)
        {
            goto End;
        }
        RefreshTaskDetail();
    End:
        return;
    }
    public void HandleOneGameTaskInfoUpdate2Event(GameTaskType type)
    {
        if(type != (GameTaskType)curTaskTypeOption.Flag)
        {
            goto End;
        }
        RefreshTaskList();
    End:
        return;
    }
}

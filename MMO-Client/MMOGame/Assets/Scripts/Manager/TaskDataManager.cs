using Google.Protobuf.Collections;
using HS.Protobuf.GameTask;
using HSFramework.MySingleton;
using Serilog;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 用于管理本地玩家的任务信息
public class TaskDataManager : SingletonNonMono<TaskDataManager>
{
    private bool m_IsInit;
    private Dictionary<GameTaskType,
                Dictionary<GameTaskState,
                    Dictionary<int, NetGameTaskNode>>> m_allTasksMap = new();
    private Dictionary<int, NetGameTaskNode> m_allTask = new();

    public bool Init()
    {
        m_allTasksMap.Add(GameTaskType.MainStory, new Dictionary<GameTaskState, Dictionary<int, NetGameTaskNode>>());
        m_allTasksMap.Add(GameTaskType.SideStory, new Dictionary<GameTaskState, Dictionary<int, NetGameTaskNode>>());
        m_allTasksMap.Add(GameTaskType.Common, new Dictionary<GameTaskState, Dictionary<int, NetGameTaskNode>>());

        // 发包获取对应信息
        TaskHandler.Instance.SendGetAllGameTasksRequest();

        return true;
    }
    public bool Clear()
    {
        m_allTask.Clear();
        foreach (var item in m_allTasksMap.Values)
        {
            item.Clear();
        }
        return true;
    }
    public void HandleGetAllGameTaskResponse(RepeatedField<NetGameTaskNode> alltasks)
    {
        m_IsInit = true;
        Clear();
        foreach (var item in alltasks)
        {
            LocalDataManager.Instance.m_taskDefineDict.TryGetValue(item.TaskId, out var def);
            if (def == null)
            {
                Log.Warning($"不存在该任务：taskId = {item.TaskId}");
                continue;
            }

            GameTaskType taskType = (GameTaskType)def.Task_type;
            m_allTasksMap.TryGetValue(taskType, out var tasks);
            if(tasks == null)
            {
                Log.Warning($"不存储当前任务类型：taskType = {taskType.ToString()}");
                continue;
            }

            GameTaskState gameTaskState = item.TaskState;
            tasks.TryGetValue(gameTaskState, out var dict);
            if(dict == null)
            {
                dict = new Dictionary<int, NetGameTaskNode>();
                tasks.Add(gameTaskState, dict);
            }
            dict.Add(item.TaskId, item);

            m_allTask.Add(item.TaskId, item);
        }
    }
    public Dictionary<GameTaskState, Dictionary<int, NetGameTaskNode>> GetTaskByTaskType(GameTaskType type)
    {
        m_allTasksMap.TryGetValue(type, out var tasks);
        return tasks;
    }
    public void ChangeGameTaskState(int taskId, GameTaskState newState, string newConditions = "")
    {
        if (!m_allTask.TryGetValue(taskId, out var task))
        {
            goto End;
        }
        // 移除
        var def = LocalDataManager.Instance.m_taskDefineDict[task.TaskId];
        var tasks = m_allTasksMap[(GameTaskType)def.Task_type];
        if (tasks == null)
        {
            Log.Warning($"不存储当前任务类型：taskType = {((GameTaskType)def.Task_type).ToString()}");
            goto End;
        }
        tasks.TryGetValue(task.TaskState, out var dict);
        if (dict == null)
        {
            Log.Warning($"不存储当前任务状态的列表：taskState = {(task.TaskState).ToString()}");
            goto End;
        }
        dict.Remove(task.TaskId);

        // 变更
        task.TaskState = newState;
        task.TaskProgress = newConditions;

        // 添加
        tasks.TryGetValue(task.TaskState, out var dict2);
        if(dict2 == null)
        {
            dict2 = new Dictionary<int, NetGameTaskNode>();
            tasks.Add(task.TaskState, dict2);
        }
        dict2.Add(task.TaskId, task);

        Kaiyun.Event.FireIn("OneGameTaskInfoUpdate2", taskId);
    End:
        return;
    }
    public void ChangeGameTaskConditions(int taskId, string newConditions)
    {
        if (!m_allTask.TryGetValue(taskId, out var task))
        {
            goto End;
        }
        task.TaskProgress = newConditions;
        Kaiyun.Event.FireIn("OneGameTaskInfoUpdate", taskId);
    End:
        return;
    }
    public void AddGameTask(NetGameTaskNode newNode)
    {
        if(!m_allTask.TryGetValue(newNode.TaskId, out var task))
        {
            goto End;
        }

        LocalDataManager.Instance.m_taskDefineDict.TryGetValue(newNode.TaskId, out var def);
        if (def == null)
        {
            Log.Warning($"不存在该任务：taskId = {newNode.TaskId}");
            goto End;
        }

        GameTaskType taskType = (GameTaskType)def.Task_type;
        m_allTasksMap.TryGetValue(taskType, out var tasks);
        if (tasks == null)
        {
            Log.Warning($"不存储当前任务类型：taskType = {taskType.ToString()}");
            goto End;
        }

        GameTaskState gameTaskState = newNode.TaskState;
        tasks.TryGetValue(gameTaskState, out var dict);
        if (dict == null)
        {
            dict = new Dictionary<int, NetGameTaskNode>();
            tasks.Add(gameTaskState, dict);
        }
        dict.Add(newNode.TaskId, newNode);
        m_allTask.Add(newNode.TaskId, newNode);

        Kaiyun.Event.FireIn("OneGameTaskInfoUpdate2", taskType);

    End:
        return;
    }
    public void RemoveGameTask(int taskId)
    {
        if (!m_allTask.TryGetValue(taskId, out var task))
        {
            goto End;
        }
        m_allTask.Remove(taskId);

        LocalDataManager.Instance.m_taskDefineDict.TryGetValue(taskId, out var def);
        GameTaskType taskType = (GameTaskType)def.Task_type;
        m_allTasksMap.TryGetValue(taskType, out var tasks);
        GameTaskState gameTaskState = task.TaskState;
        tasks.TryGetValue(gameTaskState, out var dict);
        dict.Remove(taskId);

        Kaiyun.Event.FireIn("OneGameTaskInfoUpdate2", taskType);
    End:
        return;
    }
}

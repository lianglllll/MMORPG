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
                    Dictionary<int, NetGameTaskNode>>> m_allTasks; 

    public bool Init()
    {
        m_allTasks = new();
        m_allTasks.Add(GameTaskType.MainStory, new Dictionary<GameTaskState, Dictionary<int, NetGameTaskNode>>());
        m_allTasks.Add(GameTaskType.SideStory, new Dictionary<GameTaskState, Dictionary<int, NetGameTaskNode>>());
        m_allTasks.Add(GameTaskType.Common, new Dictionary<GameTaskState, Dictionary<int, NetGameTaskNode>>());

        // 发包获取对应信息
        TaskHandler.Instance.SendGetAllGameTasksRequest();

        return true;
    }
    public bool Clear()
    {
        foreach(var item in m_allTasks.Values)
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
            m_allTasks.TryGetValue(taskType, out var tasks);
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
        }
    }
}

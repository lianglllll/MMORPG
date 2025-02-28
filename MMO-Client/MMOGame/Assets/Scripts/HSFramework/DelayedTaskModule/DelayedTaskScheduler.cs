using System;
using System.Collections.Generic;
using HSFramework.DataStruct;
using UnityEngine;
using HSFramework.MySingleton;
using HSFramework.PoolModule;

namespace HSFramework.MyDelayedTaskScheduler
{
    /// <summary>
    /// 延时任务调度器
    /// </summary>
    [DefaultExecutionOrder(1)]
    public class DelayedTaskScheduler : Singleton<DelayedTaskScheduler>, IDisposable
    {
        private bool _disposed = false;
        [SerializeField] private long CurrentTime;
        private Dictionary<string, DelayedTaskData> _taskDic = new Dictionary<string, DelayedTaskData>();
        private Dictionary<long, DelayedTaskList> _delayedTaskLists_Dict = new Dictionary<long, DelayedTaskList>();
        private Heap<DelayedTaskList> _delayedTaskLists_Queue = new Heap<DelayedTaskList>(10, HeapType.MinHeap);

        #region 时间事件管理

        /// <summary>
        /// 增加一个时间事件对象
        /// </summary>
        /// <param name="targetTime">毫秒数</param>
        /// <param name="action"></param>
        public string AddDelayedTask(long targetTime, Action action, Action earlyRemoveCallback = null)
        {
            //过时的时间，直接结束
            if (targetTime < CurrentTime)
            {
                Debug.LogError($"The time is pass. Time is {targetTime} CurrentTime is {CurrentTime}");
                return null;
            }

            //如果没注册过这个时间戳，则创建一个任务列表，并将任务列表加入到字典里
            if (!_delayedTaskLists_Dict.TryGetValue(targetTime, out var delayedTaskList))
            {
                delayedTaskList = ObjectPoolFactory.Instance.GetItem<DelayedTaskList>();

                delayedTaskList.Time = targetTime;

                delayedTaskList.DelayedTaskDataList = ObjectPoolFactory.Instance.GetItem<List<DelayedTaskData>>();
                delayedTaskList.DelayedTaskDataList.Clear();

                _delayedTaskLists_Queue.Insert(delayedTaskList);
                _delayedTaskLists_Dict.Add(targetTime, delayedTaskList);
            }

            string token = Guid.NewGuid().ToString();
            var delayedTaskData = ObjectPoolFactory.Instance.GetItem<DelayedTaskData>();
            delayedTaskData.Time = targetTime;
            delayedTaskData.Action = action;
            delayedTaskData.Token = token;
            delayedTaskData.EarlyRemoveCallback = earlyRemoveCallback;

            delayedTaskList.DelayedTaskDataList.Add(delayedTaskData);
            _taskDic.Add(token, delayedTaskData);
            return token;
        }
        public string AddDelayedTask(double lateTime, Action action, Action earlyRemoveCallback = null)
        {
            return AddDelayedTask(TimerUtil.GetLaterMilliSecondsBySecond(lateTime), action, earlyRemoveCallback);
        }

        /// <summary>
        /// 移除一个时间事件对象
        /// </summary>
        /// <param name="delayedTaskData"></param>
        /// <exception cref="Exception"></exception>
        public bool RemoveDelayedTask(string token)
        {
            if(!_taskDic.TryGetValue(token, out var delayedTaskData))
            {
                return false;
            }

            _taskDic.Remove(token);

            if (_delayedTaskLists_Dict.TryGetValue(delayedTaskData.Time, out var delayedTaskList))
            {

                bool removeSuccess = delayedTaskList.DelayedTaskDataList.Remove(delayedTaskData);
                if (removeSuccess)
                {
                    delayedTaskData.EarlyRemoveCallback?.Invoke();
                }

                
                if (delayedTaskList.DelayedTaskDataList.Count == 0)
                {
                    _delayedTaskLists_Dict.Remove(delayedTaskData.Time);
                    if (_delayedTaskLists_Queue.Delete(delayedTaskList))
                    {
                        ObjectPoolFactory.Instance.RecycleItem(delayedTaskList.DelayedTaskDataList);
                        ObjectPoolFactory.Instance.RecycleItem(delayedTaskList);
                        ObjectPoolFactory.Instance.RecycleItem(delayedTaskData);
                    }
                    else
                    {
                        ObjectPoolFactory.Instance.RecycleItem(delayedTaskData);
                        throw new Exception("DelayedTaskScheduler RemoveDelayedTask Error");
                    }
                }
            }
            else
            {
                ObjectPoolFactory.Instance.RecycleItem(delayedTaskData);
            }

            return true;
        }

        /// <summary>
        /// TODO:根据自己游戏的逻辑调整调用时机
        /// </summary>
        /// <param name="time"></param>
        public void UpdateTime(long time)
        {
            CurrentTime = time;
            while (_delayedTaskLists_Queue.Count > 0 && _delayedTaskLists_Queue.GetHead().Time <= CurrentTime)
            {
                long targetTime = _delayedTaskLists_Queue.GetHead().Time;
                _delayedTaskLists_Dict.Remove(targetTime);
                var delayedTaskList = _delayedTaskLists_Queue.DeleteHead();
                foreach (DelayedTaskData delayedTaskData in delayedTaskList)
                {
                    delayedTaskData.Action?.Invoke();
                    _taskDic.Remove(delayedTaskData.Token);
                    ObjectPoolFactory.Instance.RecycleItem(delayedTaskData);
                }

                //回收时记得把列表清空，防止下次使用时出现问题！！！！！不要问我为什么这么多感叹号 
                delayedTaskList.DelayedTaskDataList.Clear();
                ObjectPoolFactory.Instance.RecycleItem(delayedTaskList.DelayedTaskDataList);
                ObjectPoolFactory.Instance.RecycleItem(delayedTaskList);
            }
        }

        #endregion

        #region Mono方法

        protected override void Awake()
        {
            base.Awake();
            UpdateTime(TimerUtil.GetTimeStamp(true));
        }

        public void Update()
        {
            UpdateTime(TimerUtil.GetTimeStamp(true));
        }

        private void OnDestroy()
        {
            Dispose();
        }

        #endregion

        #region Dispose

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _taskDic.Clear();
                    _delayedTaskLists_Dict.Clear();
                    _delayedTaskLists_Queue?.Dispose();
                }

                _disposed = true;
            }
        }

        ~DelayedTaskScheduler()
        {
            Dispose(false);
        }

        #endregion
    }
}

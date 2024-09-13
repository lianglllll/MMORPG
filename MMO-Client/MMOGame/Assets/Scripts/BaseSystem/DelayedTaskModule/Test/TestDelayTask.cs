using System;
using System.Collections.Generic;
using System.Diagnostics;
using BaseSystem.MyDelayedTaskScheduler;
using BaseSystem.PoolModule;
using JetBrains.Annotations;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace BaseSystem.MyDelayedTaskScheduler
{
    public class TestDelayTask : MonoBehaviour
    {
        private void Start()
        {
            AddLaterExecuteFunc(2f);
           //AddLaterExecuteFunc(1.5f);
           //AddLaterExecuteFunc(1.5f);
           //AddLaterExecuteFunc(4.5f);
           //AddLaterExecuteFunc(4.5f);
           //AddLaterExecuteFunc(4.5f);
        }
        
        public int forceTestCount = 100000;
        List<string> futureEventDataList = new List<string>(100000);
        List<long> testTimes = new List<long>(100000);
        
        [ContextMenu("暴力测试")]
        public void ForceTest()
        {
            testTimes.Clear();
            for (int i = 0; i < forceTestCount; i++)
            {
                testTimes.Add(TimerUtil.GetLaterMilliSecondsBySecond(UnityEngine.Random.Range(1, 15.0f)));
            }
            futureEventDataList.Clear();

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            for (int i = 0; i < forceTestCount; i++)
            {
                futureEventDataList.Add(DelayedTaskScheduler.Instance.AddDelayedTask(testTimes[i], TestFunc));
            }

            for (int i = 0; i < forceTestCount; i++)
            {
                DelayedTaskScheduler.Instance.RemoveDelayedTask(futureEventDataList[i]);
            }
            
            stopwatch.Stop();
            Debug.Log($"暴力测试完成，共耗时{stopwatch.ElapsedMilliseconds / 1000.0f}秒");
        }

        void TestFunc()
        {
            Debug.Log("测试方法执行了");
        }

        private string AddLaterExecuteFunc(float time, Action completeAction = null, Action earlyRemoveAction = null)
        {
            var pressTime = Time.time;
            Stopwatch stopwatch = ObjectPoolFactory.Instance.GetItem<Stopwatch>();
            stopwatch.Restart();
            return DelayedTaskScheduler.Instance.AddDelayedTask(TimerUtil.GetLaterMilliSecondsBySecond(time),
                () =>
                {
                    stopwatch.Stop();
                    ObjectPoolFactory.Instance.RecycleItem(stopwatch);
                    // Debug.Log($"{time}秒后了,执行了对应方法。实际过去了{Time.time - pressTime}秒");
                    Debug.Log($"{time}秒后了,执行了对应方法。实际过去了{stopwatch.ElapsedMilliseconds / 1000.0f}秒");
                    completeAction?.Invoke();
                }, () =>
                {
                    earlyRemoveAction?.Invoke();
                    stopwatch.Stop();
                    Debug.Log($"提前移除了，已经过去了{stopwatch.ElapsedMilliseconds / 1000.0f}秒");
                    ObjectPoolFactory.Instance.RecycleItem(stopwatch);
                });
        }

        [CanBeNull] string _delayedTaskDataToken;

        void RecycleDelayedTask()
        {
            if (_delayedTaskDataToken != null)
            {
                DelayedTaskScheduler.Instance.RemoveDelayedTask(_delayedTaskDataToken);
                _delayedTaskDataToken = null;
            }
        }

        private void Update()
        {
            //持续按下时不断创建和回收
            if (Input.GetKeyDown(KeyCode.C))
            {
                RecycleDelayedTask();
                _delayedTaskDataToken = AddLaterExecuteFunc(5f,
                    () => _delayedTaskDataToken = null);
            }

            if (Input.GetKeyDown(KeyCode.R))
            {
                RecycleDelayedTask();
            }
        }
    }
}

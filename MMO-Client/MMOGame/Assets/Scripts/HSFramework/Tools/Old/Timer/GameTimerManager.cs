using HSFramework.Singleton;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



/// <summary>
/// 管理所有的计时器
/// </summary>
public class GameTimerManager : Singleton<GameTimerManager>
{
    //note:开始的时候我们要创建一些计时器，否则我们的空闲计时器中一个计时器都没有
    //1.有一个集合用来保存所有的空闲计时器
    //2.有一个集合用来保存当前正在工作的计时器
    //3.更新当前工作中的计时器
    //4.当某个计时器工作完成后，我们需要把它回收到空闲计时器集合中

    [SerializeField] private int _initMaxTimerCount;

    private Queue<GameTimer> _notWorkerTimer = new Queue<GameTimer>(); 
    private List<GameTimer> _workeringTimer = new List<GameTimer>();

    private void Start()
    {
        InitTimerManager();
    }

    private void Update()
    {
        UpdateWorkeringTimer();
    }

    private void InitTimerManager()
    {
        for(int i = 0; i < _initMaxTimerCount; i++)
        {
            CreateTimer();
        }
    }

    private void CreateTimer()
    {
        var timer = new GameTimer();
        _notWorkerTimer.Enqueue(timer);
    }

    public void TryUseOneTimer(float time,Action task)
    {
        if(_notWorkerTimer.Count == 0)
        {
            CreateTimer();
            var timer = _notWorkerTimer.Dequeue();
            timer.StartTimer(time, task);
            _workeringTimer.Add(timer);
        }
        else
        {
            var timer = _notWorkerTimer.Dequeue();
            timer.StartTimer(time, task);
            _workeringTimer.Add(timer);
        }
    }

    private void UpdateWorkeringTimer()
    {
        if (_workeringTimer.Count == 0) return;
        for(int i = 0; i < _workeringTimer.Count; i++)
        {
            if(_workeringTimer[i].GetTimerState() == TimerState.WORKERING)
            {
                _workeringTimer[i].UpdateTimer();
            }
            else
            {
                //任务完成了
                _notWorkerTimer.Enqueue(_workeringTimer[i]);
                _workeringTimer[i].ResetTimer();
                _workeringTimer.Remove(_workeringTimer[i]);
            }
        }
    }

}

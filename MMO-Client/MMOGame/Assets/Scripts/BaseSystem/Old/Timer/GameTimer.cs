using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 计时器状态
/// </summary>
public enum TimerState
{
    NOTWORKERE,//没有工作
    WORKERING,//工作中
    DONE,     //工作完成
}


public class GameTimer
{
    //1.计时时长
    //2.计时结束后执行的任务
    //3.当前计时器的状态
    //4.是否停止当前计时器

    private float _startTime;
    private Action _task;
    private bool _isStopTimer;
    private TimerState _timerState;

    public GameTimer()
    {
        ResetTimer();
    }

    //1.开始计时
    public void StartTimer(float time,Action task)
    {
        _startTime = time;
        _task = task;
        _isStopTimer = false;
        _timerState = TimerState.WORKERING;
    }

    //2.更新计时器
    public void UpdateTimer()
    {
        if (_isStopTimer) return;
        _startTime -= Time.deltaTime;
        if(_startTime < 0f)
        {
            _task?.Invoke();
            _timerState = TimerState.DONE;
            _isStopTimer = true;
        }
    }

    //3.确定定时器状态
    public TimerState GetTimerState() => _timerState;


    //4.重置计时器
    public void ResetTimer()
    {
        _startTime = 0f;
        _task = null;
        _isStopTimer = true;
        _timerState = TimerState.NOTWORKERE;
    }


}

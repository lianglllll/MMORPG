using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static TimeMachineMgr;

public class ReplayMgr : MonoBehaviour
{
    private bool isStart;
    private LinkedList<TimerInfo> _timeStateList;
    private CtlStateMachine stateMachine;
    private Action EndAction;

    public ReplayMgr()
    {
    }

    public void Init(GameObject ctlChr, LinkedList<TimerInfo> timeStateList,Action action = null)
    {
        _timeStateList = timeStateList;
        stateMachine = transform.GetComponent<CtlStateMachine>();
        if(action != null)
        {
            EndAction += action;
        }
        RePlayStart();
    }

    private void FixedUpdate()
    {
        if (isStart)
        {
            RePlay();
        }
    }

    private void RePlayStart()
    {
        isStart = true;
    }

    /// <summary>
    /// 重放
    /// </summary>
    private void RePlay()
    {
        if (_timeStateList.Count > 0)
        {
            TimerInfo info = _timeStateList.First.Value;
            transform.position = info._pos;
            transform.rotation = info._rot;
            stateMachine.SwitchState(info._state);
            if (info._eventId != 0)
            {
                //触发交互触发器，来触发某某交互事件
                InteractionManager.Instance.GetInteration(info._eventId).InteractionAction();
            }
            _timeStateList.RemoveFirst();
        }
        else
        {
            RePlayEnd();
        }
    }

    private void RePlayEnd()
    {
        isStart = false;
        //使用事件通知上级，重放完成
        EndAction?.Invoke();
    }

}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public class TimeMachineManager : MonoBehaviour
{
    /// <summary>
    /// 当前时间，游戏对象的状态
    /// </summary>
    public class TimerInfomation
    {
        public Vector3 _pos;
        public Quaternion _rot;
        //Hp
        //animation key frame


        public TimerInfomation(Vector3 pos,Quaternion rot)
        {
            this._pos = pos;
            this._rot = rot;
        }
    }

    //使用双向循环链表模拟栈
    private LinkedList<TimerInfomation> _timeStateList = new LinkedList<TimerInfomation>();
    public float RecordTime = 5f;           //记录时间状态的最大时间
    private bool _isBacktrack = false;      //是否回溯
    private Rigidbody _rb;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            StartBacktrack();
        }
        if (Input.GetMouseButtonUp(0))
        {
            StopBacktrack();
        }
    }
    private void FixedUpdate()
    {
        if (_isBacktrack)
        {
            Backtrack();
        }
        else
        {
            ReCord();
        }
    }


    private void StartBacktrack()
    {
        _isBacktrack = true;
        _rb.useGravity = false;
        _rb.isKinematic = true;//运动学
    }

    private void StopBacktrack()
    {
        _isBacktrack = false;
        _rb.useGravity = true;
        _rb.isKinematic = false;
    }

    /// <summary>
    /// 记录时间状态
    /// </summary>
    private void ReCord()
    {
        //删除过期的状态
        if(_timeStateList.Count > Mathf.Round(RecordTime / Time.deltaTime))
        {
            _timeStateList.RemoveLast();
        }

        //记录当前帧的游戏对象的状态
        _timeStateList.AddFirst(new TimerInfomation(transform.position, transform.rotation));
    }

    /// <summary>
    /// 回溯
    /// </summary>
    private void Backtrack()
    {
        if(_timeStateList.Count > 0)
        {
            TimerInfomation info = _timeStateList.First.Value;
            transform.position = info._pos;
            transform.rotation = info._rot;
            _timeStateList.RemoveFirst();
        }
    }


}

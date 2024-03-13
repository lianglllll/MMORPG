using Proto;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TimeMachineMgr : MonoBehaviour
{
    /// <summary>
    /// 当前时间，游戏对象的状态
    /// </summary>
    public struct TimerInfo
    {
        public readonly Vector3 _pos;
        public readonly Quaternion _rot;
        public readonly EntityState _state;
        public readonly int _eventId; // 触发某某事件的id

        public TimerInfo(Vector3 pos, Quaternion rot, EntityState state, int eventId = 0)
        {
            this._pos = pos;
            this._rot = rot;
            this._state = state;
            this._eventId = eventId;
        }
    }


    private LinkedList<TimerInfo> _timeStateList = new LinkedList<TimerInfo>();
    public float MaxRecordTime = 10f;               //记录时间状态的最大时间
    private float curRecordTime = 0f;               //当前的记录时间
    private bool isRecord = false;                  //是否记录
    private bool isReplay = false;                  //是否记录

    private CtlStateMachine stateMachine;

    private int cureventId = 0;
    private Button recordBtn;
    private Button replayBtn;
    private TextMeshProUGUI text;

    public GameObject prefab;

    //当前的幻影实例
    private GameObject phantomObj;

    private void Awake()
    {
        stateMachine = transform.GetComponent<CtlStateMachine>();
        var canvas = GameObject.Find("Canvas").transform;
        recordBtn = canvas.Find("RecordButton").GetComponent<Button>();
        replayBtn = canvas.Find("ReplayButton").GetComponent<Button>();
        text = canvas.Find("TipsBox/Text").GetComponent<TextMeshProUGUI>();
    }

    private void Start()
    {
        recordBtn.onClick.AddListener(RecordStart);
        replayBtn.onClick.AddListener(ReplayStart);
        Kaiyun.Event.RegisterIn("Interaction", this, "TriggerEvent");

    }

    private void OnDestroy()
    {
        Kaiyun.Event.UnregisterIn("Interaction", this, "TriggerEvent");

    }

    private void FixedUpdate()
    {
        if (isRecord)
        {
            ReCord();
        }
    }

    public void TriggerEvent(int eventId)
    {
        cureventId = eventId;
    }



    /// <summary>
    /// 开始记录
    /// </summary>
    private void RecordStart()
    {
        isRecord = true;
        curRecordTime = 0f;
        text.color = Color.green;
        text.text = "开始记录";
    }
    /// <summary>
    /// 记录时间状态
    /// </summary>
    private void ReCord()
    {
        //记录当前帧的游戏对象的状态
        _timeStateList.AddLast(new TimerInfo(transform.position, transform.rotation,stateMachine.currentEntityState, cureventId));
        cureventId = 0;

        //记录时间的限制
        curRecordTime += Time.deltaTime;
        text.text = $"开始记录:{Math.Round(curRecordTime, 1)}";
        if (curRecordTime>= MaxRecordTime)
        {
            RecordEnd();
        }
    }
    /// <summary>
    /// 记录结束
    /// </summary>
    private void RecordEnd()
    {
        isRecord = false;
        text.text = "结束记录";
    }



    /// <summary>
    /// 开始重发
    /// </summary>
    private void ReplayStart()
    {
        if (isRecord || isReplay || _timeStateList.Count <= 0) return;

        isReplay = true;
        //实例化对象
        var pos = _timeStateList.First.Value._pos;
        var rot = _timeStateList.First.Value._rot;
        phantomObj = GameObject.Instantiate(prefab, pos, rot);

        //添加重发脚本开始重放
        var rpmgr = phantomObj.AddComponent<ReplayMgr>();
        rpmgr.Init(phantomObj, _timeStateList, ReplayEnd);

        //放弃_timeStateList的所有权
        _timeStateList = new LinkedList<TimerInfo>();

        //
        text.color = Color.blue;
        text.text = "重放开始";

    }

    /// <summary>
    /// 重发结束
    /// </summary>
    private void ReplayEnd()
    {
        isReplay = false;

        //这里可以加特性之类的
        //消除实例
        GameObject.Destroy(phantomObj,5f);

        text.color = Color.blue;
        text.text = "重放结束";

    }

}

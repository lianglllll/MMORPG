using Proto;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TimeMachineMgr2 : MonoBehaviour
{
    /// <summary>
    /// 当前时间，游戏对象的状态
    /// </summary>
    public struct TimerInfo2
    {
        public readonly long Frame;
        public readonly Vector2 Movement;
        public readonly Vector3 dir;
        public readonly int EventId; // 触发某某事件的id
        public readonly float deltaTime;

        public TimerInfo2(long frame,Vector2 movement,Vector3 dir, float deltaTime, int eventId = 0)
        {
            this.Frame = frame;
            this.Movement = movement;
            this.dir = dir;
            this.deltaTime = deltaTime;
            this.EventId = eventId;
        }
    }

    private LinkedList<TimerInfo2> _timeStateList = new LinkedList<TimerInfo2>();
    public float MaxRecordTime = 10f;               //记录时间状态的最大时间
    private float curRecordTime = 0f;               //当前的记录时间
    private bool isRecord = false;                  //是否记录
    private bool isReplay = false;                  //是否记录

    private long curFrame;                          //当前帧
    private CtlStateMachine stateMachine;           //状态机
    private int cureventId = 0;                     //当前触发的时间id
    public GameObject phantomPrefab;                //幻影实体预制件
    private GameObject phantomObj;                  //当前的幻影实例
    private Vector3 phantomInitPos;             
    private Quaternion phantomInitDir;

    private Transform _curcamera;
    public Transform CurCamera
    {
        get
        {
            if(_curcamera == null)
            {
                _curcamera = Camera.main.transform;
            }
            return _curcamera;
        }
    }

    //一些调试ui
    private Button recordBtn;
    private Button replayBtn;
    private TextMeshProUGUI text;

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

    private void Update()
    {

    }

    private void LateUpdate()
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
        curFrame = 0;               //重置当前帧
        _timeStateList.Clear();


        //记录幻影起始点
        phantomInitPos = transform.position;
        phantomInitDir = transform.rotation;

    }
    /// <summary>
    /// 记录时间状态
    /// </summary>
    private void ReCord()
    {
        //记录当前帧的游戏对象的状态
        Vector2 movement = GameInputManager.Instance.Movement;
        Vector3 dir = transform.forward.normalized;

        _timeStateList.AddLast(new TimerInfo2(curFrame++,movement, dir, Time.deltaTime,cureventId));

        //重置
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
        phantomObj = GameObject.Instantiate(phantomPrefab, phantomInitPos, phantomInitDir);

        //添加重发脚本开始重放
        var syncMovement = phantomObj.AddComponent<SyncMovementController>();
        syncMovement.StartSync(this, ReplayEnd);

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


    /// <summary>
    /// 获取帧数据
    /// </summary>
    /// <returns></returns>
    public TimerInfo2 GetFrameInfo()
    {
        if(_timeStateList.Count > 0)
        {
            var info = _timeStateList.First.Value;
            _timeStateList.RemoveFirst();
            return info;
        }
        TimerInfo2 falseRes = new TimerInfo2(-1, Vector3.zero, Vector3.zero,0f);
        return falseRes;
    }

}

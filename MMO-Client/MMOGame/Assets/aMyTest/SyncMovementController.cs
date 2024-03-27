using GameClient.Entities;
using Proto;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static TimeMachineMgr2;

public class SyncMovementController : MonoBehaviour
{
    private CharacterController characterController;
    private CtlStateMachine stateMachine;

    private bool isStart;
    private TimeMachineMgr2 timeMachineMgr;
    private Action syncEnd;

    private float rotationSpeed = 8f;
    public float CurrentSpeed
    {
        get
        {
            return 3f;
        }
    }


    private void Awake()
    {
        isStart = false;
        stateMachine = GetComponent<CtlStateMachine>();
        characterController = GetComponent<CharacterController>();
    }

    void Update()
    {
        if (isStart)
        {
            _Move();
        }
    }


    public void StartSync(TimeMachineMgr2 timeMachineMgr, Action syncEnd)
    {
        this.timeMachineMgr = timeMachineMgr;
        this.syncEnd = syncEnd;
        isStart = true;
    }

    /// <summary>
    /// 角色移动
    /// </summary>
    private void _Move()
    {
        //从时间装置中获取本帧数据
        var info = timeMachineMgr.GetFrameInfo();
        if(info.Frame < 0)
        {
            isStart = false;
            stateMachine.SwitchState(EntityState.Idle);
            //可以事件回调，此次同步已经完成
            syncEnd?.Invoke();
            return;
        }

        //控制英雄移动
        float h =  info.Movement.x;
        float v = info.Movement.y;
        if (h != 0 || v != 0)
        {
            //播放跑步动画,设置motion需要的speed参数
            stateMachine.SwitchState(EntityState.Motion);
            if (stateMachine.currentEntityState == EntityState.Motion)
            {
                // 插值计算目标旋转方向
                Quaternion targetRotation = Quaternion.LookRotation(info.dir);

                // 平滑地调整角色旋转
                gameObject.transform.rotation = Quaternion.Lerp(gameObject.transform.rotation, targetRotation, info.deltaTime * rotationSpeed);

                // 移动角色
                characterController.Move(info.dir * CurrentSpeed * info.deltaTime);
            }
        }
        else
        {
            //播放待机动画
            stateMachine.SwitchState(EntityState.Idle);
        }
    }

}

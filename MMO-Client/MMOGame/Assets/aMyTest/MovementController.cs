using GameClient.Entities;
using Proto;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementController : MonoBehaviour
{
    private CharacterController characterController;
    private CtlStateMachine stateMachine;
    private Transform mainCamera;

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
        stateMachine = GetComponent<CtlStateMachine>();
        characterController = GetComponent<CharacterController>();
        mainCamera = Camera.main.transform;
    }

    void Update()
    {
        _Move();
    }

    /// <summary>
    /// 角色移动
    /// </summary>
    private void _Move()
    {
        //控制英雄移动
        float h = 0;
        float v = 0;
        if (h == 0) h = Input.GetAxis("Horizontal");
        if (v == 0) v = Input.GetAxis("Vertical");
        if (h != 0 || v != 0)
        {
            //播放跑步动画,设置motion需要的speed参数
            stateMachine.SwitchState(EntityState.Motion);
            if (stateMachine.currentEntityState == EntityState.Motion)
            {
                //摇杆控制英雄沿着摄像机的方向移动

                // 计算移动方向
                Vector3 dir = mainCamera.forward * v + mainCamera.right * h;
                dir.y = 0;
                dir.Normalize();

                // 插值计算目标旋转方向
                Quaternion targetRotation = Quaternion.LookRotation(dir);

                // 平滑地调整角色旋转
                gameObject.transform.rotation = Quaternion.Lerp(gameObject.transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);

                // 移动角色
                characterController.Move(dir * CurrentSpeed * Time.deltaTime);
            }
        }
        else
        {
            //播放待机动画
            stateMachine.SwitchState(EntityState.Idle);
        }
    }

}

using GameClient.Entities;
using Proto;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementController : MonoBehaviour
{
    private Actor owner;
    private CharacterController characterController;
    private PlayerStateMachine stateMachine;
    private GameEntity gameEntity;
    private float rotationSpeed = 8f;
    private Transform mainCamera;

    public float CurrentSpeed => owner.Speed *0.001f;
    private float acceleration = 5f;//加速度
    private float maxSpeed => owner.Speed;


    private void Awake()
    {
        stateMachine = GetComponent<PlayerStateMachine>();
        characterController = GetComponent<CharacterController>();
        gameEntity = GetComponent<GameEntity>();
        mainCamera = Camera.main.transform;


    }

    private void Start()
    {
        //CurrentSpeed = 3f;


    }

    private void OnDestroy()
    {
    }

    void Update()
    {
        _Move();
    }

    public void Init(Actor owner)
    {
        this.owner = owner;
    }

    /// <summary>
    /// 角色移动
    /// </summary>
    private void _Move()
    {
        if (stateMachine.IsSpecialState()) return;
        if (GameApp.IsInputtingChatBox) return;//todo 耦合度太高，我们使用inputsystem切换来解决这个问题

        //控制英雄移动
        float h = 0;
        float v = 0;
        if (h == 0) h = Input.GetAxis("Horizontal");
        if (v == 0) v = Input.GetAxis("Vertical");
        if (h != 0 || v != 0)
        {
            //CurrentSpeed += acceleration * Time.deltaTime;
            //CurrentSpeed =  MathF.Min(CurrentSpeed, maxSpeed);

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
            //CurrentSpeed = 0f;
            stateMachine.SwitchState(EntityState.Idle);
        }
    }



}

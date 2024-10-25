using GameClient;
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

    //自动移动需要的
    private Vector3 targetPos;
    private bool isMoveTo;
    private Action moveToOverAction;

    private void Awake()
    {
        stateMachine = GetComponent<PlayerStateMachine>();
        characterController = GetComponent<CharacterController>();
        gameEntity = GetComponent<GameEntity>();
        mainCamera = Camera.main.transform;

    }

    private void Start()
    {
        isMoveTo = false;
    }

    private void OnDestroy()
    {
    }

    void Update()
    {

        if (isMoveTo)
        {
            MoveTo();
        }

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
        float h = GameInputManager.Instance.Movement.x;
        float v = GameInputManager.Instance.Movement.y;
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

            //如果使用鼠标操作了，就停止自动移动了
            if (isMoveTo)
            {
                isMoveTo = false;
            }
        }
        else
        {
            //播放待机动画
            //CurrentSpeed = 0f;
            if (!isMoveTo) {
                stateMachine.SwitchState(EntityState.Idle);
            }
        }




    }


    /// <summary>
    /// 移动到某个点
    /// </summary>
    /// <param name="pos"></param>
    public void MoveToPostion(Vector3 pos,Action action = null)
    {
        this.targetPos = pos;
        moveToOverAction = action;
        isMoveTo = true;
    }
    private void MoveTo()
    {
        if(Vector3.Distance(targetPos,transform.position) < 0.1f)
        {
            isMoveTo = false;
            transform.position = targetPos;
            stateMachine.SwitchState(EntityState.Idle);
            moveToOverAction?.Invoke();
        }
        else
        {
            Vector3 dir = targetPos - transform.position;
            dir.y = 0;
            dir.Normalize();
            // 插值计算目标旋转方向
            Quaternion targetRotation = Quaternion.LookRotation(dir);
            // 平滑地调整角色旋转
            gameObject.transform.rotation = Quaternion.Lerp(gameObject.transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
            characterController.Move(dir * CurrentSpeed * Time.deltaTime);

            stateMachine.SwitchState(EntityState.Motion);
        }
    }

}

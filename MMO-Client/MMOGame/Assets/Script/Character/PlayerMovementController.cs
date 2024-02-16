using GameClient.Entities;
using Proto;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementController : MonoBehaviour
{
    private CharacterController characterController;
    private PlayerStateMachine stateMachine;
    public float CurrentSpeed;
    private float Walkpeed;
    private CameraManager cameraManager;

    private void Awake()
    {
        stateMachine = GetComponent<PlayerStateMachine>();
        characterController = GetComponent<CharacterController>();
        cameraManager = GetComponent<CameraManager>();
    }

    private void Start()
    {
        Walkpeed = 2f;
    }

    void Update()
    {
        _Move();
        SelectTargetObject();
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
            //播放跑步动画
            stateMachine.SwitchState(ActorState.Walk);
            if (stateMachine.currentActorState == ActorState.Walk)
            {
                //摇杆控制英雄沿着摄像机的方向移动
                Vector3 dir = cameraManager.rCamera.transform.forward * v + cameraManager.rCamera.transform.right * h;
                dir.y = 0;
                dir.Normalize();
                //hero.transform.position += dir * speed * Time.deltaTime;
                characterController.Move(dir * Walkpeed * Time.deltaTime);
                gameObject.transform.forward = dir;
            }
        }
        else
        {

            //播放待机动画
            stateMachine.SwitchState(ActorState.Idle);
 
        }
    }

    /// <summary>
    /// 选择一个目标
    /// </summary>
    public void SelectTargetObject()
    {

        //选择目标
        if (Input.GetMouseButtonDown(0))  // 当鼠标左键被按下
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);  // 从鼠标点击位置发出一条射线
            RaycastHit hitInfo;  // 存储射线投射结果的数据
            LayerMask actorLayer = LayerMask.GetMask("Actor");
            if (Physics.Raycast(ray, out hitInfo, Mathf.Infinity, actorLayer))  // 检测射线是否与特定图层的物体相交
            {
                GameObject clickedObject = hitInfo.collider.gameObject;  // 获取被点击的物体
                                                                         // 在这里可以对获取到的物体进行处理
                Debug.Log("选择目标: " + clickedObject.name);

                int entityId = clickedObject.GetComponent<GameEntity>().entityId;
                GameApp.target = EntityManager.Instance.GetEntity<Actor>(entityId);
                Kaiyun.Event.FireOut("SelectTarget");
            }
        }else if (Input.GetKeyDown(KeyCode.Space))
        {
            GameApp.target = null;
            Kaiyun.Event.FireOut("CancelSelectTarget");
        }
    }

}

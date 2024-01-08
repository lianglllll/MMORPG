using GameClient.Entities;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroController : MonoBehaviour
{
    private GameObject hero;
    public float speed = 3f; //每秒移动的距离

    private CharacterController characterController;


    //是否调整视角
    public bool AdjustView { get; set; }
    //是否调整距离
    public bool AdjustDistance { get; set; }

    private Camera rCamera;


    //记录摄像机相对位置
    Vector3 offset;

    //动画片段切换,相当于简陋的状态机
    HeroAnimations anim;

    void Start()
    {
        rCamera = Camera.main;
        hero = this.gameObject;
        //摄像机移动到英雄后方且对准英雄
        rCamera.transform.position = hero.transform.position - hero.transform.forward * 8 + Vector3.up * 3;
        rCamera.transform.LookAt(hero.transform);
        offset = rCamera.transform.position - hero.transform.position;
        anim = GetComponent<HeroAnimations>();
        characterController = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        rCamera = Camera.main;
        rCamera.transform.LookAt(hero.transform);

        //攻击
        if (Input.GetKeyDown(KeyCode.Space))
        {
            anim.PlayAttack1();
        }


        //摄像机跟随英雄移动
        rCamera.transform.position = hero.transform.position + offset;

        //鼠标滚轮控制摄像机距离英雄的距离
        float wheel = Input.GetAxis("Mouse ScrollWheel");
        if (wheel != 0)
        {
            rCamera.transform.position += rCamera.transform.forward * 1.5f * wheel;
            offset = rCamera.transform.position - hero.transform.position;
        }

        //鼠标右键控制摄像机绕英雄旋转
        if (Input.GetMouseButton(1))
        {
            //Debug.Log("Mouse 0");
            float x = Input.GetAxis("Mouse X");
            float y = Input.GetAxis("Mouse Y");
            rCamera.transform.RotateAround(hero.transform.position, Vector3.up, x * 2);
            rCamera.transform.RotateAround(hero.transform.position, rCamera.transform.right, -y * 2);
            offset = rCamera.transform.position - hero.transform.position;
        }

        //offset最大距离为20
        offset = Vector3.ClampMagnitude(offset, 20);

        this.speed = GetComponent<GameEntity>().speed;


        HeroMove();


        //用射线检测摄像机与英雄之间是否有障碍物,如果有需要将摄像机位置重置
        RaycastHit hit;
        int index = LayerMask.NameToLayer("Actor"); 
        LayerMask layerMask = 1<<LayerMask.NameToLayer("Actor");

        //射线检测摄像机和角色之间不能有除actor以外的东西
        if (Physics.Linecast(hero.transform.position + Vector3.up * 0.5f, rCamera.transform.position - Vector3.up * 0.3f, out hit,~layerMask))
        {
            //临时移动摄像机到障碍物的位置上面一丢丢
            rCamera.transform.position = hit.point + Vector3.up * 0.5f;
        }


        SelectTargetObject();
    }

    private void HeroMove()
    {
        if (GameApp.IsInputtingChatBox) return;//todo 耦合度太高，我们使用inputsystem切换来解决这个问题
        //控制英雄移动
        float h = 0;
        float v = 0;
        if (h == 0) h = Input.GetAxis("Horizontal");
        if (v == 0) v = Input.GetAxis("Vertical");
        if (h != 0 || v != 0)
        {
            //播放跑步动画
            anim.PlayRun();
            if (anim.state == HeroAnimations.HState.Run)
            {
                //摇杆控制英雄沿着摄像机的方向移动
                Vector3 dir = rCamera.transform.forward * v + rCamera.transform.right * h;
                dir.y = 0;
                dir.Normalize();
                //hero.transform.position += dir * speed * Time.deltaTime;
                characterController.Move(dir * speed * Time.deltaTime);
                hero.transform.forward = dir;
            }
        }
        else
        {
            //播放待机动画
            anim.PlayIdle();
        }
    }

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
            }
        }
    }


}

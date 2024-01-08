using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 相机管理器，挂载到控制的角色上
/// </summary>
public class CameraManager : MonoBehaviour
{
    public Camera rCamera;
    private Vector3 offset;    //记录摄像机相对位置


    private void Awake()
    {
        rCamera = Camera.main;
    }

    private void Start()
    {
        //摄像机移动到英雄后方且对准英雄
        rCamera.transform.position = gameObject.transform.position - gameObject.transform.forward * 8 + Vector3.up * 3;
        rCamera.transform.LookAt(gameObject.transform);
        offset = rCamera.transform.position - gameObject.transform.position;
    }

    private void Update()
    {
        UpdateCamera();
    }

    private void UpdateCamera()
    {
        rCamera.transform.LookAt(transform);
        //摄像机跟随英雄移动
        rCamera.transform.position = transform.position + offset;
        //鼠标滚轮控制摄像机距离英雄的距离
        float wheel = Input.GetAxis("Mouse ScrollWheel");
        if (wheel != 0)
        {
            rCamera.transform.position += rCamera.transform.forward * 1.5f * wheel;
            offset = rCamera.transform.position - transform.position;
        }
        //鼠标右键控制摄像机绕英雄旋转
        if (Input.GetMouseButton(1))
        {
            //Debug.Log("Mouse 0");
            float x = Input.GetAxis("Mouse X");
            float y = Input.GetAxis("Mouse Y");
            rCamera.transform.RotateAround(transform.position, Vector3.up, x * 2);
            rCamera.transform.RotateAround(transform.position, rCamera.transform.right, -y * 2);
            offset = rCamera.transform.position - transform.position;
        }

        //offset最大距离为20
        offset = Vector3.ClampMagnitude(offset, 20);

        //用射线检测摄像机与角色之间是否有障碍物,如果有需要将摄像机位置重置
        RaycastHit hit;
        int index = LayerMask.NameToLayer("Actor");
        LayerMask layerMask = 1 << LayerMask.NameToLayer("Actor");//加入Actor在第七层，1<<7 = 10000000
        //射线检测摄像机和角色之间不能有除actor以外的东西
        if (Physics.Linecast(transform.position + Vector3.up * 0.5f, rCamera.transform.position - Vector3.up * 0.3f, out hit, ~layerMask))
        {
            //临时移动摄像机到障碍物的位置上面一丢丢
            rCamera.transform.position = hit.point + Vector3.up * 0.5f;
        }


    }



    /// <summary>
    /// 场景切换事件回调
    /// </summary>
    public void ChangeScence()
    {
        rCamera = Camera.main;//或者说，maincamera不销毁
    }



}

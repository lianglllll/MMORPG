using GGG.Tool;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraCollider : MonoBehaviour
{

    [SerializeField, Header("最大最小偏移量")]          
    private Vector2 _maxDistanceOffset;                     //是和它的父物体为参照无做的偏移,最小起码是1
    [SerializeField, Header("检测层级"), Space(10)]
    private LayerMask _whatIsWall;
    [SerializeField, Header("射线长度"), Space(10)] 
    private float _detectionDistance;
    [SerializeField, Header("碰撞移动平滑时间"), Space(10)]
    private float _colliderSmoothTime;

    //开始的时候需要保存起始点和起始的偏移
    private Vector3 _originPosition;                                //这玩意没变过(0,0,-1)
    private float _originOffsetDistance;
    private Transform _mainCamera;

    private void Awake()
    {
        //拿摄像机
        _mainCamera = Camera.main.transform;
    }

    private void Start()
    {
        //初始化摄像机的偏移位置
        //_originPosition = transform.localPosition.normalized;//normalized归一化之后就是(0,0-1),也就是我们TP_camera空物体的后方
        _originPosition = new Vector3(0, 0, -1);

        _originOffsetDistance = _maxDistanceOffset.y;
    }



    private void Update()
    {
        UpdateMaxDistanceOffset();
        UpadateCollider();
    }



    /// <summary>
    /// 相机碰撞
    /// </summary>
    private void UpadateCollider()
    {
        var detectionDirection = transform.TransformPoint(_originPosition * _detectionDistance);                //这里转换是将以以父物体为中心的（0，0，-1）转换为世界坐标父物体正后方的方向

        if(Physics.Linecast(transform.position,detectionDirection,out var hit, _whatIsWall, QueryTriggerInteraction.Ignore))
        {
            //打到东西，就说明碰撞到东西，就让相机往前移动一段距离
            _originOffsetDistance = Mathf.Clamp(hit.distance *0.8f, _maxDistanceOffset.x, _maxDistanceOffset.y);
        }
        else
        {
            _originOffsetDistance = _maxDistanceOffset.y;
        }

        //更新相机与父物体的偏移
        _mainCamera.localPosition = Vector3.Lerp(_mainCamera.localPosition, _originPosition * (_originOffsetDistance - 0.1f), DevelopmentToos.UnTetheredLerp(_colliderSmoothTime));
    }

    private void UpdateMaxDistanceOffset()
    {
        float wheel = Input.GetAxis("Mouse ScrollWheel");
        if(wheel != 0f)
        {
            _maxDistanceOffset.y = Mathf.Clamp(_maxDistanceOffset.y + wheel, _maxDistanceOffset.x, 10);
        }
    }


}



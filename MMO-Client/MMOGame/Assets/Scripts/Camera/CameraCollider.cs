using HSFramework.Tool;
using UnityEngine;

public class CameraCollider : MonoBehaviour
{

    [SerializeField, Header("最大最小偏移量")]          
    private Vector2 _maxDistanceOffset;                     //是和它的父物体为参照无做的偏移,最小起码是1
    [SerializeField, Header("当前偏移量")]
    private float curDistanceOffset = 8f;
    [SerializeField, Header("检测层级"), Space(10)]
    private LayerMask _whatIsWall;
    [SerializeField, Header("射线长度"), Space(10)] 
    private float _detectionDistance;
    [SerializeField, Header("碰撞移动平滑时间"), Space(10)]
    private float _colliderSmoothTime;

    private Vector3 _originPosition;                                //这玩意没变过(0,0,-1)
    private float _originOffsetDistance;                            //记录原本没有碰撞到物体时的偏移
    private Transform _mainCamera;                                  //主摄像机

    private void Awake()
    {
        //拿摄像机
        _mainCamera = Camera.main.transform;
    }

    private void Start()
    {
        //初始化摄像机的偏移位置
        _originPosition = new Vector3(0, 0, -1);//我们TP_camera空物体的后方
        _originOffsetDistance = curDistanceOffset;
    }

    private void Update()
    {
        // UpdateCurDistanceOffset();
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
            _originOffsetDistance = Mathf.Clamp(hit.distance *0.8f, _maxDistanceOffset.x, curDistanceOffset);
        }
        else
        {
            _originOffsetDistance = curDistanceOffset;
        }

        //更新相机与父物体的偏移
        _mainCamera.localPosition = Vector3.Lerp(_mainCamera.localPosition, _originPosition * (_originOffsetDistance - 0.1f), DevelopmentToos.UnTetheredLerp(_colliderSmoothTime));
    }

    private void UpdateCurDistanceOffset()
    {
        float wheel = Input.GetAxis("Mouse ScrollWheel");
        if(wheel != 0f)
        {
            wheel = -wheel;
            curDistanceOffset = Mathf.Clamp(curDistanceOffset + wheel, _maxDistanceOffset.x, _maxDistanceOffset.y);
        }
    }


    //开启相机碰撞
    //关闭相机碰撞



}



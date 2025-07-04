using HSFramework.MyDelayedTaskScheduler;
using HSFramework.MySingleton;
using HSFramework.Tool;
using UnityEngine;

public class TP_CameraController : Singleton<TP_CameraController>
{
    private bool isStart;                                                                   //是否启用当前这个控制器

    [Header("相机参数配置")]
    private Transform _lookTarget;                                                          //注视对象
    [SerializeField] private float _positionOffset = 0.1f;                                  //相对于_currentLookTarget的偏移值
    [SerializeField] private float _controllerSpeed = 0.3f;                                 //相机的移动速度
    [SerializeField] private float _positionSmoothTime = 10;                                //相机移动平滑时间
    [SerializeField] private float _rotateSmoothTime = 0.1f;                                //相机旋转平滑时间
    [SerializeField] private Vector2 _cameraVerticalMaxAngle = new Vector2(-65,65);         //限制相机上下看的最大角度

    private Vector3 _currentRotateVelocity = Vector3.zero;                                  //当前相机的移动速度,这里设置为0
    private Vector2 _input;                                                                 //用于接收鼠标输入
    private Vector3 _cameraRotation;                                                        //用于保存摄像机的旋转值
    private Transform _currentLookTarget;                                                   //摄像机当前注释的目标
    private bool _isFinish;                                                                 //是否处于摄像机处决模式


    private void OnEnable()
    {
        //开启相机处决模式
        Kaiyun.Event.RegisterOut("SetMainCameraTarget", this, "SetFnishTarget");
    }
    private void OnDisable()
    {
        Kaiyun.Event.UnRegisterOut("SetMainCameraTarget", this, "SetFnishTarget");
    }
    private void Update()
    {
        //
        if (isStart)
        {
            CameraInput();
        }
    }
    private void LateUpdate()
    {
        //  LateUpdate 通常用于处理摄像机的位置和方向，因为在 Update 阶段中可能会有其他物体的运动，
        //  而 LateUpdate 则确保在摄像机更新之前处理这些变化。
        if (isStart)
        {
            UpdateCameraRotation();
            CameraPosition();
        }
    }

    /// <summary>
    /// 获取鼠标输入
    /// </summary>
    private void CameraInput()
    {
        if (_isFinish) return;
        _input.y += GameInputManager.Instance.CameraLook.x * _controllerSpeed;              //左右看，旋转摄像机y轴
        _input.x -= GameInputManager.Instance.CameraLook.y * _controllerSpeed;              //上下看，旋转摄像机x轴
        //限制仰角
        _input.x = Mathf.Clamp(_input.x, _cameraVerticalMaxAngle.x, _cameraVerticalMaxAngle.y);
    }

    /// <summary>
    /// 更新摄像机旋转
    /// </summary>
    private void UpdateCameraRotation()
    {
        _cameraRotation = Vector3.SmoothDamp(_cameraRotation, new Vector3(_input.x, _input.y, 0f),ref _currentRotateVelocity, _rotateSmoothTime);
        transform.eulerAngles = _cameraRotation;
    }

    /// <summary>
    /// 更新摄像机位置,这个玩意只更新了TP_Cmerra这个物品的位置，这个TP_Cmerra环绕我们注视的物体旋转
    /// </summary>
    private void CameraPosition()
    {
        //以_currentLookTarget位置为基准，向正后移动_positionOffset
        //tpcamera一直跟着注视对象
        Vector3 newPosition;
        if (_isFinish)
        {
            newPosition = _currentLookTarget.transform.position + _currentLookTarget.up * 0.9f + (-_currentLookTarget.transform.forward * _positionOffset);
        }
        else
        {
            newPosition =  _currentLookTarget.position + (-_currentLookTarget.transform.forward * _positionOffset);
        }
        //插值更新摄像机位置
        transform.position = Vector3.Lerp(transform.position, newPosition, DevelopmentToos.UnTetheredLerp(_positionSmoothTime));
    }

    /// <summary>
    /// player处决敌人时的回调
    /// 需要让当前的摄像机注视敌人
    /// </summary>
    /// <param name="target"></param>
    /// <param name="time"></param>
    private void SetFnishTarget(Transform target,float time)
    {
        _isFinish = true;
        _currentLookTarget = target;
        DelayedTaskScheduler.Instance.AddDelayedTask(time, ResetTarget);
        //GameTimerManager.Instance.TryUseOneTimer(time, ResetTarget);
    }

    /// <summary>
    /// 重置摄像机注视的目标
    /// </summary>
    private void ResetTarget()
    {
        _isFinish = false;
        _currentLookTarget = _lookTarget;
    }

    /// <summary>
    /// 启用第三人才摄像机控制器
    /// </summary>
    public void OnStart(Transform lookTarget)
    {
        _lookTarget = lookTarget;
        _currentLookTarget = _lookTarget;

        //摄像机的旋转offset
        _input = new Vector2(30f, 0);

        isStart = true;
        _isFinish = false;
    }
    public void OnStop()
    {
        isStart = false;
        _lookTarget = null;
        _currentLookTarget = _lookTarget;
    }
}

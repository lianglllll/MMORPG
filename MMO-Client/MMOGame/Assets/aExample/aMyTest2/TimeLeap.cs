using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeLeap : MonoBehaviour
{
    private int spaceId;
    public Vector3 SpaceOffset;

    public float duration = 0.5f;   // 抖动持续时间
    public float strength = 2f;     // 抖动强度


    public float changeColdDown = 1f;
    public float curChangeColdDown = 1f;

    private Transform _camera;
    private void Awake()
    {
        _camera = GameObject.Find("TP_Camera").GetComponent<TP_CameraController>().transform;
    }

    private void Start()
    {
        spaceId = 0;
        curChangeColdDown = 0;
    }

    private void Update()
    {
        if (GameInputManager.Instance.Grab && curChangeColdDown <= 0f)
        {
            SpaceChange();
        }
        else
        {
            curChangeColdDown -= Time.deltaTime;
        }
    }


    public void SpaceChange()
    {
        if(spaceId == 0)
        {
            transform.position = transform.position + SpaceOffset;
            spaceId = 1;
        }
        else
        {
            transform.position = transform.position - SpaceOffset;
            spaceId = 0;
        }
        curChangeColdDown = changeColdDown;
        GameTimerManager.Instance.TryUseOneTimer(0.1f, ShakeCamera);
    }

    // 在需要触发相机抖动的地方调用此方法
    public void ShakeCamera()
    {
        // 生成一个随机的抖动向量
        Vector3 shakeVector = Random.insideUnitSphere * strength;

        // 使用 DOTween 对相机位置进行动画插值，达到抖动效果
        //_camera.DOShakePosition(duration, shakeVector);
    }
}

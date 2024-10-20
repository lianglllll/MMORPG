using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
/// <summary>
/// 播放特效，有些特效不会循环
/// </summary>
public class GameEffect : MonoBehaviour
{

    public ParticleSystem particle;             //粒子系统，如果加载的不是粒子则为null
    public GameObject target;                   //跟随物体，如果此物体不为空，特效会跟随父物体移动，直到父物体变为null
    public Vector3 offset = Vector3.zero;       //相对于target的偏移
    //private float lifeTime = float.MaxValue;    //生命周期

    void Update()
    {
        if (target != null)
        {
            transform.position = target.transform.position + offset;
        }
    }

    void OnDestroy()
    {

    }

    /// <summary>
    /// 初始化
    /// </summary>
    public void Init()
    {
        particle = transform.GetComponent<ParticleSystem>();
        gameObject.SetActive(false);
    }

    /// <summary>
    /// 播放特效
    /// </summary>
    /// <param name="scale"></param>
    public void Play(float scale=1, float duration = -1)
    {
        if (particle == null) return;
        gameObject.SetActive(true);
        SetScale(transform, scale);
        particle.Play();
        //设置持续时间
        if (Mathf.Approximately(duration, -1f)){
            SetLifeTime(particle.main.duration);
        }
        else
        {
            SetLifeTime(duration);
        }

    }

    /// <summary>
    /// 设置特效缩放
    /// </summary>
    /// <param name="t"></param>
    /// <param name="scale"></param>
    public void SetScale(Transform t, float scale)
    {
        for (int i = 0; i < t.childCount; i++)
        {
            SetScale(t.GetChild(i), scale);
        }
        t.localScale = new Vector3(scale, scale, scale);
    }

    /// <summary>
    /// 设置目标
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="offset"></param>
    public void SetTarget(GameObject obj, Vector3 offset=default)
    {
        this.target = obj;
        this.offset = offset;
    }

    /// <summary>
    /// 停止特效
    /// </summary>
    public void Stop()
    {
        gameObject.SetActive(false);
    }

    /// <summary>
    /// 设置生命周期，到期自动销毁，时间指定之后无法更改
    /// </summary>
    /// <param name="time"></param>
    public void SetLifeTime(float time=0f)
    {
        if (this.gameObject != null && !this.gameObject.IsDestroyed())
        {
            MonoBehaviour.Destroy(this.gameObject, time);
        }
    }

}

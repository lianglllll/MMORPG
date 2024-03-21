using GameClient.Combat;
using Serilog;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Missile : MonoBehaviour
{
    //所属技能
    public Skill Skill { get; private set; }
    //追击目标
    public GameObject Target { get; private set; }
    //初始位置
    public Vector3 InitPos { get; private set; }

    //挂特效用
    private GameObject child;


    private void Start()
    {
        transform.localScale = Vector3.one * 0.1f;
    }

    private void FixedUpdate()
    {
        OnUpdate(Time.fixedDeltaTime);
    }

    private void OnUpdate(float fixedDeltaTime)
    {
        //如果目标消耗，则飞行物消失
        //因为我们当前生成飞行物的都是使用有目标的也就是跟踪的飞行物
        if(Target == null || Target.IsDestroyed())
        {
            gameObject.SetActive(false);
            Destroy(this.gameObject, 0.6f);
            return;
        }


        var a = transform.position;
        var b = Target.transform.position;
        Vector3 direction = (b - a).normalized;
        var distance = Skill.Define.MissileSpeed * 0.001f * fixedDeltaTime;
        //判断本帧运算是否能到达目标点
        if(distance >= Vector3.Distance(a, b))
        {
            transform.position = b;
            Destroy(gameObject, 0.1f);
        }
        else
        {
            transform.position += direction * distance;
        }

        child.transform.localPosition = Vector3.up*10f;
    }

    public void  Init(Skill skill,Vector3 initPos,GameObject target)
    {
        this.Skill = skill;
        this.Target = target;
        this.InitPos = initPos;
        transform.position = initPos;
        Log.Information("Missile InitPos:{0}", initPos);

        var prefab = Resources.Load<GameObject>(skill.Define.Missile);
        if(prefab != null)
        {
            child = Instantiate(prefab, Vector3.zero, Quaternion.identity, transform);
        }
        else
        {
            Log.Error("There is no resource named = {0}", skill.Define.Missile);
        }
    }




}

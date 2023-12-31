using GameServer.Combat;
using GameServer.Core;
using Serilog;
using System;
using System.Collections;
using System.Collections.Generic;

public class Missile 
{

    //所属技能
    public Skill Skill { get; private set; }
    //追击目标
    public SCObject Target { get; private set; }
    //初始位置
    public Vector3 InitPos { get; private set; }

    //飞行物当前位置
    public Vector3 curPosition;


    public Missile(Skill skill,Vector3 initPos,SCObject target)
    {
        this.Skill = skill;
        this.Target = target;
        this.InitPos = initPos;
        this.curPosition = initPos;
        Log.Information("Missile Position:{0}", curPosition);
    }



    public void OnUpdate(float deltaTime)
    {
        var a = this.curPosition;
        var b = Target.Position;
        Vector3 direction = (b - a).normalized;
        var distance = Skill.Define.MissileSpeed *  deltaTime;
        //判断本帧运算是否能到达目标点
        if (distance >= Vector3.Distance(a, b))
        {
            curPosition = b;
            Skill.OnHit(Target);
            Skill.Owner.currentSpace.fightManager.missiles.Remove(this);
        }
        else
        {
            curPosition += direction * distance;
        }

    }




}

using Assets.Script.Entities;
using GameClient.Combat;
using GameClient.Entities;
using Proto;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//主要记录一下全局唯一的数据
//动态的，不同于datamanager
//方便调用罢了
public class GameApp 
{
    public static int entityId;
    //全局角色
    public static Character character;
    public static Actor target;
    //当前角色对象引用
    public static GameObject myCharacter = null;
    //当前技能
    public static Skill CurrSkill;
}

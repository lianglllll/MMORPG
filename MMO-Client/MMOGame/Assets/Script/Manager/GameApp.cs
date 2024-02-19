using Assets.Script.Entities;
using GameClient.Combat;
using GameClient.Entities;
using Proto;
using Serilog;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


//主要记录一下全局唯一的数据
//动态的，不同于datamanager
//方便调用罢了
public class GameApp 
{
    //SessionId
    public static string SessionId;

    //角色的entityid
    public static int entityId;

    //全局角色
    public static Character character;
    public static Actor target;

    //当前角色对象引用
    public static GameObject myCharacter = null;

    //当前技能
    public static Skill CurrSkill;

    //战斗面板
    public static CombatPanelScript combatPanelScript;

    /// <summary>
    /// 是否正在输入,//todo 改成用事件来触发：聊天事件====》角色控制失效、聊天框跳出、
    /// </summary>
    public static bool IsInputtingChatBox
    {
        get => combatPanelScript.chatBoxScript.chatMsgInputField.isFocused;
    }

    /// <summary>
    /// 技能释放的发包
    /// </summary>
    /// <param name="skill"></param>
    public static void Spell(Skill skill)
    {
        //向服务器发送施法请求
        SpellCastRequest req = new SpellCastRequest() { Info = new CastInfo() };
        req.Info.SkillId = skill.Define.ID;
        req.Info.CasterId = GameApp.character.EntityId;
        if (skill.IsUnitTarget)
        {
            req.Info.TargetId = GameApp.target.EntityId;

        }
        else if (skill.IsPointTarget)
        {
            req.Info.Point = V3.ToVec3(GameApp.target.Position);
        }
        NetClient.Send(req);
    }

    /// <summary>
    /// 加载场景
    /// </summary>
    /// <param name="spaceId"></param>
    public static void LoadSpace(int spaceId)
    {
        //切换到对于的场景
        SpaceDefine space = DataManager.Instance.spaceDict[spaceId];
        SceneManager.LoadScene(space.Resource);
    }

    /// <summary>
    /// 传送请求发包
    /// </summary>
    /// <param name="spaceId"></param>
    public static void SpaceDeliver(int spaceId)
    {
        SpaceDeliverRequest req = new SpaceDeliverRequest();
        req.SpaceId = spaceId;
        NetClient.Send(req);
    }

}

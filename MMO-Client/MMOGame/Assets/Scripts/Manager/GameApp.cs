using GameClient.Combat;
using GameClient.Entities;
using HS.Protobuf.Login;
using HS.Protobuf.Scene;
using System;
using UnityEngine;

namespace GameClient {

    /// <summary>
    /// 服务器信息类
    /// </summary>
    [Serializable]
    public class TempServerInfo
    {
        public string name;
        public string host;
        public int port;
        public int state;
    }


    /// <summary>
    /// 主要记录一下全局唯一的数据
    /// </summary>
    public class GameApp
    {
        //当前的服务器信息
        public static WorldInfoNode curWorldInfoNode;

        //当前所在场景id
        public static int SceneId;

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
        public static CombatPanelScript _CombatPanelScript => (CombatPanelScript)UIManager.Instance.GetOpeningPanelByName("CombatPanel");

        //清空当前存储的数据
        public static void ClearGameAppData()
        {
            SceneId = -1;
            entityId = -1;
            character = null;
            target = null;
            CurrSkill = null;
        }


        /// <summary>
        /// 发送玩家复活请求
        /// </summary>
        public static void _Revive()
        {
            ReviveRequest req = new ReviveRequest();
            req.EntityId = entityId;
            NetManager.Instance.Send(req);
        }
    }
}


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
        // 当前的服务器信息
        public static WorldInfoNode curWorldInfoNode;

        // 当前角色信息
        public static int SceneId;
        public static int entityId;
        public static Character character;
        public static string chrId;
        public static Skill CurSkill;

        // 其他辅助信息
        public static Actor target;
        public static CombatPanelScript CombatPanelScript => (CombatPanelScript)UIManager.Instance.GetOpeningPanelByName("CombatPanel");

        // 辅助函数
        public static void ClearGameAppData()
        {
            SceneId = -1;
            entityId = -1;
            character = null;
            chrId = null;
            target = null;
            CurSkill = null;
        }
        public static void SendReviveReq()
        {
            ReviveRequest req = new ReviveRequest();
            req.EntityId = entityId;
            NetManager.Instance.Send(req);
        }
    }
}


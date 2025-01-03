using Assets.Script.Entities;
using GameClient.Combat;
using GameClient.Entities;
using HS.Protobuf.Scene;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameClient {

    /// <summary>
    /// 服务器信息类
    /// </summary>
    [Serializable]
    public class ServerInfo
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
        public static ServerInfo ServerInfo;

        //SessionId
        public static string SessionId;

        //当前所在场景id
        public static int SpaceId;

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
        public static CombatPanelScript _CombatPanelScript => (CombatPanelScript)UIManager.Instance.GetPanelByName("CombatPanel");

        //清空当前存储的数据
        public static void ClearGameAppData()
        {
            SpaceId = -1;
            SessionId = null;
            entityId = -1;
            character = null;
            target = null;
            CurrSkill = null;
        }


        /// <summary>
        /// 是否正在输入,//todo 改成用事件来触发：聊天事件====》角色控制失效、聊天框跳出、
        /// </summary>
        public static bool IsInputtingChatBox
        {
            get => _CombatPanelScript.chatBoxScript.chatMsgInputField.isFocused;
        }

        /// <summary>
        /// 加载场景
        /// </summary>
        /// <param name="spaceId">场景id</param>
        /// <param name="action">场景加载完成后的回调</param>
        public static void LoadSpaceWithPoster(int spaceId, Action<Scene> action)
        {
            var spaceDefine = DataManager.Instance.spaceDefineDict[spaceId];
            UnityMainThreadDispatcher.Instance().StartCoroutine(_LoadSpaceWithPoster(spaceDefine.Name,spaceDefine.Resource, action));
        }
        private static IEnumerator _LoadSpaceWithPoster(string spaceName,string path, Action<Scene> action)
        {

            //展示转场UI,这里需要在4秒内模拟到进度的百分之90，这个是模拟出来假的进度。
            ScenePoster.Instance.nameText.text = spaceName ?? "---";
            ScenePoster.Instance.SetProgress(0.9f, 4.0f);

            yield return new WaitForSeconds(ScenePoster.Instance.fadeDuration * 1.1f);

            var handle = Res.LoadSceneAsync(path);
            handle.OnLoaded = (s) =>
            {
                UnityMainThreadDispatcher.Instance().StartCoroutine(__LoadSpaceWithPoster( s,action));
            };

        }
        private static IEnumerator __LoadSpaceWithPoster(Scene s, Action<Scene> action)
        {
            yield return new WaitForSeconds(0.01f);

            //逻辑
            action?.Invoke(s);

            //完成转场ui
            ScenePoster.Instance.SetProgress(1f, 0.3f);
        }


        /// <summary>
        /// 发送玩家复活请求
        /// </summary>
        public static void _Revive()
        {
            ReviveRequest req = new ReviveRequest();
            req.EntityId = entityId;
            NetClient.Send(req);
        }
    }


}


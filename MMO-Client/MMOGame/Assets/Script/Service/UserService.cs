using Assets.Script.Entities;
using GameClient.Entities;
using Proto;
using Summer;
using Summer.Network;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



/// <summary>
/// 用户服务  //todo将散布在panel中的权力放回这里
/// </summary>
public class UserService : Singleton<UserService>, IDisposable
{

    /// <summary>
    /// 初始化，gamemanager中启用
    /// </summary>
    public void Init()
    {
        //注册特殊事件
        Kaiyun.Event.RegisterIn("GameEnter", this, "GameEnter");

    }

    /// <summary>
    /// 脚本销毁时操作
    /// </summary>
    public void Dispose()
    {
        //事件注销
        Kaiyun.Event.UnregisterIn("GameEnter", this, "GameEnter");
    }

    /// <summary>
    /// 加入游戏请求
    /// </summary>
    /// <param name="roleId"></param>
    public void GameEnter(int roleId)
    {
        //安全校验
        if (GameApp.myCharacter != null) return;
        //发送请求 
        GameEnterRequest request = new GameEnterRequest();
        request.CharacterId = roleId;
        NetClient.Send(request);
    }

}

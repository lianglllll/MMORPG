using Assets.Script.Entities;
using GameClient;
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
        MessageRouter.Instance.Subscribe<UserLoginResponse>(_UserLoginResponse);
        MessageRouter.Instance.Subscribe<GameEnterResponse>(_GameEnterResponse);
        MessageRouter.Instance.Subscribe<CharacterListResponse>(_CharacterListResponse);
        MessageRouter.Instance.Subscribe<CharacterDeleteResponse>(_CharacterDeleteResponse);
        MessageRouter.Instance.Subscribe<UserRegisterResponse>(OnRegisterResponse);
        MessageRouter.Instance.Subscribe<CharacterCreateResponse>(_CharacterCreateResponse);

    }




    /// <summary>
    /// 脚本销毁时操作
    /// </summary>
    public void Dispose()
    {
        MessageRouter.Instance.Off<UserLoginResponse>(_UserLoginResponse);
        MessageRouter.Instance.Off<GameEnterResponse>(_GameEnterResponse);
        MessageRouter.Instance.Off<CharacterListResponse>(_CharacterListResponse);
        MessageRouter.Instance.Off<CharacterDeleteResponse>(_CharacterDeleteResponse);
        MessageRouter.Instance.Off<UserRegisterResponse>(OnRegisterResponse);
        MessageRouter.Instance.Off<CharacterCreateResponse>(_CharacterCreateResponse);

    }



    /// <summary>
    /// 发送用户登陆请求
    /// </summary>
    /// <param name="username"></param>
    /// <param name="password"></param>
    public void _UserLoginRequest(string username,string password)
    {
        UserLoginRequest loginRequest = new UserLoginRequest();
        loginRequest.Username = username;
        loginRequest.Password = password;
        NetClient.Send(loginRequest);
    }

    /// <summary>
    /// 用户登录响应
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="msg"></param>
    private void _UserLoginResponse(Connection sender, UserLoginResponse msg)
    {
        var panel = UIManager.Instance.GetPanelByName("LoginPanel");
        if (panel == null) return;
        (panel as LoginPanelScript).OnLoginResponse(msg);
    }


    /// <summary>
    /// 发送用户注册请求
    /// </summary>
    /// <param name="username"></param>
    /// <param name="password"></param>
    public void _UserRegisterRequest(string username,string password)
    {
        UserRegisterRequest req = new UserRegisterRequest();
        req.Username = username;
        req.Password = password;
        NetClient.Send(req);
    }

    /// <summary>
    /// 用户注册响应
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="msg"></param>
    private void OnRegisterResponse(Connection sender, UserRegisterResponse msg)
    {
        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            UIManager.Instance.AsyncShowTopMessage(msg.Message);
            switch (msg.Code)
            {
                case 0://成功,跳转到选择角色页面
                    UIManager.Instance.ClosePanel("RegisterPanel");
                    break;
                case 1:

                    break;
            }
        });
    }



    /// <summary>
    /// 拉取user的角色列表
    /// </summary>
    public void _CharacterListRequest()
    {
        //发起角色列表的请求
        CharacterListRequest req = new CharacterListRequest();
        NetClient.Send(req);
    }

    /// <summary>
    /// 拉取user的角色列表响应
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="msg"></param>
    private void _CharacterListResponse(Connection sender, CharacterListResponse msg)
    {
        var panel = UIManager.Instance.GetPanelByName("SelectRolePanel");
        if (panel == null) return;
        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            ((SelectRolePanelScript)panel).RefreshRoleListUI(msg);
        });
    }



    /// <summary>
    /// 发送创建角色请求
    /// </summary>
    /// <param name="roleName"></param>
    /// <param name="jobId"></param>
    public void _CharacterCreateRequest(string roleName,int jobId)
    {
        CharacterCreateRequest req = new CharacterCreateRequest();
        req.Name = roleName;
        req.JobType = jobId;
        NetClient.Send(req);
    }

    /// <summary>
    /// 创建角色响应
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="msg"></param>
    private void _CharacterCreateResponse(Connection sender, CharacterCreateResponse msg)
    {
        //显示消息内容
        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            UIManager.Instance.ShowTopMessage(msg.Message);
        });

        //如果成功就进行跳转
        if (msg.Success)
        {
            //发起角色列表的请求，刷新角色列表
            UserService.Instance._CharacterListRequest();
            UnityMainThreadDispatcher.Instance().Enqueue(() => {
                UIManager.Instance.ClosePanel("CreateRolePanel");
            });
        }
    }



    /// <summary>
    /// 发送删除角色的请求
    /// </summary>
    /// <param name="chrId"></param>
    public void _CharacterDeleteRequest(int chrId)
    {
        CharacterDeleteRequest req = new CharacterDeleteRequest();
        req.CharacterId = chrId;
        NetClient.Send(req);
    }

    /// <summary>
    /// 删除角色的响应
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="msg"></param>
    private void _CharacterDeleteResponse(Connection sender, CharacterDeleteResponse msg)
    {
        //显示信息
        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            UIManager.Instance.ShowTopMessage(msg.Message);
        });

        if (msg.Success)
        {
            //发起角色列表的请求
            CharacterListRequest req = new CharacterListRequest();
            NetClient.Send(req);
        }

    }



    /// <summary>
    /// 发送进入游戏请求
    /// </summary>
    /// <param name="roleId"></param>
    public void _GameEnterRequest(int roleId)
    {
        //安全校验
        if (GameApp.character != null) return;

        //发送请求 
        GameEnterRequest request = new GameEnterRequest();
        request.CharacterId = roleId;
        NetClient.Send(request);
    }

    /// <summary>
    /// 进入游戏的响应
    /// </summary>
    private void _GameEnterResponse(Connection sender, GameEnterResponse msg)
    {
        //这里处理一些其他事情，比如说ui关闭的清理工作
    }


}

using HSFramework.MySingleton;
using Common.Summer.Core;
using Common.Summer.Net;
using GameClient;
using GameClient.Entities;
using HS.Protobuf.Game;
using HS.Protobuf.Login;
using HS.Protobuf.SceneEntity;
using System.Threading.Tasks;

public class EntryGameWorldHandler : SingletonNonMono<EntryGameWorldHandler>
{
    public void Init()
    {
        // 协议注册
        ProtoHelper.Instance.Register<GetAllWorldInfosRequest>((int)LoginProtocl.GetAllWorldInfoNodeReq);
        ProtoHelper.Instance.Register<GetAllWorldInfosResponse>((int)LoginProtocl.GetAllWorldInfoNodeResp);

        ProtoHelper.Instance.Register<GetCharacterListRequest>((int)GameProtocl.GetCharacterListReq);
        ProtoHelper.Instance.Register<GetCharacterListResponse>((int)GameProtocl.GetCharacterListResp);

        ProtoHelper.Instance.Register<CreateCharacterRequest>((int)GameProtocl.CreateCharacterReq);
        ProtoHelper.Instance.Register<CreateCharacterResponse>((int)GameProtocl.CreateCharacterResp);

        ProtoHelper.Instance.Register<DeleteCharacterRequest>((int)GameProtocl.DeleteCharacterReq);
        ProtoHelper.Instance.Register<DeleteCharacterResponse>((int)GameProtocl.DeleteCharacterResp);

        ProtoHelper.Instance.Register<EnterGameRequest>((int)GameProtocl.EnterGameReq);
        ProtoHelper.Instance.Register<EnterGameResponse>((int)GameProtocl.EnterGameResp);

        // 消息的订阅
        MessageRouter.Instance.Subscribe<GetAllWorldInfosResponse>(_HandleGetAllWorldInfosResponse);
        MessageRouter.Instance.Subscribe<GetCharacterListResponse>(_HandleGetCharacterListResponse);
        MessageRouter.Instance.Subscribe<CreateCharacterResponse>(_HandleCreateCharacterResponse);
        MessageRouter.Instance.Subscribe<DeleteCharacterResponse>(_HandleDeleteCharacterResponse);
        MessageRouter.Instance.Subscribe<EnterGameResponse>(_HandleEnterGameResponse);
    }
    public void UnInit()
    {
        MessageRouter.Instance.UnSubscribe<GetAllWorldInfosResponse>(_HandleGetAllWorldInfosResponse);
        MessageRouter.Instance.UnSubscribe<GetCharacterListResponse>(_HandleGetCharacterListResponse);
        MessageRouter.Instance.UnSubscribe<CreateCharacterResponse>(_HandleCreateCharacterResponse);
        MessageRouter.Instance.UnSubscribe<DeleteCharacterResponse>(_HandleDeleteCharacterResponse);
        MessageRouter.Instance.UnSubscribe<EnterGameResponse>(_HandleEnterGameResponse);
    }

    public void SendGetAllWorldInfosRequest()
    {
        GetAllWorldInfosRequest req = new();
        req.LoginGateToken = NetManager.Instance.m_loginGateToken;
        NetManager.Instance.Send(req);
    }
    private void _HandleGetAllWorldInfosResponse(Connection sender, GetAllWorldInfosResponse message)
    {
        var panel = UIManager.Instance.GetOpeningPanelByName("SelectWorldPanel");
        if (panel == null) return;
        UnityMainThreadDispatcher.Instance().Enqueue(() => {
            (panel as SelectWorldPanel).HandleGetAllWorldInfosResponse(message);
        });
    }

    public void SendGetCharacterListRequest()
    {
        //发起角色列表的请求
        GetCharacterListRequest req = new();
        NetManager.Instance.Send(req);
    }
    private void _HandleGetCharacterListResponse(Connection sender, GetCharacterListResponse msg)
    {
        var panel = UIManager.Instance.GetOpeningPanelByName("SelectRolePanel");
        if (panel == null) return;
        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            ((SelectRolePanelScript)panel).RefreshRoleListUI(msg);
        });
    }

    public void SendCreateCharacterRequest(string roleName, int jobId)
    {
        CreateCharacterRequest req = new();
        req.CName = roleName;
        req.ProfessionId = jobId;
        NetManager.Instance.Send(req);
    }
    private void _HandleCreateCharacterResponse(Connection sender, CreateCharacterResponse msg)
    {
        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            //显示消息内容
            if (msg.ResultMsg != null)
            {
                UIManager.Instance.ShowTopMessage(msg.ResultMsg);
            }

            //如果成功就进行跳转
            if (msg.ResultCode == 0)
            {
                //发起角色列表的请求，刷新角色列表
                SendGetCharacterListRequest();
                CreateRolePanelScript panel = (CreateRolePanelScript)UIManager.Instance.GetOpeningPanelByName("CreateRolePanel");
                panel.OnReturnBtn();
            }

        });
    }

    public void SendDeleteCharacterRequest(string chrId, string password)
    {
        if (string.IsNullOrEmpty(password)) {
            UIManager.Instance.ShowTopMessage("删除失败，密码不能为空");
            goto End;
        }
        DeleteCharacterRequest req = new();
        req.CId = chrId;

        req.Password = NetManager.Instance.curNetClient.EncryptionManager.AesEncrypt(password);
        NetManager.Instance.Send(req);
    End:
        return;
    }
    private void _HandleDeleteCharacterResponse(Connection sender, DeleteCharacterResponse msg)
    {
        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            //显示信息
            if (msg.ResultMsg != null)
            {
                UIManager.Instance.ShowTopMessage(msg.ResultMsg);
            }
            if (msg.ResultCode == 0)
            {
                //发起角色列表的请求
                GetCharacterListRequest req = new();
                NetManager.Instance.Send(req);
            }
        });



    }

    public void SendEnterGameRequest(string cId)
    {
        //安全校验
        if (GameApp.character != null) return;
        //发送请求 
        EnterGameRequest request = new();
        request.CharacterId = cId;
        NetManager.Instance.Send(request);
    }
    private void _HandleEnterGameResponse(Connection sender, EnterGameResponse msg)
    {
        if(msg.ResultCode != 0)
        {
            var panel = UIManager.Instance.GetOpeningPanelByName("SelectRolePanel");
            if (panel != null)
            {
                UnityMainThreadDispatcher.Instance().Enqueue(() => {
                    ((SelectRolePanelScript)panel).HandleStartResponse(msg.ResultCode, msg.ResultMsg);
                });
            }
            goto End;
        }

        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            // 清理旧场景的对象
            EntityManager.Instance.Clear();
            GameApp.ClearGameAppData();
            TP_CameraController.Instance.OnStop();

            NetActorNode selfActorNode = msg.SelfNetActorNode;
            int selfEntityId = selfActorNode.EntityId;
            int curSceneId = selfActorNode.SceneId;
            GameApp.SceneId = curSceneId;
            GameApp.entityId = selfEntityId;
            GameApp.chrId = msg.CharacterId;

            // 切换场景
            LocalDataManager.Instance.spaceDefineDict.TryGetValue(curSceneId, out var spaceDef);
            ScenePoster.Instance.LoadSpaceWithPoster(spaceDef.Name, spaceDef.Resource, async (scene) => {

                // 关闭选择角色的面板
                var panel = UIManager.Instance.GetOpeningPanelByName("SelectRolePanel");
                if (panel != null)
                {
                    ((SelectRolePanelScript)panel).HandleStartResponse(0 , null);
                }

                await Task.Delay(800);

                // 加载其他Actor
                foreach (var item in msg.OtherNetActorNodeList)
                {
                    EntityManager.Instance.OnActorEnterScene(item);
                }
                // 加载物品
                foreach (var item in msg.OtherNetItemNodeList)
                {
                    EntityManager.Instance.OnItemEnterScene(item);
                }

                // 最后生成自己的角色,记录本机的数据
                EntityManager.Instance.OnActorEnterScene(msg.SelfNetActorNode);
                GameApp.character = EntityManager.Instance.GetEntity<Character>(msg.SelfNetActorNode.EntityId);

                // 刷新战斗面板,因为很多ui都依赖各种entity，刷新场景它们的依赖就失效了
                // UIManager.Instance.ClosePanel("CombatPanel");
                UIManager.Instance.OpenPanel("CombatPanel");
                LocalDataManager.Instance.spaceDefineDict.TryGetValue(GameApp.SceneId, out var def);
                UIManager.Instance.ShowTopMessage("" + def.Name);

            });

        });

    End:
        return;
    }

}

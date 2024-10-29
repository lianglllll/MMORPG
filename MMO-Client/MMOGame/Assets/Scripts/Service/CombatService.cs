using Summer;
using Summer.Network;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Proto;
using System;
using Serilog;
using GameClient.Entities;
using Assets.Script.Entities;
using GameClient;
using GameClient.Combat;
using System.Threading.Tasks;

public class CombatService : Singleton<CombatService>, IDisposable
{

    /// <summary>
    /// 初始化，gamemanager中启用
    /// </summary>
    public void Init()
    {
        MessageRouter.Instance.Subscribe<SpaceEnterResponse>(_SpaceEnterResponse);
        MessageRouter.Instance.Subscribe<SpaceCharactersEnterResponse>(_SpaceCharactersEnterResponse);
        MessageRouter.Instance.Subscribe<SpaceItemEnterResponse>(_SpaceItemEnterResponse);
        MessageRouter.Instance.Subscribe<SpaceEntitySyncResponse>(_SpaceEntitySyncResponse);
        MessageRouter.Instance.Subscribe<CtlClientSpaceEntitySyncResponse>(_CtlClientSpaceEntitySyncResponse);
        MessageRouter.Instance.Subscribe<SpaceEntityLeaveResponse>(_SpaceEntityLeaveResponse);
        MessageRouter.Instance.Subscribe<SpellCastResponse>(_SpellCastResponse);
        MessageRouter.Instance.Subscribe<SpellFailResponse>(_SpellFailResponse);
        MessageRouter.Instance.Subscribe<DamageResponse>(_DamageResponse);
        MessageRouter.Instance.Subscribe<PropertyUpdateRsponse>(_PropertyUpdateRsponse);
    }

    /// <summary>
    /// 脚本销毁时操作
    /// </summary>
    public void Dispose()
    {
        MessageRouter.Instance.Off<SpaceEnterResponse>(_SpaceEnterResponse);
        MessageRouter.Instance.Off<SpaceCharactersEnterResponse>(_SpaceCharactersEnterResponse);
        MessageRouter.Instance.Off<SpaceItemEnterResponse>(_SpaceItemEnterResponse);
        MessageRouter.Instance.Off<SpaceEntitySyncResponse>(_SpaceEntitySyncResponse);
        MessageRouter.Instance.Off<CtlClientSpaceEntitySyncResponse>(_CtlClientSpaceEntitySyncResponse);
        MessageRouter.Instance.Off<SpaceEntityLeaveResponse>(_SpaceEntityLeaveResponse);
        MessageRouter.Instance.Off<SpellCastResponse>(_SpellCastResponse);
        MessageRouter.Instance.Off<SpellFailResponse>(_SpellFailResponse);
        MessageRouter.Instance.Off<DamageResponse>(_DamageResponse);
        MessageRouter.Instance.Off<PropertyUpdateRsponse>(_PropertyUpdateRsponse);
    }

    /// <summary>
    /// 进入场景的响应(entity是自己)
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="msg"></param>
    private void _SpaceEnterResponse(Connection sender, SpaceEnterResponse msg)
    {
        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            if (GameApp.character==null)
            {
                //1.切换场景
                GameApp.SpaceId = msg.Character.SpaceId;

                GameApp.LoadSpaceWithPoster(msg.Character.SpaceId, async (scene) => {

                     await Task.Delay(800);

                    //2.加载其他角色和ai
                    foreach (var item in msg.CharacterList)
                    {
                        EntityManager.Instance.OnActorEnterScene(item);
                    }

                    //3.加载物品
                    foreach (var item in msg.EItemList)
                    {
                        EntityManager.Instance.OnItemEnterScene(item);
                    }

                    //最后生成自己的角色,记录本机的数据
                    GameApp.entityId = msg.Character.Entity.Id;
                    EntityManager.Instance.OnActorEnterScene(msg.Character);
                    GameApp.character = EntityManager.Instance.GetEntity<Character>(msg.Character.Entity.Id);

                    //推入combatUI
                    UIManager.Instance.OpenPanel("CombatPanel");
                    DataManager.Instance.spaceDict.TryGetValue(GameApp.SpaceId, out var def);
                    UIManager.Instance.ShowTopMessage("" + def.Name);

                });
            }
            else if(GameApp.character.info.SpaceId != msg.Character.SpaceId)
            {
                //清理旧场景的对象
                EntityManager.Instance.Clear();
                GameApp.ClearGameAppData();
                TP_CameraController.instance.OnStop();

                //切换场景
                GameApp.SpaceId = msg.Character.SpaceId;
                GameApp.LoadSpaceWithPoster(msg.Character.SpaceId, async (scene) => {

                    await Task.Delay(800);

                    //加载其他角色和ai
                    foreach (var item in msg.CharacterList)
                    {
                        EntityManager.Instance.OnActorEnterScene(item);
                    }
                    //加载物品
                    foreach (var item in msg.EItemList)
                    {
                        EntityManager.Instance.OnItemEnterScene(item);
                    }

                    //最后生成自己的角色,记录本机的数据
                    GameApp.entityId = msg.Character.Entity.Id;
                    EntityManager.Instance.OnActorEnterScene(msg.Character);
                    GameApp.character = EntityManager.Instance.GetEntity<Character>(msg.Character.Entity.Id);

                    //刷新战斗面板,因为很多ui都依赖各种entity，刷新场景它们的依赖就失效了
                    UIManager.Instance.ClosePanel("CombatPanel");
                    UIManager.Instance.OpenPanel("CombatPanel");
                    DataManager.Instance.spaceDict.TryGetValue(GameApp.SpaceId, out var def);
                    UIManager.Instance.ShowTopMessage("" + def.Name);

                });
            }
            else
            {
            }

        });

    }

    /// <summary>
    /// 新角色进入场景通知（entity不是自己）
    /// </summary>
    /// <param name="conn"></param>
    /// <param name="msg"></param>
    private void _SpaceCharactersEnterResponse(Connection conn, SpaceCharactersEnterResponse msg)
    {
        foreach (var actorObj in msg.CharacterList)
        {
            //触发角色进入事件
            EntityManager.Instance.OnActorEnterScene(actorObj);
        }
    }

    /// <summary>
    /// entity同步信息接收
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="msg"></param>
    private void _SpaceEntitySyncResponse(Connection sender, SpaceEntitySyncResponse msg)
    {
        //注意这个由网络线程中的任务，它是并发的
        //所以对游戏对象的获取和访问都需要在主线程中完成
        EntityManager.Instance.OnEntitySync(msg.EntitySync);
    }

    /// <summary>
    /// 自己控制的角色，entity同步
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="msg"></param>
    private void _CtlClientSpaceEntitySyncResponse(Connection sender, CtlClientSpaceEntitySyncResponse msg)
    {
        EntityManager.Instance.OnCtlEntitySync(msg.EntitySync);
    }

    /// <summary>
    /// 有emtity离开当前地图
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="msg"></param>
    private void _SpaceEntityLeaveResponse(Connection sender, SpaceEntityLeaveResponse msg)
    {
        //触发角色离开事件
        EntityManager.Instance.RemoveEntity(msg.EntityId);
    }

    /// <summary>
    /// 施法失败的响应
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="msg"></param>
    private void _SpellFailResponse(Connection sender, SpellFailResponse msg)
    {
        string msgContext = "";
        switch (msg.Reason)
        {
            case CastResult.Success:
                break;
            case CastResult.IsPassive:
                msgContext = "被动技能";
                break;
            case CastResult.MpLack:
                msgContext = "MP不足";
                break;
            case CastResult.EntityDead:
                msgContext = "目标已经死亡";
                break;
            case CastResult.OutOfRange:
                msgContext = "目标超出范围";
                break;
            case CastResult.Running:
                msgContext = "技能正在释放";
                break;
            case CastResult.ColdDown:
                msgContext = "技能正在冷却";
                break;
            case CastResult.TargetError:
                msgContext = "目标错误";
                break;
        }
        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            UIManager.Instance.MessagePanel.ShowBottonMsg(msgContext);
        });
    }

    /// <summary>
    /// actor施法通知
    /// </summary>
    /// <param name="conn"></param>
    /// <param name="msg"></param>
    private void _SpellCastResponse(Connection conn, SpellCastResponse msg)
    {

        foreach (CastInfo item in msg.List)
        {
            var caster = EntityManager.Instance.GetEntity<Actor>(item.CasterId);
            if (caster == null) continue;

            var skill = caster.skillManager.GetSkill(item.SkillId);
            if (skill.IsUnitTarget)
            {
                var target = EntityManager.Instance.GetEntity<Actor>(item.TargetId);
                skill.Use(new SCEntity(target));
            }
            else if (skill.IsPointTarget)
            {

            }else if (skill.IsNoneTarget)
            {
                skill.Use(new SCEntity(caster));
            }

            //skill ui
            if (GameApp.character.EntityId == skill.Owner.EntityId)
            {
                UnityMainThreadDispatcher.Instance().Enqueue(() =>
                {
                    UIManager.Instance.MessagePanel.ShowBottonMsg(skill.Define.Name,Color.green);
                });
            }

        }
    }

    /// <summary>
    /// actor的伤害响应包
    /// </summary>
    /// <param name="conn"></param>
    /// <param name="msg"></param>
    private void _DamageResponse(Connection conn, DamageResponse msg)
    {
        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            foreach (Damage item in msg.List)
            {
                var target = GameTools.GetActorById(item.TargetId);
                if (target == null) continue;
                target.recvDamage(item);
            }
        });
    }

    /// <summary>
    /// 数值更新响应
    /// 主要一些经常修改的数据
    /// </summary>
    /// <param name="conn"></param>
    /// <param name="msg"></param>
    private void _PropertyUpdateRsponse(Connection conn, PropertyUpdateRsponse msg)
    {
        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            foreach (PropertyUpdate item in msg.List)
            {

                var actor = GameTools.GetActorById(item.EntityId);
                if (actor == null) continue;                    //防止在aoi体系下，actor突然从我们的视野中消失

                Character chr;
                switch (item.Property)
                {
                    case PropertyUpdate.Types.Prop.Hp:
                        actor.OnHpChanged(item.OldValue.FloatValue, item.NewValue.FloatValue);
                        break;
                    case PropertyUpdate.Types.Prop.Mp:
                        actor.OnMpChanged(item.OldValue.FloatValue, item.NewValue.FloatValue);
                        break;
                    case PropertyUpdate.Types.Prop.Hpmax:
                        actor.OnHpmaxChanged(item.OldValue.FloatValue, item.NewValue.FloatValue);
                        break;
                    case PropertyUpdate.Types.Prop.Mpmax:
                        actor.OnMpmaxChanged(item.OldValue.FloatValue, item.NewValue.FloatValue);
                        break;
                    case PropertyUpdate.Types.Prop.Mode:
                        actor.OnModeChanged(item.OldValue.ModeValue, item.NewValue.ModeValue);
                        break;
                    case PropertyUpdate.Types.Prop.CombatMode:
                        actor.OnCombatModeChanged(item.OldValue.CombatModeValue, item.NewValue.CombatModeValue);
                        break;
                    case PropertyUpdate.Types.Prop.Level:
                        actor.OnLevelChanged(item.OldValue.IntValue, item.NewValue.IntValue);
                        break;
                    case PropertyUpdate.Types.Prop.Name:
                        break;
                    case PropertyUpdate.Types.Prop.Exp:
                        chr = actor as Character;
                        chr.onExpChanged(item.OldValue.LongValue, item.NewValue.LongValue);
                        break;
                    case PropertyUpdate.Types.Prop.Golds:
                        chr = actor as Character;
                        chr.onGoldChanged(item.OldValue.LongValue, item.NewValue.LongValue);
                        break;
                    case PropertyUpdate.Types.Prop.Speed:
                        actor.OnSpeedChanged(item.OldValue.IntValue, item.NewValue.IntValue);
                        break;
                }

            }
        });
    }

    /// <summary>
    /// 物品进入场景
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="msg"></param>
    private void _SpaceItemEnterResponse(Connection sender, SpaceItemEnterResponse msg)
    {
        EntityManager.Instance.OnItemEnterScene(msg.NetEItem);
    }

    /// <summary>
    /// 释放技能
    /// </summary>
    /// <param name="skill"></param>
    public  void SpellSkill(Skill skill,Actor target)
    {
        if (skill == null) return;
        //向服务器发送施法请求
        SpellCastRequest req = new SpellCastRequest() { Info = new CastInfo() };
        req.Info.SkillId = skill.Define.ID;
        req.Info.CasterId = skill.Owner.EntityId ;
        if (skill.IsUnitTarget)
        {
            //传个目标id
            req.Info.TargetId = target.EntityId;
        }
        else if (skill.IsPointTarget)
        {
            //传个pos即可
            req.Info.Point = V3.ToVec3(target.Position);
        }else if (skill.IsNoneTarget)
        {
            //无目标就啥也不用填了，这种技能类似旋风斩的，跟随主角的范围伤害。在这一点上和点目标技能进行了区分。
        }
        NetClient.Send(req);
    }

    /// <summary>
    /// 传送
    /// </summary>
    /// <param name="spaceId"></param>
    public void  SpaceDeliver(int spaceId,int point)
    {
        SpaceDeliverRequest req = new SpaceDeliverRequest();
        req.SpaceId = spaceId;
        req.PointId = point;
        NetClient.Send(req);
    }


}

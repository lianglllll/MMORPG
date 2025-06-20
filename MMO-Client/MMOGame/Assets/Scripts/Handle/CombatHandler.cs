using UnityEngine;
using GameClient.Entities;
using GameClient;
using GameClient.Combat;
using Common.Summer.Core;
using Common.Summer.Net;
using HS.Protobuf.Scene;
using HS.Protobuf.Combat.Skill;
using HS.Protobuf.SceneEntity;
using HSFramework.MySingleton;
using Serilog;
using HS.Protobuf.Combat.Buff;
using System;
using GameClient.Combat.Buffs;

public class CombatHandler : SingletonNonMono<CombatHandler>
{
    /// <summary>
    /// 初始化，gamemanager中启用
    /// </summary>
    public void Init()
    {
        ProtoHelper.Instance.Register<SpellCastRequest>((int)SkillProtocol.SpellCastReq);
        ProtoHelper.Instance.Register<SpellCastResponse>((int)SkillProtocol.SpellCastResp);
        ProtoHelper.Instance.Register<SpellCastFailResponse>((int)SkillProtocol.SpellCastFailResp);
        ProtoHelper.Instance.Register<BuffOperationResponse>((int)BuffProtocol.BuffOperationResp);
        ProtoHelper.Instance.Register<ActorPropertyUpdateRsponse>((int)SceneProtocl.ActorPropertyUpdateResp);
        ProtoHelper.Instance.Register<DamageResponse>((int)SkillProtocol.DamageResp);

        MessageRouter.Instance.Subscribe<SpellCastResponse>(HandleSpellCastResponse);
        MessageRouter.Instance.Subscribe<SpellCastFailResponse>(HandleSpellFailResponse);
        MessageRouter.Instance.Subscribe<BuffOperationResponse>(HandleBuffOperationResponse);
        MessageRouter.Instance.Subscribe<ActorPropertyUpdateRsponse>(HandleActorPropertyUpdateRsponse);
        MessageRouter.Instance.Subscribe<DamageResponse>(HandleDamageResponse);



        // 老旧：
        MessageRouter.Instance.Subscribe<SpaceEntitySyncResponse>(_SpaceEntitySyncResponse);
        MessageRouter.Instance.Subscribe<CtlClientSpaceEntitySyncResponse>(_CtlClientSpaceEntitySyncResponse);
        MessageRouter.Instance.Subscribe<SpaceEntityLeaveResponse>(_SpaceEntityLeaveResponse);
        //MessageRouter.Instance.Subscribe<DamageResponse>(_DamageResponse);
        //MessageRouter.Instance.Subscribe<PropertyUpdateRsponse>(_PropertyUpdateRsponse);
    }
    public void UnInit()
    {
        MessageRouter.Instance.UnSubscribe<SpellCastResponse>(HandleSpellCastResponse);
        MessageRouter.Instance.UnSubscribe<SpellCastFailResponse>(HandleSpellFailResponse);
        MessageRouter.Instance.UnSubscribe<SpaceEntitySyncResponse>(_SpaceEntitySyncResponse);
        MessageRouter.Instance.UnSubscribe<CtlClientSpaceEntitySyncResponse>(_CtlClientSpaceEntitySyncResponse);
        MessageRouter.Instance.UnSubscribe<SpaceEntityLeaveResponse>(_SpaceEntityLeaveResponse);
        MessageRouter.Instance.UnSubscribe<ActorPropertyUpdateRsponse>(HandleActorPropertyUpdateRsponse);
        MessageRouter.Instance.UnSubscribe<DamageResponse>(HandleDamageResponse);
    }

    // skill
    public void SendSpellCastReq(Skill skill, Actor target = null)
    {
        if (skill == null) return;
        //向服务器发送施法请求
        SpellCastRequest req = new SpellCastRequest() { Info = new CastInfo() };
        req.Info.SkillId = skill.Define.ID;
        req.Info.CasterId = skill.Owner.EntityId;
        if (skill.IsUnitTarget)
        {
            //传个目标id
            req.Info.TargetId = target.EntityId;
        }
        else if (skill.IsPointTarget)
        {
            //传个pos即可
            // req.Info.Point = V3.ToNetVector3(target.Position);
        }
        else if (skill.IsNoneTarget)
        {
            //无目标就啥也不用填了，这种技能类似旋风斩的，跟随主角的范围伤害。在这一点上和点目标技能进行了区分。
        }
        NetManager.Instance.Send(req);
    }
    private void HandleSpellFailResponse(Connection sender, SpellCastFailResponse msg)
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
    private void HandleSpellCastResponse(Connection conn, SpellCastResponse msg)
    {
        if(GameApp.SceneId != msg.SceneId)
        {
            Log.Warning("非本场景{0}的消息", GameApp.SceneId);
            goto End;
        }

        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            foreach (CastInfo item in msg.List)
            {
                // Log.Warning("entityId = {0}, skillId = {1}", item.CasterId, item.SkillId);
                var caster = EntityManager.Instance.GetEntity<Actor>(item.CasterId);
                if (caster == null) continue;

                // 因为其他的远端角色我们是不需要记录他的技能的，所以我们采用懒加载
                var skill = caster.m_skillManager.GetSkillBySkillId(item.SkillId);
                if(skill == null)
                {
                    skill = new Skill(caster, item.SkillId);
                    caster.m_skillManager.AddSkill(skill);
                }

                if (skill.IsUnitTarget)
                {
                    var target = EntityManager.Instance.GetEntity<Actor>(item.TargetId);
                    skill.Use(new SCEntity(target));
                }
                else if (skill.IsPointTarget)
                {

                }
                else if (skill.IsNoneTarget)
                {
                    skill.Use(new SCEntity(caster));
                }

                // skill ui
                if (GameApp.character.EntityId == skill.Owner.EntityId)
                {
                    UIManager.Instance.MessagePanel.ShowBottonMsg(skill.Define.Name, Color.green);
                }
            }
        });
    End:
        return;
    }
    private void HandleActorPropertyUpdateRsponse(Connection conn, ActorPropertyUpdateRsponse msg)
    {
        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            foreach (var item in msg.Propertys)
            {
                // 防止在aoi体系下，actor突然从我们的视野中消失
                var actor = EntityManager.Instance.GetEntity<Actor>(item.EntityId);
                if (actor == null) continue;
                switch (item.PropertyType)
                {
                    case PropertyType.Hp:
                        actor.OnHpChanged(item.OldValue.IntValue, item.NewValue.IntValue);
                        break;
                    case PropertyType.Mp:
                        actor.OnMpChanged(item.OldValue.IntValue, item.NewValue.IntValue);
                        break;
                    case PropertyType.MaxHp:
                        actor.OnMaxHpChanged(item.OldValue.IntValue, item.NewValue.IntValue);
                        break;
                    case PropertyType.MaxMp:
                        actor.OnMaxMpChanged(item.OldValue.IntValue, item.NewValue.IntValue);
                        break;
                    case PropertyType.Speed:
                        actor.OnSpeedChanged(item.OldValue.IntValue, item.NewValue.IntValue);
                        break;
                    case PropertyType.Level:
                        actor.OnLevelChanged(item.OldValue.IntValue, item.NewValue.IntValue);
                        break;
                    case PropertyType.Exp:
                        actor.OnExpChanged(item.OldValue.LongValue, item.NewValue.LongValue);
                        break;
                }
                if(actor.EntityId == GameApp.entityId)
                {
                    Kaiyun.Event.FireOut("LocalPlayerPropertyUpdate", item.PropertyType);
                }
            }
        });
    }
    private void HandleDamageResponse(Connection conn, DamageResponse msg)
    {
        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            foreach (Damage item in msg.Damages)
            {
                var target = EntityManager.Instance.GetEntity<Actor>(item.TargetId);
                if (target == null) continue;
                target.OnRecvDamage(item);
            }
        });
    }

    // buff
    private void HandleBuffOperationResponse(Connection sender, BuffOperationResponse msg)
    {
        if (GameApp.SceneId != msg.SceneId)
        {
            Log.Warning("非本场景{0}的消息 BuffOperationResponse", GameApp.SceneId);
            goto End;
        }

        var buffInfo = msg.BuffInfo;
        Actor owner = EntityManager.Instance.GetEntity<Actor>(buffInfo.OwnerId);
        if(owner == null)
        {
            Log.Warning("BuffOperationResponse无效，actor[{0}]不存在", buffInfo.OwnerId);
            goto End;
        }

        if(msg.OperationType == BuffOperationType.BuffOperationAdd)
        {
            owner.BuffManager.AddBuff(buffInfo);
        }
        else if (msg.OperationType == BuffOperationType.BuffOperationRemove)
        {
            owner.BuffManager.RemoveBuff(msg.RemoveInstanceIds);
        }
        else if (msg.OperationType == BuffOperationType.BuffOperationUpdate)
        {
            owner.BuffManager.UpdateBuff(buffInfo);
        }
        else if (msg.OperationType == BuffOperationType.BuffOperationRemoveeAll)
        {
            owner.BuffManager.RemoveAllBuff();
        }

    End:
        return;
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
        // EntityManager.Instance.RemoveEntity(msg.EntityId);
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
        NetManager.Instance.Send(req);
    }
}

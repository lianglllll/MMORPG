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
        MessageRouter.Instance.Subscribe<SpaceEntityLeaveResponse>(_SpaceEntityLeaveResponse);
        MessageRouter.Instance.Subscribe<SpellCastResponse>(_SpellCastResponse);
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
        MessageRouter.Instance.Off<SpaceEntityLeaveResponse>(_SpaceEntityLeaveResponse);
        MessageRouter.Instance.Off<SpellCastResponse>(_SpellCastResponse);
        MessageRouter.Instance.Off<DamageResponse>(_DamageResponse);
        MessageRouter.Instance.Off<PropertyUpdateRsponse>(_PropertyUpdateRsponse);
    }

    /// <summary>
    /// 进入场景的响应
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="msg"></param>
    private void _SpaceEnterResponse(Connection sender, SpaceEnterResponse msg)
    {
        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            if(GameApp.character==null )
            {

                //1.切换场景
                GameApp.LoadSpace(msg.Character.SpaceId);

                //2.加载其他角色和ai
                foreach(var item in msg.CharacterList)
                {
                    EntityManager.Instance.OnActorEnterScene(item);
                }

                //3.加载物品
                foreach(var item in msg.ItemEntityList)
                {
                    EntityManager.Instance.OnItemEnterScene(item);
                }

                //最后生成自己的角色,记录本机的数据
                EntityManager.Instance.OnActorEnterScene(msg.Character);
                GameApp.entityId = msg.Character.Entity.Id;
                GameApp.character = EntityManager.Instance.GetEntity<Character>(msg.Character.Entity.Id);

                //推入combatUI
                UIManager.Instance.ShowMessage("进入游戏，开始你的冒险");
                GameApp.combatPanelScript = (CombatPanelScript)UIManager.Instance.OpenPanel("CombatPanel");

            }else if(GameApp.character.info.SpaceId != msg.Character.SpaceId)
            {
                //清理旧场景的对象
                EntityManager.Instance.Clear();
                //切换场景
                GameApp.LoadSpace(msg.Character.SpaceId);
                //加载其他角色和ai
                foreach (var item in msg.CharacterList)
                {
                    EntityManager.Instance.OnActorEnterScene(item);
                }
                //3.加载物品
                foreach (var item in msg.ItemEntityList)
                {
                    EntityManager.Instance.OnItemEnterScene(item);
                }

                //最后生成自己的角色,记录本机的数据
                EntityManager.Instance.OnActorEnterScene(msg.Character);
                GameApp.entityId = msg.Character.Entity.Id;
                GameApp.character = EntityManager.Instance.GetEntity<Character>(msg.Character.Entity.Id);

                //刷新战斗面板,因为很多ui都依赖各种entity，刷新场景它们的依赖就失效了
                UIManager.Instance.ClosePanel("CombatPanel");
                UIManager.Instance.OpenPanel("CombatPanel");
            }
            else
            {
                //
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
    /// 同步信息接收，去找到这个entity对象，然后更新//todo 用字典<id,gameobject>来取
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
    /// 施法通知，自己施法响应的也是从这里来的
    /// </summary>
    /// <param name="conn"></param>
    /// <param name="msg"></param>
    private void _SpellCastResponse(Connection conn, SpellCastResponse msg)
    {

        foreach (CastInfo item in msg.List)
        {
            var caster = EntityManager.Instance.GetEntity<Actor>(item.CasterId);
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

        }
    }

    /// <summary>
    /// 伤害响应包，播放一下特效或者ui。不做数值更新
    /// </summary>
    /// <param name="conn"></param>
    /// <param name="msg"></param>
    private void _DamageResponse(Connection conn, DamageResponse msg)
    {
        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            foreach (Damage item in msg.List)
            {
                var target = GameTools.GetUnit(item.TargetId);
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

                var actor = GameTools.GetUnit(item.EntityId);
                switch (item.Property)
                {
                    case PropertyUpdate.Types.Prop.Hp:
                        actor.OnHpChanged(item.OldValue.FloatValue, item.NewValue.FloatValue);
                        break;
                    case PropertyUpdate.Types.Prop.Mp:
                        actor.OnMpChanged(item.OldValue.FloatValue, item.NewValue.FloatValue);
                        break;
                    case PropertyUpdate.Types.Prop.Hpmax:
                        break;
                    case PropertyUpdate.Types.Prop.Mpmax:
                        break;
                    case PropertyUpdate.Types.Prop.State:
                        actor.OnStateChanged(item.OldValue.StateValue, item.NewValue.StateValue);
                        break;
                    case PropertyUpdate.Types.Prop.Level:
                        actor.OnLevelChanged(item.OldValue.IntValue, item.NewValue.IntValue);
                        break;
                    case PropertyUpdate.Types.Prop.Name:
                        break;
                    case PropertyUpdate.Types.Prop.Exp:
                        actor.onExpChanged(item.OldValue.LongValue, item.NewValue.LongValue);
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
        EntityManager.Instance.OnItemEnterScene(msg.NetItemEntity);
    }


}
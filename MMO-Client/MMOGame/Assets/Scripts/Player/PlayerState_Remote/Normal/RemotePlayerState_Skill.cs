using GameClient.Combat;
using GameClient.Combat.LocalSkill.Config;
using HS.Protobuf.SceneEntity;
using Serilog;
using UnityEngine;

namespace Player
{
    public class RemotePlayerState_Skill: RemotePlayerState
    {
        private LocalSkill_Config_SO curLocalSkillConfig;
        private int curHitIdx;
        private Skill curSkill;

        // 蓄气、执行
        enum SkillChildState
        {
            Intonate, Active, None, Exit
        }
        private SkillChildState skillChildState;
        private SkillChildState ChildState
        {
            get => skillChildState;
            set
            {
                skillChildState = value;
                switch (skillChildState)
                {
                    case SkillChildState.Intonate:
                        remotePlayer.PlayAnimation(curLocalSkillConfig.IntonateAnimName);
                        break;
                    case SkillChildState.Active:
                        remotePlayer.PlayAnimation(curLocalSkillConfig.ActiveAnimName);
                        break;
                }
            }
        }

        public override void Enter()
        {
            curSkill = StateMachineParameter.curSkill;
            if (curSkill == null || (curSkill.Stage != SkillStage.Intonate && curSkill.Stage != SkillStage.Active))
            {
                ChildState = SkillChildState.Exit;
                goto End;
            }

            curLocalSkillConfig = LocalDataManager.Instance.GetLocalSkillConfigSOBySkillId(curSkill.SkillId);
            if (curLocalSkillConfig == null)
            {
                ChildState = SkillChildState.Exit;
                goto End;
            }

            if (curSkill.Stage == SkillStage.Intonate)
            {
                ChildState = SkillChildState.Intonate;
            }
            else if (curSkill.Stage == SkillStage.Active)
            {
                ChildState = SkillChildState.Active;
            }

            //注册根运动
            remotePlayer.Model.SetRootMotionAction(OnRootMotion);
            remotePlayer.Model.SetSkillHitAction(OnStartSkillHitAction, OnStopSkillHitAction);
        End:
            return;
        }
        public override void Update()
        {
            switch (ChildState)
            {
                case SkillChildState.Intonate:

                    //自身特效
                    if (curSkill.Define.IntonateArt != "")
                    {
                        var prefab = Res.LoadAssetSync<GameObject>(curSkill.Define.IntonateArt);
                        var ins = GameObject.Instantiate(prefab, remotePlayer.transform);
                        GameObject.Destroy(ins, curSkill.Define.IntonateTime);
                    }
                    if (curSkill.Stage == SkillStage.Active)
                    {
                        ChildState = SkillChildState.Active;
                    }
                    else if (curSkill.Stage == SkillStage.None || curSkill.Stage == SkillStage.Colding)
                    {
                        ChildState = SkillChildState.Exit;
                    }
                    break;
                case SkillChildState.Active:
                    if (curSkill.Stage == SkillStage.Colding || curSkill.Stage == SkillStage.None)
                    {
                        ChildState = SkillChildState.Exit;
                    }
                    break;
                case SkillChildState.Exit:
                    remotePlayer.ChangeState(NetActorState.Idle);
                    return;
            }
        }
        public override void Exit()
        {
            remotePlayer.Model.ClearRootMotionAction();
            remotePlayer.Model.ClearSkillHitAction();

            curLocalSkillConfig = null;
            if (StateMachineParameter.curSkill == curSkill)
            {
                StateMachineParameter.curSkill = null;
            }
            curSkill = null;
            curHitIdx = 0;
        }

        private void OnRootMotion(Vector3 deltaPosition, Quaternion deltaRotation)
        {
            deltaPosition.y = remotePlayer.gravity * Time.deltaTime;
            remotePlayer.CharacterController.Move(deltaPosition);
        }
        private void OnStartSkillHitAction(int obj)
        {
            // 因为动作融合的问题，不同技能的事件可能带到下一个技能去了
            if(curHitIdx >= curLocalSkillConfig.attackData.Length)
            {
                curHitIdx = curLocalSkillConfig.attackData.Length - 1;
                Log.Warning("{0}技能攻击数据超出", curLocalSkillConfig.ActiveAnimName);
            }
            var attackData = curLocalSkillConfig.attackData[curHitIdx];
            if (attackData == null)
            {
                goto End;
            }
            // 技能释放时音响：例如武器挥砍空气的声音
            if (attackData.attackAudioClip != null)
            {
                remotePlayer.PlayAudio(attackData.attackAudioClip);
            }
            // 技能释放时产生的物体：例如剑气
            if (attackData.SpawnObj != null)
            {
                remotePlayer.CreateSpawnObjectAroundOwner(attackData.SpawnObj);
            }
        End:
            return;
        }
        private void OnStopSkillHitAction(int obj)
        {
            curHitIdx++;
        }
    }
}

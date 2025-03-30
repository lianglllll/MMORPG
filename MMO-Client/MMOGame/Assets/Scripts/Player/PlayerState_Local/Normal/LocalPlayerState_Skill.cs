using GameClient.Combat;
using GameClient.Combat.LocalSkill.Config;
using HS.Protobuf.SceneEntity;
using System;
using UnityEngine;

namespace Player.PlayerState
{
    public class LocalPlayerState_Skill: LocalPlayerState
    {
        private LocalSkill_Config_SO curLocalSkillConfig;
        private int curHitIdx;
        private Skill curSkill;

        // 蓄气、执行
        enum SkillChildState
        {
            Intonate, Active,None,Exit
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
                        player.PlayAnimation(curLocalSkillConfig.IntonateAnimName);
                        break;
                    case SkillChildState.Active:
                        player.PlayAnimation(curLocalSkillConfig.ActiveAnimName) ;
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
            if(curLocalSkillConfig == null)
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
            player.Model.SetRootMotionAction(OnRootMotion);
            player.Model.SetSkillHitAction(OnStartSkillHitAction, OnStopSkillHitAction);
            curHitIdx = 0;
        End:
            return;
        }
        public override void Update()
        {
            // 闪避打断
            if (GameInputManager.Instance.Shift && player.IsCanCancelSkill)
            {
                curSkill.CancelSkill();
                player.ChangeState(NetActorState.Evade);
                goto End;
            }

            switch (ChildState)
            {
                case SkillChildState.Intonate:

                    //自身特效
                    if (curSkill.Define.IntonateArt != "")
                    {
                        var prefab = Res.LoadAssetSync<GameObject>(curSkill.Define.IntonateArt);
                        var ins = GameObject.Instantiate(prefab, player.transform);
                        GameObject.Destroy(ins, curSkill.Define.IntonateTime);
                    }
                    if (curSkill.Stage == SkillStage.Active)
                    {
                        ChildState = SkillChildState.Active;
                    }else if(curSkill.Stage == SkillStage.None || curSkill.Stage == SkillStage.Colding)
                    {
                        ChildState = SkillChildState.Exit;
                    }
                    break;
                case SkillChildState.Active:
                    if(curSkill.Stage == SkillStage.Colding || curSkill.Stage == SkillStage.None)
                    {
                        ChildState = SkillChildState.Exit;
                    }
                    break;
                case SkillChildState.Exit:
                    player.ChangeState(NetActorState.Idle);
                    goto End;
            }
        End:
            return;
        }
        public override void Exit()
        {
            player.Model.ClearRootMotionAction();
            curLocalSkillConfig = null;
            // 正常解释才走这个分支。
            if(StateMachineParameter.curSkill == curSkill)
            {
                StateMachineParameter.curSkill = null;
                Kaiyun.Event.FireIn("SkillActiveEnd");
            }
            curSkill = null;
        }

        private void OnRootMotion(Vector3 deltaPosition, Quaternion deltaRotation)
        {
            deltaPosition.y = player.gravity * Time.deltaTime;
            player.CharacterController.Move(deltaPosition);
        }
        private void OnStartSkillHitAction(int obj)
        {
            if(curLocalSkillConfig == null)
            {
                goto End;
            }
            var attackData = curLocalSkillConfig.attackData[curHitIdx];
            if(attackData == null)
            {
                goto End;
            }
            // 技能释放时音响：例如武器挥砍空气的声音
            if(attackData.attackAudioClip != null)
            {
                player.PlayAudio(attackData.attackAudioClip);
            }
            // 技能释放时产生的物体：例如剑气
            if(attackData.SpawnObj != null)
            {
                player.CreateSpawnObjectAroundOwner(attackData.SpawnObj);
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

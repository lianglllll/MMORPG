using GameClient.Combat;
using HS.Protobuf.SceneEntity;
using UnityEngine;

namespace Player
{
    public class CtrlState_Skill: CtrlState
    {
        private Skill curSkill => ShareParameter.curSkill;
        //蓄气、执行
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
                        player.PlayAnimation(curSkill.Define.IntonateAnimName);
                        break;
                    case SkillChildState.Active:
                        player.PlayAnimation(curSkill.Define.ActiveAnimName) ;
                        break;
                }
            }
        }
        public override void Enter()
        {
            if(curSkill == null)
            {
                ChildState = SkillChildState.Exit;
            }
            else if (curSkill.Stage == SkillStage.Intonate)
            {
                ChildState = SkillChildState.Intonate;
            }
            else if (curSkill.Stage == SkillStage.Active)
            {
                ChildState = SkillChildState.Active;
            }
            else
            {
                ChildState = SkillChildState.Exit;
            }
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
                    player.ChangeState(ActorState.Idle);
                    return;
            }
        }
        public override void Exit()
        {
            ShareParameter.curSkill = null;
        }

    }
}

using HS.Protobuf.SceneEntity;

namespace Player
{
    public class CtrlState_Hurt: CtrlState
    {
        public override void Enter()
        {
            player.PlayAnimation("Hurt");

            //看向敌人
            var target = ShareParameter.attacker;
            if (target != null)
            {
                player.DirectLookTarget(target.renderObj.transform.position);
                ShareParameter.attacker = null;
            }

        }

        public override void Update()
        {
            //todo,僵直时间应该像眩晕那样由服务器控制
            if(CheckAnimatorStateName("Hurt",out var time) && time > 0.9f)
            {
                player.ChangeState(ActorState.Idle);
            }
        }

        public override void Exit()
        {
        }

    }
}

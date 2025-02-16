using HSFramework.AI.StateMachine;
using Player.Controller;


namespace Player
{
    public class LocalPlayerState : StateBase
    {
        protected LocalPlayerController player;
        protected StateMachineParameter ShareParameter => player.StateMachineParameter;

        public override void Init(IStateMachineOwner owner)
        {
            base.Init(owner);
            player = (LocalPlayerController)owner;
        }

        //由于animator的缺陷，第一次到达update时，动画可能还不是jump（过渡状态）
        protected virtual bool CheckAnimatorStateName(string name, out float time)
        {
            //处于动画过渡阶段的考虑，优先判断下一个状态
            //假设现在处于A->B的过渡状态，在状态机的逻辑上我们已经是B了，但是动画状态机还认为我们是A
            var nextInfo = player.Model.Animator.GetNextAnimatorStateInfo(0);
            if (nextInfo.IsName(name))
            {
                time = nextInfo.normalizedTime;
                return true;
            }

            var info = player.Model.Animator.GetCurrentAnimatorStateInfo(0);
            time = info.normalizedTime;
            return info.IsName(name);
        }

    }

}

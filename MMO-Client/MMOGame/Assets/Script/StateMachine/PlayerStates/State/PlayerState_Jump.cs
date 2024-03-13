using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState_Jump : PlayerState
{

    public PlayerState_Jump(PlayerStateMachine stateMachine)
    {
        Initialize(stateMachine);
    }

    public override void Enter()
    {
        //animator.Play("JumpUp");
        //animator.Play("JumpFall");
        //animator.Play("JumpDown");

    }

    public override void LogicUpdate()
    {
        //这个估计是使用pos中的y轴数值变换做实现，比如说进入jump状态的时候，就记录每一次jump中y的数值，根据本次y和上次y的数值做跳跃动作的变换。
    }


    public override void Exit()
    {

    }

}

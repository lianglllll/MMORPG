using GameClient.Combat;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class StateMachine : MonoBehaviour
{
    protected IState currentState;

    private void Update()
    {
        currentState.LogicUpdate();
    }

    private void FixedUpdate()
    {
        currentState.PhysicUpdate();
    }

    //当前状态的启动
    protected void SwitchOn(IState newState)
    {
        currentState = newState;
        currentState.Enter();
    }

    //公有的状态切换
    public void SwitchState(IState newState)
    {
        currentState.Exit();
        SwitchOn(newState);
    }
}

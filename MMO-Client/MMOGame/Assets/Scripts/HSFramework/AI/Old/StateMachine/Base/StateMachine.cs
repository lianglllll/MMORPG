using GameClient.Combat;
using GameClient.Entities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

 namespace BaseSystem.AI
{
    public class StateMachine : MonoBehaviour
    {
        protected IState currentState;
        protected virtual void Update()
        {
            currentState.LogicUpdate();
        }
        protected virtual void FixedUpdate()
        {
            currentState.PhysicUpdate();
        }

        //状态切换
        protected void SwitchState(IState newState)
        {
            currentState?.Exit();
            SwitchOn(newState);
        }

        //当前状态的启动
        protected void SwitchOn(IState newState)
        {
            currentState = newState;
            currentState.Enter();
        }

    }
}



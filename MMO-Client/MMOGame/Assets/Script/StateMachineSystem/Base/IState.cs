using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IState
{
    void Enter();
    void Exit();
    void LogicUpdate();//状态的逻辑更新
    void PhysicUpdate();
}

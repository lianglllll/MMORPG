using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 状态接口
/// </summary>
public interface IState
{
    void Enter();
    void Exit();
    void LogicUpdate();//状态的逻辑更新
    void PhysicUpdate();
}

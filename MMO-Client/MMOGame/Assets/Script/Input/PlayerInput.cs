using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;



/// <summary>
/// 需要挂载到玩家对象上
/// </summary>
public class PlayerInput : MonoBehaviour
{
    //我们动作的集合
    PlayerInputActions playerInputActions;

    //获取两个动作的信号
    public bool Jump => playerInputActions.GamePlay.Jump.WasPerformedThisFrame();
    public bool StopJump => playerInputActions.GamePlay.Jump.WasReleasedThisFrame();

    Vector2 wasd => playerInputActions.GamePlay.Move.ReadValue<Vector2>();
    public float AxisX => wasd.x;
    public float AxisY => wasd.y;
    public bool Move => AxisX != 0f || AxisY != 0f;



    private void Awake()
    {
        playerInputActions = new PlayerInputActions();
    }

    //启动GamePlay动作表
    //启用某个动作表，只需要调用它的Enable函数就可以了
    public void EnableGameplayInputs()
    {
        playerInputActions.GamePlay.Enable();
    }





}

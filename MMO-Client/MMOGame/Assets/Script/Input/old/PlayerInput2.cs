using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

[CreateAssetMenu(menuName ="player Input")]
public class PlayerInput2 : ScriptableObject, PlayerInputActions.IGamePlayActions
{
    private PlayerInputActions playInputActions;

    public event UnityAction<Vector2> onMove = delegate { };
    public event UnityAction onStopMove = delegate { };


    //init
    void OnEnable()
    {
        playInputActions = new PlayerInputActions();
        playInputActions.GamePlay.SetCallbacks(this);//登记gameplay动作表回调
    }



    //启用gameplay动作表
    public void EnableGamePlayInput()
    {
        playInputActions.GamePlay.Enable();

        //鼠标隐藏
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    //暂停动作表
    public void OnDisable()
    {
        DisableAllInput();
    }

    //禁用所有的输入
    public void DisableAllInput()
    {
        playInputActions.GamePlay.Disable();
    }


    public void OnJump(InputAction.CallbackContext context)
    {
        throw new System.NotImplementedException();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if(context.phase == InputActionPhase.Performed)
        {
            onMove.Invoke(context.ReadValue<Vector2>());
        }else if(context.phase == InputActionPhase.Canceled)
        {
            onStopMove.Invoke();
        }
    }
}

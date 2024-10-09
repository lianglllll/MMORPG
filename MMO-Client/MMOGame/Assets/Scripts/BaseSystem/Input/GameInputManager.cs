using BaseSystem.Singleton;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameInputManager : Singleton<GameInputManager>
{
    private InputActions _inputActions;

    public Vector2 Movement => _inputActions.GameInput.Movement.ReadValue<Vector2>();           //获取二维输入
    public Vector2 CameraLook => _inputActions.GameInput.CameraLook.ReadValue<Vector2>();
    public bool Run => _inputActions.GameInput.Run.phase == InputActionPhase.Performed;
    public bool Jump => _inputActions.GameInput.Jump.triggered;
    public bool Shift => _inputActions.GameInput.Run.triggered;
    public bool LAttack => _inputActions.GameInput.LAttack.triggered;
    public bool LAttackPerFormed => _inputActions.GameInput.LAttack.phase == InputActionPhase.Performed;
    public bool RAttack => _inputActions.GameInput.RAttack.triggered;
    public bool RAttackPerFormed => _inputActions.GameInput.RAttack.phase == InputActionPhase.Performed;
    public bool Defense => _inputActions.GameInput.Grab.triggered;



    public bool Climb => _inputActions.GameInput.Climb.triggered;
    public bool Grab => _inputActions.GameInput.Grab.triggered;
    public bool TakeOut => _inputActions.GameInput.TakeOut.triggered;
    public bool Dash => _inputActions.GameInput.Dash.triggered;
    public bool Equip => _inputActions.GameInput.EquipWP.triggered;
    public bool Quit => _inputActions.GameInput.Quit.triggered;
    public bool Enter => _inputActions.GameInput.Enter.triggered;

    public bool AnyKey => _inputActions.GameInput.AnyKey.triggered;



    protected override void Awake()
    {
        base.Awake();
        _inputActions ??= new InputActions();       //判断是否为null，如果是就new一个新的        
    }

    private void OnEnable()
    {
        _inputActions.Enable();//启用所有map

        //可以创建多张输入表
        //_inputActions.GameInput.Enable();
        //_inputActions.GameInput.Disable();
    }

    private void OnDisable()
    {
        _inputActions.Disable();
    }

}

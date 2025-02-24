using HSFramework.Singleton;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameInputManager : Singleton<GameInputManager>
{
    private InputActions _inputActions;
    private Dictionary<string, List<string>>  m_originalActionBindings = new Dictionary<string, List<string>>();

    #region 输入

    public Vector2 Movement => _inputActions.GameInput.Movement.ReadValue<Vector2>();           //获取二维输入
    public Vector2 CameraLook => _inputActions.GameInput.CameraLook.ReadValue<Vector2>();
    public bool SustainLeftShift => _inputActions.GameInput.Run.phase == InputActionPhase.Performed;
    public bool Jump => _inputActions.GameInput.Jump.triggered;
    public bool Shift => _inputActions.GameInput.Run.triggered;
    public bool LAttack => _inputActions.GameInput.LAttack.triggered;
    public bool LAttackPerFormed => _inputActions.GameInput.LAttack.phase == InputActionPhase.Performed;
    public bool RAttack => _inputActions.GameInput.RAttack.triggered;
    public bool RAttackPerFormed => _inputActions.GameInput.RAttack.phase == InputActionPhase.Performed;
    public bool Defense => _inputActions.GameInput.Grab.triggered;
    public bool SustainLeftAlt => _inputActions.GameInput.LetfAlt.phase == InputActionPhase.Performed;
    public bool Space => _inputActions.GameInput.Space.triggered;
    public bool Crouch => _inputActions.GameInput.Crouch.triggered;
    public bool KeyOneDown => _inputActions.GameInput.KeyOne.triggered;
    public bool KeyTwoDown => _inputActions.GameInput.KeyTwo.triggered;
    public bool SustainQ => _inputActions.GameInput.Q.phase == InputActionPhase.Performed;
    public bool SustainE => _inputActions.GameInput.E.phase == InputActionPhase.Performed;
    public bool AnyKey => _inputActions.GameInput.AnyKey.triggered;
    public bool ESC => _inputActions.GameInput.KeyEsc.triggered;

    #endregion

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


    private void Start()
    {
        GetAllActionBindings();
    }

    private void Update()
    {

    }

    public void Change()
    {
        InputAction jumpAction = _inputActions.GameInput.Jump;
        if (jumpAction != null)
        {
            // Disable the action before modifying it
            jumpAction.Disable();
            var newKey = "a";
            // Clear existing bindings
            jumpAction.ApplyBindingOverride(0, $"<Keyboard>/{newKey}");

            // Re-enable the action
            jumpAction.Enable();
        }
    }

    public void  GetAllActionBindings()
    {
        //todo获取 的时候顺便吧action也保存起来。
        // 遍历每个动作映射
        foreach (var map in _inputActions.asset.actionMaps)
        {
            // 遍历每个动作
            foreach (var action in map.actions)
            {
                var bindingPaths = new List<string>();

                // 获取每个绑定的有效路径
                foreach (var binding in action.bindings)
                {
                    bindingPaths.Add(binding.effectivePath);
                }

                // 将动作名称和绑定路径添加到字典中
                m_originalActionBindings[action.name] = bindingPaths;
            }
        }

    }


}

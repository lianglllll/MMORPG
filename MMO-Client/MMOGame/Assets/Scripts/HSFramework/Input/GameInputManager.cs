/*
using HSFramework.MySingleton;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public enum GameInputMode
{
    Game,UI
}

public class GameInputManager : Singleton<GameInputManager>
{
    private GameInputMode m_gameInputMode;
    private InputActions _inputActions;
    private Dictionary<string, List<string>>  m_originalActionBindings = new Dictionary<string, List<string>>();

    #region Game Input

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
    public bool Space => _inputActions.GameInput.Space.triggered;
    public bool Crouch => _inputActions.GameInput.Crouch.triggered;
    public bool KeyOneDown => _inputActions.GameInput.KeyOne.triggered;
    public bool KeyTwoDown => _inputActions.GameInput.KeyTwo.triggered;
    public bool KeyThreeDown => _inputActions.GameInput.KeyThree.triggered;

    public bool SustainQ => _inputActions.GameInput.Q.phase == InputActionPhase.Performed;
    public bool SustainE => _inputActions.GameInput.E.phase == InputActionPhase.Performed;
    public bool AnyKey => _inputActions.GameInput.AnyKey.triggered;
    public bool GI_ESC => _inputActions.GameInput.KeyEsc.triggered;
    public bool SustainLeftAlt => _inputActions.GameInput.LetfAlt.phase == InputActionPhase.Performed;

    #endregion

    #region UI Input

    public bool UI_ESC => _inputActions.UIInput.KeyEsc.triggered;

    #endregion

    public GameInputMode GameInputMode => m_gameInputMode;

    protected override void Awake()
    {
        base.Awake();
        _inputActions ??= new InputActions();       //判断是否为null，如果是就new一个新的        
    }
    private void Start()
    {

    }
    private void OnEnable()
    {
        m_gameInputMode = GameInputMode.UI;
        _inputActions.GameInput.Disable();
        _inputActions.UIInput.Enable();

        GetAllActionBindings();
    }
    private void OnDisable()
    {
        _inputActions.Disable();
    }
    private void Update()
    {

    }


    public void SwitchGameInputMode(GameInputMode mode)
    {
        if (m_gameInputMode == mode)
        {
            goto End;
        }
        _inputActions.Disable();
        m_gameInputMode = mode;
        switch (m_gameInputMode)
        {
            case GameInputMode.Game:
                _inputActions.GameInput.Enable();
                break;
            case GameInputMode.UI:
                _inputActions.UIInput.Enable();
                break;
        }

    End:
        return;
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

*/


using HSFramework.MySingleton;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public enum GameInputMode
{
    Game,
    UI,
    Cinematic
}

public class GameInputManager : Singleton<GameInputManager>
{
    #region Structures
    [Serializable]
    public class InputConfig
    {
        public float inputBufferTime = 0.15f;
        public float analogDeadzone = 0.1f;
        public float holdThreshold = 0.5f;
    }

    private class InputState
    {
        // 新增中间状态缓冲区
        private bool _pendingPress;
        private bool _pendingRelease;

        public bool Pressed { get; private set; }
        public bool Released { get; private set; }
        public bool Holding { get; private set; }
        public float HoldDuration { get; private set; }
        public float LastPressedTime { get; private set; }

        private readonly InputAction _action;

        public InputState(InputAction action)
        {
            _action = action;

            // 事件回调只修改中间状态
            action.started += _ => {
                _pendingPress = true;
                LastPressedTime = Time.time;
            };

            action.canceled += _ => {
                _pendingRelease = true;
                Holding = false;
                HoldDuration = 0f;
            };
        }

        public void UpdateState()
        {
            // Step 1: 转移中间状态到正式状态
            Pressed = _pendingPress;
            Released = _pendingRelease;

            // Step 2: 重置中间状态
            _pendingPress = false;
            _pendingRelease = false;

            // Step 3: 更新持续状态
            var isPressed = _action?.IsPressed() ?? false;

            if (isPressed)
            {
                HoldDuration += Time.deltaTime;
                Holding = HoldDuration > Instance._config.holdThreshold;
            }
            else
            {
                // 确保在释放时重置持续状态
                if (Released)
                {
                    HoldDuration = 0f;
                    Holding = false;
                }
            }
        }

        public bool HasBufferedInput(float currentTime) =>
            Pressed && (currentTime - LastPressedTime <= Instance._config.inputBufferTime);

        public void Reset()
        {
            Pressed = false;
            Released = false;
            Holding = false;
            HoldDuration = 0f;
            LastPressedTime = 0f;
            _pendingPress = false;
            _pendingRelease = false;
        }
    }
    #endregion

    [SerializeField] private InputConfig _config;

    private InputActions _actions;
    private GameInputMode _currentMode;
    private Dictionary<string, InputState> _gameStates = new();
    private Dictionary<string, InputState> _uiStates = new();

    public GameInputMode GameInputMode => _currentMode;

    #region Game

    public Vector2 Movement => GetAnalogValue("Movement");
    public Vector2 CameraLook => GetAnalogValue("CameraLook");

    public bool SustainQ => GetState("Q").Holding;
    public bool SustainE => GetState("E").Holding;
    public bool Crouch => GetState("C").Pressed;
    public bool Equip => GetState("Z").Pressed;

    public bool SustainLeftShift => GetState("LeftShift").Holding;
    public bool SustainLeftAlt => GetState("LetfAlt").Holding;
    public bool Shift => GetState("LeftShift").Pressed;

    public bool LAttackHolding => GetState("MouseLeft").Holding;
    public bool LAttackPressed => GetState("MouseLeft").Pressed;
    public bool LAttackReleased => GetState("MouseLeft").Released;
    public bool RAttack => GetState("MouseRight").Pressed;

    public bool Space => GetState("Space").Pressed;

    public bool KeyOneDown => GetState("KeyOne").Pressed;
    public bool KeyTwoDown => GetState("KeyTwo").Pressed;
    public bool KeyThreeDown => GetState("KeyThree").Pressed;
    public bool KeyFourDown => GetState("KeyFour").Pressed;
    public bool KeyOneReleased => GetState("KeyOne").Released;
    public bool KeyTwoReleased => GetState("KeyTwo").Released;
    public bool KeyThreeReleased => GetState("KeyThree").Released;
    public bool KeyFourReleased => GetState("KeyFour").Released;


    public bool AnyKey => GetState("AnyKey").Pressed;
    public bool GI_ESC => GetState("KeyEsc").Pressed;
    public bool GI_Enter => GetState("Enter").Pressed;
    public bool GI_Tab => GetState("Tab").Pressed;
    public bool GI_Caps => GetState("Caps").Pressed;

    // 遗弃
    public bool Defense => GetState("F").Pressed;

    #endregion

    #region UI
    public bool UI_ESC => GetUIState("KeyEsc").Pressed;
    public bool UI_Enter => GetUIState("KeyEnter").Pressed;

    #endregion

    #region Lifecycle
    protected override void Awake()
    {
        base.Awake();
        InitializeInputSystem();
    }
    private void Update()
    {
        var currentTime = Time.time;
        UpdateAllStates(_gameStates, currentTime);
        UpdateAllStates(_uiStates, currentTime);
    }
    private void OnDestroy()
    {
        DisposeInputSystem();
    }
    #endregion

    #region Core System
    private void InitializeInputSystem()
    {
        _actions = new InputActions();

        // 初始化游戏输入状态
        var gameInputMap = _actions.GameInput.Get();
        foreach (var action in gameInputMap)
        {
            _gameStates[action.name] = new InputState(action);
        }

        // 初始化UI输入状态
        var uiInputMap = _actions.UIInput.Get();
        foreach (var action in uiInputMap)
        {
            _uiStates[action.name] = new InputState(action);
        }

        SwitchInputMode(GameInputMode.UI);
    }

    private void DisposeInputSystem()
    {
        _actions?.Disable();
        _actions = null;
    }

    public void SwitchInputMode(GameInputMode newMode)
    {
        if (_currentMode == newMode) return;

        // 禁用当前模式
        switch (_currentMode)
        {
            case GameInputMode.Game:
                _actions.GameInput.Disable();
                break;
            case GameInputMode.UI:
                _actions.UIInput.Disable();
                break;
        }

        // 启用新模式
        switch (newMode)
        {
            case GameInputMode.Game:
                _actions.GameInput.Enable();
                break;
            case GameInputMode.UI:
                _actions.UIInput.Enable();
                break;
        }

        _currentMode = newMode;
        ResetAllStates();
    }
    #endregion

    #region State Management
    private void UpdateAllStates(Dictionary<string, InputState> states, float currentTime)
    {
        foreach (var state in states.Values)
        {
            state.UpdateState();
        }
    }

    private void ResetAllStates()
    {
        foreach (var state in _gameStates.Values)
        {
            state.UpdateState();
        }
        foreach (var state in _uiStates.Values)
        {
            state.UpdateState();
        }
    }
    #endregion

    #region Helper Methods
    private InputState GetState(string actionName)
    {
        if (!_gameStates.TryGetValue(actionName, out var state))
        {
            Debug.LogWarning($"动作 '{actionName}' 未注册");
            return new InputState(null);
        }
        return state;
    }

    private InputState GetUIState(string actionName) =>
        _uiStates.TryGetValue(actionName, out var state) ? state : null;

    private Vector2 GetAnalogValue(string actionName)
    {
        // 从GameInput的ActionMap中查找动作
        var action = _actions.GameInput.Get().FindAction(actionName);
        if (action == null) return Vector2.zero;

        var value = action.ReadValue<Vector2>();
        return value.magnitude > _config.analogDeadzone ? value : Vector2.zero;
    }

    private bool CheckBufferedInput(string actionName)
    {
        var state = GetState(actionName);
        return state != null && state.HasBufferedInput(Time.time);
    }

    public void RemapBinding(string actionName, string newPath, int bindingIndex = 0)
    {
        // 优先从GameInput查找
        var action = _actions.GameInput.Get().FindAction(actionName)
                   ?? _actions.UIInput.Get().FindAction(actionName);

        if (action != null)
        {
            action.Disable();
            action.ApplyBindingOverride(bindingIndex, new InputBinding(newPath));
            action.Enable();
        }
    }

    public void ResetAllBindings()
    {
        foreach (var map in _actions.asset.actionMaps)
        {
            foreach (var action in map.actions)
            {
                action.RemoveAllBindingOverrides();
            }
        }
    }
    #endregion
}


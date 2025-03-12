using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : Singleton<InputManager>, GameInput.IGamePlayActions, GameInput.IUIActions
{
    #region 初始化

    private GameInput gameInput;
    private bool isInitialized;

    public void Init()
    {
        if (isInitialized) return;
        gameInput = new GameInput();
        gameInput.GamePlay.SetCallbacks(this);
        gameInput.UI.SetCallbacks(this);
        SetGamePlay();
    }

    public void UnInit()
    {
        gameInput?.Dispose();
        instance = null;
    }

    private void SetGamePlay()
    {
        gameInput.GamePlay.Enable();
        gameInput.UI.Disable();
    }

    private void SetUI()
    {
        gameInput.UI.Enable();
        gameInput.GamePlay.Disable();
    }

    #endregion


    public event Action<Vector2> MoveEvent;
    public event Action FlashEvent;
    public event Action MenuEvent;
    public event Action InfoEvent;
    public event Action InfoCancelledEvent;
    public event Action CommonAttackEvent;
    public event Action CommonSkillEvent;
    public event Action UltimateSkillEvent;
    public event Action ResumeEvent;

    /// <summary>
    /// 移动
    /// </summary>
    public void OnMove(InputAction.CallbackContext context)
    {
        MoveEvent?.Invoke(context.ReadValue<Vector2>());
    }

    /// <summary>
    /// 闪避
    /// </summary>
    public void OnFlash(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            FlashEvent?.Invoke();
        }
    }

    /// <summary>
    /// 菜单
    /// </summary>    
    public void OnMenu(InputAction.CallbackContext context)
    {
        LogSystem.Log($"Phase:{context.phase} Menu:{context.ReadValue<Vector2>()}");
        if (context.phase == InputActionPhase.Started)
        {
            MenuEvent?.Invoke();
            SetUI();
        }
    }

    /// <summary>
    /// 战斗信息
    /// </summary>
    public void OnInfo(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            InfoEvent?.Invoke();
        }
        else if (context.phase == InputActionPhase.Canceled)
        {
            InfoCancelledEvent?.Invoke();
        }
    }

    /// <summary>
    /// 普通攻击
    /// </summary>
    public void OnCommonAttack(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            CommonAttackEvent?.Invoke();
        }
    }

    /// <summary>
    /// 普通技能
    /// </summary>
    public void OnCommonSkill(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            CommonSkillEvent?.Invoke();
        }
    }

    /// <summary>
    /// 终极技能
    /// </summary>
    public void OnUltimateSkill(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            UltimateSkillEvent?.Invoke();
        }
    }

    /// <summary>
    /// 关闭菜单
    /// </summary>
    public void OnResume(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            ResumeEvent?.Invoke();
            SetGamePlay();
        }
    }
}
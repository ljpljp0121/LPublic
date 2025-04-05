using System;
using cfg.role;
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

    /// <summary>
    /// 移动
    /// </summary>
    public void OnMove(InputAction.CallbackContext context)
    {
        LogSystem.Log($"Phase:{context.phase} Move:{context.ReadValue<Vector2>()}");
        EventSystem.DispatchEvent(new EOnInputMove(context.ReadValue<Vector2>()));
    }

    /// <summary>
    /// 闪避
    /// </summary>
    public void OnFlash(InputAction.CallbackContext context)
    {
        LogSystem.Log($"Phase:{context.phase} Flash:{context.ReadValue<float>()}");
        if (context.phase == InputActionPhase.Started)
        {
            EventSystem.DispatchEvent(new EOnInputFlash());
        }
    }

    /// <summary>
    /// 菜单
    /// </summary>    
    public void OnMenu(InputAction.CallbackContext context)
    {
        LogSystem.Log($"Phase:{context.phase} Menu:{context.ReadValue<float>()}");
        if (context.phase == InputActionPhase.Started)
        {
            EventSystem.DispatchEvent(new EOnInputMenu());
            SetUI();
        }
    }

    /// <summary>
    /// 战斗信息
    /// </summary>
    public void OnInfo(InputAction.CallbackContext context)
    {
        LogSystem.Log($"Phase:{context.phase} Info:{context.ReadValue<float>()}");
        if (context.phase == InputActionPhase.Started)
        {
            EventSystem.DispatchEvent(new EOnInputInfo());
        }
        else if (context.phase == InputActionPhase.Canceled)
        {
            EventSystem.DispatchEvent(new EOnInputInfoCancelled());
        }
    }

    /// <summary>
    /// 普通攻击
    /// </summary>
    public void OnCommonAttack(InputAction.CallbackContext context)
    {
        LogSystem.Log($"Phase:{context.phase} CommonAttack:{context.ReadValue<float>()}");
        if (context.phase == InputActionPhase.Started)
        {
            EventSystem.DispatchEvent(new EOnInputCommonAttack());
        }
    }

    /// <summary>
    /// 普通技能
    /// </summary>
    public void OnCommonSkill(InputAction.CallbackContext context)
    {
        LogSystem.Log($"Phase:{context.phase} CommonSkill:{context.ReadValue<float>()}");
        if (context.phase == InputActionPhase.Started)
        {
            EventSystem.DispatchEvent(new EOnInputCommonSkill());
        }
    }

    /// <summary>
    /// 终极技能
    /// </summary>
    public void OnUltimateSkill(InputAction.CallbackContext context)
    {
        LogSystem.Log($"Phase:{context.phase} UltimateSkill:{context.ReadValue<float>()}");
        if (context.phase == InputActionPhase.Started)
        {
            EventSystem.DispatchEvent(new EOnInputUltimateSkill());
        }
    }

    /// <summary>
    /// 关闭菜单
    /// </summary>
    public void OnResume(InputAction.CallbackContext context)
    {
        LogSystem.Log($"Phase:{context.phase} Resume:{context.ReadValue<float>()}");
        if (context.phase == InputActionPhase.Started)
        {
            EventSystem.DispatchEvent(new EOnInputResume());
            SetGamePlay();
        }
    }
}
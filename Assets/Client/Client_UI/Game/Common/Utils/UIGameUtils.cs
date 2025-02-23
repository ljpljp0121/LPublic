using System;
using UnityEngine.UI;

public static class UIGameUtils
{
    public static Button SetButton(this Button button, Action onClick)
    {
        if (button == null) return null;
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => { onClick?.Invoke(); });
        return button;
    }
}
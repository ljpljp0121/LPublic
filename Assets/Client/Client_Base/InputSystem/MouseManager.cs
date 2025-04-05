using UnityEngine;

public class MouseManager
{
    public static void SetMouseLocked(bool locked)
    {
        Cursor.visible = !locked;
        Cursor.lockState = locked ? CursorLockMode.Locked : CursorLockMode.None;
    }
}
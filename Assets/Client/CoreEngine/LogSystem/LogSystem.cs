using UnityEngine;

/// <summary>
/// Debug系统
/// </summary>
public static class LogSystem
{
    static LogSystem()
    {
    }

    public static void Log(string message)
    {
        Debug.Log(message);
    }

    public static void Error(string message)
    {
        Debug.LogError(message);
    }

    public static void Warning(string message)
    {
        Debug.LogWarning(message);
    }
}
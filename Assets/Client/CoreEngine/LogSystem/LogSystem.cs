using System;
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

    public static void Error(Exception e)
    {
        Debug.LogError(e);
    }

    public static void Error(string message, Exception e)
    {
        Debug.LogError(string.Format(message, e));
    }

    public static void Warning(string message)
    {
        Debug.LogWarning(message);
    }
}
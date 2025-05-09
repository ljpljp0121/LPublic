using System;

/// <summary>
/// 事件系统
/// </summary>
public static class EventSystem
{
    private static EventModule eventModule;

    public static void Init()
    {
        eventModule = new EventModule();
    }

    #region 添加监听

    /// <summary>
    /// 添加无参事件
    /// </summary>
    public static void RegisterEvent(string eventName, Action action)
    {
        eventModule.RegisterEvent(eventName, action);
    }

    // <summary>
    // 添加1参事件监听
    // </summary>
    public static void RegisterEvent<T>(string eventName, Action<T> action)
    {
        eventModule.RegisterEvent(eventName, action);
    }

    // <summary>
    // 添加2参事件监听
    // </summary>
    public static void RegisterEvent<T0, T1>(string eventName, Action<T0, T1> action)
    {
        eventModule.RegisterEvent(eventName, action);
    }

    // <summary>
    // 添加3参事件监听
    // </summary>
    public static void RegisterEvent<T0, T1, T2>(string eventName, Action<T0, T1, T2> action)
    {
        eventModule.RegisterEvent(eventName, action);
    }

    // <summary>
    // 添加4参事件监听
    // </summary>
    public static void RegisterEvent<T0, T1, T2, T3>(string eventName, Action<T0, T1, T2, T3> action)
    {
        eventModule.RegisterEvent(eventName, action);
    }

    // <summary>
    // 添加5参事件监听
    // </summary>
    public static void RegisterEvent<T0, T1, T2, T3, T4>(string eventName, Action<T0, T1, T2, T3, T4> action)
    {
        eventModule.RegisterEvent(eventName, action);
    }

    // <summary>
    // 添加6参事件监听
    // </summary>
    public static void RegisterEvent<T0, T1, T2, T3, T4, T5>(string eventName, Action<T0, T1, T2, T3, T4, T5> action)
    {
        eventModule.RegisterEvent(eventName, action);
    }

    // <summary>
    // 添加7参事件监听
    // </summary>
    public static void RegisterEvent<T0, T1, T2, T3, T4, T5, T6>(string eventName,
        Action<T0, T1, T2, T3, T4, T5, T6> action)
    {
        eventModule.RegisterEvent(eventName, action);
    }

    // <summary>
    // 添加8参事件监听
    // </summary>
    public static void RegisterEvent<T0, T1, T2, T3, T4, T5, T6, T7>(string eventName,
        Action<T0, T1, T2, T3, T4, T5, T6, T7> action)
    {
        eventModule.RegisterEvent(eventName, action);
    }

    #endregion

    #region 触发监听

    /// <summary>
    /// 触发无参的事件
    /// </summary>
    public static void DispatchEvent(string eventName)
    {
        eventModule.DispatchEvent(eventName);
    }

    /// <summary>
    /// 触发1参的事件
    /// </summary>
    public static void DispatchEvent<T>(string eventName, T arg0)
    {
        eventModule.DispatchEvent(eventName, arg0);
    }

    /// <summary>
    /// 触发2参的事件
    /// </summary>
    public static void DispatchEvent<T0, T1>(string eventName, T0 arg0, T1 arg1)
    {
        eventModule.DispatchEvent(eventName, arg0, arg1);
    }

    /// <summary>
    /// 触发3参的事件
    /// </summary>
    public static void DispatchEvent<T0, T1, T2>(string eventName, T0 arg0, T1 arg1, T2 arg2)
    {
        eventModule.DispatchEvent(eventName, arg0, arg1, arg2);
    }

    /// <summary>
    /// 触发4参的事件
    /// </summary>
    public static void DispatchEvent<T0, T1, T2, T3>(string eventName, T0 arg0, T1 arg1, T2 arg2, T3 arg3)
    {
        eventModule.DispatchEvent(eventName, arg0, arg1, arg2, arg3);
    }

    /// <summary>
    /// 触发5参的事件
    /// </summary>
    public static void DispatchEvent<T0, T1, T2, T3, T4>(string eventName, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
    {
        eventModule.DispatchEvent(eventName, arg0, arg1, arg2, arg3, arg4);
    }

    /// <summary>
    /// 触发6参的事件
    /// </summary>
    public static void DispatchEvent<T0, T1, T2, T3, T4, T5>(string eventName, T0 arg0, T1 arg1, T2 arg2, T3 arg3,
        T4 arg4, T5 arg5)
    {
        eventModule.DispatchEvent(eventName, arg0, arg1, arg2, arg3, arg4, arg5);
    }

    /// <summary>
    /// 触发7参的事件
    /// </summary>
    public static void DispatchEvent<T0, T1, T2, T3, T4, T5, T6>(string eventName, T0 arg0, T1 arg1, T2 arg2, T3 arg3,
        T4 arg4, T5 arg5, T6 arg6)
    {
        eventModule.DispatchEvent(eventName, arg0, arg1, arg2, arg3, arg4, arg5, arg6);
    }

    /// <summary>
    /// 触发8参的事件
    /// </summary>
    public static void DispatchEvent<T0, T1, T2, T3, T4, T5, T6, T7>(string eventName, T0 arg0, T1 arg1, T2 arg2,
        T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7)
    {
        eventModule.DispatchEvent(eventName, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7);
    }

    #endregion

    #region 移除监听

    /// <summary>
    /// 移除无参的事件监听
    /// </summary>
    public static void RemoveEvent(string eventName, Action action)
    {
        eventModule.RemoveEvent(eventName, action);
    }

    /// <summary>
    /// 移除1参的事件监听
    /// </summary>
    public static void RemoveEvent<T>(string eventName, Action<T> action)
    {
        eventModule.RemoveEvent(eventName, action);
    }

    public static void RemoveEvent<T0, T1>(string eventName, Action<T0, T1> action)
    {
        eventModule.RemoveEvent(eventName, action);
    }

    public static void RemoveEvent<T0, T1, T2>(string eventName, Action<T0, T1, T2> action)
    {
        eventModule.RemoveEvent(eventName, action);
    }

    public static void RemoveEvent<T0, T1, T2, T3>(string eventName, Action<T0, T1, T2, T3> action)
    {
        eventModule.RemoveEvent(eventName, action);
    }

    public static void RemoveEvent<T0, T1, T2, T3, T4>(string eventName, Action<T0, T1, T2, T3, T4> action)
    {
        eventModule.RemoveEvent(eventName, action);
    }

    public static void RemoveEvent<T0, T1, T2, T3, T4, T5>(string eventName, Action<T0, T1, T2, T3, T4, T5> action)
    {
        eventModule.RemoveEvent(eventName, action);
    }

    public static void RemoveEvent<T0, T1, T2, T3, T4, T5, T6>(string eventName,
        Action<T0, T1, T2, T3, T4, T5, T6> action)
    {
        eventModule.RemoveEvent(eventName, action);
    }

    public static void RemoveEvent<T0, T1, T2, T3, T4, T5, T6, T7>(string eventName,
        Action<T0, T1, T2, T3, T4, T5, T6, T7> action)
    {
        eventModule.RemoveEvent(eventName, action);
    }

    public static void RemoveEvent(string eventName)
    {
        eventModule.RemoveEvent(eventName);
    }

    public static void Clear()
    {
        eventModule.Clear();
    }

    #endregion

    #region 类型事件

    public static void RegisterEvent<T>(Action<T> action) where T : BaseEvent
    {
        RegisterEvent<T>(typeof(T).Name, action);
    }

    public static void RemoveEvent<T>(Action<T> action) where T : BaseEvent
    {
        RemoveEvent(typeof(T).Name, action);
    }

    public static void RemoveEvent<T>() where T : BaseEvent
    {
        RemoveEvent(typeof(T).Name);
    }

    public static void DispatchEvent<T>(T arg) where T : BaseEvent
    {
        DispatchEvent(typeof(T).Name, arg);
    }

    #endregion
}

/// <summary>
/// 事件基类,所有类型事件都要继承这个类
/// </summary>
public class BaseEvent
{
}
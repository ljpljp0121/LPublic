using System;
using System.Collections.Generic;

public class EventModule
{
    private static ObjectPoolModule objectPoolModule = new ObjectPoolModule();
    private Dictionary<string, IEventInfo> eventInfoDic = new Dictionary<string, IEventInfo>();

    #region 内部接口和类

    private interface IEventInfo
    {
        void Destory();
    }

    /// <summary>
    /// 无参-事件信息
    /// </summary>
    private class EventInfo : IEventInfo
    {
        public Action action;

        public void Init(Action action)
        {
            this.action = action;
        }

        public void Destory()
        {
            action = null;
            objectPoolModule.PushObject(this);
        }
    }

    /// <summary>
    /// 多参Action事件信息
    /// </summary>
    private class MultipleParameterEventInfo<TAction> : IEventInfo where TAction : MulticastDelegate
    {
        public TAction action;

        public void Init(TAction action)
        {
            this.action = action;
        }

        public void Destory()
        {
            action = null;
            objectPoolModule.PushObject(this);
        }
    }

    #endregion

    #region 添加监听

    /// <summary>
    /// 添加无参事件
    /// </summary>
    public void RegisterEvent(string eventName, Action action)
    {
        // 有没有对应的事件可以监听
        if (eventInfoDic.ContainsKey(eventName))
        {
            (eventInfoDic[eventName] as EventInfo).action += action;
        }
        // 没有的话，需要新增 到字典中，并添加对应的Action
        else
        {
            EventInfo _eventInfo = objectPoolModule.GetObject<EventInfo>();
            if (_eventInfo == null) _eventInfo = new EventInfo();
            _eventInfo.Init(action);
            eventInfoDic.Add(eventName, _eventInfo);
        }
    }

    // <summary>
    // 添加多参事件监听
    // </summary>
    public void RegisterEvent<TAction>(string eventName, TAction action) where TAction : MulticastDelegate
    {
        // 有没有对应的事件可以监听
        if (eventInfoDic.TryGetValue(eventName, out IEventInfo eventInfo))
        {
            MultipleParameterEventInfo<TAction> _info = (MultipleParameterEventInfo<TAction>)eventInfo;
            _info.action = (TAction)Delegate.Combine(_info.action, action);
        }
        else AddMultipleParameterEventInfo(eventName, action);
    }

    private void AddMultipleParameterEventInfo<TAction>(string eventName, TAction action)
        where TAction : MulticastDelegate
    {
        MultipleParameterEventInfo<TAction> _newEventInfo =
            objectPoolModule.GetObject<MultipleParameterEventInfo<TAction>>();
        if (_newEventInfo == null) _newEventInfo = new MultipleParameterEventInfo<TAction>();
        _newEventInfo.Init(action);
        eventInfoDic.Add(eventName, _newEventInfo);
    }

    #endregion

    #region 触发事件

    /// <summary>
    /// 触发无参的事件
    /// </summary>
    public void DispatchEvent(string eventName)
    {
        if (eventInfoDic.ContainsKey(eventName))
        {
            ((EventInfo)eventInfoDic[eventName]).action?.Invoke();
        }
    }

    /// <summary>
    /// 触发1个参数的事件
    /// </summary>
    public void DispatchEvent<T>(string eventName, T arg)
    {
        if (eventInfoDic.TryGetValue(eventName, out IEventInfo eventInfo))
            ((MultipleParameterEventInfo<Action<T>>)eventInfo).action?.Invoke(arg);
    }

    /// <summary>
    /// 触发2个参数的事件
    /// </summary>
    public void DispatchEvent<T0, T1>(string eventName, T0 arg0, T1 arg1)
    {
        if (eventInfoDic.TryGetValue(eventName, out IEventInfo eventInfo))
            ((MultipleParameterEventInfo<Action<T0, T1>>)eventInfo).action?.Invoke(arg0, arg1);
    }

    /// <summary>
    /// 触发3个参数的事件
    /// </summary>
    public void DispatchEvent<T0, T1, T2>(string eventName, T0 arg0, T1 arg1, T2 arg2)
    {
        if (eventInfoDic.TryGetValue(eventName, out IEventInfo eventInfo))
            ((MultipleParameterEventInfo<Action<T0, T1, T2>>)eventInfo).action?.Invoke(arg0, arg1, arg2);
    }

    /// <summary>
    /// 触发4个参数的事件
    /// </summary>
    public void DispatchEvent<T0, T1, T2, T3>(string eventName, T0 arg0, T1 arg1, T2 arg2, T3 arg3)
    {
        if (eventInfoDic.TryGetValue(eventName, out IEventInfo eventInfo))
            ((MultipleParameterEventInfo<Action<T0, T1, T2, T3>>)eventInfo).action?.Invoke(arg0, arg1, arg2, arg3);
    }

    /// <summary>
    /// 触发5个参数的事件
    /// </summary>
    public void DispatchEvent<T0, T1, T2, T3, T4>(string eventName, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
    {
        if (eventInfoDic.TryGetValue(eventName, out IEventInfo eventInfo))
            ((MultipleParameterEventInfo<Action<T0, T1, T2, T3, T4>>)eventInfo).action?.Invoke(arg0, arg1, arg2, arg3,
                arg4);
    }

    /// <summary>
    /// 触发6个参数的事件
    /// </summary>
    public void DispatchEvent<T0, T1, T2, T3, T4, T5>(string eventName, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4,
        T5 arg5)
    {
        if (eventInfoDic.TryGetValue(eventName, out IEventInfo eventInfo))
            ((MultipleParameterEventInfo<Action<T0, T1, T2, T3, T4, T5>>)eventInfo).action?.Invoke(arg0, arg1, arg2,
                arg3, arg4, arg5);
    }

    /// <summary>
    /// 触发7个参数的事件
    /// </summary>
    public void DispatchEvent<T0, T1, T2, T3, T4, T5, T6>(string eventName, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4,
        T5 arg5, T6 arg6)
    {
        if (eventInfoDic.TryGetValue(eventName, out IEventInfo eventInfo))
            ((MultipleParameterEventInfo<Action<T0, T1, T2, T3, T4, T5, T6>>)eventInfo).action?.Invoke(arg0, arg1, arg2,
                arg3, arg4, arg5, arg6);
    }

    /// <summary>
    /// 触发8个参数的事件
    /// </summary>
    public void DispatchEvent<T0, T1, T2, T3, T4, T5, T6, T7>(string eventName, T0 arg0, T1 arg1, T2 arg2, T3 arg3,
        T4 arg4, T5 arg5, T6 arg6, T7 arg7)
    {
        if (eventInfoDic.TryGetValue(eventName, out IEventInfo eventInfo))
            ((MultipleParameterEventInfo<Action<T0, T1, T2, T3, T4, T5, T6, T7>>)eventInfo).action?.Invoke(arg0, arg1,
                arg2, arg3, arg4, arg5, arg6, arg7);
    }

    #endregion

    #region 移除监听

    /// <summary>
    /// 移除无参的事件监听
    /// </summary>
    public void RemoveEvent(string eventName, Action action)
    {
        if (eventInfoDic.TryGetValue(eventName, out IEventInfo eventInfo))
        {
            ((EventInfo)eventInfo).action -= action;
        }
    }

    /// <summary>
    /// 移除有参数的事件监听
    /// </summary>
    public void RemoveEvent<TAction>(string eventName, TAction action) where TAction : MulticastDelegate
    {
        if (eventInfoDic.TryGetValue(eventName, out IEventInfo eventInfo))
        {
            MultipleParameterEventInfo<TAction> info = (MultipleParameterEventInfo<TAction>)eventInfo;
            info.action = (TAction)Delegate.Remove(info.action, action);
        }
    }

    /// <summary>
    /// 移除一个事件的所有监听
    /// </summary>
    public void RemoveEvent(string eventName)
    {
        if (eventInfoDic.Remove(eventName, out IEventInfo eventInfo))
        {
            eventInfo.Destory();
        }
    }

    /// <summary>
    /// 清空事件中心
    /// </summary>
    public void Clear()
    {
        foreach (string eventName in eventInfoDic.Keys)
        {
            eventInfoDic[eventName].Destory();
        }
        eventInfoDic.Clear();
    }

    #endregion
}
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public enum EventType
{
    OnMouseEnter = 1000,
    OnMouseExit = 1001,
    OnClick = 1002,
    OnClickDown = 1003,
    OnClickUp = 1004,
    OnDrag = 1005,
    OnBeginDrag = 1006,
    OnEndDrag = 1007,
    OnCollisionEnter = 1008,
    OnCollisionStay = 1009,
    OnCollisionExit = 1010,
    OnCollisionEnter2D = 1011,
    OnCollisionStay2D = 1012,
    OnCollisionExit2D = 1013,
    OnTriggerEnter = 1014,
    OnTriggerStay = 1015,
    OnTriggerExit = 1016,
    OnTriggerEnter2D = 1017,
    OnTriggerStay2D = 1018,
    OnTriggerExit2D = 1019,
    OnReleaseAddressableAsset = 1020,
    OnDestroy = 1021,
}

public interface IMouseEvent : IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, IPointerDownHandler,
    IPointerUpHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
{
}

/// <summary>
/// 事件工具
/// </summary>
public class EventListener : MonoBehaviour, IMouseEvent
{
    private static ObjectPoolModule poolModule = new ObjectPoolModule();

    private EventListenerData data;
    private EventListenerData Data
    {
        get
        {
            if (data == null)
            {
                data = poolModule.GetObject<EventListenerData>();
                if (data == null) data = new EventListenerData();
            }
            return data;
        }
    }

    #region 内部类及接口

    /// <summary>
    /// 持有关键字典数据，主要用于将这个引用放入对象池中
    /// </summary>
    private class EventListenerData
    {
        public Dictionary<int, IEventListenerEventInfos> EventInfoDic = new Dictionary<int, IEventListenerEventInfos>();
    }

    private interface IEventListenerEventInfo<T>
    {
        void TriggerEvent(T eventData);
        void Destroy();
    }

    /// <summary>
    /// 某个事件中一个事件的数据包装类
    /// </summary>
    private class EventListenerEventInfo<T, TEventArg> : IEventListenerEventInfo<T>
    {
        // T：事件本身的参数（PointerEventData、Collision）
        // TEventArg:事件的参数
        public Action<T, TEventArg> Action;
        public TEventArg Arg;

        public void Init(Action<T, TEventArg> action, TEventArg args = default(TEventArg))
        {
            this.Action = action;
            this.Arg = args;
        }

        public void Destroy()
        {
            this.Action = null;
            this.Arg = default(TEventArg);
            poolModule.PushObject(this);
        }

        public void TriggerEvent(T eventData)
        {
            Action?.Invoke(eventData, Arg);
        }
    }

    /// <summary>
    /// 某个事件中一个事件的数据包装类（无参）
    /// </summary>
    private class EventListenerEventInfo<T> : IEventListenerEventInfo<T>
    {
        // T：事件本身的参数（PointerEventData、Collision）
        public Action<T> Action;

        public void Init(Action<T> action)
        {
            this.Action = action;
        }

        public void Destroy()
        {
            this.Action = null;
            poolModule.PushObject(this);
        }

        public void TriggerEvent(T eventData)
        {
            Action?.Invoke(eventData);
        }
    }

    private interface IEventListenerEventInfos
    {
        void RemoveAll();
    }

    /// <summary>
    /// 一类事件的数据包装类型：包含多个JKEventListenerEventInfo
    /// </summary>
    private class EventListenerEventInfos<T> : IEventListenerEventInfos
    {
        // 所有的事件
        private List<IEventListenerEventInfo<T>> eventList = new List<IEventListenerEventInfo<T>>();

        /// <summary>
        /// 添加事件 无参
        /// </summary>
        public void AddListener(Action<T> action)
        {
            EventListenerEventInfo<T> info = poolModule.GetObject<EventListenerEventInfo<T>>();
            if (info == null) info = new EventListenerEventInfo<T>();
            info.Init(action);
            eventList.Add(info);
        }

        /// <summary>
        /// 添加事件 有参
        /// </summary>
        public void AddListener<TEventArg>(Action<T, TEventArg> action, TEventArg args = default(TEventArg))
        {
            EventListenerEventInfo<T, TEventArg> info = poolModule.GetObject<EventListenerEventInfo<T, TEventArg>>();
            if (info == null) info = new EventListenerEventInfo<T, TEventArg>();
            info.Init(action, args);
            eventList.Add(info);
        }

        public void TriggerEvent(T eventData)
        {
            for (int i = 0; i < eventList.Count; i++)
            {
                eventList[i].TriggerEvent(eventData);
            }
        }

        /// <summary>
        /// 移除事件（无参）
        /// 同一个函数+参数注册过多次，无论如何该方法只会移除一个事件
        /// </summary>
        public void RemoveListener(Action<T> action)
        {
            for (int i = 0; i < eventList.Count; i++)
            {
                EventListenerEventInfo<T> eventInfo = eventList[i] as EventListenerEventInfo<T>;
                if (eventInfo == null) continue; // 类型不符

                // 找到这个事件，查看是否相等
                if (eventInfo.Action.Equals(action))
                {
                    // 移除
                    eventInfo.Destroy();
                    eventList.RemoveAt(i);
                    return;
                }
            }
        }

        /// <summary>
        /// 移除事件（有参）
        /// 同一个函数+参数注册过多次，无论如何该方法只会移除一个事件
        /// </summary>
        public void RemoveListener<TEventArg>(Action<T, TEventArg> action)
        {
            for (int i = 0; i < eventList.Count; i++)
            {
                EventListenerEventInfo<T, TEventArg> eventInfo =
                    eventList[i] as EventListenerEventInfo<T, TEventArg>;
                if (eventInfo == null) continue; // 类型不符

                // 找到这个事件，查看是否相等
                if (eventInfo.Action.Equals(action))
                {
                    // 移除
                    eventInfo.Destroy();
                    eventList.RemoveAt(i);
                    return;
                }
            }
        }

        /// <summary>
        /// 移除全部，全部放进对象池
        /// </summary>
        public void RemoveAll()
        {
            for (int i = 0; i < eventList.Count; i++)
            {
                eventList[i].Destroy();
            }
            eventList.Clear();
            poolModule.PushObject(this);
        }
    }

    #endregion

    #region 外部接口

    /// <summary>
    /// 添加无参事件 
    /// </summary>
    public void AddListener<T>(int eventTypeInt, Action<T> action)
    {
        if (Data.EventInfoDic.TryGetValue(eventTypeInt, out IEventListenerEventInfos info))
        {
            ((EventListenerEventInfos<T>)info).AddListener(action);
        }
        else
        {
            EventListenerEventInfos<T> infos = poolModule.GetObject<EventListenerEventInfos<T>>();
            if (infos == null) infos = new EventListenerEventInfos<T>();
            infos.AddListener(action);
            Data.EventInfoDic.Add(eventTypeInt, infos);
        }
    }

    /// <summary>
    /// 添加事件（有参）
    /// </summary>
    public void AddListener<T, TEventArg>(int eventTypeInt, Action<T, TEventArg> action, TEventArg args)
    {
        if (Data.EventInfoDic.TryGetValue(eventTypeInt, out IEventListenerEventInfos info))
        {
            ((EventListenerEventInfos<T>)info).AddListener(action, args);
        }
        else
        {
            EventListenerEventInfos<T> infos = poolModule.GetObject<EventListenerEventInfos<T>>();
            if (infos == null) infos = new EventListenerEventInfos<T>();
            infos.AddListener(action, args);
            Data.EventInfoDic.Add(eventTypeInt, infos);
        }
    }

    /// <summary>
    /// 添加事件（无参）
    /// </summary>
    public void AddListener<T>(EventType eventType, Action<T> action)
    {
        AddListener((int)eventType, action);
    }

    /// <summary>
    /// 添加事件（有参）
    /// </summary>
    public void AddListener<T, TEventArg>(EventType eventType, Action<T, TEventArg> action, TEventArg args)
    {
        AddListener((int)eventType, action, args);
    }

    /// <summary>
    /// 移除事件（无参）
    /// </summary>
    public void RemoveListener<T>(int eventTypeInt, Action<T> action)
    {
        if (Data.EventInfoDic.TryGetValue(eventTypeInt, out IEventListenerEventInfos info))
        {
            ((EventListenerEventInfos<T>)info).RemoveListener(action);
        }
    }

    /// <summary>
    /// 移除事件（无参）
    /// </summary>
    public void RemoveListener<T>(EventType eventType, Action<T> action)
    {
        RemoveListener((int)eventType, action);
    }

    /// <summary>
    /// 移除事件（有参）
    /// </summary>
    public void RemoveListener<T, TEventArg>(int eventTypeInt, Action<T, TEventArg> action)
    {
        if (Data.EventInfoDic.TryGetValue(eventTypeInt, out IEventListenerEventInfos info))
        {
            ((EventListenerEventInfos<T>)info).RemoveListener(action);
        }
    }

    /// <summary>
    /// 移除事件（有参）
    /// </summary>
    public void RemoveListener<T, TEventArg>(EventType eventType, Action<T, TEventArg> action)
    {
        RemoveListener((int)eventType, action);
    }

    /// <summary>
    /// 移除某一个事件类型下的全部事件
    /// </summary>
    /// <param name="eventType"></param>
    public void RemoveAllListener(int eventType)
    {
        if (Data.EventInfoDic.TryGetValue(eventType, out IEventListenerEventInfos infos))
        {
            infos.RemoveAll();
            Data.EventInfoDic.Remove(eventType);
        }
    }

    /// <summary>
    /// 移除某一个事件类型下的全部事件
    /// </summary>
    public void RemoveAllListener(EventType eventType)
    {
        RemoveAllListener((int)eventType);
    }

    /// <summary>
    /// 移除全部事件
    /// </summary>
    public void RemoveAllListener()
    {
        foreach (IEventListenerEventInfos infos in Data.EventInfoDic.Values)
        {
            infos.RemoveAll();
        }

        data.EventInfoDic.Clear();
        // 将整个数据容器放入对象池
        poolModule.PushObject(data);
        data = null;
    }

    /// <summary>
    /// 触发事件
    /// </summary>
    public void TriggerAction<T>(int eventTypeInt, T eventData)
    {
        if (Data.EventInfoDic.TryGetValue(eventTypeInt, out IEventListenerEventInfos infos))
        {
            (infos as EventListenerEventInfos<T>).TriggerEvent(eventData);
        }
    }

    /// <summary>
    /// 触发事件
    /// </summary>
    public void TriggerAction<T>(EventType eventType, T eventData)
    {
        TriggerAction<T>((int)eventType, eventData);
    }

    #endregion

    #region 鼠标事件

    public void OnPointerEnter(PointerEventData eventData)
    {
        TriggerAction(EventType.OnMouseEnter, eventData);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        TriggerAction(EventType.OnMouseExit, eventData);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        TriggerAction(EventType.OnClick, eventData);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        TriggerAction(EventType.OnClickDown, eventData);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        TriggerAction(EventType.OnClickUp, eventData);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        TriggerAction(EventType.OnBeginDrag, eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        TriggerAction(EventType.OnEndDrag, eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        TriggerAction(EventType.OnDrag, eventData);
    }

    #endregion

    #region 碰撞事件

    private void OnCollisionEnter(Collision collision)
    {
        TriggerAction(EventType.OnCollisionEnter, collision);
    }

    private void OnCollisionStay(Collision collision)
    {
        TriggerAction(EventType.OnCollisionStay, collision);
    }

    private void OnCollisionExit(Collision collision)
    {
        TriggerAction(EventType.OnCollisionExit, collision);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        TriggerAction(EventType.OnCollisionEnter2D, collision);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        TriggerAction(EventType.OnCollisionStay2D, collision);
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        TriggerAction(EventType.OnCollisionExit2D, collision);
    }

    #endregion

    #region 触发事件

    private void OnTriggerEnter(Collider other)
    {
        TriggerAction(EventType.OnTriggerEnter, other);
    }

    private void OnTriggerStay(Collider other)
    {
        TriggerAction(EventType.OnTriggerStay, other);
    }

    private void OnTriggerExit(Collider other)
    {
        TriggerAction(EventType.OnTriggerExit, other);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        TriggerAction(EventType.OnTriggerEnter2D, collision);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        TriggerAction(EventType.OnTriggerStay2D, collision);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        TriggerAction(EventType.OnTriggerExit2D, collision);
    }

    #endregion

    #region 销毁事件

    private void OnDestroy()
    {
        TriggerAction(EventType.OnReleaseAddressableAsset, gameObject);
        TriggerAction(EventType.OnDestroy, gameObject);

        // 销毁所有数据，并将一些数据放回对象池中
        RemoveAllListener();
    }

    #endregion
}
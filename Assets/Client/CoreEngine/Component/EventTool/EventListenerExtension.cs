using System;
using UnityEngine;
using UnityEngine.EventSystems;

public static class EventListenerExtension
{
    #region 工具函数

    private static EventListener GetOrAddEventListener(Component component)
    {
        EventListener listener = component.GetComponent<EventListener>();
        return listener == null ? component.gameObject.AddComponent<EventListener>() : listener;
    }

    public static void AddEventListener<T>(this Component component, EventType eventType, Action<T> action)
    {
        AddEventListener(component, (int)eventType, action);
    }

    public static void AddEventListener<T>(this Component component, int customEventTypeInt, Action<T> action)
    {
        EventListener listener = GetOrAddEventListener(component);
        listener.AddListener(customEventTypeInt, action);
    }

    public static void AddEventListener<T, TEventArg>(this Component component, EventType eventType,
        Action<T, TEventArg> action, TEventArg args = default(TEventArg))
    {
        AddEventListener(component, (int)eventType, action, args);
    }

    public static void AddEventListener<T, TEventArg>(this Component component, int customEventTypeInt,
        Action<T, TEventArg> action, TEventArg args = default(TEventArg))
    {
        EventListener listener = GetOrAddEventListener(component);
        listener.AddListener(customEventTypeInt, action, args);
    }

    public static void RemoveEventListener<T, TEventArg>(this Component component, int customEventTypeInt,
        Action<T, TEventArg> action)
    {
        EventListener listener = component.GetComponent<EventListener>();
        if (listener != null) listener.RemoveListener(customEventTypeInt, action);
    }

    public static void RemoveEventListener<T, TEventArg>(this Component component, EventType eventType,
        Action<T, TEventArg> action)
    {
        RemoveEventListener(component, (int)eventType, action);
    }

    public static void RemoveEventListener<T>(this Component component, int customEventTypeInt, Action<T> action)
    {
        EventListener listener = component.GetComponent<EventListener>();
        if (listener != null) listener.RemoveListener(customEventTypeInt, action);
    }

    public static void RemoveEventListener<T>(this Component component, EventType eventType, Action<T> action)
    {
        RemoveEventListener(component, (int)eventType, action);
    }

    public static void RemoveAllListener(this Component component, int customEventTypeInt)
    {
        EventListener listener = component.GetComponent<EventListener>();
        if (listener != null) listener.RemoveAllListener(customEventTypeInt);
    }

    public static void RemoveAllListener(this Component component, EventType eventType)
    {
        RemoveAllListener(component, (int)eventType);
    }

    public static void RemoveAllListener(this Component component)
    {
        EventListener listener = component.GetComponent<EventListener>();
        if (listener != null) listener.RemoveAllListener();
    }

    public static void TriggerCustomEvent<T>(this Component component, int customEventTypeInt, T eventData)
    {
        EventListener listener = GetOrAddEventListener(component);
        listener.TriggerAction<T>(customEventTypeInt, eventData);
    }

    #endregion

    #region 鼠标相关事件

    public static void OnMouseEnter<TEventArg>(this Component component, Action<PointerEventData, TEventArg> action,
        TEventArg args = default(TEventArg))
    {
        AddEventListener(component, EventType.OnMouseEnter, action, args);
    }

    public static void OnMouseEnter(this Component component, Action<PointerEventData> action)
    {
        AddEventListener(component, EventType.OnMouseEnter, action);
    }

    public static void OnMouseExit<TEventArg>(this Component component, Action<PointerEventData, TEventArg> action,
        TEventArg args = default(TEventArg))
    {
        AddEventListener(component, EventType.OnMouseExit, action, args);
    }

    public static void OnMouseExit(this Component component, Action<PointerEventData> action)
    {
        AddEventListener(component, EventType.OnMouseExit, action);
    }

    public static void OnClick<TEventArg>(this Component component, Action<PointerEventData, TEventArg> action,
        TEventArg args = default(TEventArg))
    {
        AddEventListener(component, EventType.OnClick, action, args);
    }

    public static void OnClick(this Component component, Action<PointerEventData> action)
    {
        AddEventListener(component, EventType.OnClick, action);
    }


    public static void OnClickDown<TEventArg>(this Component component, Action<PointerEventData, TEventArg> action,
        TEventArg args = default(TEventArg))
    {
        AddEventListener(component, EventType.OnClickDown, action, args);
    }

    public static void OnClickDown(this Component component, Action<PointerEventData> action)
    {
        AddEventListener(component, EventType.OnClickDown, action);
    }

    public static void OnClickUp<TEventArg>(this Component component, Action<PointerEventData, TEventArg> action,
        TEventArg args = default(TEventArg))
    {
        AddEventListener(component, EventType.OnClickUp, action, args);
    }

    public static void OnClickUp(this Component component, Action<PointerEventData> action)
    {
        AddEventListener(component, EventType.OnClickUp, action);
    }


    public static void OnDrag<TEventArg>(this Component component, Action<PointerEventData, TEventArg> action,
        TEventArg args = default(TEventArg))
    {
        AddEventListener(component, EventType.OnDrag, action, args);
    }

    public static void OnDrag(this Component component, Action<PointerEventData> action)
    {
        AddEventListener(component, EventType.OnDrag, action);
    }

    public static void OnBeginDrag<TEventArg>(this Component component, Action<PointerEventData, TEventArg> action,
        TEventArg args = default(TEventArg))
    {
        AddEventListener(component, EventType.OnBeginDrag, action, args);
    }

    public static void OnBeginDrag(this Component component, Action<PointerEventData> action)
    {
        AddEventListener(component, EventType.OnBeginDrag, action);
    }

    public static void OnEndDrag<TEventArg>(this Component component, Action<PointerEventData, TEventArg> action,
        TEventArg args = default(TEventArg))
    {
        AddEventListener(component, EventType.OnEndDrag, action, args);
    }

    public static void OnEndDrag(this Component component, Action<PointerEventData> action)
    {
        AddEventListener(component, EventType.OnEndDrag, action);
    }

    public static void RemoveOnClick<TEventArg>(this Component component, Action<PointerEventData, TEventArg> action)
    {
        RemoveEventListener(component, EventType.OnClick, action);
    }

    public static void RemoveOnClick(this Component component, Action<PointerEventData> action)
    {
        RemoveEventListener(component, EventType.OnClick, action);
    }

    public static void RemoveOnClickDown<TEventArg>(this Component component,
        Action<PointerEventData, TEventArg> action)
    {
        RemoveEventListener(component, EventType.OnClickDown, action);
    }

    public static void RemoveOnClickDown(this Component component, Action<PointerEventData> action)
    {
        RemoveEventListener(component, EventType.OnClickDown, action);
    }

    public static void RemoveOnMouseEnter<TEventArg>(this Component component,
        Action<PointerEventData, TEventArg> action)
    {
        RemoveEventListener(component, EventType.OnMouseEnter, action);
    }

    public static void RemoveOnMouseEnter(this Component component, Action<PointerEventData> action)
    {
        RemoveEventListener(component, EventType.OnMouseEnter, action);
    }

    public static void RemoveOnMouseExit<TEventArg>(this Component component,
        Action<PointerEventData, TEventArg> action)
    {
        RemoveEventListener(component, EventType.OnMouseExit, action);
    }

    public static void RemoveOnMouseExit(this Component component, Action<PointerEventData> action)
    {
        RemoveEventListener(component, EventType.OnMouseExit, action);
    }

    public static void RemoveOnClickUp<TEventArg>(this Component component, Action<PointerEventData, TEventArg> action)
    {
        RemoveEventListener(component, EventType.OnClickUp, action);
    }

    public static void RemoveOnClickUp(this Component component, Action<PointerEventData> action)
    {
        RemoveEventListener(component, EventType.OnClickUp, action);
    }

    public static void RemoveOnDrag<TEventArg>(this Component component, Action<PointerEventData, TEventArg> action)
    {
        RemoveEventListener(component, EventType.OnDrag, action);
    }

    public static void RemoveOnDrag(this Component component, Action<PointerEventData> action)
    {
        RemoveEventListener(component, EventType.OnDrag, action);
    }

    public static void RemoveOnBeginDrag<TEventArg>(this Component component,
        Action<PointerEventData, TEventArg> action)
    {
        RemoveEventListener(component, EventType.OnBeginDrag, action);
    }

    public static void RemoveOnBeginDrag(this Component component, Action<PointerEventData> action)
    {
        RemoveEventListener(component, EventType.OnBeginDrag, action);
    }

    public static void RemoveOnEndDrag<TEventArg>(this Component component, Action<PointerEventData, TEventArg> action)
    {
        RemoveEventListener(component, EventType.OnEndDrag, action);
    }

    public static void RemoveOnEndDrag(this Component component, Action<PointerEventData> action)
    {
        RemoveEventListener(component, EventType.OnEndDrag, action);
    }

    #endregion

    #region 碰撞相关事件

    public static void OnCollisionEnter<TEventArg>(this Component com, Action<Collision, TEventArg> action,
        TEventArg args = default(TEventArg))
    {
        AddEventListener(com, EventType.OnCollisionEnter, action, args);
    }

    public static void OnCollisionEnter(this Component com, Action<Collision> action)
    {
        AddEventListener(com, EventType.OnCollisionEnter, action);
    }


    public static void OnCollisionStay<TEventArg>(this Component com, Action<Collision, TEventArg> action,
        TEventArg args = default(TEventArg))
    {
        AddEventListener(com, EventType.OnCollisionStay, action, args);
    }

    public static void OnCollisionStay(this Component com, Action<Collision> action)
    {
        AddEventListener(com, EventType.OnCollisionStay, action);
    }

    public static void OnCollisionExit<TEventArg>(this Component com, Action<Collision, TEventArg> action,
        TEventArg args = default(TEventArg))
    {
        AddEventListener(com, EventType.OnCollisionExit, action, args);
    }

    public static void OnCollisionExit(this Component com, Action<Collision> action)
    {
        AddEventListener(com, EventType.OnCollisionExit, action);
    }

    public static void OnCollisionEnter2D<TEventArg>(this Component com, Action<Collision2D, TEventArg> action,
        TEventArg args = default(TEventArg))
    {
        AddEventListener(com, EventType.OnCollisionEnter2D, action, args);
    }

    public static void OnCollisionEnter2D(this Component com, Action<Collision2D> action)
    {
        AddEventListener(com, EventType.OnCollisionEnter2D, action);
    }

    public static void OnCollisionStay2D<TEventArg>(this Component com, Action<Collision2D, TEventArg> action,
        TEventArg args = default(TEventArg))
    {
        AddEventListener(com, EventType.OnCollisionStay2D, action, args);
    }

    public static void OnCollisionStay2D(this Component com, Action<Collision2D> action)
    {
        AddEventListener(com, EventType.OnCollisionStay2D, action);
    }

    public static void OnCollisionExit2D<TEventArg>(this Component com, Action<Collision2D, TEventArg> action,
        TEventArg args = default(TEventArg))
    {
        AddEventListener(com, EventType.OnCollisionExit2D, action, args);
    }

    public static void OnCollisionExit2D(this Component com, Action<Collision2D> action)
    {
        AddEventListener(com, EventType.OnCollisionExit2D, action);
    }

    public static void RemoveOnCollisionEnter<TEventArg>(this Component com, Action<Collision, TEventArg> action)
    {
        RemoveEventListener(com, EventType.OnCollisionEnter, action);
    }

    public static void RemoveOnCollisionEnter(this Component com, Action<Collision> action)
    {
        RemoveEventListener(com, EventType.OnCollisionEnter, action);
    }

    public static void RemoveOnCollisionStay<TEventArg>(this Component com, Action<Collision, TEventArg> action)
    {
        RemoveEventListener(com, EventType.OnCollisionStay, action);
    }

    public static void RemoveOnCollisionStay(this Component com, Action<Collision> action)
    {
        RemoveEventListener(com, EventType.OnCollisionStay, action);
    }

    public static void RemoveOnCollisionExit<TEventArg>(this Component com, Action<Collision, TEventArg> action)
    {
        RemoveEventListener(com, EventType.OnCollisionExit, action);
    }

    public static void RemoveOnCollisionExit(this Component com, Action<Collision> action)
    {
        RemoveEventListener(com, EventType.OnCollisionExit, action);
    }

    public static void RemoveOnCollisionEnter2D<TEventArg>(this Component com, Action<Collision2D, TEventArg> action)
    {
        RemoveEventListener(com, EventType.OnCollisionEnter2D, action);
    }

    public static void RemoveOnCollisionEnter2D(this Component com, Action<Collision2D> action)
    {
        RemoveEventListener(com, EventType.OnCollisionEnter2D, action);
    }

    public static void RemoveOnCollisionStay2D<TEventArg>(this Component com, Action<Collision2D, TEventArg> action)
    {
        RemoveEventListener(com, EventType.OnCollisionStay2D, action);
    }

    public static void RemoveOnCollisionStay2D(this Component com, Action<Collision2D> action)
    {
        RemoveEventListener(com, EventType.OnCollisionStay2D, action);
    }

    public static void RemoveOnCollisionExit2D<TEventArg>(this Component com, Action<Collision2D, TEventArg> action)
    {
        RemoveEventListener(com, EventType.OnCollisionExit2D, action);
    }

    public static void RemoveOnCollisionExit2D(this Component com, Action<Collision2D> action)
    {
        RemoveEventListener(com, EventType.OnCollisionExit2D, action);
    }

    #endregion

    #region 触发相关事件

    public static void OnTriggerEnter<TEventArg>(this Component com, Action<Collider, TEventArg> action,
        TEventArg args = default(TEventArg))
    {
        AddEventListener(com, EventType.OnTriggerEnter, action, args);
    }

    public static void OnTriggerEnter(this Component com, Action<Collider> action)
    {
        AddEventListener(com, EventType.OnTriggerEnter, action);
    }

    public static void OnTriggerStay<TEventArg>(this Component com, Action<Collider, TEventArg> action,
        TEventArg args = default(TEventArg))
    {
        AddEventListener(com, EventType.OnTriggerStay, action, args);
    }

    public static void OnTriggerStay(this Component com, Action<Collider> action)
    {
        AddEventListener(com, EventType.OnTriggerStay, action);
    }

    public static void OnTriggerExit<TEventArg>(this Component com, Action<Collider, TEventArg> action,
        TEventArg args = default(TEventArg))
    {
        AddEventListener(com, EventType.OnTriggerExit, action, args);
    }

    public static void OnTriggerExit(this Component com, Action<Collider> action)
    {
        AddEventListener(com, EventType.OnTriggerExit, action);
    }

    public static void OnTriggerEnter2D<TEventArg>(this Component com, Action<Collider2D, TEventArg> action,
        TEventArg args = default(TEventArg))
    {
        AddEventListener(com, EventType.OnTriggerEnter2D, action, args);
    }

    public static void OnTriggerEnter2D(this Component com, Action<Collider2D> action)
    {
        AddEventListener(com, EventType.OnTriggerEnter2D, action);
    }

    public static void OnTriggerStay2D<TEventArg>(this Component com, Action<Collider2D, TEventArg> action,
        TEventArg args = default(TEventArg))
    {
        AddEventListener(com, EventType.OnTriggerStay2D, action, args);
    }

    public static void OnTriggerStay2D(this Component com, Action<Collider2D> action)
    {
        AddEventListener(com, EventType.OnTriggerStay2D, action);
    }

    public static void OnTriggerExit2D<TEventArg>(this Component com, Action<Collider2D, TEventArg> action,
        TEventArg args = default(TEventArg))
    {
        AddEventListener(com, EventType.OnTriggerExit2D, action, args);
    }

    public static void OnTriggerExit2D(this Component com, Action<Collider2D> action)
    {
        AddEventListener(com, EventType.OnTriggerExit2D, action);
    }

    public static void RemoveOnTriggerEnter<TEventArg>(this Component com, Action<Collider, TEventArg> action)
    {
        RemoveEventListener(com, EventType.OnTriggerEnter, action);
    }

    public static void RemoveOnTriggerEnter(this Component com, Action<Collider> action)
    {
        RemoveEventListener(com, EventType.OnTriggerEnter, action);
    }

    public static void RemoveOnTriggerStay<TEventArg>(this Component com, Action<Collider, TEventArg> action)
    {
        RemoveEventListener(com, EventType.OnTriggerStay, action);
    }

    public static void RemoveOnTriggerStay(this Component com, Action<Collider> action)
    {
        RemoveEventListener(com, EventType.OnTriggerStay, action);
    }

    public static void RemoveOnTriggerExit<TEventArg>(this Component com, Action<Collider, TEventArg> action)
    {
        RemoveEventListener(com, EventType.OnTriggerExit, action);
    }

    public static void RemoveOnTriggerExit(this Component com, Action<Collider> action)
    {
        RemoveEventListener(com, EventType.OnTriggerExit, action);
    }

    public static void RemoveOnTriggerEnter2D<TEventArg>(this Component com, Action<Collider2D, TEventArg> action)
    {
        RemoveEventListener(com, EventType.OnTriggerEnter2D, action);
    }

    public static void RemoveOnTriggerEnter2D(this Component com, Action<Collider2D> action)
    {
        RemoveEventListener(com, EventType.OnTriggerEnter2D, action);
    }

    public static void RemoveOnTriggerStay2D<TEventArg>(this Component com, Action<Collider2D, TEventArg> action)
    {
        RemoveEventListener(com, EventType.OnTriggerStay2D, action);
    }

    public static void RemoveOnTriggerStay2D(this Component com, Action<Collider2D> action)
    {
        RemoveEventListener(com, EventType.OnTriggerStay2D, action);
    }

    public static void RemoveOnTriggerExit2D<TEventArg>(this Component com, Action<Collider2D, TEventArg> action)
    {
        RemoveEventListener(com, EventType.OnTriggerExit2D, action);
    }

    public static void RemoveOnTriggerExit2D(this Component com, Action<Collider2D> action)
    {
        RemoveEventListener(com, EventType.OnTriggerExit2D, action);
    }

    #endregion

    #region 资源相关事件

    public static void OnReleaseAddressableAsset<TEventArg>(this Component com, Action<GameObject, TEventArg> action,
        TEventArg args = default(TEventArg))
    {
        AddEventListener(com, EventType.OnReleaseAddressableAsset, action, args);
    }

    public static void OnReleaseAddressableAsset(this Component com, Action<GameObject> action)
    {
        AddEventListener(com, EventType.OnReleaseAddressableAsset, action);
    }

    public static void RemoveOnReleaseAddressableAsset<TEventArg>(this Component com,
        Action<GameObject, TEventArg> action)
    {
        RemoveEventListener(com, EventType.OnReleaseAddressableAsset, action);
    }

    public static void RemoveOnReleaseAddressableAsset(this Component com, Action<GameObject> action)
    {
        RemoveEventListener(com, EventType.OnReleaseAddressableAsset, action);
    }

    public static void OnDestroy<TEventArg>(this Component com, Action<GameObject, TEventArg> action,
        TEventArg args = default(TEventArg))
    {
        AddEventListener(com, EventType.OnDestroy, action, args);
    }

    public static void OnDestroy(this Component com, Action<GameObject> action)
    {
        AddEventListener(com, EventType.OnDestroy, action);
    }

    public static void RemoveOnDestroy<TEventArg>(this Component com, Action<GameObject, TEventArg> action)
    {
        RemoveEventListener(com, EventType.OnDestroy, action);
    }

    public static void RemoveOnDestroy(this Component com, Action<GameObject> action)
    {
        RemoveEventListener(com, EventType.OnDestroy, action);
    }

    #endregion
}
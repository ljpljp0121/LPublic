using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;


public class ComponentDestroyProxy : MonoBehaviour
{
    public event Action OnDestroyed;
    private void OnDestroy() => OnDestroyed?.Invoke();
}

/// <summary>
/// 组件系统
/// </summary>
public class ComponentSystem : MonoBehaviour
{
    private readonly ComponentContainer container = new();
    private readonly LinkedList<IComponent> components = new();
    private List<IUpdatable> updatableList = new();
    private List<IFixedUpdatable> fixedUpdatableList = new();
    private List<ILateUpdatable> lateUpdatableList = new();

    private void Awake() => Initialize();

    private void OnDestroy()
    {
        foreach (var c in components.ToArray())
            RemoveComponent(c);
    }

    private void Initialize()
    {
        var foundComponents = GetComponentsInChildren<IComponent>(true)
            .OrderBy(GetComponentPriority)
            .ToList();

        foreach (var c in foundComponents)
        {
            OrderComponent(c);
        }

        //注册组件
        foreach (var c in foundComponents)
        {
            try
            {
                container.Register(c);
                Debug.Log($"组件注册成功!! {c.GetType().Name}");
            }
            catch (Exception e)
            {
                Debug.LogError($"{c.GetType().Name}组件注册失败: {e}");
            }
        }
        //注入依赖
        foreach (var c in foundComponents)
        {
            try
            {
                DependencyInjector.InjectDependencies(c, container);
                Debug.Log($"组件注入依赖成功!! {c.GetType().Name}");
            }
            catch (Exception e)
            {
                Debug.LogError($"{c.GetType().Name}组件注入依赖失败: {e}");
            }
        }
        //初始化组件
        foreach (var c in foundComponents)
        {
            try
            {
                c.Init();
                Debug.Log($"组件初始化成功!! {c.GetType().Name}");
            }
            catch (Exception e)
            {
                LogSystem.Error($"组件初始化失败: {e}");
            }
        }

        updatableList = components.OfType<IUpdatable>().ToList();
        fixedUpdatableList = components.OfType<IFixedUpdatable>().ToList();
        lateUpdatableList = components.OfType<ILateUpdatable>().ToList();
        MonoSystem.AddUpdate(OnUpdate);
        MonoSystem.AddFixedUpdate(OnFixedUpdate);
        MonoSystem.AddLateUpdate(OnLateUpdate);
    }

    private void OrderComponent(IComponent component)
    {
        if (component is MonoBehaviour mb && !mb.TryGetComponent<ComponentDestroyProxy>(out _))
        {
            var proxy = mb.gameObject.AddComponent<ComponentDestroyProxy>();
            proxy.OnDestroyed += () => RemoveComponent(component);
        }

        var node = components.First;
        while (node != null && GetComponentPriority(node.Value) <= GetComponentPriority(component))
            node = node.Next;

        if (node != null)
            components.AddBefore(node, component);
        else
            components.AddLast(component);
    }

    private void OnUpdate()
    {
        foreach (var t in updatableList)
        {
            try
            {
                t.OnUpdate();
            }
            catch (Exception e)
            {
                Debug.LogError($"{t.GetType().Name}更新失败 OnUpdate: {e}");
            }
        }
    }

    private void OnFixedUpdate()
    {
        foreach (var t in fixedUpdatableList)
        {
            try
            {
                t.OnFixedUpdate();
            }
            catch (Exception e)
            {
                Debug.LogError($"{t.GetType().Name}更新失败 OnFixedUpdate: {e}");
            }
        }
    }

    private void OnLateUpdate()
    {
        foreach (var t in lateUpdatableList)
        {
            try
            {
                t.OnLateUpdate();
            }
            catch (Exception e)
            {
                Debug.LogError($"{t.GetType().Name}更新失败 OnLateUpdate: {e}");
            }
        }
    }

    private int GetComponentPriority(IComponent component)
    {
        var attr = component.GetType()
            .GetCustomAttribute<InitializeOrderAttribute>();
        return attr?.Order ?? int.MaxValue;
    }

    #region 添加删除组件

    /// <summary>
    /// 添加组件
    /// </summary>
    public T AddComponent<T>(GameObject target = null) where T : MonoBehaviour, IComponent
    {
        target = target ?? gameObject;
        var comp = target.AddComponent<T>();
        OrderComponent(comp);
        container.Register(comp);
        DependencyInjector.InjectDependencies(comp, container);
        comp.Init();
        return comp;
    }

    /// <summary>
    /// 移除组件
    /// </summary>
    public void RemoveComponent(IComponent component)
    {
        if (component is MonoBehaviour mb)
        {
            components.Remove(component);
            updatableList.Remove(component as IUpdatable);
            fixedUpdatableList.Remove(component as IFixedUpdatable);
            lateUpdatableList.Remove(component as ILateUpdatable);
            container.Unregister(component);
            component.UnInit();
            Destroy(mb);
        }
    }

    #endregion
}
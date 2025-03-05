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
            RegisterComponent(c);

        updatableList = components.OfType<IUpdatable>().ToList();
        fixedUpdatableList = components.OfType<IFixedUpdatable>().ToList();
        lateUpdatableList = components.OfType<ILateUpdatable>().ToList();
        MonoSystem.AddUpdate(OnUpdate);
        MonoSystem.AddFixedUpdate(OnFixedUpdate);
        MonoSystem.AddLateUpdate(OnLateUpdate);
    }

    private void RegisterComponent(IComponent component)
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

        try
        {
            container.Register(component);
            DependencyInjector.InjectDependencies(component, container);
            component.Init();
            Debug.Log($"组件初始化成功!! {component.GetType().Name}");
        }
        catch (Exception e)
        {
            Debug.LogError($"{component.GetType().Name}初始化失败: {e}");
        }

        if (component is IUpdatable updatable)
            updatableList.Add(updatable);
        if (component is IFixedUpdatable fixedUpdatable)
            fixedUpdatableList.Add(fixedUpdatable);
        if (component is ILateUpdatable lateUpdatable)
            lateUpdatableList.Add(lateUpdatable);
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
        RegisterComponent(comp);
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
            Destroy(mb);
        }
    }

    #endregion
}

/// <summary>
///  组件容器
/// </summary>
public class ComponentContainer
{
    private readonly Dictionary<Type, List<object>> componentsDic = new();
    private readonly Dictionary<Type, List<Type>> interfaceMap = new();

    /// <summary>
    /// 注册组件
    /// </summary>
    public void Register(object component)
    {
        var type = component.GetType();

        // 注册具体类型
        RegisterType(type, component);

        // 注册接口
        foreach (var interfaceType in type.GetInterfaces())
            RegisterInterface(interfaceType, type, component);
    }

    /// <summary>
    /// 获取所有组件
    /// </summary>
    public IEnumerable<object> GetAll(Type type)
    {
        // 直接类型匹配
        if (componentsDic.TryGetValue(type, out var list))
            return list;

        // 接口派生类型匹配
        if (type.IsInterface && interfaceMap.TryGetValue(type, out var implTypes))
        {
            return implTypes.SelectMany(t =>
                componentsDic.TryGetValue(t, out var components)
                    ? components
                    : Enumerable.Empty<object>());
        }

        return Enumerable.Empty<object>();
    }

    private void RegisterType(Type type, object component)
    {
        if (!componentsDic.ContainsKey(type))
            componentsDic[type] = new List<object>();
        componentsDic[type].Add(component);
    }

    private void RegisterInterface(Type interfaceType, Type implementType, object component)
    {
        if (!interfaceMap.ContainsKey(interfaceType))
            interfaceMap[interfaceType] = new List<Type>();

        if (!interfaceMap[interfaceType].Contains(implementType))
            interfaceMap[interfaceType].Add(implementType);

        RegisterType(interfaceType, component);
    }

    public void Unregister(object component)
    {
        var type = component.GetType();

        // 清理具体类型
        if (componentsDic.TryGetValue(type, out var set))
        {
            set.Remove(component);
            if (set.Count == 0) componentsDic.Remove(type);
        }

        // 清理接口映射
        foreach (var interfaceType in type.GetInterfaces())
        {
            if (interfaceMap.TryGetValue(interfaceType, out var implTypes))
            {
                foreach (var implType in implTypes.ToArray())
                {
                    if (componentsDic.TryGetValue(implType, out var implSet))
                    {
                        implSet.Remove(component);
                        if (implSet.Count == 0) componentsDic.Remove(implType);
                    }
                }
            }
        }
    }
}


/// <summary>
/// 依赖注入器
/// </summary>
public static class DependencyInjector
{
    private static readonly Dictionary<Type, Action<object, ComponentContainer>> _cache = new();

    public static void InjectDependencies(IComponent component, ComponentContainer container)
    {
        var type = component.GetType();

        if (!_cache.TryGetValue(type, out var injector))
        {
            injector = CreateInjector(type);
            _cache[type] = injector;
        }

        injector(component, container);
    }

    private static Action<object, ComponentContainer> CreateInjector(Type componentType)
    {
        var injectActions = new List<Action<object, ComponentContainer>>();

        foreach (var interfaceType in componentType.GetInterfaces()
                     .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IRequire<>)))
        {
            var dependencyType = interfaceType.GetGenericArguments()[0];
            var method = interfaceType.GetMethod("SetDependency");

            injectActions.Add((instance, container) =>
            {
                var dependencies = container.GetAll(dependencyType).ToList();

                if (dependencyType.IsGenericType &&
                    dependencyType.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                {
                    var elementType = dependencyType.GetGenericArguments()[0];
                    var listType = typeof(List<>).MakeGenericType(elementType);
                    var list = (IList)Activator.CreateInstance(listType);

                    foreach (var item in container.GetAll(elementType))
                        list.Add(item);

                    method.Invoke(instance, new[] { list });
                }
                else if (dependencies.Count == 1)
                {
                    method.Invoke(instance, new[] { dependencies[0] });
                }
                else if (dependencies.Count > 1)
                {
                    LogSystem.Error($"发现多个{dependencyType.Name}依赖，请使用IEnumerable<{dependencyType.Name}>");
                }
            });
        }

        return (instance, container) => injectActions.ForEach(a => a(instance, container));
    }
}
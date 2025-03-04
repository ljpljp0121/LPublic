using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;


/// <summary>
/// 组件系统
/// </summary>
public class ComponentSystem : MonoBehaviour
{
    private ComponentContainer container = new();
    private List<IComponent> components = new();

    private void Awake() => Initialize();

    private void Initialize()
    {
        components = GetComponentsInChildren<IComponent>(true)
            .OrderBy(GetComponentPriority)
            .ToList();
        components.ForEach(c => container.Register(c));

        components.ForEach(c => DependencyInjector.InjectDependencies(c, container));

        components.ForEach(c =>
        {
            try
            {
                c.Init();
            }
            catch (Exception e)
            {
                Debug.LogError($"{c.GetType().Name}初始化失败: {e}");
            }
        });
    }

    private int GetComponentPriority(IComponent component)
    {
        var attr = component.GetType()
            .GetCustomAttribute<InitializeOrderAttribute>();
        return attr?.Order ?? int.MaxValue;
    }
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
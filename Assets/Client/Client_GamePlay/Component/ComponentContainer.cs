using System;
using System.Collections.Generic;
using System.Linq;

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

        RegisterType(type, component);

        foreach (var interfaceType in type.GetInterfaces())
            RegisterInterface(interfaceType, type, component);
    }

    /// <summary>
    /// 获取所有组件
    /// </summary>
    public IEnumerable<object> GetAll(Type type)
    {
        var components = new List<object>();
        if (componentsDic.TryGetValue(type, out var list))
        {
            components.AddRange(list.Where(c => c != null));
        }
        if (type.IsInterface && interfaceMap.TryGetValue(type, out var implTypes))
        {
            foreach (var implType in implTypes)
            {
                if (componentsDic.TryGetValue(implType, out var implComponents))
                {
                    components.AddRange(implComponents.Where(c => c != null));
                }
            }
        }
        return components.Distinct();
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
        if (componentsDic.TryGetValue(type, out var list))
        {
            list.RemoveAll(c => c == null || c.Equals(component));
            if (list.Count == 0) componentsDic.Remove(type);
        }
        foreach (var interfaceType in type.GetInterfaces())
        {
            if (interfaceMap.TryGetValue(interfaceType, out var implTypes))
            {
                foreach (var implType in implTypes.ToArray())
                {
                    if (componentsDic.TryGetValue(implType, out var implList))
                    {
                        implList.RemoveAll(c => c == null || c.Equals(component));
                        if (implList.Count == 0) componentsDic.Remove(implType);
                    }
                }
            }
        }
    }
}
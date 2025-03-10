using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

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
                if (dependencyType.IsGenericType &&
                    dependencyType.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                {
                    var elementType = dependencyType.GetGenericArguments()[0];
                    var dynamicCollectionType = typeof(DynamicDependencyCollection<>).MakeGenericType(elementType);
                    var dynamicCollection = Activator.CreateInstance(dynamicCollectionType, container);
                    method.Invoke(instance, new[] { dynamicCollection });
                }
                else
                {
                    var dependencies = container.GetAll(dependencyType).ToList();
                    if (dependencies.Count == 1)
                    {
                        method.Invoke(instance, new[] { dependencies[0] });
                    }
                    else if (dependencies.Count > 1)
                    {
                        LogSystem.Error($"发现多个{dependencyType.Name}依赖，请使用IEnumerable<{dependencyType.Name}>");
                    }
                }
            });
        }

        return (instance, container) => injectActions.ForEach(a => a(instance, container));
    }
}
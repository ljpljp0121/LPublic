using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class DynamicDependencyCollection<T> : IEnumerable<T>
{
    private readonly ComponentContainer _container;
    private List<T> _cachedList;
    private int _lastComponentCount;

    public DynamicDependencyCollection(ComponentContainer container)
    {
        _container = container;
        _cachedList = new List<T>();
        _lastComponentCount = 0;
    }

    public IEnumerator<T> GetEnumerator()
    {
        var currentComponents = _container.GetAll(typeof(T))
            .Cast<T>()
            .Where(c => c != null) // 过滤已销毁的组件
            .ToList();

        if (currentComponents.Count != _lastComponentCount ||
            currentComponents.Except(_cachedList).Any())
        {
            _cachedList = currentComponents;
            _lastComponentCount = currentComponents.Count;
        }
        return _cachedList.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
using UnityEngine;

/// <summary>
/// 实体基类,每个对象都必须挂载，会初始化所有IComponent组件
/// </summary>
[RequireComponent(typeof(ComponentSystem))]
public abstract class Entity : MonoBehaviour
{
}

/// <summary>
/// 组件接口
/// </summary>
public interface IComponent
{
    void Init();
}

/// <summary>
/// 依赖接口
/// </summary>
public interface IRequire<T> where T : class
{
    void SetDependency(T dependency);
}
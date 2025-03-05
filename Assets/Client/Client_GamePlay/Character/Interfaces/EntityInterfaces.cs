/// <summary>
/// 组件接口
/// </summary>
public interface IComponent
{
    void Init();
}

public interface IUpdatable
{
    void OnUpdate();
}

public interface IFixedUpdatable
{
    void OnFixedUpdate();
}

public interface ILateUpdatable
{
    void OnLateUpdate();
}

/// <summary>
/// 依赖接口
/// </summary>
public interface IRequire<T> where T : class
{
    void SetDependency(T dependency);
}
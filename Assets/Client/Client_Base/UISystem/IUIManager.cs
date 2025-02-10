using System;

/// <summary>
/// 界面管理器接口。
/// </summary>
public interface IUIManager
{
    /// <summary>
    /// 获取界面组数量。
    /// </summary>
    int UIGroupCount { get; }
    /// <summary>
    /// 获取或设置界面实例对象池自动释放可释放对象的间隔秒数。
    /// </summary>
    float InstanceAutoReleaseInterval { get; set; }
    /// <summary>
    /// 获取或设置界面实例对象池的容量。
    /// </summary>
    int InstanceCapacity { get; set; }
    /// <summary>
    /// 获取或设置界面实例对象池对象过期秒数。
    /// </summary>
    float InstanceExpireTime { get; set; }
    /// <summary>
    /// 获取或设置界面实例对象池的优先级。
    /// </summary>
    int InstancePriority { get; set; }

    /// <summary>
    /// 打开界面成功事件。
    /// </summary>
    event EventHandler<OpenUISuccessEvent> OpenUIFormSuccess;

    /// <summary>
    /// 打开界面失败事件。
    /// </summary>
    event EventHandler<OpenUIFailEvent> OpenUIFormFailure;

    /// <summary>
    /// 打开界面更新事件。
    /// </summary>
    event EventHandler<OpenUIUpdateEvent> OpenUIFormUpdate;

    /// <summary>
    /// 打开界面时加载依赖资源事件。
    /// </summary>
    event EventHandler<OpenUIDependencyAssetEvent> OpenUIFormDependencyAsset;

    /// <summary>
    /// 关闭界面完成事件。
    /// </summary>
    event EventHandler<CloseUICompleteEvent> CloseUIFormComplete;
}
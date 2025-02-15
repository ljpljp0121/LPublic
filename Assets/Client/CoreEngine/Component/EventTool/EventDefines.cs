#region 资源包加载更新相关

using YooAsset;

/// <summary>
/// 用户尝试再次初始化资源包
/// </summary>
public class UserTryInitialize : BaseEvent
{
}

/// <summary>
/// 用户尝试再次更新静态版本
/// </summary>
public class UserTryUpdatePackageVersion : BaseEvent
{
}

/// <summary>
/// 用户尝试再次更新补丁清单
/// </summary>
public class UserTryUpdatePatchManifest : BaseEvent
{
}

/// <summary>
/// 用户尝试再次下载网络文件
/// </summary>
public class UserTryDownloadWebFiles : BaseEvent
{
}


/// <summary>
/// 下载进度更新
/// </summary>
public class DownloadProgressUpdate : BaseEvent
{
    public int TotalDownloadCount;
    public int CurrentDownloadCount;
    public long TotalDownloadSizeBytes;
    public long CurrentDownloadSizeBytes;

    public DownloadProgressUpdate(DownloadUpdateData updateData)
    {
        TotalDownloadCount = updateData.TotalDownloadCount;
        CurrentDownloadCount = updateData.CurrentDownloadCount;
        TotalDownloadSizeBytes = updateData.TotalDownloadBytes;
        CurrentDownloadSizeBytes = updateData.CurrentDownloadBytes;
    }
}

#endregion
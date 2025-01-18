#region 资源包加载更新相关

using YooAsset;

/// <summary>
/// 用户尝试再次初始化资源包
/// </summary>
public class UserTryInitialize
{
}

/// <summary>
/// 用户开始下载网络文件
/// </summary>
public class UserBeginDownloadWebFiles
{
}

/// <summary>
/// 用户尝试再次更新静态版本
/// </summary>
public class UserTryUpdatePackageVersion
{
}

/// <summary>
/// 用户尝试再次更新补丁清单
/// </summary>
public class UserTryUpdatePatchManifest
{
}

/// <summary>
/// 用户尝试再次下载网络文件
/// </summary>
public class UserTryDownloadWebFiles
{
}

/// <summary>
/// 补丁包初始化失败
/// </summary>
public class InitializeFailed
{
}

/// <summary>
/// 补丁流程步骤改变
/// </summary>
public class PatchStatesChange
{
    public string Tips;

    public PatchStatesChange(string tips)
    {
        Tips = tips;
    }
}

/// <summary>
/// 发现更新文件
/// </summary>
public class FoundUpdateFiles
{
    public int TotalCount;
    public long TotalSizeBytes;

    public FoundUpdateFiles(int totalCount, long totalSizeBytes)
    {
        TotalCount = totalCount;
        TotalSizeBytes = totalSizeBytes;
    }
}

/// <summary>
/// 下载进度更新
/// </summary>
public class DownloadProgressUpdate
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

/// <summary>
/// 资源版本号更新失败
/// </summary>
public class PackageVersionUpdateFailed
{
}

/// <summary>
/// 补丁清单更新失败
/// </summary>
public class PatchManifestUpdateFailed
{
}

/// <summary>
/// 网络文件下载失败
/// </summary>
public class WebFileDownloadFailed
{
    public string FileName;
    public string Error;

    public WebFileDownloadFailed(DownloadErrorData errorData)
    {
        FileName = errorData.FileName;
        Error = errorData.ErrorInfo;
    }
}

#endregion
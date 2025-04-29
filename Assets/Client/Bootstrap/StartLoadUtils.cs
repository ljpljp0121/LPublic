
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using YooAsset;


public class StartLoadUtils
{
    private const int MAX_RETRY_TIME = 3;

    public static async Task InitDll(string packageName, EPlayMode playMode)
    {
        Debug.Log($"Start Load Dll Package{packageName}");
        await InitPackage(packageName, playMode);
        string packageVersion = await UpdatePackageVersion(packageName);
        await UpdatePackageManifest(packageName, packageVersion);
        if (playMode == EPlayMode.EditorSimulateMode || playMode == EPlayMode.OfflinePlayMode)
        {
            await ClearPackageCache(packageName);
        }
        else if (playMode == EPlayMode.HostPlayMode)
        {
            var downloader = CreatePackageDownloader(packageName);
            if (downloader.TotalDownloadCount == 0)
            {
                Debug.Log("Not found any download files !");
            }
            else
            {
                await DownloadPackageFiles(downloader);
            }
        }
        Debug.Log($"Load Dll success Package{packageName}");
    }

    public static async Task InitResource(string packageName, EPlayMode playMode)
    {
        Debug.Log($"start load assetPackage Package{packageName}");
        await InitPackage(packageName, playMode);
        string packageVersion = await UpdatePackageVersion(packageName);
        await UpdatePackageManifest(packageName, packageVersion);
        if (playMode == EPlayMode.EditorSimulateMode || playMode == EPlayMode.OfflinePlayMode)
        {
            await ClearPackageCache(packageName);
        }
        else
        {
            var downloader = CreatePackageDownloader(packageName);
            if (downloader.TotalDownloadCount == 0)
            {
                Debug.Log("Not found any download files !");
            }
            else
            {
                await DownloadPackageFiles(downloader);
            }
        }
        Debug.Log($"load assetPackage success Package{packageName}");
    }

    /// <summary>
    /// 初始化资源包
    /// </summary>
    public static async Task InitPackage(string packageName, EPlayMode playMode)
    {
        int maxRetryTime = MAX_RETRY_TIME;
        Debug.Log($"Init assetPackage!!! Package: {packageName} EPlayMode: {playMode}");
        var package = YooAssets.TryGetPackage(packageName) ?? YooAssets.CreatePackage(packageName);
        InitializationOperation initializationOperation = null;

        // 编辑器下的模拟模式
        if (playMode == EPlayMode.EditorSimulateMode)
        {
            Debug.Log("EditorSimulateMode！！！");
            PackageInvokeBuildResult simulateBuildResult = EditorSimulateModeHelper.SimulateBuild(packageName);
            EditorSimulateModeParameters createParameters = new EditorSimulateModeParameters();
            createParameters.EditorFileSystemParameters =
                FileSystemParameters.CreateDefaultEditorFileSystemParameters(simulateBuildResult.PackageRootDirectory);
            initializationOperation = package.InitializeAsync(createParameters);
        }
        // 单机运行模式
        if (playMode == EPlayMode.OfflinePlayMode)
        {
            Debug.Log("OfflinePlayMode！！！");
            var createParameters = new OfflinePlayModeParameters();
            createParameters.BuildinFileSystemParameters =
                FileSystemParameters.CreateDefaultBuildinFileSystemParameters();
            initializationOperation = package.InitializeAsync(createParameters);
        }
        // 联机运行模式
        if (playMode == EPlayMode.HostPlayMode)
        {
            Debug.Log("HostPlayMode！！！");
            string defaultHostServer = GetHostServerURL(packageName);
            string fallbackHostServer = GetHostServerURL(packageName);
            IRemoteServices remoteServices = new RemoteServices(defaultHostServer, fallbackHostServer);
            var createParameters = new HostPlayModeParameters();
            createParameters.BuildinFileSystemParameters =
                FileSystemParameters.CreateDefaultBuildinFileSystemParameters();
            createParameters.CacheFileSystemParameters =
                FileSystemParameters.CreateDefaultCacheFileSystemParameters(remoteServices);
            initializationOperation = package.InitializeAsync(createParameters);
        }

        await initializationOperation;
        if (initializationOperation.Status != EOperationStatus.Succeed)
        {
            Debug.LogError($"Init assetPackage Failed!!! retry!!! {initializationOperation.Error}");
        }
        else
        {
            Debug.Log($"Init assetPackage  success !!!");
        }
    }

    /// <summary>
    /// 更新资源版本号
    /// </summary>
    public static async Task<string> UpdatePackageVersion(string packageName)
    {
        var package = YooAssets.GetPackage(packageName);
        var operation = package.RequestPackageVersionAsync();
        await operation;
        if (operation.Status != EOperationStatus.Succeed)
        {
            Debug.LogError($"UpdatePackageVersion Failed!!! {operation.Error}");
            return string.Empty;
        }
        else
        {
            Debug.Log($"UpdatePackageVersion Success!!! Request package version : {operation.PackageVersion}");
            return operation.PackageVersion;
        }
    }

    /// <summary>
    /// 更新资源清单
    /// </summary>
    public static async Task UpdatePackageManifest(string packageName, string packageVersion)
    {
        var package = YooAssets.GetPackage(packageName);
        var operation = package.UpdatePackageManifestAsync(packageVersion);
        await operation;
        if (operation.Status != EOperationStatus.Succeed)
        {
            Debug.LogError($"UpdatePackageManifest Failed!!! {operation.Error}");
        }
        else
        {
            Debug.Log($"UpdatePackageManifest  Success!!!");
        }
    }

    /// <summary>
    /// 创建资源下载器
    /// </summary>
    public static ResourceDownloaderOperation CreatePackageDownloader(string packageName)
    {
        Debug.Log($"Create Downloader  Package{packageName}");
        var package = YooAssets.GetPackage(packageName);
        int downloadingMaxNum = 10;
        int failedTryAgain = 3;
        ResourceDownloaderOperation downloader = package.CreateResourceDownloader(downloadingMaxNum, failedTryAgain);
        return downloader;
    }

    /// <summary>
    /// 下载资源文件
    /// </summary>
    public static async Task DownloadPackageFiles(ResourceDownloaderOperation downloader)
    {
        downloader.DownloadErrorCallback = (data) =>
        {
            Debug.Log($"下载出错：文件名：{data.FileName}，错误信息：{data.ErrorInfo}");
        };
        downloader.DownloadUpdateCallback = (data) =>
        {
            Debug.Log($"文件总数：{data.TotalDownloadCount},已下载文件数：{data.CurrentDownloadCount}," +
                      $"下载总大小：{data.TotalDownloadBytes},已下载大小{data.CurrentDownloadBytes}");
        };
        downloader.DownloadFileBeginCallback = (data) =>
        {
            Debug.Log($"开始下载：文件名：{data.FileName},文件大小：{data.FileSize}");
        };

        downloader.BeginDownload();
        await downloader;
        if (downloader.Status != EOperationStatus.Succeed)
        {
            Debug.LogError($"下载失败：{downloader.Error}");
        }
        else
        {
            Debug.Log($"下载成功");
        }
    }

    /// <summary>
    /// 清理资源缓存
    /// </summary>
    public static async Task ClearPackageCache(string packageName)
    {
        Debug.Log($"加载完成 开始清理资源缓存!!  Package{packageName}");
        var package = YooAssets.GetPackage(packageName);
        var operation = package.ClearCacheFilesAsync(EFileClearMode.ClearUnusedBundleFiles);
        await operation;
    }

    /// <summary>
    /// 获取资源服务器地址
    /// </summary>
    private static string GetHostServerURL(string packageName)
    {
        string bucketName = "unity-2540297235";
        string endpoint = "oss-cn-hangzhou.aliyuncs.com";
        string version = Bootstrap.Instance.Version;
        string hostServerIP = $"https://{bucketName}.{endpoint}/LPublic/{version}/{packageName}";

        return hostServerIP;

        // #if UNITY_EDITOR
        //         if (UnityEditor.EditorUserBuildSettings.activeBuildTarget == UnityEditor.BuildTarget.Android)
        //             return $"{hostServerIP}";
        //         else if (UnityEditor.EditorUserBuildSettings.activeBuildTarget == UnityEditor.BuildTarget.iOS)
        //             return $"{hostServerIP}";
        //         else if (UnityEditor.EditorUserBuildSettings.activeBuildTarget == UnityEditor.BuildTarget.WebGL)
        //             return $"{hostServerIP}";
        //         else
        //             return $"{hostServerIP}";
        // #else
        //         if (Application.platform == RuntimePlatform.Android)
        //             return $"{hostServerIP}";
        //         else if (Application.platform == RuntimePlatform.IPhonePlayer)
        //             return $"{hostServerIP}";
        //         else if (Application.platform == RuntimePlatform.WebGLPlayer)
        //             return $"{hostServerIP}";
        //         else
        //             return $"{hostServerIP}";
        // #endif
    }
}

using System.Collections;
using UnityEngine;
using YooAsset;

/// <summary>
/// 初始化资源包
/// </summary>
public class InitializePackage : StateBase
{
    public override void Enter()
    {
        StartLoading.SetProgress("初始化游戏资源!", 10);
        MonoSystem.BeginCoroutine(InitPackage());
    }

    private IEnumerator InitPackage()
    {
        if (!TryGetShareData("PlayMode", out EPlayMode playMode))
        {
            LogSystem.Error("没有设置对应的资源运行类型");
        }

        if (!TryGetShareData("PackageName", out string packageName))
        {
            LogSystem.Error("设置的资源包名称不存在");
        }

        var package = YooAssets.TryGetPackage(packageName);
        if (package == null)
            package = YooAssets.CreatePackage(packageName);

        InitializationOperation initializationOperation = null;

        // 编辑器下的模拟模式
        if (playMode == EPlayMode.EditorSimulateMode)
        {
            LogSystem.Log("编辑器模拟模式！！！");
            PackageInvokeBuildResult simulateBuildResult = EditorSimulateModeHelper.SimulateBuild(packageName);
            EditorSimulateModeParameters createParameters = new EditorSimulateModeParameters();
            createParameters.EditorFileSystemParameters =
                FileSystemParameters.CreateDefaultEditorFileSystemParameters(simulateBuildResult.PackageRootDirectory);
            initializationOperation = package.InitializeAsync(createParameters);
        }

        // 单机运行模式
        if (playMode == EPlayMode.OfflinePlayMode)
        {
            LogSystem.Log("单机运行模式！！！");
            var createParameters = new OfflinePlayModeParameters();
            createParameters.BuildinFileSystemParameters =
                FileSystemParameters.CreateDefaultBuildinFileSystemParameters();
            initializationOperation = package.InitializeAsync(createParameters);
        }

        // 联机运行模式
        if (playMode == EPlayMode.HostPlayMode)
        {
            LogSystem.Log("联机运行模式！！！");
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

        yield return initializationOperation;

        // 初始化失败 界面提示
        if (initializationOperation.Status != EOperationStatus.Succeed)
        {
            LogSystem.Error($"{initializationOperation.Error}");
            EventSystem.DispatchEvent<UserTryInitialize>(new UserTryInitialize());
        }
        else
        {
            stateMachine.ChangeState<UpdatePackageVersion>();
        }
    }

    /// <summary>
    /// 获取资源服务器地址
    /// </summary>
    private string GetHostServerURL(string packageName)
    {
        string bucketName = "unity-2540297235";
        string endpoint = "oss-cn-hangzhou.aliyuncs.com";
        string version = CoreEngineRoot.Version;
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
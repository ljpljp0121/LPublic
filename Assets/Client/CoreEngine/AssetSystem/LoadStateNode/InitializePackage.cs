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
        EventSystem.DispatchEvent<PatchStatesChange>(new PatchStatesChange("初始化资源包!"));
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
            string defaultHostServer = GetHostServerURL();
            string fallbackHostServer = GetHostServerURL();
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
            EventSystem.DispatchEvent<InitializeFailed>(new InitializeFailed());
        }
        else
        {
            stateMachine.ChangeState<UpdatePackageVersion>();
        }
    }

    /// <summary>
    /// 获取资源服务器地址
    /// </summary>
    private string GetHostServerURL()
    {
        //string hostServerIP = "http://127.0.0.1:8088"; //安卓模拟器地址
        string hostServerIP = "http://127.0.0.1:8088";
        string appVersion = "v1.0";

#if UNITY_EDITOR
        if (UnityEditor.EditorUserBuildSettings.activeBuildTarget == UnityEditor.BuildTarget.Android)
            return $"{hostServerIP}/LPublic/Android";
        else if (UnityEditor.EditorUserBuildSettings.activeBuildTarget == UnityEditor.BuildTarget.iOS)
            return $"{hostServerIP}/LPublic/IPhone";
        else if (UnityEditor.EditorUserBuildSettings.activeBuildTarget == UnityEditor.BuildTarget.WebGL)
            return $"{hostServerIP}/LPublic/WebGL";
        else
            return $"{hostServerIP}/LPublic/PC";
#else
        if (Application.platform == RuntimePlatform.Android)
            return $"{hostServerIP}/LPublic/Android";
        else if (Application.platform == RuntimePlatform.IPhonePlayer)
            return $"{hostServerIP}/LPublic/IPhone";
        else if (Application.platform == RuntimePlatform.WebGLPlayer)
            return $"{hostServerIP}/LPublic/WebGL";
        else
            return $"{hostServerIP}/LPublic/PC";
#endif
    }
}
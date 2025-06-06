using System.Collections;
using YooAsset;

/// <summary>
/// 更新资源清单
/// </summary>
public class UpdatePackageManifest : StateBase
{
    public override void Enter()
    {
        StartLoading.SetProgress("更新资源清单", 30);
        MonoSystem.BeginCoroutine(UpdateManifest());
    }

    private IEnumerator UpdateManifest()
    {
        yield return CoroutineTool.WaitForSecondsRealtime(0.5f);
        if (!TryGetShareData("PackageName", out string packageName))
        {
            LogSystem.Error("设置的资源包名称不存在");
        }
        if (!TryGetShareData("PackageVersion", out string packageVersion))
        {
            LogSystem.Error("资源版本号不存在");
        }
        if (!TryGetShareData("PlayMode", out EPlayMode playMode))
        {
            LogSystem.Error("没有设置对应的资源运行类型");
        }
        var package = YooAssets.GetPackage(packageName);
        var operation = package.UpdatePackageManifestAsync(packageVersion);
        yield return operation;

        if (operation.Status != EOperationStatus.Succeed)
        {
            LogSystem.Error(operation.Error);
            EventSystem.DispatchEvent<UserTryUpdatePatchManifest>(new UserTryUpdatePatchManifest());
        }
        else if (playMode == EPlayMode.HostPlayMode)
        {
            stateMachine.ChangeState<CreatePackageDownloader>();
        }
        else if (playMode == EPlayMode.OfflinePlayMode || playMode == EPlayMode.EditorSimulateMode)
        {
            stateMachine.ChangeState<ClearPackageCache>();
        }
    }
}
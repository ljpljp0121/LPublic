using System.Collections;
using YooAsset;

/// <summary>
/// 更新资源版本号
/// </summary>
public class UpdatePackageVersion : StateBase
{
    public override void Enter()
    {
        EventSystem.DispatchEvent<PatchStatesChange>(new PatchStatesChange("获取最新的资源版本!"));
        MonoSystem.BeginCoroutine(UpdateVersion());
    }

    private IEnumerator UpdateVersion()
    {
        yield return CoroutineTool.WaitForSecondsRealtime(0.5f);

        if (!TryGetShareData("PackageName", out string packageName))
        {
            LogSystem.Error("设置的资源包名称不存在");
        }
        var package = YooAssets.GetPackage(packageName);
        var operation = package.RequestPackageVersionAsync();
        yield return operation;

        if (operation.Status != EOperationStatus.Succeed)
        {
            LogSystem.Error(operation.Error);
            EventSystem.DispatchEvent<PackageVersionUpdateFailed>(new PackageVersionUpdateFailed());
        }
        else
        {
            LogSystem.Log($"Request package version : {operation.PackageVersion}");
            AddShareData("PackageVersion", operation.PackageVersion);
            stateMachine.ChangeState<UpdatePackageManifest>();
        }
    }
}
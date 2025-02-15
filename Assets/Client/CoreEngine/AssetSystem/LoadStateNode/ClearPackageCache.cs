using YooAsset;

public class ClearPackageCache : StateBase
{
    public override void Enter()
    {
        StartLoading.SetProgress("清理资源缓存", 100);
        if (!TryGetShareData("PackageName", out string packageName))
        {
            LogSystem.Error("设置的资源包名称不存在");
        }
        var package = YooAssets.GetPackage(packageName);
        var operation = package.ClearCacheFilesAsync(EFileClearMode.ClearUnusedBundleFiles);
        operation.Completed += OperationCompleted;
    }

    private void OperationCompleted(AsyncOperationBase obj)
    {
        stateMachine.ChangeState<UpdaterDone>();
    }
}
using YooAsset;

public class DownLoadPackageOver : StateBase
{
    public override void Enter()
    {
        if (!TryGetShareData("PackageName", out string packageName))
        {
            LogSystem.Error("设置的资源包名称不存在");
        }

        var package = YooAssets.GetPackage(packageName);
        if (package != null && packageName == "DllPackage")
        {
            stateMachine.ChangeState<LoadPackageDll>();
        }
        else
        {
            stateMachine.ChangeState<ClearPackageCache>();
        }
    }
}
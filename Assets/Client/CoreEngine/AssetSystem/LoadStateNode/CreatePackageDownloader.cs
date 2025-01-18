using System.Collections;
using YooAsset;

/// <summary>
/// 创建文件下载器
/// </summary>
public class CreatePackageDownloader : StateBase
{
    public override void Enter()
    {
        EventSystem.DispatchEvent<PatchStatesChange>(new PatchStatesChange("创建补丁下载器！"));
        MonoSystem.BeginCoroutine(CreateDownloader());
    }

    private IEnumerator CreateDownloader()
    {
        yield return CoroutineTool.WaitForSecondsRealtime(0.5f);
        if (!TryGetShareData("PackageName", out string packageName))
        {
            LogSystem.Error("设置的资源包名称不存在");
        }
        var package = YooAssets.GetPackage(packageName);
        int downloadingMaxNum = 10;
        int failedTryAgain = 3;
        ResourceDownloaderOperation downloader = package.CreateResourceDownloader(downloadingMaxNum, failedTryAgain);
        AddShareData("Downloader", downloader);

        if (downloader.TotalDownloadCount == 0)
        {
            LogSystem.Log("Not found any download files !");
            stateMachine.ChangeState<LoadPackageDll>();
        }
        else
        {
            // 注意：开发者需要在下载前检测磁盘空间不足
            int totalDownloadCount = downloader.TotalDownloadCount;
            long totalDownloadBytes = downloader.TotalDownloadBytes;
            EventSystem.DispatchEvent<FoundUpdateFiles>(new FoundUpdateFiles(totalDownloadCount, totalDownloadBytes));
        }
    }
}
using System.Collections;
using UnityEngine;
using YooAsset;

/// <summary>
/// 创建文件下载器
/// </summary>
public class CreatePackageDownloader : StateBase
{
    public override void Enter()
    {
        StartLoading.SetProgress("创建补丁下载器！", 40);
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
            if (packageName == "DllPackage")
            {
                stateMachine.ChangeState<LoadPackageDll>();
            }
            else
            {
                stateMachine.ChangeState<ClearPackageCache>();
            }
        }
        else
        {
            stateMachine.ChangeState<DownloadPackageFiles>();
        }
    }
}
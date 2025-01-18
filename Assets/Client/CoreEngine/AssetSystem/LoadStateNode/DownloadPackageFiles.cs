using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YooAsset;

/// <summary>
/// 下载更新文件
/// </summary>
public class DownloadPackageFiles : StateBase
{
    public override void Enter()
    {
        EventSystem.DispatchEvent<PatchStatesChange>(new PatchStatesChange("开始下载补丁文件!"));
        MonoSystem.BeginCoroutine(BeginDownload());
    }

    private IEnumerator BeginDownload()
    {
        if (TryGetShareData("Downloader", out ResourceDownloaderOperation downloader))
        {
            LogSystem.Error("没找到资源下载句柄!");
        }

        downloader.DownloadErrorCallback = (data) =>
        {
            EventSystem.DispatchEvent<WebFileDownloadFailed>(new WebFileDownloadFailed(data));
            LogSystem.Log(string.Format("下载出错：文件名：{0}，错误信息：{1}", data.FileName, data.ErrorInfo));
        };
        downloader.DownloadUpdateCallback = (data) =>
        {
            EventSystem.DispatchEvent<DownloadProgressUpdate>(new DownloadProgressUpdate(data));
            LogSystem.Log(string.Format("文件总数：{0}，已下载文件数：{1}，下载总大小：{2}，已下载大小{3}", data.TotalDownloadCount,
                data.CurrentDownloadCount, data.TotalDownloadBytes, data.CurrentDownloadBytes));
        };
        downloader.DownloadFileBeginCallback = (data) =>
        {
            LogSystem.Log(string.Format("开始下载：文件名：{0}，文件大小：{1}", data.FileName, data.FileSize));
        };

        downloader.BeginDownload();
        yield return downloader;

        if (downloader.Status != EOperationStatus.Succeed)
        {
            LogSystem.Log("更新失败");
            yield break;
        }
        else
        {
            LogSystem.Log("更新成功");
        }
        stateMachine.ChangeState<DownLoadPackageOver>();
    }
}
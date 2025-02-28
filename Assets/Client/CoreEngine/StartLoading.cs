using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class StartLoading : MonoBehaviour
{
    public static StartLoading Instance { get; set; }

    public TextMeshProUGUI InfoText;
    public Slider ProgressBar;
    public TextMeshProUGUI ProgressText;

    private bool hasError = false;
    static string cacheInfo = "";
    static int cacheProgress = 0;
    static string cacheError = "";


    void Awake()
    {
        Instance = this;
        EventSystem.RegisterEvent<DownloadProgressUpdate>(OnDownloadProgressUpdate);
    }

    private void OnDestroy()
    {
        EventSystem.RemoveEvent<DownloadProgressUpdate>();
    }

    private void OnEnable()
    {
        SetProgress(cacheInfo, cacheProgress);
        if (!string.IsNullOrEmpty(cacheError))
        {
            SetError(cacheError);
        }
    }


    void SetProgressImp(string info, int progress)
    {
        if (hasError)
            return;
        InfoText.text = info.ToUpper();
        DOTween.Kill(ProgressBar);
        float targetValue = progress * 0.01f;
        DOTween.To(() => ProgressBar.value, x => ProgressBar.value = x, targetValue, 0.3f)
            .SetEase((Ease.OutQuad))
            .OnUpdate(() => { ProgressText.text = $"{progress}%"; });
    }

    void SetErrorImp(string error)
    {
        hasError = true;
        InfoText.text = $"<color=red>{error.ToUpper()}</color>";
    }

    public static void SetProgress(string info, int progress)
    {
        if (Instance != null)
        {
            Instance.SetProgressImp(info, progress);
        }
        else
        {
            cacheInfo = info;
            cacheProgress = progress;
        }
    }

    public static void SetError(string error)
    {
        if (Instance != null)
        {
            Instance.SetErrorImp(error);
        }
        else
        {
            cacheError = error;
        }
    }

    public static void Open()
    {
        if (Instance != null)
        {
            Instance.gameObject.SetActive(true);
        }
    }

    public static void Close()
    {
        if (Instance != null)
        {
            Instance.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// 下载进度更新
    /// </summary>
    private void OnDownloadProgressUpdate(DownloadProgressUpdate obj)
    {
        float value = (float)obj.CurrentDownloadCount / obj.TotalDownloadCount;
        string currentSizeMB = (obj.CurrentDownloadSizeBytes / 1048576f).ToString("f1");
        string totalSizeMB = (obj.TotalDownloadSizeBytes / 1048576f).ToString("f1");
        string tips = $"{obj.CurrentDownloadCount}/{obj.TotalDownloadCount} {currentSizeMB}MB/{totalSizeMB}MB";
        SetProgress(tips, (int)(value * 100));
    }
}
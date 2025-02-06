using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PatchWindow : MonoBehaviour
{
    /// <summary>
    /// 对话框封装类
    /// </summary>
    private class MessageBox
    {
        private GameObject cloneObject;
        private Text content;
        private Button btnOK;
        private Action clickOK;

        public bool ActiveSelf => cloneObject.activeSelf;

        public void Create(GameObject cloneObject)
        {
            this.cloneObject = cloneObject;
            content = cloneObject.transform.Find("txt_content").GetComponent<Text>();
            btnOK = cloneObject.transform.Find("btn_ok").GetComponent<Button>();
            btnOK.onClick.AddListener(OnClickYes);
        }

        public void Show(string content, System.Action clickOK)
        {
            this.content.text = content;
            this.clickOK = clickOK;
            cloneObject.SetActive(true);
            cloneObject.transform.SetAsLastSibling();
        }

        public void Hide()
        {
            content.text = string.Empty;
            clickOK = null;
            cloneObject.SetActive(false);
        }

        private void OnClickYes()
        {
            clickOK?.Invoke();
            Hide();
        }
    }

    private readonly List<MessageBox> msgBoxList = new List<MessageBox>();

    //UGUI相关
    private GameObject messageBoxObj;
    private Slider slider;
    private Text tips;

    void Awake()
    {
        slider = transform.Find("UIWindow/Slider").GetComponent<Slider>();
        tips = transform.Find("UIWindow/Slider/txt_tips").GetComponent<Text>();
        tips.text = "开始初始化世界";
        messageBoxObj = transform.Find("UIWindow/MessgeBox").gameObject;
        messageBoxObj.SetActive(false);

        EventSystem.RegisterEvent<InitializeFailed>(OnInitializeFailed);
        EventSystem.RegisterEvent<PatchStatesChange>(OnPatchStatesChange);
        EventSystem.RegisterEvent<FoundUpdateFiles>(OnFoundUpdateFiles);
        EventSystem.RegisterEvent<DownloadProgressUpdate>(OnDownloadProgressUpdate);
        EventSystem.RegisterEvent<PackageVersionUpdateFailed>(OnPackageVersionUpdateFailed);
        EventSystem.RegisterEvent<PatchManifestUpdateFailed>(OnPatchManifestUpdateFailed);
        EventSystem.RegisterEvent<WebFileDownloadFailed>(OnWebFileDownloadFailed);
    }

    private void OnDestroy()
    {
        EventSystem.RemoveEvent<InitializeFailed>();
        EventSystem.RemoveEvent<PatchStatesChange>();
        EventSystem.RemoveEvent<FoundUpdateFiles>();
        EventSystem.RemoveEvent<DownloadProgressUpdate>();
        EventSystem.RemoveEvent<PackageVersionUpdateFailed>();
        EventSystem.RemoveEvent<PatchManifestUpdateFailed>();
        EventSystem.RemoveEvent<WebFileDownloadFailed>();
    }

    private void ShowMessageBox(string content, Action clickOK)
    {
        MessageBox msgBox = null;
        for (int i = 0; i < msgBoxList.Count; i++)
        {
            var item = msgBoxList[i];
            if (item.ActiveSelf == false)
            {
                msgBox = item;
                break;
            }
        }
        // 如果没有可用的对话框，则创建一个新的对话框
        if (msgBox == null)
        {
            msgBox = new MessageBox();
            var cloneObject = GameObject.Instantiate(messageBoxObj, messageBoxObj.transform.parent);
            msgBox.Create(cloneObject);
            msgBoxList.Add(msgBox);
        }
        // 显示对话框
        msgBox.Show(content, clickOK);
    }

    #region 事件相关

    /// <summary>
    /// 补丁包初始化失败
    /// </summary>
    private void OnInitializeFailed(InitializeFailed obj)
    {
        ShowMessageBox($"初始化失败，点击重新初始化",
            () => { EventSystem.DispatchEvent<UserTryInitialize>(new UserTryInitialize()); });
    }

    /// <summary>
    /// 补丁流程步骤改变
    /// </summary>
    private void OnPatchStatesChange(PatchStatesChange obj)
    {
        tips.text = obj.Tips;
    }

    /// <summary>
    /// 发现更新文件
    /// </summary>
    private void OnFoundUpdateFiles(FoundUpdateFiles obj)
    {
        float sizeMB = obj.TotalSizeBytes / 1048576f;
        sizeMB = Mathf.Clamp(sizeMB, 0.1f, float.MaxValue);
        string totalSizeMB = sizeMB.ToString("f1");
        EventSystem.DispatchEvent<UserBeginDownloadWebFiles>(new UserBeginDownloadWebFiles());
    }

    /// <summary>
    /// 下载进度更新
    /// </summary>
    private void OnDownloadProgressUpdate(DownloadProgressUpdate obj)
    {
        slider.value = (float)obj.CurrentDownloadCount / obj.TotalDownloadCount;
        string currentSizeMB = (obj.CurrentDownloadSizeBytes / 1048576f).ToString("f1");
        string totalSizeMB = (obj.TotalDownloadSizeBytes / 1048576f).ToString("f1");
        tips.text = $"{obj.CurrentDownloadCount}/{obj.TotalDownloadCount} {currentSizeMB}MB/{totalSizeMB}MB";
    }

    /// <summary>
    /// 资源版本号更新失败
    /// </summary>
    private void OnPackageVersionUpdateFailed(PackageVersionUpdateFailed obj)
    {
        ShowMessageBox($"资源版本号更新失败，请检查网络",
            () => { EventSystem.DispatchEvent<UserTryUpdatePackageVersion>(new UserTryUpdatePackageVersion()); });
    }

    /// <summary>
    /// 补丁清单更新失败
    /// </summary>
    private void OnPatchManifestUpdateFailed(PatchManifestUpdateFailed obj)
    {
        ShowMessageBox($"补丁清单更新失败，请检查网络",
            () => { EventSystem.DispatchEvent<UserTryUpdatePatchManifest>(new UserTryUpdatePatchManifest()); });
    }

    /// <summary>
    /// 网络文件下载失败
    /// </summary>
    private void OnWebFileDownloadFailed(WebFileDownloadFailed obj)
    {
        ShowMessageBox($"文件下载失败，点击重新下载",
            () => { EventSystem.DispatchEvent<UserTryDownloadWebFiles>(new UserTryDownloadWebFiles()); });
    }

    #endregion
}
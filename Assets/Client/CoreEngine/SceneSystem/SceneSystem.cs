using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using YooAsset;

public static class SceneSystem
{
    public static string assetPath = "Assets/Bundle/Scenes/";

    public static void LoadScene(string sceneName, LoadSceneMode loadMode = LoadSceneMode.Single,
        LocalPhysicsMode physicsMode = LocalPhysicsMode.None)
    {
        SceneHandle handle = YooAssets.LoadSceneSync($"{assetPath}{sceneName}", loadMode, physicsMode);
    }

    /// <summary>
    /// �첽���س���,��Э�̵ķ�ʽ
    /// </summary>
    public static void LoadSceneAsyncByCor(string sceneName, Action<float> onProgress = null, Action onComplete = null,
        LoadSceneMode loadMode = LoadSceneMode.Single,
        LocalPhysicsMode physicsMode = LocalPhysicsMode.None, bool suspendLoad = false)
    {
        MonoSystem.BeginCoroutine(LoadSceneRoutine(sceneName, onProgress, onComplete, loadMode, physicsMode, suspendLoad));
    }

    private static IEnumerator LoadSceneRoutine(string sceneName, Action<float> onProgress = null, Action onComplete = null,
        LoadSceneMode loadMode = LoadSceneMode.Single,
        LocalPhysicsMode physicsMode = LocalPhysicsMode.None, bool suspendLoad = false)
    {
        SceneHandle handle =
            YooAssets.LoadSceneAsync($"{assetPath}{sceneName}", loadMode, physicsMode, suspendLoad);
        handle.Completed += (obj) =>
        {
            Debug.Log($"��������: {handle.SceneName}");
            onComplete?.Invoke();
            obj.Release();
        };
        while (!handle.IsDone)
        {
            Debug.Log($"��ǰ�������ؽ���:{handle.Progress}");
            onProgress?.Invoke(handle.Progress);
            yield return 1;
        }
    }

    /// <summary>
    /// �첽���س���,Task��ʽ
    /// </summary>
    public static async Task LoadSceneAsync(string sceneName, Action<float> onProgress = null, Action onComplete = null,
        LoadSceneMode loadMode = LoadSceneMode.Single,
        LocalPhysicsMode physicsMode = LocalPhysicsMode.None, bool suspendLoad = false)
    {
        SceneHandle handle = YooAssets.LoadSceneAsync($"{assetPath}{sceneName}", loadMode, physicsMode, suspendLoad);

        try
        {
            while (!handle.IsDone)
            {
                onProgress?.Invoke(handle.Progress);
                await Task.Yield();
            }
            if (handle.Status == EOperationStatus.Succeed)
            {
                Debug.Log($"�����������: {handle.SceneName}");
                onComplete?.Invoke();
            }
            else
            {
                throw new Exception($"��������ʧ��: {sceneName}");
            }
        }
        finally
        {
            handle.Release();
        }
    }
}
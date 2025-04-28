using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using YooAsset;

public static class SceneSystem
{
    private static string assetPath = "Assets/Bundle/Scenes/";

    public static void LoadScene(string sceneName, LoadSceneMode loadMode = LoadSceneMode.Single,
        LocalPhysicsMode physicsMode = LocalPhysicsMode.None)
    {
        SceneHandle handle = YooAssets.LoadSceneSync($"{assetPath}{sceneName}", loadMode, physicsMode);
    }

    /// <summary>
    /// �첽���س���,Task��ʽ
    /// </summary>
    public static async Task LoadSceneAsync(string sceneName, Action onComplete = null,
        LoadSceneMode loadMode = LoadSceneMode.Single,
        LocalPhysicsMode physicsMode = LocalPhysicsMode.None, bool suspendLoad = false)
    {
        SceneHandle handle = YooAssets.LoadSceneAsync($"{assetPath}{sceneName}", loadMode, physicsMode, suspendLoad);

        await handle.Task;

        Debug.Log($"�����������: {handle.SceneName}");

        handle.Release();
        onComplete?.Invoke();
    }
}
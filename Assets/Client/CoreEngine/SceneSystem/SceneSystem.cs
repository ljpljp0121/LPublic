using System;
using System.Collections;
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

    public static void LoadSceneAsync(string sceneName, Action<float> onProgress = null, Action callback = null,
        LoadSceneMode loadMode = LoadSceneMode.Single,
        LocalPhysicsMode physicsMode = LocalPhysicsMode.None, bool suspendLoad = false)
    {
        SceneHandle handle = YooAssets.LoadSceneAsync($"{assetPath}{sceneName}.unity", loadMode, physicsMode, suspendLoad);
        Coroutine loadCoroutine = MonoSystem.BeginCoroutine(Loading(handle, onProgress));
        handle.Completed += (obj) =>
        {
            MonoSystem.EndCoroutine(loadCoroutine);
            callback?.Invoke();
            obj.Release();
        };
    }

    private static IEnumerator Loading(SceneHandle handle, Action<float> onProgress)
    {
        var wait = new WaitForSecondsRealtime(0.05f);
        while (handle is { IsDone: false })
        {
            Debug.Log(handle.Progress);
            onProgress?.Invoke(handle.Progress);
            yield return wait;
        }

        onProgress?.Invoke(1f);
    }
}
using System;
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

    public static void LoadSceneAsync(string sceneName, Action callback = null,
        LoadSceneMode loadMode = LoadSceneMode.Single,
        LocalPhysicsMode physicsMode = LocalPhysicsMode.None, bool suspendLoad = false)
    {
        SceneHandle handle = YooAssets.LoadSceneAsync($"{assetPath}{sceneName}", loadMode, physicsMode, suspendLoad);
        handle.Completed += (obj) =>
        {
            callback?.Invoke();
            obj.Release();
        };
    }
}
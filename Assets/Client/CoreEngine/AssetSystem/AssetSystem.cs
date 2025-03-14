using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using YooAsset;

public static class AssetSystem
{
    private static ResourcePackage package;

    private static string assetPath = "Assets/Bundle/";

    public static IEnumerator Init(string dllPackageName, string resourcePackageName, EPlayMode playMode)
    {
        //初始化资源系统
        YooAssets.Initialize();
        yield return InitDll(dllPackageName, playMode);
        yield return InitResource(resourcePackageName, playMode);
        //设置默认的资源包
        package = YooAssets.GetPackage(resourcePackageName);
        YooAssets.SetDefaultPackage(package);
    }

    private static IEnumerator InitDll(string packageName, EPlayMode playMode)
    {
        //开始补丁更新流程
        PatchOperation operation = new PatchOperation(packageName, playMode);
        YooAssets.StartOperation(operation);
        yield return operation;
    }

    private static IEnumerator InitResource(string packageName, EPlayMode playMode)
    {
        //开始补丁更新流程
        PatchOperation operation = new PatchOperation(packageName, playMode);
        YooAssets.StartOperation(operation);
        yield return operation;
    }

    #region 普通Class对象

    /// <summary>
    /// 获取实例-普通Class
    /// 如果类型需要缓存，会从对象池中获取
    /// 如果对象池没有或new一个返回
    /// </summary>
    public static T GetOrNew<T>() where T : class, new()
    {
        T obj = PoolSystem.GetObject<T>();
        if (obj == null) obj = new T();
        return obj;
    }

    // <summary>
    /// 获取实例-普通Class
    /// 如果类型需要缓存，会从对象池中获取
    /// 如果对象池没有或new一个返回
    /// </summary>
    /// <param name="keyName">对象池中的名称</param>
    public static T GetOrNew<T>(string keyName) where T : class, new()
    {
        T obj = PoolSystem.GetObject<T>(keyName);
        if (obj == null) obj = new T();
        return obj;
    }

    /// <summary>
    /// 卸载普通对象，这里是使用对象池的方式
    /// </summary>
    public static void PushObjectInPool(System.Object obj)
    {
        PoolSystem.PushObject(obj);
    }

    /// <summary>
    /// 普通对象（非GameObject）放置对象池中
    /// 基于KeyName存放
    /// </summary>
    public static void PushObjectInPool(object obj, string keyName)
    {
        PoolSystem.PushObject(obj, keyName);
    }

    /// <summary>
    /// 初始化对象池并设置容量
    /// </summary>
    /// <param name="keyName">资源名称</param>
    /// <param name="maxCapacity">容量限制，超出时会销毁而不是进入对象池，-1代表无限</param>
    /// <param name="defaultQuantity">默认容量，填写会向池子中放入对应数量的对象，0代表不预先放入</param>
    public static void InitObjectPool<T>(string keyName, int maxCapacity = -1, int defaultQuantity = 0) where T : new()
    {
        PoolSystem.InitObjectPool<T>(keyName, maxCapacity, defaultQuantity);
    }

    /// <summary>
    /// 初始化对象池并设置容量
    /// </summary>
    /// <param name="maxCapacity">容量限制，超出时会销毁而不是进入对象池，-1代表无限</param>
    /// <param name="defaultQuantity">默认容量，填写会向池子中放入对应数量的对象，0代表不预先放入</param>
    public static void InitObjectPool<T>(int maxCapacity = -1, int defaultQuantity = 0) where T : new()
    {
        PoolSystem.InitObjectPool<T>(maxCapacity, defaultQuantity);
    }

    /// <summary>
    /// 初始化一个普通C#对象池类型
    /// </summary>
    /// <param name="keyName">keyName</param>
    /// <param name="maxCapacity">容量，超出时会丢弃而不是进入对象池，-1代表无限</param>
    public static void InitObjectPool(string keyName, int maxCapacity = -1)
    {
        PoolSystem.InitObjectPool(keyName, maxCapacity);
    }

    /// <summary>
    /// 初始化对象池
    /// </summary>
    /// <param name="type">资源类型</param>
    /// <param name="maxCapacity">容量限制，超出时会销毁而不是进入对象池，-1代表无限</param>
    public static void InitObjectPool(Type type, int maxCapacity = -1)
    {
        PoolSystem.InitObjectPool(type, maxCapacity);
    }

    #endregion

    #region GameObject游戏对象

    /// <summary>
    /// 初始化一个GameObject类型的对象池类型
    /// </summary>  
    /// <param name="keyName">资源名称</param>
    /// <param name="assetName">AB资源名称</param>
    public static void InitGameObjectPoolForKeyName(string keyName, int maxCapacity = -1, string assetName = null,
        int defaultQuantity = 0)
    {
        if (defaultQuantity <= 0 || assetName == null)
        {
            PoolSystem.InitGameObjectPool(keyName, maxCapacity, null, 0);
        }
        else
        {
            GameObject[] gameObjects = new GameObject[defaultQuantity];
            for (int i = 0; i < defaultQuantity; i++)
            {
                int index = i;
                AssetHandle handle = YooAssets.LoadAssetAsync($"{assetPath}{assetName}");
                handle.Completed += obj =>
                {
                    if (obj.Status == EOperationStatus.Succeed)
                    {
                        gameObjects[index] = obj.AssetObject as GameObject;
                        if (gameObjects.All(go => go != null)) // 确保所有对象都已加载
                        {
                            // 所有加载完成，初始化对象池
                            PoolSystem.InitGameObjectPool(keyName, maxCapacity, gameObjects);
                        }
                    }
                    else
                    {
                        LogSystem.Error($"加载资源失败: {assetName}");
                        PoolSystem.InitGameObjectPool(keyName, maxCapacity, gameObjects);
                        return;
                    }
                };
            }
        }
    }

    /// <summary>
    /// 初始化对象池并设置容量
    /// </summary>
    /// <param name="assetName">AB资源名称</param>
    public static void InitGameObjectPoolForAssetName(string assetName, int maxCapacity = -1, int defaultQuantity = 0)
    {
        InitGameObjectPoolForKeyName(assetName, maxCapacity, assetName, defaultQuantity);
    }

    /// <summary>
    /// 游戏物体放置对象池中
    /// </summary>
    /// <param name="keyName">对象池中的key</param>
    /// <param name="gameObject">放入的物体</param>
    public static void PushGameObjectInPool(string keyName, GameObject gameObject)
    {
        PoolSystem.PushGameObject(keyName, gameObject);
    }

    /// <summary>
    /// 加载游戏物体
    /// 会自动检查对象池中是否包含，如果包含则返回对象池中的
    /// </summary>
    /// <param name="assetName">对象池中的Key</param>
    /// <param name="parent">父物体</param>
    /// <param name="autoRelease">物体销毁时，会自动去调用一次Addressables.Release</param>
    public static GameObject InstantiateGameObject(Transform parent, string assetName, bool autoRelease = true)
    {
        GameObject go = PoolSystem.GetGameObject(assetName, parent);
        if (go.IsNull() == false) return go;
        else
        {
            AssetHandle handle = YooAssets.LoadAssetSync($"{assetPath}{assetName}");
            go = handle.InstantiateSync();
            if (autoRelease)
            {
                go.transform.OnReleaseAsset(AutomaticReleaseAssetAction);
            }
        }

        return go;
    }

    /// <summary>
    /// 异步加载游戏物体
    /// 会自动检查对象池中是否包含，如果包含则返回对象池中的
    /// </summary>
    /// <param name="assetName">对象池中的分组名称</param>
    /// <param name="parent">父物体</param>
    /// <param name="callback">加载完成后的回调</param>
    /// <param name="autoRelease">物体销毁时，会自动去调用一次Addressables.Release</param>
    public static void InstantiateGameObjectAsync(Transform parent, string assetName, Action<GameObject> callback,
        bool autoRelease = true)
    {
        var go = PoolSystem.GetGameObject(assetName, parent);
        if (go.IsNull() == false) callback?.Invoke(go);
        else
        {
            AssetHandle handle = YooAssets.LoadAssetAsync($"{assetPath}{assetName}");
            handle.Completed += (obj) =>
            {
                go = obj.InstantiateSync(parent);
                if (autoRelease)
                {
                    go.transform.OnReleaseAsset(AutomaticReleaseAssetAction);
                }

                callback?.Invoke(go);
            };
        }
    }

    /// <summary>
    /// 自动释放资源事件，基于事件工具
    /// </summary>
    private static void AutomaticReleaseAssetAction(string assetName)
    {
        package.TryUnloadUnusedAsset(assetName);
    }

    #endregion

    #region 游戏资源Asset

    /// <summary>
    /// 加载Unity资源  如AudioClip Sprite 预制体
    /// </summary>
    /// <param name="assetName">AB资源名称</param>
    public static T LoadAsset<T>(string assetName) where T : UnityEngine.Object
    {
        AssetHandle handle = YooAssets.LoadAssetSync<T>($"{assetPath}{assetName}");
        if (handle?.AssetObject == null)
        {
            LogSystem.Error($"加载资源失败: {assetName}");
            return null;
        }
        return handle.AssetObject as T;
    }

    /// <summary>
    /// 异步加载Unity资源 AudioClip Sprite GameObject(预制体)
    /// </summary>
    /// <typeparam name="T">资源类型</typeparam>
    /// <param name="assetName">AB资源名称</param>
    /// <param name="callBack">回调函数</param>
    public static void LoadAssetAsync<T>(string assetName, Action<T> callBack) where T : UnityEngine.Object
    {
        AssetHandle handle = YooAssets.LoadAssetAsync<T>($"{assetPath}{assetName}");
        if (handle?.AssetObject == null)
        {
            LogSystem.Error($"加载资源失败: {assetName}");
            return;
        }
        handle.Completed += (obj) => { callBack?.Invoke(obj.AssetObject as T); };
    }

    public static Task<T> LoadAssetAsync<T>(string assetName) where T : UnityEngine.Object
    {
        var tcs = new TaskCompletionSource<T>();

        LoadAssetAsync<T>(assetName, obj =>
        {
            if (obj != null)
                tcs.SetResult(obj);
            else
                tcs.SetException(new Exception($"加载失败: {assetName}"));
        });
        return tcs.Task;
    }


    /// <summary>
    /// 释放资源
    /// </summary>
    /// <typeparam name="T">对象类型</typeparam>
    /// <param name="obj">具体对象</param>
    public static void UnloadAsset(AssetHandle handle)
    {
        handle.Release();
    }

    #endregion
}
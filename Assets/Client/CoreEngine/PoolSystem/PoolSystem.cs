using UnityEngine;

/// <summary>
/// 对象池系统
/// </summary>
public static class PoolSystem
{
    public const string PoolLayerGameObjectName = "PoolLayer";
    private static GameObjectPoolModule gameObjectPoolModule;
    private static ObjectPoolModule objectPoolModule;
    private static Transform poolRoot;

    public static void Init()
    {
        gameObjectPoolModule = new GameObjectPoolModule();
        objectPoolModule = new ObjectPoolModule();
        poolRoot = new GameObject("PoolRoot").transform;
        poolRoot.position = Vector3.zero;
        poolRoot.SetParent(CoreEngineRoot.RootTransform);
        gameObjectPoolModule.Init(poolRoot);
    }

    #region GameObjet对象池相关

    /// <summary>
    /// 初始化一个GameObject类型的对象池类型
    /// </summary>
    /// <param name="keyName">资源名称</param>
    /// <param name="maxCapacity">容量限制，超出时会销毁而不是进入对象池，-1代表无限</param>
    /// <param name="defaultQuantity">默认容量，填写会向池子中放入对应数量的对象，0代表不预先放入</param>
    /// <param name="prefab">填写默认容量时预先放入的对象</param>
    public static void InitGameObjectPool(string keyName, int maxCapacity = -1, GameObject prefab = null,
        int defaultQuantity = 0)
    {
        gameObjectPoolModule.InitObjectPool(keyName, maxCapacity, prefab, defaultQuantity);
    }

    /// <summary>
    /// 初始化对象池
    /// </summary>
    /// <param name="keyName"></param>
    /// <param name="maxCapacity">最大容量，-1代表无限</param>
    /// <param name="gameObjects">默认要放进来的对象数组</param>
    public static void InitGameObjectPool(string keyName, int maxCapacity, GameObject[] gameObjects = null)
    {
        gameObjectPoolModule.InitObjectPool(keyName, maxCapacity, gameObjects);
    }

    /// <summary>
    /// 初始化对象池并设置容量
    /// </summary>
    /// <param name="maxCapacity">容量限制，超出时会销毁而不是进入对象池，-1代表无限</param>
    /// <param name="defaultQuantity">默认容量，填写会向池子中放入对应数量的对象，0代表不预先放入</param>
    /// <param name="prefab">填写默认容量时预先放入的对象</param>
    public static void InitGameObjectPool(GameObject prefab, int maxCapacity = -1, int defaultQuantity = 0)
    {
        InitGameObjectPool(prefab.name, maxCapacity, prefab, defaultQuantity);
    }

    /// <summary>
    /// 获取GameObject，没有则返回Null
    /// </summary>
    public static GameObject GetGameObject(string keyName, Transform parent = null)
    {
        GameObject go = gameObjectPoolModule.GetObject(keyName, parent);
        return go;
    }

    /// <summary>
    /// 获取GameObject，没有则返回Null
    /// T:组件
    /// </summary>
    public static T GetGameObject<T>(string keyName, Transform parent = null) where T : Component
    {
        GameObject go = GetGameObject(keyName, parent);
        if (go != null) return go.GetComponent<T>();
        else return null;
    }

    /// <summary>
    /// 游戏物体放置对象池中
    /// </summary>
    /// <param name="keyName">对象池中的key</param>
    /// <param name="obj">放入的物体</param>
    public static bool PushGameObject(string keyName, GameObject obj)
    {
        if (!obj.IsNull())
        {
            bool res = gameObjectPoolModule.PushObject(keyName, obj);
            return res;
        }
        else
        {
            LogSystem.Error("您正在将Null放置对象池");
            return false;
        }
    }

    /// <summary>
    /// 游戏物体放置对象池中
    /// </summary>
    /// <param name="obj">放入的物体,Key默认为物体名称</param>
    public static bool PushGameObject(GameObject obj)
    {
        return PushGameObject(obj.name, obj);
    }

    /// <summary>
    /// 清除某个游戏物体在对象池中的所有数据
    /// </summary>
    public static void ClearGameObject(string keyName)
    {
        gameObjectPoolModule.Clear(keyName);
    }

    #endregion

    #region Object对象池相关

    /// <summary>
    /// 初始化对象池并设置容量
    /// </summary>
    /// <param name="keyName">资源名称</param>
    /// <param name="maxCapacity">容量限制，超出时会销毁而不是进入对象池，-1代表无限</param>
    /// <param name="defaultQuantity">默认容量，填写会向池子中放入对应数量的对象，0代表不预先放入</param>
    public static void InitObjectPool<T>(string keyName, int maxCapacity = -1, int defaultQuantity = 0) where T : new()
    {
        objectPoolModule.InitObjectPool<T>(keyName, maxCapacity, defaultQuantity);
    }

    /// <summary>
    /// 初始化对象池并设置容量
    /// </summary>
    /// <param name="maxCapacity">容量限制，超出时会销毁，-1代表无限</param>
    /// <param name="defaultQuantity">默认容量，填写会预先向池子中放入对应数量的对象，0代表不预先放入</param>
    public static void InitObjectPool<T>(int maxCapacity = -1, int defaultQuantity = 0) where T : new()
    {
        InitObjectPool<T>(typeof(T).FullName, maxCapacity, defaultQuantity);
    }

    /// <summary>
    /// 初始化一个普通C#对象池类型
    /// </summary>
    /// <param name="keyName">keyName</param>
    /// <param name="maxCapacity">容量，超出时会丢弃，-1代表无限</param>
    public static void InitObjectPool(string keyName, int maxCapacity = -1)
    {
        objectPoolModule.InitObjectPool(keyName, maxCapacity);
    }

    /// <summary>
    /// 初始化对象池
    /// </summary>
    /// <param name="type">资源类型</param>
    /// <param name="maxCapacity">容量限制，超出时会销毁而不是进入对象池，-1代表无限</param>
    public static void InitObjectPool(System.Type type, int maxCapacity = -1)
    {
        objectPoolModule.InitObjectPool(type, maxCapacity);
    }

    /// <summary>
    /// 获取普通对象（非GameObject）
    /// </summary>
    public static T GetObject<T>() where T : class
    {
        return GetObject<T>(typeof(T).FullName);
    }

    public static T GetObject<T>(string keyName) where T : class
    {
        object obj = GetObject(keyName);
        if (obj == null) return null;
        else return (T)obj;
    }

    public static object GetObject(System.Type type)
    {
        return GetObject(type.FullName);
    }

    public static object GetObject(string keyName)
    {
        object obj = objectPoolModule.GetObject(keyName);
        return obj;
    }

    /// <summary>
    /// 普通对象（非GameObject）放置对象池中
    /// 基于类型存放
    /// </summary>
    public static bool PushObject(object obj)
    {
        return PushObject(obj, obj.GetType().FullName);
    }

    /// <summary>
    /// 普通对象（非GameObject）放置对象池中
    /// 基于KeyName存放
    /// </summary>
    public static bool PushObject(object obj, string keyName)
    {
        if (obj == null)
        {
            LogSystem.Error("您正在将Null放置对象池");
            return false;
        }
        else
        {
            bool res = objectPoolModule.PushObject(obj, keyName);
            return res;
        }
    }

    /// <summary>
    /// 清理某个C#类型数据
    /// </summary>
    public static void ClearObject<T>()
    {
        ClearObject(typeof(T).FullName);
    }

    public static void ClearObject(System.Type type)
    {
        ClearObject(type.FullName);
    }

    public static void ClearObject(string keyName)
    {
        objectPoolModule.ClearObject(keyName);
    }

    #endregion

    #region 对象池通用

    /// <summary>
    /// 清除全部
    /// </summary>
    public static void ClearAll(bool clearGameObject = true, bool clearCSharpObject = true)
    {
        if (clearGameObject)
        {
            gameObjectPoolModule.ClearAll();
        }
        if (clearCSharpObject)
        {
            objectPoolModule.ClearAll();
        }
    }

    #endregion
}
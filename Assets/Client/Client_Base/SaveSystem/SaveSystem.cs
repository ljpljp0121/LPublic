using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using UnityEngine;

/// <summary>
/// 单个存档的数据
/// </summary>
[Serializable]
public class SaveItem
{
    public int SaveID;
    private DateTime lastSaveTime;
    public DateTime LastSaveTime
    {
        get
        {
            if (lastSaveTime == default(DateTime))
            {
                DateTime.TryParse(LastSaveTimeString, out lastSaveTime);
            }
            return lastSaveTime;
        }
    }
    [SerializeField] private string LastSaveTimeString; // Json不支持DateTime，用来持久化的

    public SaveItem(int saveID, DateTime lastSaveTime)
    {
        this.SaveID = saveID;
        this.lastSaveTime = lastSaveTime;
        LastSaveTimeString = lastSaveTime.ToString(CultureInfo.InvariantCulture);
    }

    public void UpdateTime(DateTime lastSaveTime)
    {
        this.lastSaveTime = lastSaveTime;
        LastSaveTimeString = lastSaveTime.ToString(CultureInfo.InvariantCulture);
    }
}

public interface IBinarySerializer
{
    public byte[] Serialize<T>(T obj) where T : class;
    public T Deserialize<T>(byte[] bytes) where T : class;
}

/// <summary>
/// 存档系统
/// </summary>
public static class SaveSystem
{
    private static IBinarySerializer binarySerializer;

    #region 存档系统、存档系统数据类及所有用户存档、设置存档数据

    /// <summary>
    /// 存档系统数据类
    /// </summary>
    [Serializable]
    private class SaveSystemData
    {
        // 当前的存档ID
        public int currID = 0;
        // 所有存档的列表
        public List<SaveItem> saveItemList = new List<SaveItem>();
    }

    private static SaveSystemData saveSystemData;

    // 存档的保存
    private const string saveDirName = "saveData";
    // 设置的保存：1.全局数据的保存（分辨率、按键设置） 2.存档的设置保存。
    // 常规情况下，存档系统自行维护
    private const string settingDirName = "setting";

    // 存档文件夹路径
    private static string saveDirPath;
    private static string settingDirPath;

    // 存档中对象的缓存字典 
    // <存档ID,<文件名称，实际的对象>>
    private static Dictionary<int, Dictionary<string, object>> cacheDic =
        new Dictionary<int, Dictionary<string, object>>();

    [InitOnLoad]
    public static void Init()
    {
        // binarySerializer =
        saveDirPath = Application.persistentDataPath + "/" + saveDirName;
        settingDirPath = Application.persistentDataPath + "/" + settingDirName;
#if UNITY_EDITOR
        if (!UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode)
        {
            return;
        }
#endif
        CheckAndCreateDir();
        // 初始化SaveSystemData
        InitSaveSystemData();
        // 避免Editor环境下使用了上一次运行的缓存
        ClearCache();
    }

    #endregion

    #region 获取、删除所有用户存档

    /// <summary>
    /// 获取所有存档
    /// 按创建时间升序
    /// </summary>
    public static List<SaveItem> GetAllSaveItem()
    {
        return saveSystemData.saveItemList;
    }

    /// <summary>
    /// 获取所有存档
    /// 按创建时间降序
    /// </summary>
    public static List<SaveItem> GetAllSaveItemByCreatTime()
    {
        List<SaveItem> saveItems = new List<SaveItem>(saveSystemData.saveItemList);
        saveItems.Reverse();
        return saveItems;
    }

    /// <summary>
    /// 获取所有存档
    /// 按更新时间降序
    /// </summary>
    public static List<SaveItem> GetAllSaveItemByUpdateTime()
    {
        List<SaveItem> saveItems = new List<SaveItem>(saveSystemData.saveItemList);
        OrderByUpdateTimeComparer orderBy = new OrderByUpdateTimeComparer();
        saveItems.Sort(orderBy);
        return saveItems;
    }

    private class OrderByUpdateTimeComparer : IComparer<SaveItem>
    {
        public int Compare(SaveItem x, SaveItem y)
        {
            return y.LastSaveTime.CompareTo(x.LastSaveTime);
        }
    }

    /// <summary>
    /// 获取所有存档
    /// </summary>
    public static List<SaveItem> GetAllSaveItem<T>(Func<SaveItem, T> orderFunc, bool isDescending = false)
    {
        return isDescending
            ? saveSystemData.saveItemList.OrderByDescending(orderFunc).ToList()
            : saveSystemData.saveItemList.OrderBy(orderFunc).ToList();
    }

    public static void DeleteAllSaveItem()
    {
        if (Directory.Exists(saveDirPath))
        {
            // 直接删除目录
            Directory.Delete(saveDirPath, true);
        }
        CheckAndCreateDir();
        InitSaveSystemData();
    }

    public static void DeleteAll()
    {
        ClearCache();
        DeleteAllSaveItem();
        DeleteAllSetting();
    }

    #endregion

    #region 创建、获取、删除某一项用户存档

    /// <summary>
    /// 获取SaveItem
    /// </summary>
    public static SaveItem GetSaveItem(int id)
    {
        for (int i = 0; i < saveSystemData.saveItemList.Count; i++)
        {
            if (saveSystemData.saveItemList[i].SaveID == id)
            {
                return saveSystemData.saveItemList[i];
            }
        }
        return null;
    }

    /// <summary>
    /// 获取SaveItem
    /// </summary>
    public static SaveItem GetSaveItem(SaveItem saveItem)
    {
        return GetSaveItem(saveItem.SaveID);
    }

    /// <summary>
    /// 添加一个存档
    /// </summary>
    /// <returns></returns>
    public static SaveItem CreateSaveItem()
    {
        SaveItem saveItem = new SaveItem(saveSystemData.currID, DateTime.Now);
        saveSystemData.saveItemList.Add(saveItem);
        saveSystemData.currID += 1;
        // 更新SaveSystemData 写入磁盘
        UpdateSaveSystemData();
        return saveItem;
    }

    /// <summary>
    /// 删除存档
    /// </summary>
    /// <param name="saveID">存档的ID</param>
    public static void DeleteSaveItem(int saveID)
    {
        if (!saveSystemData.saveItemList.Exists(x => x.SaveID == saveID))
            throw new ArgumentException($"存档ID {saveID} 不存在");
        string itemDir = GetSavePath(saveID, false);
        // 如果路径存在 且 有效
        if (itemDir != null)
        {
            // 把这个存档下的文件递归删除
            Directory.Delete(itemDir, true);
        }
        saveSystemData.saveItemList.Remove(GetSaveItem(saveID));
        // 移除缓存
        RemoveCache(saveID);
        // 更新SaveSystemData 写入磁盘
        UpdateSaveSystemData();
    }

    /// <summary>
    /// 删除存档
    /// </summary>
    public static void DeleteSaveItem(SaveItem saveItem)
    {
        DeleteSaveItem(saveItem.SaveID);
    }

    #endregion

    #region 更新、获取、删除用户存档缓存

    /// <summary>
    /// 设置缓存
    /// </summary>
    /// <param name="saveID">存档ID</param>
    /// <param name="fileName">文件名称</param>
    /// <param name="saveObject">要缓存的对象</param>
    private static void SetCache(int saveID, string fileName, object saveObject)
    {
        if (cacheDic.ContainsKey(saveID))
        {
            if (cacheDic[saveID].ContainsKey(fileName))
            {
                cacheDic[saveID][fileName] = saveObject;
            }
            else
            {
                cacheDic[saveID].Add(fileName, saveObject);
            }
        }
        else
        {
            cacheDic.Add(saveID, new Dictionary<string, object>() { { fileName, saveObject } });
        }
    }

    /// <summary>
    /// 获取缓存
    /// </summary>
    /// <param name="saveID">存档ID</param>
    /// <param name="fileName">文件名</param>
    private static T GetCache<T>(int saveID, string fileName) where T : class
    {
        // 缓存字典中是否有这个SaveID
        if (cacheDic.ContainsKey(saveID))
        {
            // 这个存档中有没有这个文件
            if (cacheDic[saveID].ContainsKey(fileName))
            {
                return cacheDic[saveID][fileName] as T;
            }
            else
            {
                return null;
            }
        }
        else
        {
            return null;
        }
    }

    /// <summary>
    /// 移除缓存
    /// </summary>
    private static void RemoveCache(int saveID)
    {
        cacheDic.Remove(saveID);
    }

    /// <summary>
    /// 移除缓存中的某一个对象
    /// </summary>
    private static void RemoveCache(int saveID, string fileName)
    {
        cacheDic[saveID].Remove(fileName);
    }

    public static void ClearCache()
    {
        cacheDic.Clear();
    }

    #endregion

    #region 保存、获取、删除用户存档中某一对象

    /// <summary>
    /// 保存对象至某个存档中
    /// </summary>
    /// <param name="saveObject">要保存的对象</param>
    /// <param name="saveFileName">保存的文件名称</param>
    /// <param name="saveID">存档的ID</param>
    public static void SaveObject(object saveObject, string saveFileName, int saveID = 0)
    {
        // 存档所在的文件夹路径
        string dirPath = GetSavePath(saveID, true);
        // 具体的对象要保存的路径
        string savePath = dirPath + "/" + saveFileName;
        // 具体的保存
        SaveFile(saveObject, savePath);
        // 更新存档时间
        GetSaveItem(saveID).UpdateTime(DateTime.Now);
        // 更新SaveSystemData 写入磁盘
        UpdateSaveSystemData();

        // 更新缓存
        SetCache(saveID, saveFileName, saveObject);
    }

    public static void SaveObject(object saveObject, string saveFileName, SaveItem saveItem)
    {
        SaveObject(saveObject, saveFileName, saveItem.SaveID);
    }

    public static void SaveObject(object saveObject, int saveID = 0)
    {
        SaveObject(saveObject, saveObject.GetType().Name, saveID);
    }

    public static void SaveObject(object saveObject, SaveItem saveItem)
    {
        SaveObject(saveObject, saveObject.GetType().Name, saveItem);
    }

    /// <summary>
    /// 从某个具体的存档中加载某个可迁移对象
    /// </summary>
    /// <typeparam name="T">要返回的实际类型</typeparam>
    /// <param name="saveFileName">文件名称</param>
    /// <param name="saveID">存档ID</param>
    public static T LoadMigratableObject<T>(string saveFileName, int saveID = 0) where T : class, IMigratable
    {
        T obj = GetCache<T>(saveID, saveFileName);
        if (obj == null)
        {
            // 存档所在的文件夹路径
            string dirPath = GetSavePath(saveID);
            if (dirPath == null) return null;
            // 具体的对象要保存的路径
            string savePath = dirPath + "/" + saveFileName;
            obj = LoadFile<T>(savePath);
            if (obj != null)
            {
                bool migrationSuccess = MigrationManager.TryMigrate(obj);
                if (migrationSuccess)
                {
                    SaveFile(obj, savePath);
                }
            }
            SetCache(saveID, saveFileName, obj);
        }
        return obj;
    }

    public static T LoadMigratableObject<T>(string saveFileName, SaveItem saveItem) where T : class, IMigratable
    {
        return LoadMigratableObject<T>(saveFileName, saveItem.SaveID);
    }

    public static T LoadMigratableObject<T>(int saveID = 0) where T : class, IMigratable
    {
        return LoadMigratableObject<T>(typeof(T).Name, saveID);
    }

    public static T LoadMigratableObject<T>(SaveItem saveItem) where T : class, IMigratable
    {
        return LoadMigratableObject<T>(typeof(T).Name, saveItem.SaveID);
    }

    /// <summary>
    /// 从某个具体的存档中加载某个对象
    /// </summary>
    /// <typeparam name="T">要返回的实际类型</typeparam>
    /// <param name="saveFileName">文件名称</param>
    /// <param name="saveID">存档ID</param>
    public static T LoadObject<T>(string saveFileName, int saveID = 0) where T : class
    {
        T obj = GetCache<T>(saveID, saveFileName);
        if (obj == null)
        {
            // 存档所在的文件夹路径
            string dirPath = GetSavePath(saveID);
            if (dirPath == null) return null;
            // 具体的对象要保存的路径
            string savePath = dirPath + "/" + saveFileName;
            obj = LoadFile<T>(savePath);
            SetCache(saveID, saveFileName, obj);
        }
        return obj;
    }

    public static T LoadObject<T>(string saveFileName, SaveItem saveItem) where T : class
    {
        return LoadObject<T>(saveFileName, saveItem.SaveID);
    }

    public static T LoadObject<T>(int saveID = 0) where T : class
    {
        return LoadObject<T>(typeof(T).Name, saveID);
    }

    public static T LoadObject<T>(SaveItem saveItem) where T : class
    {
        return LoadObject<T>(typeof(T).Name, saveItem.SaveID);
    }

    /// <summary>
    /// 删除某个存档中的某个对象
    /// </summary>
    /// <param name="saveFileName">文件名称</param>
    /// <param name="saveID">存档的ID</param>
    public static void DeleteObject<T>(string saveFileName, int saveID) where T : class
    {
        //清空缓存中对象
        if (GetCache<T>(saveID, saveFileName) != null)
        {
            RemoveCache(saveID, saveFileName);
        }
        // 存档对象所在的文件路径
        string dirPath = GetSavePath(saveID);
        string savePath = dirPath + "/" + saveFileName;
        //删除对应的文件
        File.Delete(savePath);
    }

    public static void DeleteObject<T>(string saveFileName, SaveItem saveItem) where T : class
    {
        DeleteObject<T>(saveFileName, saveItem.SaveID);
    }

    public static void DeleteObject<T>(int saveID) where T : class
    {
        DeleteObject<T>(typeof(T).Name, saveID);
    }

    public static void DeleteObject<T>(SaveItem saveItem) where T : class
    {
        DeleteObject<T>(typeof(T).Name, saveItem.SaveID);
    }

    #endregion

    #region 保存、获取全局设置存档

    /// <summary>
    /// 加载设置，全局生效，不关乎任何一个存档
    /// </summary>
    public static T LoadSetting<T>(string fileName) where T : class
    {
        return LoadFile<T>(settingDirPath + "/" + fileName);
    }

    public static T LoadSetting<T>() where T : class
    {
        return LoadSetting<T>(typeof(T).Name);
    }

    /// <summary>
    /// 保存设置，全局生效，不关乎任何一个存档
    /// </summary>
    public static void SaveSetting(object saveObject, string fileName)
    {
        SaveFile(saveObject, settingDirPath + "/" + fileName);
    }

    public static void SaveSetting(object saveObject)
    {
        SaveSetting(saveObject, saveObject.GetType().Name);
    }

    public static void DeleteAllSetting()
    {
        if (Directory.Exists(settingDirPath))
        {
            Directory.Delete(settingDirPath, true);
        }
        CheckAndCreateDir();
    }

    #endregion

    #region 内部工具函数

    /// <summary>
    /// 获取存档系统数据
    /// </summary>
    /// <returns></returns>
    private static void InitSaveSystemData()
    {
        saveSystemData = LoadFile<SaveSystemData>(saveDirPath + "/SaveSystemData");
        if (saveSystemData == null)
        {
            saveSystemData = new SaveSystemData();
            UpdateSaveSystemData();
        }
    }

    /// <summary>
    /// 更新存档系统数据
    /// </summary>
    private static void UpdateSaveSystemData()
    {
        SaveFile(saveSystemData, saveDirPath + "/SaveSystemData");
    }

    /// <summary>
    /// 检查路径并创建目录
    /// </summary>
    private static void CheckAndCreateDir()
    {
        Debug.Log("本地存档路径：" + saveDirPath);
        // 确保路径的存在
        if (Directory.Exists(saveDirPath) == false)
        {
            Directory.CreateDirectory(saveDirPath);
        }
        if (Directory.Exists(settingDirPath) == false)
        {
            Directory.CreateDirectory(settingDirPath);
        }
    }

    /// <summary>
    /// 获取某个存档的路径
    /// </summary>
    /// <param name="saveID">存档ID</param>
    /// <param name="createDir">如果不存在这个路径，是否需要创建</param>
    /// <returns></returns>
    private static string GetSavePath(int saveID, bool createDir = true)
    {
        // 验证是否有某个存档
        if (GetSaveItem(saveID) == null) throw new Exception("SaveID 存档不存在！");

        string saveDir = saveDirPath + "/" + saveID;
        // 确定文件夹是否存在
        if (Directory.Exists(saveDir) == false)
        {
            if (createDir)
            {
                Directory.CreateDirectory(saveDir);
            }
            else
            {
                return null;
            }
        }

        return saveDir;
    }

    /// <summary>
    /// 保存文件
    /// </summary>
    /// <param name="saveObject">保存的对象</param>
    /// <param name="path">保存的路径</param>
    private static void SaveFile(object saveObject, string path)
    {
        // if (saveObject is IMigratable migratable)
        // {
        //     Type type = saveObject.GetType();
        //     int currentVersion = MigrationManager.GetCurrentVersion(type);
        //     migratable.DataVersion = currentVersion;
        // }

        switch (CoreEngineRoot.Instance.SaveSystemType)
        {
            case SaveSystemType.Binary:
                if (binarySerializer == null || saveObject.GetType() == typeof(SaveSystemData))
                    IOTool.SaveFile(saveObject, path);
                else
                {
                    byte[] bytes = binarySerializer.Serialize(saveObject);
                    File.WriteAllBytes(path, bytes);
                }
                break;
            case SaveSystemType.Json:
                IOTool.SaveJson(saveObject, path);
                break;
        }
    }

    /// <summary>
    /// 加载文件
    /// </summary>
    /// <typeparam name="T">加载后要转为的类型</typeparam>
    /// <param name="path">加载路径</param>
    private static T LoadFile<T>(string path) where T : class
    {
        switch (CoreEngineRoot.Instance.SaveSystemType)
        {
            case SaveSystemType.Binary:
                if (binarySerializer == null || typeof(T) == typeof(SaveSystemData)) return IOTool.LoadFile<T>(path);
                else
                {
                    if (!File.Exists(path))
                    {
                        return null;
                    }
                    FileStream file = new FileStream(path, FileMode.Open);
                    byte[] bytes = new byte[file.Length];
                    file.Read(bytes, 0, bytes.Length);
                    file.Close();
                    return binarySerializer.Deserialize<T>(bytes);
                }
            case SaveSystemType.Json:
                return IOTool.LoadJson<T>(path);
        }
        return null;
    }

    #endregion
}
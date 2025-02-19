using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine;

/// <summary>
/// 本地存储工具
/// </summary>
public static class IOTool
{
    private static BinaryFormatter binaryFormatter = new BinaryFormatter();

    /// <summary>
    /// 保存Json
    /// </summary>
    /// <param name="saveObj">保存的类</param>
    /// <param name="path">路径</param>
    public static void SaveJson(object saveObj, string path)
    {
        string jsonData = JsonUtility.ToJson(saveObj);
        string encryptedData = EncryptionUtility.Encrypt(jsonData);
        File.WriteAllText(path, encryptedData);
    }

    /// <summary>
    /// 读取Json为指定的类型对象
    /// </summary>
    public static T LoadJson<T>(string path) where T : class
    {
        if (!File.Exists(path))
        {
            return null;
        }
        string encryptedData = File.ReadAllText(path);
        string jsonData = EncryptionUtility.Decrypt(encryptedData);
        return JsonUtility.FromJson<T>(jsonData);
    }

    /// <summary>
    /// 保存文件
    /// </summary>
    /// <param name="saveObject">保存对象</param>
    /// <param name="path">保存路径</param>
    public static void SaveFile(object saveObject, string path)
    {
        FileStream f = new FileStream(path, FileMode.OpenOrCreate);
        // 二进制的方式把对象写进文件
        binaryFormatter.Serialize(f, saveObject);
        f.Dispose();
    }

    /// <summary>
    /// 加载文件
    /// </summary>
    /// <typeparam name="T">加载类型</typeparam>
    /// <param name="path">加载路径</param>
    public static T LoadFile<T>(string path) where T : class
    {
        if (!File.Exists(path))
        {
            return null;
        }
        FileStream file = new FileStream(path, FileMode.Open);
        T obj = (T)binaryFormatter.Deserialize(file);
        file.Dispose();
        return obj;
    }
}
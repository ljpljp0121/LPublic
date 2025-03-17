#if UNITY_EDITOR
using System;
using System.IO;
using System.Linq;
using UnityEditorInternal;
using UnityEngine;

/// <summary>
/// 基于ScriptableObject实现的泛型单例基类
/// 用于创建可持久化保存的全局数据容器
/// </summary>
public class ScriptableSingleton<T> : ScriptableObject where T : ScriptableObject
{
    private static T instance;
    public static T Instance
    {
        get
        {
            if (!instance)
            {

            }

            return instance;
        }
    }

    /// <summary>
    /// 加载或创建持久化数据
    /// </summary>
    public static T LoadOrCreate()
    {
        string filePath = GetFilePath();
        if (!string.IsNullOrEmpty(filePath))
        {
            var arr = InternalEditorUtility.LoadSerializedFileAndForget(filePath);
            instance = arr.Length > 0 ? arr[0] as T : instance ?? CreateInstance<T>();
        }
        else
        {
            LogSystem.Error($"保存位置 {nameof(ScriptableSingleton<T>)} 无效!!!");
        }
        return instance;
    }

    /// <summary>
    /// 持久化保存数据到文件
    /// </summary>
    public static void Save(bool saveAsText = true)
    {
        if (!instance)
        {
            LogSystem.Error("Cannot save ScriptableSingleton: no instance!");
            return;
        }

        string filePath = GetFilePath();
        if (!string.IsNullOrEmpty(filePath))
        {
            string directoryName = Path.GetDirectoryName(filePath);
            if (directoryName != null && !Directory.Exists(directoryName))
            {
                Directory.CreateDirectory(directoryName);
            }

            UnityEngine.Object[] obj = { instance };
            InternalEditorUtility.SaveToSerializedFileAndForget(obj, filePath, saveAsText);
        }
    }

    /// <summary>
    /// 动态更新实例（用于热重载或数据重置）
    /// </summary>
    public static void UpdateAsset(T asset)
    {
        if (asset == null) return;
        instance = asset;
    }

    protected static string GetFilePath()
    {
        return typeof(T).GetCustomAttributes(inherit: true)
            .Where(v => v is FilePathAttribute)
            .Cast<FilePathAttribute>()
            .FirstOrDefault()
            ?.filePath;
    }
}

[AttributeUsage(AttributeTargets.Class)]
public class FilePathAttribute : Attribute
{
    public string filePath;

    public FilePathAttribute(string path)
    {
        if (string.IsNullOrEmpty(path))
        {
            throw new ArgumentException("Invalid relative path (it is empty)");
        }
        if (path[0] == '/')
        {
            path = path.Substring(1);
        }
        filePath = path;
    }
}

#endif
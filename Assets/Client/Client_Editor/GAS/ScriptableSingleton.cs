#if UNITY_EDITOR
using System;
using System.IO;
using System.Linq;
using UnityEditorInternal;
using UnityEngine;

/// <summary>
/// ����ScriptableObjectʵ�ֵķ��͵�������
/// ���ڴ����ɳ־û������ȫ����������
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
    /// ���ػ򴴽��־û�����
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
            LogSystem.Error($"����λ�� {nameof(ScriptableSingleton<T>)} ��Ч!!!");
        }
        return instance;
    }

    /// <summary>
    /// �־û��������ݵ��ļ�
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
    /// ��̬����ʵ�������������ػ��������ã�
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
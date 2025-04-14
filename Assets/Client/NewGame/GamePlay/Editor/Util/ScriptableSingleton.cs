#if UNITY_EDITOR
namespace Game.Editor
{
    using System;
    using System.IO;
    using System.Linq;
    using UnityEditorInternal;
    using UnityEngine;

    public class ScriptableSingleton<T> : ScriptableObject where T : ScriptableObject
    {
        private static T instance;
        public static T Instance
        {
            get
            {
                if (!instance)
                {
                    LoadOrCreate();
                }
                return instance;
            }
        }

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
                Debug.LogError($" {nameof(ScriptableSingleton<T>)} 的保存地址无效");
            }
            return instance;
        }

        public static void Save(bool saveAsText = true)
        {
            if (!instance)
            {
                Debug.LogError("保存失败,未获取到单例");
                return;
            }
            string filePath = GetFilePath();
            if (!string.IsNullOrEmpty(filePath))
            {
                string directoryName = Path.GetDirectoryName(filePath);
                if (!Directory.Exists(directoryName))
                {
                    Directory.CreateDirectory(directoryName);
                }
                UnityEngine.Object[] obj = { instance };
                InternalEditorUtility.SaveToSerializedFileAndForget(obj, filePath, saveAsText);
            }
            else
            {
                Debug.LogError($" {nameof(ScriptableSingleton<T>)} 的保存地址无效");
            }
        }

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
                ?.filepath;
        }
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class FilePathAttribute : Attribute
    {
        internal string filepath;
        
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
            filepath = path;
        }
    }
}
#endif

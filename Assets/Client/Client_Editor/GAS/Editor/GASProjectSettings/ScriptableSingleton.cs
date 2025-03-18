#if UNITY_EDITOR
namespace GAS.Editor
{
    using System;
    using System.IO;
    using System.Linq;
    using UnityEditor;
    using UnityEditorInternal;
    using UnityEngine;
    
    /// <summary>
    /// 单例SO,适用于全局SO配置
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
                    LoadOrCreate();
                }
                return instance;
            }
        }
        /// <summary>
        /// 懒加载全局配置
        /// </summary>
        /// <returns></returns>
        public static T LoadOrCreate()
        {
            string filePath = GetFilePath();
            if (!string.IsNullOrEmpty(filePath))
            {
                var arr = InternalEditorUtility.LoadSerializedFileAndForget(filePath);
                instance = arr.Length > 0 ? arr[0] as T : instance??CreateInstance<T>();
            }
            else
            {
                Debug.LogError($"save location of {nameof(ScriptableSingleton<T>)} is invalid");
            }
            return instance;
        }

        /// <summary>
        /// 保存全局配置
        /// </summary>
        /// <param name="saveAsText">是否保存为文本</param>
        public static void Save(bool saveAsText = true)
        {
            if (!instance)
            {
                Debug.LogError("Cannot save ScriptableSingleton: no instance!");
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
                //Debug.Log($"Saved ScriptableSingleton to {filePath}");
            }
        }

        /// <summary>
        /// 更新全局配置
        /// </summary>
        public static void UpdateAsset(T asset)
        {
            if (asset == null) return;
            instance = asset;
        }
        
        /// <summary>
        /// 获取路径
        /// </summary>
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
        /// <summary>
        /// 单例存放路径
        /// </summary>
        /// <param name="path">相对 Project 路径</param>
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
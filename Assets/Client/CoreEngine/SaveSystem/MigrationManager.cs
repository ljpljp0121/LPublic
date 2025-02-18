
using System.Collections.Generic;
using System;
using System.Diagnostics;

/// <summary>
/// 迁移管理器
/// </summary>
public static class MigrationManager
{
    private static Dictionary<Type, Dictionary<int, Action<object>>> migrations = new();

    /// <summary>
    /// 注册迁移方法
    /// </summary>
    /// <typeparam name="T">存储数据类型</typeparam>
    /// <param name="fromVersion">数据版本</param>
    /// <param name="migration">迁移方法</param>
    public static void RegisterMigration<T>(int fromVersion, Action<T> migration) where T : class
    {
        Type type = typeof(T);
        if (!migrations.ContainsKey(type))
        {
            migrations[type] = new Dictionary<int, Action<object>>();
        }
        migrations[type][fromVersion] = obj => migration(obj as T);
    }

    /// <summary>
    /// 尝试迁移
    /// </summary>
    /// <typeparam name="T">存储数据类型</typeparam>
    /// <param name="obj">数据</param>
    /// <returns></returns>
    public static bool TryMigrate<T>(T obj) where T : class, IMigratable
    {
        Type type = typeof(T);
        if (!migrations.ContainsKey(type))
        {
            return false;
        }

        int loadedVersion = obj.DataVersion;
        int currentVersion = GetCurrentVersion(type);

        while (loadedVersion < currentVersion)
        {
            if (migrations[type].TryGetValue(loadedVersion, out var migration))
            {
                migration(obj);
                loadedVersion++;
                obj.DataVersion = loadedVersion;
            }
            else
            {
                LogSystem.Error($"没有找到{type}类型的对于版本{loadedVersion}的迁移方法");
                return false;
            }
        }
        return true;
    }

    public static int GetCurrentVersion(Type type)
    {
        var field = type.GetField("CurrentVersion",
            System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
        return (field != null) ? (int)field.GetValue(null) : -1;
    }
}


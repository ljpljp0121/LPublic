using System;
using UnityEngine;

/// <summary>
/// 所有支持版本迁移的存储类都需要实现这个接口
/// </summary>
public interface IMigratable
{
    int DataVersion { get; set; }
}


/// <summary>
/// 演示存储类
/// </summary>
[Serializable]
public class PlayerData : IMigratable
{
    public static int CurrentVersion = 2; // 当前版本号
    [SerializeField] private int dataVersion = CurrentVersion; // 数据版本号
    public string playerName;
    public int playerLevel;

    public int DataVersion
    {
        get => dataVersion;
        set => dataVersion = value;
    }
}



using System;

using UnityEngine;

/// <summary>
/// 所有支持版本迁移的存储类都需要实现这个接口
/// </summary>
public interface IMigratable
{
    const int CurrentVersion = 1; 
    int DataVersion { get; set; }
}


/// <summary>
/// 演示存储类
/// </summary>
[Serializable]
public class PlayerData : IMigratable
{
    public const int CurrentVersion = 2; // 当前版本号
    [SerializeField] private int dataVersion = CurrentVersion; // 数据版本号
    public string playerName;
    public int playerLevel;

    public int DataVersion
    {
        get => dataVersion;
        set => dataVersion = value;
    }

    // 构造函数或迁移方法处理旧数据
    public PlayerData()
    {
        // 初始化默认值
    }
}
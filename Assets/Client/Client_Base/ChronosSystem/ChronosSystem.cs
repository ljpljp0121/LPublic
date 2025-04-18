using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

/// <summary>
/// 全局时间刻度管理器
/// </summary>
public class ChronosSystem : SingletonMono<ChronosSystem>
{
    [InitOnLoad]
    private static void Init()
    {
        Instance.InitTimeScales(new TimeScaleGroup[]
        {
            new TimeScaleGroup("GlobalTimeGroup",1,false),
        });
    }

    [ShowInInspector]
    public Dictionary<string, TimeScaleGroup> TimeScalesByName { get; } = new Dictionary<string, TimeScaleGroup>();

    /// <summary>
    /// 获取时间刻度组,通过名称,如果不存在,会根据给的参数选择创建一个新的时间刻度组
    /// </summary>
    public TimeScaleGroup GetTimeScaleGroup(string timeScaleName, float initScaleValue = 1f,
        bool autoRemoveWhenEmpty = true, bool createIfNull = true)
    {
        if (TimeScalesByName.TryGetValue(timeScaleName, out var timeScale))
            return timeScale;

        if (!createIfNull) return null;

        timeScale = new TimeScaleGroup(timeScaleName, initScaleValue, autoRemoveWhenEmpty);
        TimeScalesByName.TryAdd(timeScaleName, timeScale);
        return timeScale;
    }

    /// <summary>
    /// 添加时间刻度组,如果已经存在,则返回false
    /// </summary>
    public bool AddTimeScaleGroup(TimeScaleGroup timeScaleGroup)
    {
        if (TimeScalesByName.TryGetValue(timeScaleGroup.Name, out var timeScale))
        {
            return false;
        }
        TimeScalesByName.Add(timeScaleGroup.Name, timeScaleGroup);
        return true;
    }

    /// <summary>
    /// 移除指定名称的时间刻度组
    /// </summary>
    public void RemoveTimeScaleGroup(string timeScaleName)
    {
        if (TimeScalesByName.Remove(timeScaleName, out var timeScaleGroup))
            timeScaleGroup.Dispose();
    }

    /// <summary>
    /// 初始化最初时间刻度组,它们一般都不会被销毁
    /// </summary>
    public void InitTimeScales(TimeScaleGroup[] initTimeScaleGroups)
    {
        foreach (var timeScale in initTimeScaleGroups)
        {
            if (timeScale == null) continue;

            var scaleName = timeScale.Name;
            var trimName = timeScale.Name.Trim();

            if (scaleName != trimName)
            {
                Debug.LogWarning($"TimeScale{scaleName} has trailing spaces,please check it!");
                scaleName = trimName;
            }

            if (!TimeScalesByName.TryAdd(scaleName, timeScale))
            {
                Debug.LogWarning($"TimeScale with name {scaleName} already exists!");
                continue;
            }

            if (timeScale.AutoRemoveWhenEmpty)
            {
                Debug.LogWarning(
                    $"TimeScale {scaleName} has AutoRemoveWhenEmpty set to true. For initial TimeScales, this is not allowed. Will set to false.");
                timeScale.AutoRemoveWhenEmpty = false;
            }
        }
    }
}
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class ChronosSystem : SingletonMono<ChronosSystem>
{
    [ShowInInspector]
    public Dictionary<string, TimeScaleGroup> TimeScales { get; } = new Dictionary<string, TimeScaleGroup>();

    public TimeScaleGroup GetTimeScaleGroup(string timeScaleName, float initScaleValue = 1f,
        bool autoRemoveWhenEmpty = true, bool createIfNull = true)
    {
        if (TimeScales.TryGetValue(timeScaleName, out var timeScale))
            return timeScale;

        if (!createIfNull) return null;

        timeScale = new TimeScaleGroup();
        timeScale.Init(timeScaleName, initScaleValue, autoRemoveWhenEmpty);
        return timeScale;
    }

    public void RemoveTimeScaleGroup(string timeScaleName)
    {
        if (TimeScales.Remove(timeScaleName, out var timeScaleGroup))
            timeScaleGroup.Dispose();
    }

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

            if (!TimeScales.TryAdd(scaleName, timeScale))
            {
                Debug.LogWarning($"TimeScale with name {scaleName} already exists!");
                continue;
            }

            timeScale.Init(scaleName, timeScale.ScaleValue, timeScale.AutoRemoveWhenEmpty);
            if (timeScale.AutoRemoveWhenEmpty)
            {
                Debug.LogWarning(
                    $"TimeScale {scaleName} has AutoRemoveWhenEmpty set to true. For initial TimeScales, this is not allowed. Will set to false.");
                timeScale.AutoRemoveWhenEmpty = false;
            }
        }
    }
}
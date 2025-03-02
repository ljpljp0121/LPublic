using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

/// <summary>
/// 技能自定义事件数据
/// </summary>
[Serializable]
public class SkillCustomEventData
{
    [NonSerialized, OdinSerialize] [DictionaryDrawerSettings(KeyLabel = "事件开始帧", ValueLabel = "帧事件")]
    public Dictionary<int, SkillCustomEvent> FrameData = new();
}
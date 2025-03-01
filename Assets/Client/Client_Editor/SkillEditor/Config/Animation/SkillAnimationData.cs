using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

/// <summary>
/// 技能动画数据
/// </summary>
[Serializable]
public class SkillAnimationData
{
    [NonSerialized, OdinSerialize] [DictionaryDrawerSettings(KeyLabel = "事件开始帧", ValueLabel = "帧事件")]
    public Dictionary<int, SkillAnimationEvent> FrameData = new();
}
using Sirenix.Serialization;
using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;

/// <summary>
/// 技能音效数据
/// </summary>
[Serializable]
public class SkillAudioData
{
    /// <summary>
    /// 音效帧事件
    /// </summary>
    [NonSerialized, OdinSerialize] [DictionaryDrawerSettings(KeyLabel = "事件开始帧", ValueLabel = "帧事件")]
    public List<SkillAudioEvent> FrameData = new();
}
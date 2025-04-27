using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

/// <summary>
/// 技能碰撞检测数据
/// </summary>
[Serializable]
public class SkillColliderData
{
    /// <summary>
    /// 碰撞检测帧事件
    /// </summary>
    [NonSerialized, OdinSerialize] [DictionaryDrawerSettings(KeyLabel = "事件开始帧", ValueLabel = "帧事件")]
    public List<SkillColliderEvent> FrameData = new();
}
using System;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

/// <summary>
/// 技能片段配置
/// </summary>
[CreateAssetMenu(menuName = "Config/Skill/SkillClip", fileName = "SkillClip")]
public class SkillClip : SerializedScriptableObject
{
    [LabelText("技能名称")] public string SkillName;
    [LabelText("帧率上限")] public int FrameCount = 100;
    [LabelText("帧率")] public int FrameRate = 30;

    [NonSerialized, OdinSerialize] public SkillCustomEventData SkillCustomEventData = new SkillCustomEventData();

    [NonSerialized, OdinSerialize] public SkillAnimationData SkillAnimationData = new SkillAnimationData();

    [NonSerialized, OdinSerialize] public SkillAudioData SkillAudioData = new SkillAudioData();

    [NonSerialized, OdinSerialize] public SkillEffectData SkillEffectData = new SkillEffectData();

    [NonSerialized, OdinSerialize] public SkillColliderData SkillColliderData = new SkillColliderData();
}
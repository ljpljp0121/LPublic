using System;
using UnityEngine;

/// <summary>
/// 动画帧事件数据
/// </summary>
[Serializable]
public class SkillAnimationEvent : SkillFrameEventBase
{
    public AnimationClip AnimationClip; //动画片段
    public bool ApplyRootMotion; //支持根运动
    public float TransitionTime = 0.25f; //过渡时间

#if UNITY_EDITOR
    public int DurationFrame;
#endif
}
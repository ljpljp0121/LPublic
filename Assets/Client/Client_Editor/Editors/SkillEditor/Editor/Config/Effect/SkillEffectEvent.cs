using System;
using UnityEngine;

/// <summary>
/// 特效事件
/// </summary>
[Serializable]
public class SkillEffectEvent : SkillFrameEventBase
{
#if UNITY_EDITOR
    public string TrackName = "特效轨道";
#endif
    public int FrameIndex;
    public GameObject EffectPrefab; //特效预制体
    public Vector3 Position; //播放位置
    public Vector3 Rotation; //播放旋转
    public Vector3 Scale; //缩放大小
    public int Duration; //持续帧数
    public bool AutoDestroy; //是否自动销毁
}
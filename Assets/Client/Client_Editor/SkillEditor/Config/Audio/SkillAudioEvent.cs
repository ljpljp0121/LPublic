using System;
using UnityEngine;


/// <summary>
/// 音效帧事件
/// </summary>
[Serializable]
public class SkillAudioEvent : SkillFrameEventBase
{
#if UNITY_EDITOR
    public string TrackName = "音效轨道";
#endif
    public int FrameIndex;//事件开始帧
    public AudioClip AudioClip; //音效片段
    public float Volume = 1; //音效音量
}
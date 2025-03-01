using UnityEngine;
using System;
using System.Reflection;

/// <summary>
/// 技能编辑器音效播放工具
/// 音效在UnityEditor下没有提供播放功能，这个功能没有公开，所以只能利用反射实现
/// </summary>
public static class EditorAudioUtility
{
    private static readonly MethodInfo PlayClipMethodInfo;
    private static readonly MethodInfo StopAllClipMethodInfo;

    static EditorAudioUtility()
    {
        Assembly editorAssembly = typeof(UnityEditor.AudioImporter).Assembly;
        Type utilClassType = editorAssembly.GetType("UnityEditor.AudioUtil");
        PlayClipMethodInfo = utilClassType.GetMethod("PlayPreviewClip",
            BindingFlags.Static | BindingFlags.Public, null,
            new[] { typeof(AudioClip), typeof(int), typeof(bool) }, null);
        StopAllClipMethodInfo = utilClassType.GetMethod("StopAllPreviewClips",
            BindingFlags.Static | BindingFlags.Public);
    }

    /// <summary>
    /// 播放音效
    /// </summary>
    public static void PlayAudio(AudioClip clip, float start)
    {
        PlayClipMethodInfo.Invoke(null, new object[] { clip, (int)(start * clip.frequency), false });
    }

    /// <summary>
    /// 停止所有的音效
    /// </summary>
    public static void StopAllAudio()
    {
        StopAllClipMethodInfo.Invoke(null, null);
    }
}
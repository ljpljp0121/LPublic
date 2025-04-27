using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// 音效片段Inspector窗口类
/// </summary>
public class SkillAudioEventInspector : SkillEventDataInspectorBase<AudioTrackItem, AudioTrack>
{
    private FloatField voluemField;

    public override void OnDraw()
    {
        //音效资源
        ObjectField audioClipAssetField = new ObjectField("音效资源");
        audioClipAssetField.objectType = typeof(AudioClip);
        audioClipAssetField.value = trackItem.AudioEvent.AudioClip;
        audioClipAssetField.RegisterValueChangedCallback(AudioClipAssetFieldValueChanged);
        root.Add(audioClipAssetField);

        root.Add(voluemField);
    }
    
    private void AudioClipAssetFieldValueChanged(ChangeEvent<UnityEngine.Object> evt)
    {
        AudioClip audioClip = evt.newValue as AudioClip;
        trackItem.AudioEvent.AudioClip = audioClip;
        trackItem.ResetView();
    }

}

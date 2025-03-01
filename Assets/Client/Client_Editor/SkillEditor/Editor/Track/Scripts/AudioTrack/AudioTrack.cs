using System.Collections.Generic;
using UnityEngine.UIElements;

/// <summary>
/// 音效轨道类
/// </summary>
public class AudioTrack : SkillTrackBase
{
    private SkillMultiLineTrackStyle trackStyle;
    private List<AudioTrackItem> trackItemList = new List<AudioTrackItem>();

    /// <summary>
    /// 当前配置文件中的技能音效数据
    /// </summary>
    public SkillAudioData AudioData => SkillEditorWindow.Instance.SkillClip.SkillAudioData;

    public override void Init(VisualElement menuParent, VisualElement contentParent, float frameWidth)
    {
        base.Init(menuParent, contentParent, frameWidth);
        trackStyle = new SkillMultiLineTrackStyle();
        trackStyle.Init(menuParent, contentParent, "音效配置", AddChildTrack
            , CheckRemoveChildTrack, SwapChildTrack, UpdateChildTrackName);

        ResetView();
    }

    public override void ResetView(float frameWidth)
    {
        base.ResetView(frameWidth);
        //销毁当前的
        foreach (AudioTrackItem item in trackItemList)
        {
            item.Destroy();
        }
        trackItemList.Clear();
        if (SkillEditorWindow.Instance.SkillClip == null)
        {
            return;
        }
        //基于音效数据绘制轨道
        foreach (SkillAudioEvent item in AudioData.FrameData)
        {
            CreateItem(item);
        }
    }

    /// <summary>
    /// 创建音效片段
    /// 调用它在当前轨道，
    /// 生成指定skillAudioEvent对应的音效片段
    /// </summary>
    private void CreateItem(SkillAudioEvent skillAudioEvent)
    {
        AudioTrackItem item = new AudioTrackItem();
        item.Init(this, frameWidth, skillAudioEvent, trackStyle.AddChildTrack());
        item.SetTrackName(skillAudioEvent.TrackName);
        trackItemList.Add(item);
    }

    /// <summary>
    /// 更新子轨道名称
    /// </summary>
    private void UpdateChildTrackName(SkillMultiLineTrackStyle.ChildTrack childTrack, string name)
    {
        //同步给配置
        AudioData.FrameData[childTrack.GetIndex()].TrackName = name;
        SkillEditorWindow.Instance.SaveClip();
    }

    /// <summary>
    /// 添加子轨道
    /// 在数据和视图层面上都添加
    /// </summary>
    private void AddChildTrack()
    {
        SkillAudioEvent skillAudioEvent = new SkillAudioEvent();
        AudioData.FrameData.Add(skillAudioEvent);
        CreateItem(skillAudioEvent);
        SkillEditorWindow.Instance.SaveClip();
    }

    /// <summary>
    /// 检查是否能够删除对应索引的子轨道
    /// </summary>
    private bool CheckRemoveChildTrack(int index)
    {
        if (index < 0 || index >= trackItemList.Count)
        {
            return false;
        }
        if (AudioData.FrameData[index] != null)
        {
            AudioData.FrameData.RemoveAt(index);
            trackItemList.RemoveAt(index);
            SkillEditorWindow.Instance.SaveClip();
        }
        return true;
    }

    /// <summary>
    /// 交换子轨道的位置
    /// </summary>
    private void SwapChildTrack(int index1, int index2)
    {
        SkillAudioEvent event1 = AudioData.FrameData[index1];
        SkillAudioEvent event2 = AudioData.FrameData[index2];

        AudioData.FrameData[index1] = event2;
        AudioData.FrameData[index2] = event1;
    }

    public override void Destroy()
    {
        trackStyle.Destroy();
    }

    public override void OnPlay(int startFrameIndex)
    {
        base.OnPlay(startFrameIndex);
        for (int i = 0; i < AudioData.FrameData.Count; i++)
        {
            SkillAudioEvent audioEvent = AudioData.FrameData[i];
            if (audioEvent.AudioClip == null) continue;

            int audioFrameCount = (int)(audioEvent.AudioClip.length * SkillEditorWindow.Instance.SkillClip.FrameRate);
            int audioLastFrameIndex = audioFrameCount + audioEvent.FrameIndex;
            //开始位置在左边 && 长度大于当前选中帧
            //说明选中位置在该音频片段之间
            if (audioEvent.FrameIndex < startFrameIndex
                && audioLastFrameIndex > startFrameIndex)
            {
                int offset = startFrameIndex - audioEvent.FrameIndex;
                float playRate = (float)offset / audioFrameCount;
                EditorAudioUtility.PlayAudio(audioEvent.AudioClip, playRate);
            }
            else if (audioEvent.FrameIndex == startFrameIndex)
            {
                EditorAudioUtility.PlayAudio(audioEvent.AudioClip, 0);
            }
        }
    }

    public override void TickView(int frameIndex)
    {
        base.TickView(frameIndex);
        if (SkillEditorWindow.Instance.IsPlaying)
        {
            for (int i = 0; i < AudioData.FrameData.Count; i++)
            {
                SkillAudioEvent audioEvent = AudioData.FrameData[i];
                if (audioEvent.AudioClip != null && audioEvent.FrameIndex == frameIndex)
                {
                    EditorAudioUtility.PlayAudio(audioEvent.AudioClip, 0);
                }
            }
        }
    }
}
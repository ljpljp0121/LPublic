using UnityEditor;
using UnityEngine.UIElements;

/// <summary>
/// 音效轨道片段样式
/// </summary>
public class SkillAudioTrackItemStyle : SkillTrackItemStyleBase
{
    protected override string TrackItemAssetPath =>
        SkillEditorWindow.SkillEditorAssetPath + "TrackItem/AudioTrackItem.uxml";

    private Label TitleLabel;

    public VisualElement MainDragArea { get; private set; }
    public VisualElement AudioOverLine { get; private set; }
    public bool isInit { get; private set; }

    /// <summary>
    /// 初始化音效片段
    /// </summary>
    public void Init(SkillMultiLineTrackStyle.ChildTrack childTrack)
    {
        if (isInit) return;

        TitleLabel = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(TrackItemAssetPath).Instantiate().Query<Label>();
        Root = TitleLabel;
        childTrack.InitContent(Root);
        MainDragArea = Root.Q<VisualElement>(nameof(MainDragArea));
        AudioOverLine = Root.Q<VisualElement>(nameof(AudioOverLine));
        isInit = true;
    }

    /// <summary>
    /// 刷新视图
    /// </summary>
    public void ResetView(float frameUnitWidth, SkillAudioEvent skillAudioEvent)
    {
        if (!isInit) return;
        if (skillAudioEvent.AudioClip != null)
        {
            SetTitle(skillAudioEvent.AudioClip.name);
            SetWidth(frameUnitWidth * skillAudioEvent.AudioClip.length *
                     SkillEditorWindow.Instance.SkillClip.FrameRate);
            SetPosition(frameUnitWidth * skillAudioEvent.FrameIndex);
        }
        else
        {
            SetTitle("");
            SetWidth(0);
            SetPosition(0);
        }
    }

    /// <summary>
    /// 设置音效片段标题
    /// </summary>
    public void SetTitle(string title)
    {
        TitleLabel.text = title;
    }
}
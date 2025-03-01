using UnityEditor;
using UnityEngine.UIElements;

public class SkillAnimationTrackItemStyle : SkillTrackItemStyleBase
{
    protected override string TrackItemAssetPath =>
        SkillEditorWindow.SkillEditorAssetPath + "TrackItem/AnimationTrackItem.uxml";

    private Label TitleLabel;
    public VisualElement MainDragArea { get; private set; }
    public VisualElement AnimationOverLine { get; private set; }

    /// <summary>
    /// 初始化动画片段
    /// </summary>
    public void Init(SkillTrackStyleBase trackStyle)
    {
        TitleLabel = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(TrackItemAssetPath).Instantiate().Query<Label>();
        Root = TitleLabel;
        MainDragArea = Root.Q<VisualElement>(nameof(MainDragArea));
        AnimationOverLine = Root.Q<VisualElement>(nameof(AnimationOverLine));
        trackStyle.AddItem(Root);
    }
    
    public void ResetView(float frameIndex, float frameUnitWidth, SkillAnimationEvent skillAnimationEvent)
    {
        SetTitle(skillAnimationEvent.AnimationClip.name);
        SetWidth(frameUnitWidth * skillAnimationEvent.DurationFrame);
        SetPosition(frameUnitWidth * frameIndex);
    }
    
    public void SetTitle(string title)
    {
        TitleLabel.text = title;
    }
}
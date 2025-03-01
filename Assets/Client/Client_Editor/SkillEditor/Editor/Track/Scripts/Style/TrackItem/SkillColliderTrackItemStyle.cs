using UnityEditor;
using UnityEngine.UIElements;

/// <summary>
/// 攻击碰撞盒轨道片段样式
/// </summary>
public class SkillColliderTrackItemStyle : SkillTrackItemStyleBase
{
    protected override string TrackItemAssetPath =>
        SkillEditorWindow.SkillEditorAssetPath + "TrackItem/AttackDetectionTrackItem.uxml";

    private Label TitleLabel;

    public VisualElement MainDragArea { get; private set; }
    public VisualElement ColliderOverLine { get; private set; }
    public bool IsInit { get; private set; }
    
    /// <summary>
    /// 初始化碰撞盒
    /// </summary>
    public void Init(SkillMultiLineTrackStyle.ChildTrack childTrack)
    {
        if (IsInit) return;
        TitleLabel = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(TrackItemAssetPath).Instantiate().Query<Label>();
        Root = TitleLabel;
        childTrack.InitContent(Root);
        MainDragArea = Root.Q<VisualElement>(nameof(MainDragArea));
        ColliderOverLine = Root.Q<VisualElement>(nameof(ColliderOverLine));
        IsInit = true;
    }

    /// <summary>
    /// 刷新视图
    /// </summary>
    public void ResetView(float frameUnitWidth, SkillColliderEvent skillColliderEvent)
    {
        if (!IsInit) return;
        SetTitle("");
        SetWidth(frameUnitWidth * skillColliderEvent.DurationFrame);
        SetPosition(frameUnitWidth * skillColliderEvent.FrameIndex);
    }

    public void SetTitle(string title)
    {
        TitleLabel.text = title;
    }
}
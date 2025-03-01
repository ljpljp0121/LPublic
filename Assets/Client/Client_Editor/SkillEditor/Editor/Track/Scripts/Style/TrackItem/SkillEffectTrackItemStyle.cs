using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// 特效轨道片段样式
/// </summary>
public class SkillEffectTrackItemStyle : SkillTrackItemStyleBase
{
    protected override string TrackItemAssetPath =>
        SkillEditorWindow.SkillEditorAssetPath + "TrackItem/EffectTrackItem.uxml";

    private Label TitleLabel;

    public VisualElement MainDragArea { get; private set; }
    public bool isInit { get; private set; }

    /// <summary>
    /// 初始化特效片段
    /// </summary>
    public void Init(SkillMultiLineTrackStyle.ChildTrack childTrack)
    {
        if (isInit) return;

        TitleLabel = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(TrackItemAssetPath).Instantiate().Query<Label>();
        Root = TitleLabel;
        childTrack.InitContent(Root);
        MainDragArea = Root.Q<VisualElement>(nameof(MainDragArea));
        isInit = true;
    }

    /// <summary>
    /// 刷新视图
    /// </summary>
    public void ResetView(float frameUnitWidth, SkillEffectEvent skillEffectEvent)
    {
        if (!isInit) return;
        if (skillEffectEvent.EffectPrefab != null)
        {
            SetTitle(skillEffectEvent.EffectPrefab.name);
            SetWidth(frameUnitWidth * skillEffectEvent.Duration);
            SetPosition(frameUnitWidth * skillEffectEvent.FrameIndex);
        }
        else
        {
            SetTitle("");
            SetWidth(0);
            SetPosition(0);
        }
    }

    /// <summary>
    /// 设置特效片段标题
    /// </summary>
    public void SetTitle(string title)
    {
        TitleLabel.text = title;
    }
}
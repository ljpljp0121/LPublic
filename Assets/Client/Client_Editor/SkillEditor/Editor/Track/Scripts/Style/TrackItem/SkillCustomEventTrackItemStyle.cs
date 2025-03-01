using UnityEditor;
using UnityEngine.UIElements;

/// <summary>
/// 技能轨道自定义事件的样式
/// </summary>
public class SkillCustomEventTrackItemStyle : SkillTrackItemStyleBase
{
    protected override string TrackItemAssetPath =>
        SkillEditorWindow.SkillEditorAssetPath + "TrackItem/CustomEventTrackItem.uxml";

    /// <summary>
    /// 初始化自定义事件片段
    /// </summary>
    public void Init(SkillTrackStyleBase trackStyle)
    {
        Root = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(TrackItemAssetPath).Instantiate().Query<Label>();
        trackStyle.AddItem(Root);
    }

    /// <summary>
    /// 刷新视图
    /// </summary>
    public void ResetView(float frameIndex, float frameUnitWidth, SkillCustomEvent skillCustomEvent)
    {
        SetWidth(frameUnitWidth);
        SetPosition(frameUnitWidth * frameIndex);
    }
}
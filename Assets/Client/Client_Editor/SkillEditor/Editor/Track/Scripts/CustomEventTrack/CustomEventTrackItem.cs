using System;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// 自定义事件轨道Item
/// </summary>
public class CustomEventTrackItem : TrackItemBase<CustomEventTrack>
{
    public SkillCustomEvent CustomEvent { get; private set; }
    public SkillCustomEventTrackItemStyle TrackItemStyle { get; private set; }

    public static CustomEventTrackItem CurrentSelectItem;

    /// <summary>
    /// 初始化自定义事件轨道片段
    /// </summary>
    public void Init(CustomEventTrack track, SkillTrackStyleBase parentTrackStytle, int frameIndex,
        float frameUnitWidth, SkillCustomEvent customEvent)
    {
        this.frameUnitWidth = frameUnitWidth;
        this.frameIndex = frameIndex;
        this.track = track;
        this.CustomEvent = customEvent;

        TrackItemStyle = new SkillCustomEventTrackItemStyle();
        itemStyle = TrackItemStyle;
        TrackItemStyle.Init(parentTrackStytle);

        normalColor = new Color(0.25f, 0.62f, 1f, 1f);
        selectColor = new Color(0.16f, 0.5f, 1f, 0.85f);
        OnUnSelect();
        TrackItemStyle.Root.RegisterCallback<MouseDownEvent>(MouseDown);
        ResetView(frameUnitWidth);
    }

    public override void ResetView(float frameUnitWidth)
    {
        base.ResetView(frameUnitWidth);
        TrackItemStyle.SetPosition(frameIndex * frameUnitWidth - frameUnitWidth / 2);
        TrackItemStyle.SetWidth(frameUnitWidth);
    }

    /// <summary>
    /// 鼠标点击自定义事件区域
    /// 会根据片段当前状态做不同行为
    /// 如果已经选中，点击其他空白区域，事件索引移到选中索引
    /// 如果选中已选中事件，则取消选中
    /// </summary>
    /// <param name="evt"></param>
    private void MouseDown(MouseDownEvent evt)
    {
        if (CurrentSelectItem == this) OnUnSelect();
        else Select();
    }

    public override void OnSelect()
    {
        CurrentSelectItem = this;
        base.OnSelect();
    }

    public override void Select()
    {
        base.Select();
    }

    public override void OnUnSelect()
    {
        CurrentSelectItem = null;
        base.OnUnSelect();
    }

    /// <summary>
    /// 改变片段帧索引
    /// </summary>
    public void ChangeFrameIndex(int newIndex)
    {
        track.SetFrameIndex(frameIndex, newIndex);
        frameIndex = newIndex;
        SkillEditorInspector.Instance.SetTrackItemFrameIndex(frameIndex);
        ResetView();
    }
}
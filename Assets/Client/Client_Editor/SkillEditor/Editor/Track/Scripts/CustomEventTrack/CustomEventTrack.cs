using System;
using System.Collections.Generic;
using UnityEngine.UIElements;

/// <summary>
/// 自定义事件轨道
/// </summary>
public class CustomEventTrack : SkillTrackBase
{
    private SkillSingleLineTrackStyle trackStyle;
    private readonly Dictionary<int, CustomEventTrackItem> trackItemDic = new Dictionary<int, CustomEventTrackItem>();

    /// <summary>
    /// 当前配置文件中的技能自定义事件数据
    /// </summary>
    public SkillCustomEventData CustomEventData => SkillEditorWindow.Instance.SkillClip.SkillCustomEventData;

    public override void Init(VisualElement menuParent, VisualElement contentParent, float frameWidth)
    {
        base.Init(menuParent, contentParent, frameWidth);
        trackStyle = new SkillSingleLineTrackStyle();
        trackStyle.Init(menuParent, contentParent, "事件配置");
        trackStyle.contentRoot.RegisterCallback<MouseDownEvent>(ContentMouseDown);
    }

    /// <summary>
    /// 在事件轨道上点击
    /// 如果事件轨道以片段为检测区域，那么就很容易会跟丢鼠标导致滑动失败，所以改由父级检测
    /// </summary>
    /// <param name="evt"></param>
    /// <exception cref="NotImplementedException"></exception>
    private void ContentMouseDown(MouseDownEvent evt)
    {
        int frameIndex = SkillEditorWindow.Instance.GetFrameIndexByPos(evt.localMousePosition.x);
        if (CustomEventData.FrameData.ContainsKey(frameIndex)) return;
        //变换位置
        if (CustomEventTrackItem.CurrentSelectItem != null)
        {
            CustomEventTrackItem.CurrentSelectItem.ChangeFrameIndex(frameIndex);
        }
        //添加轨道片段
        else
        {
            SkillCustomEvent skillCustomEvent = new SkillCustomEvent();
            CustomEventData.FrameData.Add(frameIndex, skillCustomEvent);
            SkillEditorWindow.Instance.SaveClip();
            CreateItem(frameIndex, skillCustomEvent);
        }
    }


    public override void ResetView(float frameWidth)
    {
        base.ResetView(frameWidth);
        //销毁当前已有
        foreach (var item in trackItemDic.Values)
        {
            trackStyle.RemoveItem(item.itemStyle.Root);
        }
        trackItemDic.Clear();
        if (SkillEditorWindow.Instance.SkillClip == null)
        {
            return;
        }
        //根据数据绘制TrackItem
        foreach (var item in CustomEventData.FrameData)
        {
            CreateItem(item.Key, item.Value);
        }
    }

    /// <summary>
    /// 创建自定义事件片段
    /// 调用它在当前轨道，
    /// 指定的帧索引处生成对应的事件片段
    /// </summary>
    private void CreateItem(int frameIndex, SkillCustomEvent skillCustomEvent)
    {
        CustomEventTrackItem trackItem = new CustomEventTrackItem();
        trackItem.Init(this, trackStyle, frameIndex, frameWidth, skillCustomEvent);
        trackItemDic.Add(frameIndex, trackItem);
    }

    /// <summary>
    /// 将oldIndex的数据转移到newIndex
    /// 在拖拽轨道片段结束后调用它更新片段的帧索引
    /// </summary>
    /// <param name="oldIndex"></param>
    /// <param name="newIndex"></param>
    public void SetFrameIndex(int oldIndex, int newIndex)
    {
        if (CustomEventData.FrameData.Remove(oldIndex, out SkillCustomEvent customEvent))
        {
            CustomEventData.FrameData.Add(newIndex, customEvent);
            trackItemDic.Remove(oldIndex, out CustomEventTrackItem item);
            trackItemDic.Add(newIndex, item);
        }
    }

    /// <summary>
    /// 删除指定帧索引处的动画片段
    /// </summary>
    /// <param name="frameIndex"></param>
    public override void DeleteTrackItem(int frameIndex)
    {
        CustomEventData.FrameData.Remove(frameIndex);
        if (trackItemDic.Remove(frameIndex, out CustomEventTrackItem item))
        {
            trackStyle.RemoveItem(item.itemStyle.Root);
        }
    }

    public override void Destroy()
    {
        trackStyle.Destroy();
    }
}
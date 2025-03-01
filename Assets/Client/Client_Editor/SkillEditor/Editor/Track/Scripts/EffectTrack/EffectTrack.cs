using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// 特效轨道类
/// </summary>
public class EffectTrack : SkillTrackBase
{
    private SkillMultiLineTrackStyle trackStyle;
    private readonly List<EffectTrackItem> trackItemList = new List<EffectTrackItem>();

    public static Transform EffectParent { get; private set; }

    /// <summary>
    /// 当前配置文件中的技能特效数据
    /// </summary>
    public SkillEffectData EffectData => SkillEditorWindow.Instance.SkillClip.SkillEffectData;

    public override void Init(VisualElement menuParent, VisualElement contentParent, float frameWidth)
    {
        base.Init(menuParent, contentParent, frameWidth);
        trackStyle = new SkillMultiLineTrackStyle();
        trackStyle.Init(menuParent, contentParent, "特效配置", AddChildTrack
            , CheckRemoveChildTrack, SwapChildTrack, UpdateChildTrackName);
        if (SkillEditorWindow.Instance.IsEditorScene)
        {
            EffectParent = GameObject.Find("Effects").transform;
            EffectParent.position = Vector3.zero;
            EffectParent.rotation = Quaternion.identity;
            for (int i = EffectParent.childCount - 1; i >= 0; i--)
            {
                Object.DestroyImmediate(EffectParent.GetChild(i).gameObject);
            }
        }
        ResetView();
    }

    public override void ResetView(float frameWidth)
    {
        base.ResetView(frameWidth);
        //销毁当前的
        foreach (EffectTrackItem item in trackItemList)
        {
            item.Destroy();
        }
        trackItemList.Clear();
        //基于音效数据绘制轨道
        foreach (SkillEffectEvent item in EffectData.FrameData)
        {
            CreateItem(item);
        }
    }

    /// <summary>
    /// 创建特效片段
    /// 调用它在当前轨道，
    /// 生成指定skillEffectEvent对应的特效片段
    /// </summary>
    private void CreateItem(SkillEffectEvent skillEffectEvent)
    {
        EffectTrackItem item = new EffectTrackItem();
        item.Init(this, frameWidth, skillEffectEvent, trackStyle.AddChildTrack());
        item.SetTrackName(skillEffectEvent.TrackName);
        trackItemList.Add(item);
    }

    /// <summary>
    /// 更新子轨道名称
    /// </summary>
    private void UpdateChildTrackName(SkillMultiLineTrackStyle.ChildTrack childTrack, string name)
    {
        //同步给配置
        EffectData.FrameData[childTrack.GetIndex()].TrackName = name;
        SkillEditorWindow.Instance.SaveClip();
    }

    /// <summary>
    /// 添加子轨道
    /// 在数据和视图层面上都添加
    /// </summary>
    private void AddChildTrack()
    {
        SkillEffectEvent skillEffectEvent = new SkillEffectEvent();
        EffectData.FrameData.Add(skillEffectEvent);
        CreateItem(skillEffectEvent);
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
        SkillEffectEvent effectEvent = EffectData.FrameData[index];
        if (effectEvent != null)
        {
            EffectData.FrameData.RemoveAt(index);
            SkillEditorWindow.Instance.SaveClip();
            trackItemList[index].CleanEffectPreviewObj();
            trackItemList.RemoveAt(index);
        }
        return true;
    }

    /// <summary>
    /// 交换子轨道的位置
    /// </summary>
    private void SwapChildTrack(int index1, int index2)
    {
        SkillEffectEvent event1 = EffectData.FrameData[index1];
        SkillEffectEvent event2 = EffectData.FrameData[index2];

        EffectData.FrameData[index1] = event2;
        EffectData.FrameData[index2] = event1;
    }

    public override void Destroy()
    {
        trackStyle.Destroy();
        foreach (var item in trackItemList)
        {
            item.CleanEffectPreviewObj();
        }
    }

    public override void TickView(int frameIndex)   
    {
        base.TickView(frameIndex);
        foreach (var item in trackItemList)
        {
            item.TickView(frameIndex);
        }
    }
}
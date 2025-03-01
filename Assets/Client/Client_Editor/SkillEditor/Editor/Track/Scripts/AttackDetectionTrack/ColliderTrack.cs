using System.Collections.Generic;
using UnityEngine.UIElements;

/// <summary>
/// 攻击检测轨道类
/// </summary>
public class ColliderTrack : SkillTrackBase
{
    private SkillMultiLineTrackStyle trackStyle;
    private List<ColliderTrackItem> trackItemList = new List<ColliderTrackItem>();

    /// <summary>
    /// 当前配置文件中的攻击检测数据
    /// </summary>
    public SkillColliderData AttackDetectionData => SkillEditorWindow.Instance.SkillClip.SkillColliderData;

    public override void Init(VisualElement menuParent, VisualElement contentParent, float frameWidth)
    {
        base.Init(menuParent, contentParent, frameWidth);
        trackStyle = new SkillMultiLineTrackStyle();
        trackStyle.Init(menuParent, contentParent, "攻击检测配置", AddChildTrack
            , CheckRemoveChildTrack, SwapChildTrack, UpdateChildTrackName);

        ResetView();
    }

    public override void ResetView(float frameWidth)
    {
        base.ResetView(frameWidth);
        //销毁当前的
        foreach (ColliderTrackItem item in trackItemList)
        {
            item.Destroy();
        }
        trackItemList.Clear();
        if (SkillEditorWindow.Instance.SkillClip == null)
        {
            return;
        }
        //基于攻击检测绘制轨道
        foreach (var item in AttackDetectionData.FrameData)
        {
            CreateItem(item);
        }
    }

    /// <summary>
    /// 创建攻击检测片段
    /// 调用它在当前轨道，
    /// 生成指定SkillAttackDetectionEvent对应的攻击检测片段
    /// </summary>
    private void CreateItem(SkillColliderEvent skillColliderEvent)
    {
        ColliderTrackItem item = new ColliderTrackItem();
        item.Init(this, frameWidth, skillColliderEvent, trackStyle.AddChildTrack());
        item.SetTrackName(skillColliderEvent.TrackName);
        trackItemList.Add(item);
    }

    /// <summary>
    /// 更新子轨道名称
    /// </summary>
    private void UpdateChildTrackName(SkillMultiLineTrackStyle.ChildTrack childTrack, string name)
    {
        //同步给配置
        AttackDetectionData.FrameData[childTrack.GetIndex()].TrackName = name;
        SkillEditorWindow.Instance.SaveClip();
    }

    /// <summary>
    /// 添加子轨道
    /// 在数据和视图层面上都添加
    /// </summary>
    private void AddChildTrack()
    {
        SkillColliderEvent colliderEvent = new SkillColliderEvent();
        AttackDetectionData.FrameData.Add(colliderEvent);
        CreateItem(colliderEvent);
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
        if (AttackDetectionData.FrameData[index] != null)
        {
            AttackDetectionData.FrameData.RemoveAt(index);
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
        SkillColliderEvent event1 = AttackDetectionData.FrameData[index1];
        SkillColliderEvent event2 = AttackDetectionData.FrameData[index2];

        AttackDetectionData.FrameData[index1] = event2;
        AttackDetectionData.FrameData[index2] = event1;
    }

    public override void Destroy()
    {
        trackStyle.Destroy();
    }

    public override void DrawGizmos()
    {
        for (int i = 0; i < trackItemList.Count; i++)
        {
            int currentFrameIndex = SkillEditorWindow.Instance.CurrentSelectFrameIndex;
            SkillColliderEvent detectionEvent = trackItemList[i].ColliderEvent;
            if (currentFrameIndex < detectionEvent.FrameIndex ||
                currentFrameIndex > detectionEvent.FrameIndex + detectionEvent.DurationFrame)
            {
                continue;
            }
            trackItemList[i].DrawGizmos();
        }
    }

    public override void OnSceneGUI()
    {
        for (int i = 0; i < trackItemList.Count; i++)
        {
            int currentFrameIndex = SkillEditorWindow.Instance.CurrentSelectFrameIndex;
            SkillColliderEvent detectionEvent = trackItemList[i].ColliderEvent;
            if (currentFrameIndex < detectionEvent.FrameIndex ||
                currentFrameIndex > detectionEvent.FrameIndex + detectionEvent.DurationFrame)
            {
                continue;
            }
            if (SkillEditorInspector.CurrentTrackItem != trackItemList[i])
            {
                continue;
            }
            trackItemList[i].OnSceneGUI();
        }
    }
}
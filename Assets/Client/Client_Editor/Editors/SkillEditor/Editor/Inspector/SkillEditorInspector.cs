using System;
using UnityEditor;
using UnityEngine.UIElements;

[CustomEditor(typeof(SkillEditorWindow))]
public class SkillEditorInspector : Editor
{
    public static SkillEditorInspector Instance { get; private set; }
    public static TrackItemBase CurrentTrackItem { get; private set; }
    private static SkillTrackBase currentTrack;
    private int trackItemFrameIndex;

    /// <summary>
    /// 设置轨道中的片段
    /// 显示选中的轨道片段
    /// </summary>
    /// <param name="trackItem">轨道片段</param>
    /// <param name="track">轨道</param>
    public static void SetTrackItem(TrackItemBase trackItem, SkillTrackBase track)
    {
        if (CurrentTrackItem != null)
        {
            CurrentTrackItem.OnUnSelect();
        }
        CurrentTrackItem = trackItem;
        CurrentTrackItem.OnSelect();
        currentTrack = track;
        //避免已经打开了不刷新数据
        if (Instance != null) Instance.Show();
    }

    private void OnDestroy()
    {
        if (CurrentTrackItem != null)
        {
            CurrentTrackItem.OnUnSelect();
            CurrentTrackItem = null;
            currentTrack = null;
        }
    }

    private VisualElement root;

    public override VisualElement CreateInspectorGUI()
    {
        Instance = this;
        root = new VisualElement();
        Show();
        return root;
    }

    private SkillEventDataInspectorBase eventDataInspector;


    /// <summary>
    /// 根据当前轨道片段类型显示Inspector内容
    /// </summary>
    public void Show()
    {
        Clean();
        if (CurrentTrackItem == null) return;
        trackItemFrameIndex = CurrentTrackItem.FrameIndex;
        Type itemType = CurrentTrackItem.GetType();
        eventDataInspector = null;
        if (itemType == typeof(AnimationTrackItem))
        {
            eventDataInspector = new SkillAnimationEventInspector();
        }
        else if (itemType == typeof(AudioTrackItem))
        {
            eventDataInspector = new SkillAudioEventInspector();
        }
        else if (itemType == typeof(EffectTrackItem))
        {
            eventDataInspector = new SkillEffectEventInspector();
        }
        else if (itemType == typeof(ColliderTrackItem))
        {
            eventDataInspector = new SkillColliderEventInspector();
        }
        else if (itemType == typeof(CustomEventTrackItem))
        {
            eventDataInspector = new SkillCustomEventInspector();
        }
        eventDataInspector?.Draw(root, CurrentTrackItem, currentTrack);
    }

    /// <summary>
    /// 清除窗口，在更新窗口时先调用此方法
    /// </summary>
    private void Clean()
    {
        if (root != null)
        {
            for (int i = root.childCount - 1; i >= 0; i--)
            {
                root.RemoveAt(i);
            }
        }
    }

    /// <summary>
    /// 更新选中轨道片段的起始帧索引
    /// 为了在拖动轨道片段时也能即时更新
    /// </summary>
    public void SetTrackItemFrameIndex(int trackItemFrameIndex)
    {
        this.trackItemFrameIndex = trackItemFrameIndex;
        eventDataInspector.SetFrameIndex(trackItemFrameIndex);
    }
}

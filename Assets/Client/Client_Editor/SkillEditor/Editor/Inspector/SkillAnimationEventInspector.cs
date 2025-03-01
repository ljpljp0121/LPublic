using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// 动画片段Inspector窗口类
/// </summary>
public class SkillAnimationEventInspector : SkillEventDataInspectorBase<AnimationTrackItem, AnimationTrack>
{
    private IntegerField trackLengthField;
    private FloatField transitionTimeField;
    private Toggle rootMotionToggle;
    private Toggle mainWeaponOnLeftHandToggle;
    private Label clipFrameCountLabel;
    private Label isLoopLabel;

    public override void OnDraw()
    {
        //动画资源
        ObjectField animationClipAssetField = new ObjectField("动画资源");
        animationClipAssetField.objectType = typeof(AnimationClip);
        animationClipAssetField.value = trackItem.AnimationEvent.AnimationClip;
        animationClipAssetField.RegisterValueChangedCallback(AnimationClipAssetFieldValueChanged);
        root.Add(animationClipAssetField);

        //根运动
        rootMotionToggle = new Toggle("应用根运动");
        rootMotionToggle.value = trackItem.AnimationEvent.ApplyRootMotion;
        rootMotionToggle.RegisterValueChangedCallback(RootMotionToggleValueChanged);
        root.Add(rootMotionToggle);

        //轨道长度
        trackLengthField = new IntegerField("轨道长度(帧数)");
        trackLengthField.value = trackItem.AnimationEvent.DurationFrame;
        trackLengthField.RegisterCallback<FocusInEvent>(TrackLengthFieldFocusIn);
        trackLengthField.RegisterCallback<FocusOutEvent>(TrackLengthFieldFocusOut);
        root.Add(trackLengthField);

        //过渡时间
        transitionTimeField = new FloatField("过渡时间");
        transitionTimeField.value = trackItem.AnimationEvent.TransitionTime;
        transitionTimeField.RegisterCallback<FocusInEvent>(TransitionTimeFieldFocusIn);
        transitionTimeField.RegisterCallback<FocusOutEvent>(TransitionTimeFieldFocusOut);
        root.Add(transitionTimeField);

        //动画相关信息
        int clipFrameCount = (int)(trackItem.AnimationEvent.AnimationClip.length
                            * trackItem.AnimationEvent.AnimationClip.frameRate);
        clipFrameCountLabel = new Label("动画资源长度:" + clipFrameCount);
        bool isLoop = trackItem.AnimationEvent.AnimationClip.isLooping;
        isLoopLabel = new Label("循环动画:" + isLoop);

        //删除
        Button deleteBtn = new Button(DeleteAnimationTrackItemButtonClick);
        deleteBtn.text = "删除";
        deleteBtn.style.backgroundColor = new Color(1, 0, 0, 0.5f);

        //设置持续帧数至选中帧
        Button setFrameBtn = new Button(SetAnimationFrameBtnClick);
        setFrameBtn.text = "设置持续帧数至选中帧";
        root.Add(setFrameBtn);

        root.Add(clipFrameCountLabel);
        root.Add(isLoopLabel);
        root.Add(deleteBtn);
    }

    private void AnimationClipAssetFieldValueChanged(ChangeEvent<UnityEngine.Object> evt)
    {
        AnimationClip clip = evt.newValue as AnimationClip;
        //修改自身显示效果
        clipFrameCountLabel.text = "动画资源长度:" + (int)(clip.length * clip.frameRate);
        isLoopLabel.text = "循环动画:" + clip.isLooping;
        //保存到配置
        trackItem.AnimationEvent.AnimationClip = clip;
        trackItem.ResetView();
        SkillEditorWindow.Instance.TickSkill();
    }
    private void RootMotionToggleValueChanged(ChangeEvent<bool> evt)
    {
        trackItem.AnimationEvent.ApplyRootMotion = evt.newValue;
        SkillEditorWindow.Instance.TickSkill();
    }
    int oldDurationValue;
    private void TrackLengthFieldFocusIn(FocusInEvent evt)
    {
        oldDurationValue = trackLengthField.value;
    }
    private void TrackLengthFieldFocusOut(FocusOutEvent evt)
    {
        if (trackLengthField.value != oldDurationValue)
        {
            //安全校验
            if (!(track.CheckFrameIndexOnDrag(itemFrameIndex + trackLengthField.value, itemFrameIndex, false)))
            {
                trackLengthField.value = oldDurationValue;
            }
            else
            {
                //修改数据，刷新视图
                trackItem.AnimationEvent.DurationFrame = trackLengthField.value;
                trackItem.CheckFrameCount();
                SkillEditorWindow.Instance.SaveClip();
                trackItem.ResetView();
            }
            SkillEditorWindow.Instance.TickSkill();
        }
    }
    private void SetAnimationFrameBtnClick()
    {
        TrackLengthFieldFocusIn(null);
        trackLengthField.value = SkillEditorWindow.Instance.CurrentSelectFrameIndex - trackItem.FrameIndex;
        TrackLengthFieldFocusOut(null);
    }
    float oldTransitionTime;
    private void TransitionTimeFieldFocusIn(FocusInEvent evt)
    {
        oldTransitionTime = transitionTimeField.value;
    }
    private void TransitionTimeFieldFocusOut(FocusOutEvent evt)
    {
        if (!Mathf.Approximately(transitionTimeField.value, oldTransitionTime))
        {
            trackItem.AnimationEvent.TransitionTime = transitionTimeField.value;
        }
    }
    private void DeleteAnimationTrackItemButtonClick()
    {
        track.DeleteTrackItem(itemFrameIndex);
        Selection.activeObject = null;
        SkillEditorWindow.Instance.TickSkill();
    }
}


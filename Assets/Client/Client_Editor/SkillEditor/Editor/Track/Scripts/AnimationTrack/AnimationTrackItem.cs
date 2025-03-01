using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// 动画轨道Item
/// </summary>
public class AnimationTrackItem : TrackItemBase<AnimationTrack>
{
    private SkillAnimationEvent animationEvent; //Item对应的动画帧事件
    public SkillAnimationEvent AnimationEvent
    {
        get => animationEvent;
    }


    private SkillAnimationTrackItemStyle trackItemStyle;


    /// <summary>
    /// 初始化动画轨道片段
    /// </summary>
    public void Init(AnimationTrack track, SkillTrackStyleBase parentTrackStytle, int frameIndex, float frameUnitWidth,
        SkillAnimationEvent animationEvent)
    {
        this.frameUnitWidth = frameUnitWidth;
        this.frameIndex = frameIndex;
        this.track = track;
        this.animationEvent = animationEvent;
        trackItemStyle = new SkillAnimationTrackItemStyle();
        itemStyle = trackItemStyle;
        trackItemStyle.Init(parentTrackStytle);

        trackItemStyle.MainDragArea.RegisterCallback<MouseMoveEvent>(MouseMove);
        trackItemStyle.MainDragArea.RegisterCallback<MouseOutEvent>(MouseOut);
        trackItemStyle.MainDragArea.RegisterCallback<MouseUpEvent>(MouseUp);
        trackItemStyle.MainDragArea.RegisterCallback<MouseDownEvent>(MouseDown);

        normalColor = new Color(0.25f, 0.62f, 1f, 1f);
        selectColor = new Color(0.16f, 0.5f, 1f, 0.85f);
        OnUnSelect();

        ResetView(frameUnitWidth);
    }

    public override void ResetView(float frameUnitWidth)
    {
        base.ResetView(frameUnitWidth);
        trackItemStyle.ResetView(frameIndex, frameUnitWidth, animationEvent);

        int animationClipFrameCount =
            (int)(animationEvent.AnimationClip.length * animationEvent.AnimationClip.frameRate);
        //计算动画结束线的位置
        if (animationClipFrameCount > animationEvent.DurationFrame)
        {
            trackItemStyle.AnimationOverLine.style.display = DisplayStyle.None;
        }
        else
        {
            trackItemStyle.AnimationOverLine.style.display = DisplayStyle.Flex;
            Vector3 overLinePos = trackItemStyle.AnimationOverLine.transform.position;
            overLinePos.x = animationClipFrameCount * frameUnitWidth -
                            trackItemStyle.AnimationOverLine.style.width.value.value; //线条自身宽度为2 
            trackItemStyle.AnimationOverLine.transform.position = overLinePos;
        }
        track.TickView(SkillEditorWindow.Instance.CurrentSelectFrameIndex);
    }

    #region 鼠标交互

    private bool mouseDrag = false;
    private float startDragPosX;
    private int startDragFrameIndex;

    /// <summary>
    /// 鼠标移动
    /// 如果是按下状态，那么调用它来使得可以移动动画片段
    /// </summary>
    private void MouseMove(MouseMoveEvent evt)
    {
        if (mouseDrag)
        {
            float offsetPos = evt.mousePosition.x - startDragPosX;
            int offsetFrame = Mathf.RoundToInt(offsetPos / frameUnitWidth);
            int targetFrameIndex = startDragFrameIndex + offsetFrame;
            bool checkDrag = false;
            if (targetFrameIndex < 0) return;
            if (offsetFrame < 0) checkDrag = track.CheckFrameIndexOnDrag(targetFrameIndex, startDragFrameIndex, true);
            else if (offsetFrame > 0)
                checkDrag = track.CheckFrameIndexOnDrag(targetFrameIndex + animationEvent.DurationFrame,
                    startDragFrameIndex, false);
            else return;

            if (checkDrag)
            {
                //确定修改的数据
                //刷新视图
                frameIndex = targetFrameIndex;
                //超过右侧边界，自动拓展边界
                CheckFrameCount();

                //刷新视图
                ResetView(frameUnitWidth);
            }
        }
    }

    /// <summary>
    /// 鼠标移出
    /// 如果是按下状态，应当结算拖动后的更改
    /// </summary>
    private void MouseOut(MouseOutEvent evt)
    {
        if (mouseDrag) ApplyDrag();
        mouseDrag = false;
    }

    /// <summary>
    /// 鼠标按下
    /// 进入按下状态，之后可以移动动画片段
    /// </summary>
    private void MouseDown(MouseDownEvent evt)
    {
        startDragPosX = evt.mousePosition.x;
        startDragFrameIndex = frameIndex;
        mouseDrag = true;
        Select();
    }

    /// <summary>
    /// 鼠标抬起
    /// 结算拖动后的更改
    /// </summary>
    private void MouseUp(MouseUpEvent evt)
    {
        if (mouseDrag) ApplyDrag();
        mouseDrag = false;
    }

    /// <summary>
    /// 对数据应用拖动后的改动
    /// </summary>
    private void ApplyDrag()
    {
        if (startDragFrameIndex != frameIndex)
        {
            track.SetFrameIndex(startDragFrameIndex, frameIndex);
            SkillEditorInspector.Instance.SetTrackItemFrameIndex(frameIndex);
        }
    }

    /// <summary>
    /// 检查帧的数量是否超过边界
    /// 超过会自动拓展
    /// </summary>
    public void CheckFrameCount()
    {
        if (frameIndex + animationEvent.DurationFrame > SkillEditorWindow.Instance.SkillClip.FrameCount)
        {
            SkillEditorWindow.Instance.CurrentFrameCount = frameIndex + animationEvent.DurationFrame;
        }
    }

    #endregion

    public override void OnConfigChanged()
    {
        animationEvent = track.AnimationData.FrameData[FrameIndex];
    }
}
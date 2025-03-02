using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class AudioTrackItem : TrackItemBase<AudioTrack>
{
    private SkillAudioTrackItemStyle trackItemStyle;
    private SkillMultiLineTrackStyle.ChildTrack childTrackStyle;
    private SkillAudioEvent audioEvent; //Item对应的动画帧事件
    public SkillAudioEvent AudioEvent
    {
        get => audioEvent;
    }


    /// <summary>
    /// 初始化音效轨道片段
    /// </summary>
    public void Init(AudioTrack track, float frameUnitWidth, SkillAudioEvent audioEvent,
        SkillMultiLineTrackStyle.ChildTrack childTrackStyle)
    {
        this.track = track;
        this.frameIndex = audioEvent.FrameIndex;
        this.childTrackStyle = childTrackStyle;
        this.audioEvent = audioEvent;

        trackItemStyle = new SkillAudioTrackItemStyle();
        itemStyle = trackItemStyle;

        childTrackStyle.contentRoot.RegisterCallback<DragUpdatedEvent>(OnDragUpdate);
        childTrackStyle.contentRoot.RegisterCallback<DragExitedEvent>(OnDragExit);

        normalColor = new Color(0.25f, 0.62f, 1f, 1f);
        selectColor = new Color(0.16f, 0.5f, 1f, 0.85f);
        ResetView(frameUnitWidth);
    }

    /// <summary>
    /// 刷新音频片段视图
    /// 如果没有初始化，会自动初始化
    /// </summary>
    /// <param name="frameUnitWidth"></param>
    public override void ResetView(float frameUnitWidth)
    {
        base.ResetView(frameUnitWidth);

        if (audioEvent.AudioClip != null)
        {
            if (!trackItemStyle.isInit)
            {
                trackItemStyle.Init(childTrackStyle);
                trackItemStyle.MainDragArea.RegisterCallback<MouseMoveEvent>(MouseMove);
                trackItemStyle.MainDragArea.RegisterCallback<MouseOutEvent>(MouseOut);
                trackItemStyle.MainDragArea.RegisterCallback<MouseUpEvent>(MouseUp);
                trackItemStyle.MainDragArea.RegisterCallback<MouseDownEvent>(MouseDown);
            }
        }
        trackItemStyle.ResetView(frameUnitWidth, audioEvent);
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

            if (targetFrameIndex < 0 || offsetFrame == 0) return;

            //确定修改的数据
            //刷新视图
            frameIndex = targetFrameIndex;
            audioEvent.FrameIndex = frameIndex;
            //超过右侧边界，自动拓展边界
            //CheckFrameCount();

            //刷新视图
            ResetView(frameUnitWidth);
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
            audioEvent.FrameIndex = frameIndex;
            SkillEditorInspector.Instance.SetTrackItemFrameIndex(frameIndex);
        }
    }

    /// <summary>
    /// 检查帧的数量是否超过边界
    /// 超过会自动拓展
    /// </summary>
    public void CheckFrameCount()
    {
        int frameCount = (int)(audioEvent.AudioClip.length * SkillEditorWindow.Instance.SkillClip.FrameRate);
        if (frameIndex + frameCount > SkillEditorWindow.Instance.SkillClip.FrameCount)
        {
            SkillEditorWindow.Instance.CurrentFrameCount = frameIndex + frameCount;
        }
    }

    #endregion

    public void Destroy()
    {
        childTrackStyle.Destroy();
    }

    /// <summary>
    /// 设置子轨道名称
    /// </summary>
    public void SetTrackName(string name)
    {
        childTrackStyle.SetTrackName(name);
    }

    #region 拖拽资源

    /// <summary>
    /// 鼠标拖动资源事件
    /// 拖动资源必须是音频
    /// 为了能够将Project中的音频资源拖入轨道后自动生成
    /// </summary>
    private void OnDragUpdate(DragUpdatedEvent evt)
    {
        //监听用户拖拽的是否是动画
        UnityEngine.Object[] objs = DragAndDrop.objectReferences;
        if (objs.Length > 0)
        {
            AudioClip[] clips = objs.OfType<AudioClip>().ToArray();
            if (clips.Length == objs.Length)
            {
                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
            }
        }
    }

    /// <summary>
    /// 鼠标拖动结束事件
    /// 为了在资源拖拽结束后
    /// 能够将Project中的音频资源拖入轨道后自动生成
    /// 会自动做校验(比如资源中有非音频类型，位置不正确等)
    /// </summary>
    private void OnDragExit(DragExitedEvent evt)
    {
        UnityEngine.Object[] objs = DragAndDrop.objectReferences;
        //没有拖拽的数据就返回
        if (objs.Length == 0)
        {
            return;
        }

        AudioClip[] clips = objs.OfType<AudioClip>().ToArray();
        //拖拽数据中有非AudioClip类型
        if (clips.Length != objs.Length)
        {
            return;
        }

        int selectFrameIndex = SkillEditorWindow.Instance.GetFrameIndexByPos(evt.localMousePosition.x);
        if (selectFrameIndex >= 0)
        {
            //构建默认的音效数据
            audioEvent.AudioClip = clips[0];
            audioEvent.FrameIndex = selectFrameIndex;
            this.frameIndex = selectFrameIndex;
            ResetView();
            SkillEditorWindow.Instance.SaveClip();
        }
    }

    #endregion
}
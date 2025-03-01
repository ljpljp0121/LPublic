using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// 动画轨道
/// </summary>
public class AnimationTrack : SkillTrackBase
{
    private SkillSingleLineTrackStyle trackStyle;
    private readonly Dictionary<int, AnimationTrackItem> trackItemDic = new Dictionary<int, AnimationTrackItem>();

    /// <summary>
    /// 动画数据
    /// </summary>
    public SkillAnimationData AnimationData => SkillEditorWindow.Instance.SkillClip.SkillAnimationData;


    public override void Init(VisualElement menuParent, VisualElement contentParent, float frameWidth)
    {
        base.Init(menuParent, contentParent, frameWidth);
        trackStyle = new SkillSingleLineTrackStyle();
        trackStyle.Init(menuParent, contentParent, "动画配置");
        trackStyle.contentRoot.RegisterCallback<DragUpdatedEvent>(OnDragUpdate);
        trackStyle.contentRoot.RegisterCallback<DragExitedEvent>(OnDragExit);
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
        foreach (var item in AnimationData.FrameData)
        {
            CreateItem(item.Key, item.Value);
        }
    }

    /// <summary>
    /// 创建动画片段
    /// </summary>
    private void CreateItem(int frameIndex, SkillAnimationEvent skillAnimationEvent)
    {
        AnimationTrackItem trackItem = new AnimationTrackItem();
        trackItem.Init(this, trackStyle, frameIndex, frameWidth, skillAnimationEvent);
        trackItemDic.Add(frameIndex, trackItem);
    }

    #region 拖拽资源

    /// <summary>
    /// 鼠标拖动资源事件
    /// 拖动资源必须时动画，可以同时选中多个资源拖拽
    /// 为了能够将Project中的动画资源拖入轨道后自动生成
    /// </summary>
    private void OnDragUpdate(DragUpdatedEvent evt)
    {
        //监听用户拖拽的是否是动画
        UnityEngine.Object[] objs = DragAndDrop.objectReferences;
        if (objs.Length > 0)
        {
            AnimationClip[] clips = objs.OfType<AnimationClip>().ToArray();
            if (clips.Length == objs.Length)
            {
                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
            }
        }
    }

    /// <summary>
    /// 鼠标拖动结束事件
    /// 为了在资源拖拽结束后(可以同时选中多个资源拖拽)
    /// 能够将Project中的动画资源拖入轨道后自动生成
    /// 会自动做校验(比如资源中有非动画类型，位置不正确等)
    /// </summary>
    private void OnDragExit(DragExitedEvent evt)
    {
        UnityEngine.Object[] objs = DragAndDrop.objectReferences;
        //没有拖拽的数据就返回
        if (objs.Length == 0)
        {
            return;
        }

        AnimationClip[] clips = objs.OfType<AnimationClip>().ToArray();
        //拖拽数据中有非AnimationClip类型
        if (clips.Length != objs.Length)
        {
            return;
        }

        //放置动画资源
        //当前选中位置检测能否放置动画
        int selectFrameIndex = SkillEditorWindow.Instance.GetFrameIndexByPos(evt.localMousePosition.x);
        bool canPlace = true;
        //选中的资源们的持续帧数，如果为-1代表可以用原本AnimationClip的持续时间
        int[] durationFrame = new int[clips.Length];
        for (int i = 0; i < durationFrame.Length; i++)
        {
            durationFrame[i] = -1;
        }

        //每一个资源原本持续的帧数
        int[] clipFrameCount = new int[clips.Length];
        for (int i = 0; i < clipFrameCount.Length; i++)
        {
            clipFrameCount[i] = (int)(clips[i].length * clips[i].frameRate);
        }

        int nextTrackItem = -1;
        int currentOffset = int.MaxValue;

        foreach (var item in AnimationData.FrameData)
        {
            //不允许选中帧在TrackItem中间(动画事件的起点到终点之间)
            if (selectFrameIndex > item.Key && selectFrameIndex < item.Value.DurationFrame + item.Key)
            {
                //不能放置
                canPlace = false;
                break;
            }
            //找到当前选中位置右侧最近的TrackItem
            if (item.Key > selectFrameIndex)
            {
                int tempOffset = item.Key - selectFrameIndex;
                if (tempOffset < currentOffset)
                {
                    currentOffset = tempOffset;
                    nextTrackItem = item.Key;
                }
            }
        }

        //实际的放置
        if (canPlace)
        {
            //如果右边有其他TrackItem，要考虑Track不能重叠的问题
            //右边没有其他TrackItem
            if (nextTrackItem == -1)
            {
                for (int i = 0; i < clips.Length; i++)
                {
                    durationFrame[i] = clipFrameCount[i];
                }
            }
            //右边有
            else
            {
                //设置每一个资源应当持续的帧数
                for (int i = 0; i < clips.Length; i++)
                {
                    int offset = clipFrameCount[i] - currentOffset;
                    if (offset < 0)
                    {
                        durationFrame[i] = clipFrameCount[i];
                    }
                    else
                    {
                        durationFrame[i] = offset;
                        //第i个资源都已经不能完全放下，i后面剩下的直接不需要了
                        Array.Resize(ref clips, i + 1);
                        break;
                    }
                    currentOffset -= clipFrameCount[i];
                }
            }

            //构建拖入资源的动画数据
            for (int i = 0; i < clips.Length; i++)
            {
                SkillAnimationEvent animationEvent = new SkillAnimationEvent()
                {
                    AnimationClip = clips[i],
                    DurationFrame = durationFrame[i],
                    TransitionTime = 0.25f,
                };
                AnimationData.FrameData.Add(selectFrameIndex, animationEvent);
                SkillEditorWindow.Instance.SaveClip();

                //创建一个新的Item
                CreateItem(selectFrameIndex, animationEvent);
                selectFrameIndex += durationFrame[i];
            }
        }
    }

    /// <summary>
    /// 检查拖拽后的帧索引是否有效
    /// 即不允许targetIndex在某个TrackItem中间
    /// </summary>
    /// <returns></returns>
    public bool CheckFrameIndexOnDrag(int targetIndex, int selfIndex, bool isLeft)
    {
        foreach (var item in AnimationData.FrameData)
        {
            if (item.Key == selfIndex) continue;

            //向左移动&&原先在其右边 && 目标没有重叠
            if (isLeft && selfIndex > item.Key && targetIndex < item.Key + item.Value.DurationFrame)
            {
                return false;
            }
            else if (!isLeft && selfIndex < item.Key && targetIndex > item.Key)
            {
                return false;
            }
        }
        return true;
    }

    /// <summary>
    /// 将oldIndex的数据转移到newIndex
    /// 在拖拽动画片段结束后调用它更新片段的帧索引
    /// </summary>
    /// <param name="oldIndex"></param>
    /// <param name="newIndex"></param>
    public void SetFrameIndex(int oldIndex, int newIndex)
    {
        if (AnimationData.FrameData.Remove(oldIndex, out SkillAnimationEvent animationEvent))
        {
            AnimationData.FrameData.Add(newIndex, animationEvent);
            trackItemDic.Remove(oldIndex, out AnimationTrackItem item);
            trackItemDic.Add(newIndex, item);
        }
    }

    #endregion

    /// <summary>
    /// 删除指定帧索引处的动画片段
    /// </summary>
    /// <param name="frameIndex"></param>
    public override void DeleteTrackItem(int frameIndex)
    {
        AnimationData.FrameData.Remove(frameIndex);
        if (trackItemDic.Remove(frameIndex, out AnimationTrackItem item))
        {
            trackStyle.RemoveItem(item.itemStyle.Root);
        }
    }

    /// <summary>
    /// 轨道每次修改时应当调用它，
    /// 保证轨道数据是实时的
    /// </summary>
    public override void OnConfigChanged()
    {
        foreach (var item in trackItemDic.Values)
        {
            item.OnConfigChanged();
        }
    }

    /// <summary>
    /// 重写方法:
    /// 驱动动画视图
    /// 根据给定的帧索引去采样动画轨道并体现到视图中
    /// 视图支持根运动的
    /// </summary>
    public override void TickView(int frameIndex)
    {
        GameObject previewGameObject = SkillEditorWindow.Instance.PreviewCharacterObj;
        if (previewGameObject != null)
        {
            UpdatePosture(frameIndex);
            previewGameObject.transform.position = GetPositionForRootMotion(frameIndex);
        }
    }

    /// <summary>
    /// 获取根运动下当前帧所在的位置
    /// </summary>
    public Vector3 GetPositionForRootMotion(int frameIndex, bool recover = false)
    {
        GameObject previewGameObject = SkillEditorWindow.Instance.PreviewCharacterObj;
        Animator animator = previewGameObject.GetComponentInChildren<Animator>();
        //根据帧找到目前是哪个动画
        Dictionary<int, SkillAnimationEvent> frameData = AnimationData.FrameData;

        Vector3 rootMotionTotalPos = Vector3.zero;
        //利用排序字典
        SortedDictionary<int, SkillAnimationEvent> frameDataSortedDic =
            new SortedDictionary<int, SkillAnimationEvent>(frameData);
        int[] keys = frameDataSortedDic.Keys.ToArray();
        for (int i = 0; i < keys.Length; i++)
        {
            int keyFrame = keys[i];
            SkillAnimationEvent animationEvent = frameDataSortedDic[keyFrame];
            //只考虑根运动配置的动画
            if (animationEvent.ApplyRootMotion == false) continue;

            int nextKeyFrame = 0;
            if (i + 1 < keys.Length)
                nextKeyFrame = keys[i + 1];
            //最后一个动画
            else
                nextKeyFrame = SkillEditorWindow.Instance.SkillClip.FrameCount;

            bool isBreak = false; //标记是最后一次采样
            if (nextKeyFrame > frameIndex)
            {
                nextKeyFrame = frameIndex;
                isBreak = true;
            }
            //持续的帧数 = 下一个动画的起始帧数 - 这个动画的起始帧数
            int durationFrameCount = nextKeyFrame - keyFrame;
            if (durationFrameCount > 0)
            {
                //动画资源总帧数
                float clipFrameCount = animationEvent.AnimationClip.length *
                                       SkillEditorWindow.Instance.SkillClip.FrameRate;
                //计算总的的播放进度
                float totalProgress = durationFrameCount / clipFrameCount;
                //播放次数
                int playTimes = 0;
                //最终不完整的一次播放
                float lastProgress = 0;
                //只有循环动画才能采样多次
                if (animationEvent.AnimationClip.isLooping)
                {
                    playTimes = (int)totalProgress;
                    lastProgress = totalProgress - (int)totalProgress;
                }
                else
                {
                    //不循环的动画，如果播放进度超过1，约束为1
                    //进度没超过1，那么播放次数为0，lastProgress为 totalProgress.
                    if (totalProgress < 1f)
                    {
                        lastProgress = totalProgress;
                        playTimes = 0;
                    }
                    else if (totalProgress >= 1f)
                    {
                        playTimes = 1;
                        lastProgress = 0;
                    }
                }
                //采样计算
                animator.applyRootMotion = true;
                if (playTimes >= 1)
                {
                    //采样一次动画的完整进度
                    animationEvent.AnimationClip.SampleAnimation(previewGameObject,
                        animationEvent.AnimationClip.length);
                    Vector3 samplePos = previewGameObject.transform.position;
                    rootMotionTotalPos += samplePos * playTimes;
                }
                if (lastProgress > 0)
                {
                    //采样一次动画的不完整进度
                    animationEvent.AnimationClip.SampleAnimation(previewGameObject,
                        lastProgress * animationEvent.AnimationClip.length);
                    rootMotionTotalPos += previewGameObject.transform.position;
                }
            }

            if (isBreak) break;
        }
        if (recover)
        {
            UpdatePosture(SkillEditorWindow.Instance.CurrentSelectFrameIndex);
        }
        return rootMotionTotalPos;
    }

    /// <summary>
    /// 更新当前帧的姿态
    /// </summary>
    private void UpdatePosture(int frameIndex)
    {
        GameObject previewGameObject = SkillEditorWindow.Instance.PreviewCharacterObj;
        Animator animator = previewGameObject.GetComponent<Animator>();
        //根据帧找到目前是哪个动画
        Dictionary<int, SkillAnimationEvent> frameData = AnimationData.FrameData;

        //找到距离这一帧左边最近的一个动画，也就是当前要播放的动画
        int currentOffset = int.MaxValue; //最近的索引距离当前选中帧的偏移量
        int animationEventIndex = -1;
        int tempOffset;
        foreach (var item in frameData)
        {
            tempOffset = frameIndex - item.Key;
            if (item.Key < frameIndex && tempOffset < currentOffset)
            {
                currentOffset = tempOffset;
                animationEventIndex = item.Key;
            }
        }
        if (animationEventIndex != -1)
        {
            SkillAnimationEvent animationEvent = frameData[animationEventIndex];
            //动画资源总帧数
            float clipFrameCount = animationEvent.AnimationClip.length * animationEvent.AnimationClip.frameRate;
            //计算当前播放进度
            float progress = currentOffset / clipFrameCount;
            //循环动画的处理
            if (progress > 1 && animationEvent.AnimationClip.isLooping)
            {
                progress -= (int)progress;
            }
            animator.applyRootMotion = animationEvent.AnimationClip.hasRootCurves;
            //TODO 播放动画
            // SkillPlayerComponent skillPlayer = previewGameObject.GetComponent<SkillPlayerComponent>();
            animationEvent.AnimationClip.SampleAnimation(previewGameObject,
                progress * animationEvent.AnimationClip.length);
        }
    }

    public override void Destroy()
    {
        trackStyle.Destroy();
    }
}
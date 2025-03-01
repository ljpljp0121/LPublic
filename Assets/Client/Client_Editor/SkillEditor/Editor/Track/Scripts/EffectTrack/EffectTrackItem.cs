using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// 特效轨道片段
/// </summary>
public class EffectTrackItem : TrackItemBase<EffectTrack>
{
    private SkillEffectTrackItemStyle trackItemStyle;
    private SkillMultiLineTrackStyle.ChildTrack childTrackStyle;
    public SkillEffectEvent EffectEvent { get; private set; }


    /// <summary>
    /// 初始化音效轨道片段
    /// </summary>
    public void Init(EffectTrack track, float frameUnitWidth, SkillEffectEvent effectEvent,
        SkillMultiLineTrackStyle.ChildTrack childTrackStyle)
    {
        this.track = track;
        this.frameIndex = effectEvent.FrameIndex;
        this.childTrackStyle = childTrackStyle;
        this.EffectEvent = effectEvent;

        trackItemStyle = new SkillEffectTrackItemStyle();
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

        if (EffectEvent.EffectPrefab != null)
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
        trackItemStyle.ResetView(frameUnitWidth, EffectEvent);

        //强行重新生成预览
        CleanEffectPreviewObj();
        TickView(SkillEditorWindow.Instance.CurrentSelectFrameIndex);
    }

    /// <summary>
    /// 销毁子轨道
    /// </summary>
    public void Destroy()
    {
        CleanEffectPreviewObj();
        childTrackStyle.Destroy();
    }

    /// <summary>
    /// 清理生成的粒子预制体对象
    /// </summary>
    public void CleanEffectPreviewObj()
    {
        if (effectPreviewObj != null)
        {
            Object.DestroyImmediate(effectPreviewObj);
            effectPreviewObj = null;
        }
    }

    /// <summary>
    /// 设置子轨道名称
    /// </summary>
    public void SetTrackName(string name)
    {
        childTrackStyle.SetTrackName(name);
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
            EffectEvent.FrameIndex = frameIndex;

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
            EffectEvent.FrameIndex = frameIndex;
            SkillEditorInspector.Instance.SetTrackItemFrameIndex(frameIndex);
        }
    }

    #endregion

    #region 拖拽资源

    /// <summary>
    /// 鼠标拖动资源事件
    /// 拖动资源必须是特效
    /// 为了能够将Project中的特效资源拖入轨道后自动生成
    /// </summary>
    private void OnDragUpdate(DragUpdatedEvent evt)
    {
        //监听用户拖拽的是否是动画
        UnityEngine.Object[] objs = DragAndDrop.objectReferences;
        if (objs.Length > 0)
        {
            GameObject[] prefabs = objs.OfType<GameObject>().ToArray();
            if (prefabs.Length == objs.Length)
            {
                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
            }
        }
    }

    /// <summary>
    /// 鼠标拖动结束事件
    /// 为了在资源拖拽结束后
    /// 能够将Project中的特效资源拖入轨道后自动生成
    /// 会自动做校验(比如资源中有非特效类型，位置不正确等)
    /// </summary>
    private void OnDragExit(DragExitedEvent evt)
    {
        UnityEngine.Object[] objs = DragAndDrop.objectReferences;
        //没有拖拽的数据就返回
        if (objs.Length == 0)
        {
            return;
        }

        GameObject[] prefabs = objs.OfType<GameObject>().ToArray();
        //拖拽数据中有非AudioClip类型
        if (prefabs.Length != objs.Length)
        {
            return;
        }

        int selectFrameIndex = SkillEditorWindow.Instance.GetFrameIndexByPos(evt.localMousePosition.x);
        if (selectFrameIndex >= 0)
        {
            //构建默认的特效数据
            EffectEvent.FrameIndex = selectFrameIndex;
            EffectEvent.EffectPrefab = prefabs[0];
            EffectEvent.Position = Vector3.zero;
            EffectEvent.Rotation = Vector3.zero;
            EffectEvent.Scale = Vector3.one;
            EffectEvent.AutoDestroy = true;

            ParticleSystem[] particleSystems = prefabs[0].GetComponentsInChildren<ParticleSystem>();
            int max = -1;
            for (int i = 0; i < particleSystems.Length; i++)
            {
                if (particleSystems[i].main.duration > max)
                {
                    max = (int)(particleSystems[i].main.duration * SkillEditorWindow.Instance.SkillClip.FrameRate);
                }
            }
            EffectEvent.Duration = max;

            this.frameIndex = selectFrameIndex;
            ResetView();
            SkillEditorWindow.Instance.SaveClip();
        }
    }

    #endregion

    #region 预览

    private GameObject effectPreviewObj;

    /// <summary>
    /// 特效片段自己管理自己的特效
    /// 采样资源并呈现 
    /// </summary>
    public void TickView(int frameIndex)
    {
        if (EffectEvent.EffectPrefab == null || SkillEditorWindow.Instance.PreviewCharacterObj == null) return;
        //是不是在播放范围内
        int durationFrame = EffectEvent.Duration;
        if (EffectEvent.FrameIndex <= frameIndex && EffectEvent.FrameIndex + durationFrame > frameIndex)
        {
            Object.DestroyImmediate(effectPreviewObj);
            Transform characterRoot = SkillEditorWindow.Instance.PreviewCharacterObj.transform;

            Vector3 pos = characterRoot.TransformPoint(EffectEvent.Position);
            Vector3 rot = characterRoot.eulerAngles + EffectEvent.Rotation;

            //实例化
            effectPreviewObj = Object.Instantiate(EffectEvent.EffectPrefab, pos, Quaternion.Euler(rot),
                EffectTrack.EffectParent);
            effectPreviewObj.name = EffectEvent.EffectPrefab.name;
            effectPreviewObj.transform.localScale = EffectEvent.Scale;

            //粒子模拟
            ParticleSystem[] particleSystems = effectPreviewObj.GetComponentsInChildren<ParticleSystem>();
            for (int i = 0; i < particleSystems.Length; i++)
            {
                int simulateFrame = frameIndex - EffectEvent.FrameIndex;
                particleSystems[i].Simulate((float)simulateFrame / SkillEditorWindow.Instance.SkillClip.FrameRate);
            }
        }
        else
        {
            CleanEffectPreviewObj();
        }
    }

    /// <summary>
    /// 应用模型Transform属性
    /// </summary>
    public void ApplyModelTransform()
    {
        if (effectPreviewObj != null)
        {
            Transform characterRoot = SkillEditorWindow.Instance.PreviewCharacterObj.transform;
            //获取模拟坐标
            Vector3 rootPos = SkillEditorWindow.Instance.GetPositionForRootMotion(EffectEvent.FrameIndex, true);
            Vector3 oldPos = characterRoot.position;

            //把角色临时设置到播放坐标
            characterRoot.position = rootPos;
            EffectEvent.Position = characterRoot.InverseTransformPoint(effectPreviewObj.transform.position);
            EffectEvent.Rotation = effectPreviewObj.transform.eulerAngles - characterRoot.eulerAngles;
            EffectEvent.Scale = effectPreviewObj.transform.localScale;

            characterRoot.position = oldPos;
        }
    }

    #endregion
}
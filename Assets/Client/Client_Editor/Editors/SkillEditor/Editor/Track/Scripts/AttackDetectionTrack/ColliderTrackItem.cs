using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;


public class ColliderTrackItem : TrackItemBase<ColliderTrack>
{
    private SkillColliderTrackItemStyle trackItemStyle;
    private SkillMultiLineTrackStyle.ChildTrack childTrackStyle;
    private SkillColliderEvent colliderEvent;
    public SkillColliderEvent ColliderEvent => colliderEvent;

    /// <summary>
    /// 初始化攻击检测轨道片段
    /// </summary>
    public void Init(ColliderTrack track, float frameUnitWidth, SkillColliderEvent attackDetectionEvent,
        SkillMultiLineTrackStyle.ChildTrack childTrackStyle)
    {
        this.track = track;
        this.frameIndex = attackDetectionEvent.FrameIndex;
        this.childTrackStyle = childTrackStyle;
        this.colliderEvent = attackDetectionEvent;
        trackItemStyle = new SkillColliderTrackItemStyle();
        itemStyle = trackItemStyle;

        normalColor = new Color(0.25f, 0.62f, 1f, 1f);
        selectColor = new Color(0.16f, 0.5f, 1f, 0.85f);
        ResetView(frameUnitWidth);
    }

    /// <summary>
    /// 刷新攻击检测片段视图
    /// 如果没有初始化，会自动初始化
    /// </summary>
    /// <param name="frameUnitWidth"></param>
    public override void ResetView(float frameUnitWidth)
    {
        base.ResetView(frameUnitWidth);

        if (!trackItemStyle.IsInit)
        {
            trackItemStyle.Init(childTrackStyle);
            trackItemStyle.MainDragArea.RegisterCallback<MouseMoveEvent>(MouseMove);
            trackItemStyle.MainDragArea.RegisterCallback<MouseOutEvent>(MouseOut);
            trackItemStyle.MainDragArea.RegisterCallback<MouseUpEvent>(MouseUp);
            trackItemStyle.MainDragArea.RegisterCallback<MouseDownEvent>(MouseDown);
        }
        trackItemStyle.ResetView(frameUnitWidth, colliderEvent);
    }

    /// <summary>
    /// 销毁轨道片段
    /// </summary>
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

    #region 鼠标交互

    private bool mouseDrag = false;
    private float startDragPosX;
    private int startDragFrameIndex;

    /// <summary>
    /// 鼠标移动
    /// 如果是按下状态，那么调用它来使得可以移动攻击检测片段
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
            colliderEvent.FrameIndex = frameIndex;
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
            colliderEvent.FrameIndex = frameIndex;
            SkillEditorInspector.Instance.SetTrackItemFrameIndex(frameIndex);
        }
    }

    /// <summary>
    /// 检查帧的数量是否超过边界
    /// 超过会自动拓展
    /// </summary>
    public void CheckFrameCount()
    {
        int frameCount = colliderEvent.DurationFrame;
        if (frameIndex + frameCount > SkillEditorWindow.Instance.SkillClip.FrameCount)
        {
            SkillEditorWindow.Instance.CurrentFrameCount = frameIndex + frameCount;
        }
    }

    #endregion

    #region Gizmos和SceneGUI绘制

    /// <summary>
    /// 绘制当前轨道片段需要在场景中呈现的一些事物
    /// 比如攻击检测的范围等
    /// </summary>
    public void DrawGizmos()
    {
        SkillGizmosTool.DrawDetection(colliderEvent,
            SkillEditorWindow.Instance.PreviewCharacterObj.GetComponent<SkillEditorPlayer>());
    }

    /// <summary>
    /// 绘制当前轨道片段需要在场景中呈现的GUI
    /// 比如可以移动场景中范围的三个箭头等
    /// </summary>
    public void OnSceneGUI()
    {
        Transform transform = SkillEditorWindow.Instance.PreviewCharacterObj.transform;
        switch (colliderEvent.SkillColliderType)
        {
            case SkillColliderType.Box:
                BoxSkillCollider boxDetection = (BoxSkillCollider)colliderEvent.SkillColliderData;
                Quaternion boxQuaternion = transform.rotation * Quaternion.Euler(boxDetection.Rotation);
                Vector3 boxPos = transform.TransformPoint(boxDetection.Position);
                EditorGUI.BeginChangeCheck();
                Handles.TransformHandle(ref boxPos, ref boxQuaternion, ref boxDetection.Scale);
                if (EditorGUI.EndChangeCheck())
                {
                    boxDetection.Position = transform.InverseTransformPoint(boxPos);
                    boxDetection.Rotation = (boxQuaternion * Quaternion.Inverse(transform.rotation)).eulerAngles;
                    SkillEditorInspector.SetTrackItem(this, track);
                }
                break;
            case SkillColliderType.Sphere:
                SphereSkillCollider sphereDetection = (SphereSkillCollider)colliderEvent.SkillColliderData;
                Vector3 spherePos = transform.TransformPoint(sphereDetection.Position);
                Quaternion sphereQuaternion = Quaternion.identity;
                EditorGUI.BeginChangeCheck();
                Handles.TransformHandle(ref spherePos, ref sphereQuaternion, ref sphereDetection.Radius);
                if (EditorGUI.EndChangeCheck())
                {
                    sphereDetection.Position = transform.InverseTransformPoint(spherePos);
                    SkillEditorInspector.SetTrackItem(this, track);
                }
                break;
            case SkillColliderType.Fan:
                FanSkillCollider fanDetection = (FanSkillCollider)colliderEvent.SkillColliderData;
                Quaternion fanQuaternion = transform.rotation * Quaternion.Euler(fanDetection.Rotation);
                Vector3 fanPos = transform.TransformPoint(fanDetection.Position);
                //x:角度 y:高度 z:外圈半径
                Vector3 fanScale = new Vector3(fanDetection.Angle, fanDetection.Height, fanDetection.OutsideRadius);
                EditorGUI.BeginChangeCheck();
                Handles.TransformHandle(ref fanPos, ref fanQuaternion, ref fanScale);
                float insideRadiusHandle = Handles.ScaleSlider(fanDetection.InsideRadius, fanPos, -transform.forward,
                    Quaternion.identity, 1.5f, 0.1f);
                if (EditorGUI.EndChangeCheck())
                {
                    fanDetection.Position = transform.InverseTransformPoint(fanPos);
                    fanDetection.Rotation = (fanQuaternion * Quaternion.Inverse(transform.rotation)).eulerAngles;
                    fanDetection.Angle = fanScale.x;
                    fanDetection.Height = fanScale.y;
                    fanDetection.OutsideRadius = fanScale.z;
                    fanDetection.InsideRadius = insideRadiusHandle;
                    SkillEditorInspector.SetTrackItem(this, track);
                }
                break;
        }
    }

    #endregion
}
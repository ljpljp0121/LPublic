using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class SkillEditorWindow : EditorWindow
{
    public static SkillEditorWindow Instance;

    public const string SkillEditorAssetPath = "Assets/Client/Client_Editor/SkillEditor/Editor/Track/Assets/";

    [MenuItem("Project/技能系统/技能编辑器")]
    public static void ShowSkillEditorWindow()
    {
        SkillEditorWindow wnd = GetWindow<SkillEditorWindow>();
        wnd.titleContent = new GUIContent("技能编辑器");
    }

    private VisualElement root;

    /// <summary>
    /// 绘制GUI
    /// </summary>
    public void CreateGUI()
    {
        Instance = this;
        root = rootVisualElement;
        var visualTree =
            AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(SkillEditorAssetPath + "SkillEditorWindow.uxml");
        VisualElement labelFromUXML = visualTree.Instantiate();
        root.Add(labelFromUXML);
        InitTopMenu();
        InitTimeShaft();
        InitConsole();
        InitContent();
        if (SkillClip != null)
        {
            SkillConfigField.value = SkillClip;
            CurrentFrameCount = SkillClip.FrameCount;
        }
        else
        {
            CurrentFrameCount = 100;
        }
        if (PreviewCharacterObj != null)
        {
            PreviewCharacterField.value = PreviewCharacterObj;
        }
        CurrentSelectFrameIndex = 0;
    }

    #region 编辑器变量

    public GameObject PreviewCharacterObj { get; private set; } //预览角色对象
    public SkillClip SkillClip { get; private set; } //技能配置文件
    private readonly SkillEditorConfig skillEditorConfig = new SkillEditorConfig(); //编辑器配置
    private int currentSelectFrameIndex = -1; //当前选中的帧索引

    /// <summary>
    /// 当前选中的帧索引
    /// 给帧索引赋值更新值时，它的值不会超过帧数总值，也不会小于0
    /// 如果超过了会自动扩容帧数总值
    /// </summary>
    public int CurrentSelectFrameIndex
    {
        get => currentSelectFrameIndex;
        private set
        {
            int old = currentSelectFrameIndex;
            //如果超出范围，更新最大帧
            if (value > CurrentFrameCount)
            {
                CurrentFrameCount = value;
            }
            currentSelectFrameIndex = Mathf.Clamp(value, 0, CurrentFrameCount);
            CurrentFrameField.value = currentSelectFrameIndex;
            if (old != currentSelectFrameIndex)
            {
                UpdateTimerShaftView();
                TickSkill();
            }
        }
    }

    private int currentFrameCount;
    /// <summary>
    ///当前SkillConfig的帧数总值
    ///修改时会自动同步给SkillConfig
    /// </summary>
    public int CurrentFrameCount
    {
        get => currentFrameCount;
        set
        {
            currentFrameCount = value;
            FrameCountField.value = currentFrameCount;
            //同步给SkillConfig
            if (SkillClip != null)
            {
                SkillClip.FrameCount = currentFrameCount;
            }
            UpdateContentSize();
        }
    }

    #endregion

    #region 头部菜单TopMenu

    private const string SKILL_EDITOR_SCENE_PATH = SkillEditorAssetPath + "SkillEditorScene.unity";
    private const string RUN_TIME_SCENE_PATH = "Assets/Scenes/StartScene.unity";
    private string oldScenePath = "";

    private Button LoadEditorSceneBtn;
    private Button LoadOldSceneBtn;
    private Button SkillBasicBtn;
    private Button ResetTrackBtn;
    private ObjectField PreviewCharacterPrefabField;
    private ObjectField PreviewCharacterField;
    private ObjectField SkillConfigField;

    /// <summary>
    /// 初始化头部菜单
    /// </summary>
    private void InitTopMenu()
    {
        LoadEditorSceneBtn = root.Q<Button>(nameof(LoadEditorSceneBtn));
        LoadEditorSceneBtn.clicked += LoadEditorSceneBtnClick;

        LoadOldSceneBtn = root.Q<Button>(nameof(LoadOldSceneBtn));
        LoadOldSceneBtn.clicked += LoadOldSceneBtnClick;

        SkillBasicBtn = root.Q<Button>(nameof(SkillBasicBtn));
        SkillBasicBtn.clicked += SkillBasicBtnClick;

        ResetTrackBtn = root.Q<Button>(nameof(ResetTrackBtn));
        ResetTrackBtn.clicked += ResetTrackBtnClick;

        PreviewCharacterField = root.Q<ObjectField>(nameof(PreviewCharacterField));
        PreviewCharacterField.RegisterValueChangedCallback(PreviewCharacterFieldValueChanged);

        SkillConfigField = root.Q<ObjectField>(nameof(SkillConfigField));
        SkillConfigField.objectType = typeof(SkillClip);
        SkillConfigField.RegisterValueChangedCallback(SkillConfigFieldValueChanged);
    }

    /// <summary>
    /// 是否在编辑器场景
    /// </summary>
    public bool IsEditorScene
    {
        get
        {
            string currentScenePath = SceneManager.GetActiveScene().path;
            return currentScenePath == SKILL_EDITOR_SCENE_PATH;
        }
    }

    /// <summary>
    ///加载编辑器场景
    ///根据给的skillEditorScenePath参数来加载编辑器场景
    /// </summary>
    private void LoadEditorSceneBtnClick()
    {
        string currentScenePath = SceneManager.GetActiveScene().path;
        //避免新旧都是编辑器场景
        if (currentScenePath != SKILL_EDITOR_SCENE_PATH)
        {
            oldScenePath = currentScenePath;
            EditorSceneManager.OpenScene(SKILL_EDITOR_SCENE_PATH);
        }
    }

    /// <summary>
    /// 回到原来场景
    /// 默认回到启动场景
    /// </summary>
    private void LoadOldSceneBtnClick()
    {
        if (!string.IsNullOrEmpty(oldScenePath))
        {
            string currentScenePath = EditorSceneManager.GetActiveScene().path;
            //只有当前场景和旧场景不是同一个场景才有切换意义
            if (currentScenePath != oldScenePath)
            {
                EditorSceneManager.OpenScene(oldScenePath);
            }
        }
        else
        {
            oldScenePath = RUN_TIME_SCENE_PATH;
            if (EditorSceneManager.GetActiveScene().path != oldScenePath)
            {
                EditorSceneManager.OpenScene(oldScenePath);
            }
        }
    }

    /// <summary>
    /// 查看技能基本信息 
    /// 点击查看技能基本信息按钮后会调用
    /// </summary>
    private void SkillBasicBtnClick()
    {
        if (SkillClip != null)
        {
            Selection.activeObject = SkillClip;
        }
    }

    /// <summary>
    /// 适配轨道长度，将最大帧数重置为所有轨道中的最大值
    /// </summary>
    private void ResetTrackBtnClick()
    {
        int maxFrameCount = 0;
        if (SkillClip == null)
            maxFrameCount = 100;
        else
        {
            foreach (var item in SkillClip.SkillAnimationData.FrameData)
            {
                maxFrameCount = Mathf.Max(maxFrameCount, item.Key + item.Value.DurationFrame);
            }
        }
        CurrentFrameCount = maxFrameCount;
    }

    /// <summary>
    /// 修改角色预览对象
    /// 用于演示技能配置
    /// </summary>
    private void PreviewCharacterFieldValueChanged(ChangeEvent<UnityEngine.Object> evt)
    {
        PreviewCharacterObj = (GameObject)evt.newValue;
    }

    /// <summary>
    /// 修改技能配置文件
    /// 为了在技能配置文件窗口替换其他技能配置能及时更新窗口
    /// </summary>
    private void SkillConfigFieldValueChanged(ChangeEvent<UnityEngine.Object> evt)
    {
        SaveClip();
        SkillClip = evt.newValue as SkillClip;
        CurrentSelectFrameIndex = 0;
        if (SkillClip == null)
        {
            CurrentFrameCount = 100;
            UpdateTimerShaftView();
        }
        else
        {
            CurrentFrameCount = SkillClip.FrameCount;
        }
        //修改后刷新轨道
        ResetTrack();
    }

    #endregion

    #region 时间轴TimeShaft

    private IMGUIContainer TimeShaft;
    private IMGUIContainer SelectLine;
    private VisualElement ContentContainer;
    private VisualElement ContentViewPort;

    //当前内容区域的偏移坐标
    private float CurrentContentOffsetPos => Mathf.Abs(ContentContainer.transform.position.x);
    //当前选中帧的位置
    private float CurrentSelectFramePos => CurrentSelectFrameIndex * skillEditorConfig.FrameUnitWidth;
    private bool timeShaftMouseEnter = false;

    /// <summary>
    /// 初始化时间轴
    /// </summary>
    private void InitTimeShaft()
    {
        ScrollView MainContentScroll = root.Q<ScrollView>(nameof(MainContentScroll));
        ContentContainer = MainContentScroll.Q<VisualElement>("unity-content-container");
        ContentViewPort = MainContentScroll.Q<VisualElement>("unity-content-viewport");

        TimeShaft = root.Q<IMGUIContainer>(nameof(TimeShaft));
        TimeShaft.onGUIHandler = DrawTimeShaft;
        TimeShaft.RegisterCallback<WheelEvent>(TimeShaftWheel);
        TimeShaft.RegisterCallback<MouseDownEvent>(TimeShaftMouseDown);
        TimeShaft.RegisterCallback<MouseMoveEvent>(TimeShaftMouseMove);
        TimeShaft.RegisterCallback<MouseUpEvent>(TimeShaftMouseUp);
        TimeShaft.RegisterCallback<MouseOutEvent>(TimeShaftMouseOut);

        SelectLine = root.Q<IMGUIContainer>(nameof(SelectLine));
        SelectLine.onGUIHandler = DrawSelectLine;
    }

    /// <summary>
    /// 绘制时间轴
    /// 绘制类似Animation的时间轴
    /// 能够支持缩放，动态绘制
    /// </summary>
    private void DrawTimeShaft()
    {
        Handles.BeginGUI();
        Handles.color = Color.green;
        Rect rect = TimeShaft.contentRect;
        //起始索引
        int index = Mathf.CeilToInt(CurrentContentOffsetPos / skillEditorConfig.FrameUnitWidth);
        //计算绘制起点的偏移
        float startOffset = 0;
        if (index > 0)
        {
            startOffset = skillEditorConfig.FrameUnitWidth -
                          (CurrentContentOffsetPos % skillEditorConfig.FrameUnitWidth);
        }
        //tickStep = 10 + 1 - ()
        int tickStep = (SkillEditorConfig.MaxFrameWidthLv + 1 -
                        (skillEditorConfig.FrameUnitWidth / SkillEditorConfig.DefaultFrameUnitWidth)) / 2;
        if (tickStep == 0) tickStep = 1; //避免为0
        for (float i = startOffset; i < rect.width; i += skillEditorConfig.FrameUnitWidth)
        {
            // 绘制长线条、文本
            if (index % tickStep == 0)
            {
                Handles.DrawLine(new Vector3(i, rect.height - 10), new Vector3(i, rect.height));
                string indexStr = index.ToString();
                GUI.Label(new Rect(i - indexStr.Length * 4.5f, 0, 35, 20), indexStr);
            }
            //绘制普通的线条
            else
            {
                Handles.DrawLine(new Vector3(i, rect.height - 5), new Vector3(i, rect.height));
            }
            index++;
        }

        Handles.EndGUI();
    }

    /// <summary>
    /// 鼠标滚轮在时间轴上滑动
    /// 当鼠标在时间轴上滚动滚轮时它的刻度大小会变化
    /// </summary>
    private void TimeShaftWheel(WheelEvent evt)
    {
        int delta = (int)evt.delta.y;
        skillEditorConfig.FrameUnitWidth = Mathf.Clamp(skillEditorConfig.FrameUnitWidth - delta,
            SkillEditorConfig.DefaultFrameUnitWidth,
            SkillEditorConfig.MaxFrameWidthLv * SkillEditorConfig.DefaultFrameUnitWidth);
        UpdateTimerShaftView();
        UpdateContentSize();
        ResetTrack();
    }

    /// <summary>
    /// 鼠标在时间轴上按下
    /// 会让选中线的位置卡在选择帧的位置上
    /// </summary>
    private void TimeShaftMouseDown(MouseDownEvent evt)
    {
        //让选中线的位置卡在帧的位置上
        timeShaftMouseEnter = true;
        IsPlaying = false;
        int newValue = GetFrameIndexByMousePos(evt.localMousePosition.x);
        if (newValue != CurrentSelectFrameIndex)
        {
            CurrentSelectFrameIndex = newValue;
        }
    }

    /// <summary>
    /// 鼠标在时间轴上移动
    /// 选中线的位置会随鼠标移动变化
    /// </summary>
    private void TimeShaftMouseMove(MouseMoveEvent evt)
    {
        if (timeShaftMouseEnter)
        {
            int newValue = GetFrameIndexByMousePos(evt.localMousePosition.x);
            if (newValue != CurrentSelectFrameIndex)
            {
                CurrentSelectFrameIndex = newValue;
            }
        }
    }

    /// <summary>
    /// 鼠标在时间轴上抬起
    /// </summary>
    private void TimeShaftMouseUp(MouseUpEvent evt)
    {
        timeShaftMouseEnter = false;
    }

    /// <summary>
    /// 鼠标在时间轴上离开
    /// </summary>
    private void TimeShaftMouseOut(MouseOutEvent evt)
    {
        timeShaftMouseEnter = false;
    }

    /// <summary>
    /// 根据鼠标坐标获取帧索引
    /// </summary>
    public int GetFrameIndexByMousePos(float x)
    {
        return GetFrameIndexByPos(x + CurrentContentOffsetPos);
    }

    /// <summary>
    /// 根据视图位置获取帧索引
    /// 指的是放置轨道的视图，让轨道和帧索引对应
    /// </summary>
    public int GetFrameIndexByPos(float x)
    {
        return Mathf.RoundToInt(x / skillEditorConfig.FrameUnitWidth);
    }

    /// <summary>
    /// 绘制选中线
    /// 类似Animation，绘制一条从头到尾的一条线。
    /// </summary>
    private void DrawSelectLine()
    {
        //判断当前选中帧是否在视图范围内
        if (CurrentSelectFramePos >= CurrentContentOffsetPos)
        {
            Handles.BeginGUI();
            Handles.color = Color.yellow;
            Handles.DrawLine(new Vector3(CurrentSelectFramePos - CurrentContentOffsetPos, 0),
                new Vector3(CurrentSelectFramePos - CurrentContentOffsetPos,
                    ContentViewPort.contentRect.height + TimeShaft.contentRect.height));
            Handles.EndGUI();
        }
    }

    /// <summary>
    /// 重新绘制时间轴视图
    /// 让时间轴视图能够即时刷新
    /// 比如放大缩小时间轴，修改技能配置之类的
    /// </summary>
    private void UpdateTimerShaftView()
    {
        TimeShaft.MarkDirtyLayout(); //标记为需要重新绘制的
        SelectLine.MarkDirtyLayout(); //标记为需要重新绘制的
    }

    #endregion

    #region 控制台Console

    private Button PrevFrameBtn;
    private Button PlayBtn;
    private Button NextFrameBtn;
    private Toggle IsLoopToggle;
    private IntegerField CurrentFrameField;
    private IntegerField FrameCountField;

    /// <summary>
    /// 初始化控制台
    /// </summary>
    private void InitConsole()
    {
        PrevFrameBtn = root.Q<Button>(nameof(PrevFrameBtn));
        PrevFrameBtn.clicked += PrevFrameBtnClick;

        PlayBtn = root.Q<Button>(nameof(PlayBtn));
        PlayBtn.clicked += PlayBtnClick;

        NextFrameBtn = root.Q<Button>(nameof(NextFrameBtn));
        NextFrameBtn.clicked += NextFrameBtnClick;

        IsLoopToggle = root.Q<Toggle>(nameof(IsLoopToggle));
        IsLoopToggle.RegisterValueChangedCallback(IsLoopToggleValueChanged);

        CurrentFrameField = root.Q<IntegerField>(nameof(CurrentFrameField));
        CurrentFrameField.RegisterValueChangedCallback(CurrentFrameFieldValueChanged);

        FrameCountField = root.Q<IntegerField>(nameof(FrameCountField));
        FrameCountField.RegisterValueChangedCallback(FrameCountFieldValueChanged);
    }

    /// <summary>
    /// 点击上一帧按钮
    /// 当前选中索引-1
    /// </summary>
    private void PrevFrameBtnClick()
    {
        CurrentSelectFrameIndex--;
        IsPlaying = false;
    }

    /// <summary>
    /// 点击播放按钮
    /// 点击播放和暂停动画
    /// </summary>
    private void PlayBtnClick()
    {
        IsPlaying = !IsPlaying;
    }

    /// <summary>
    /// 点击下一帧按钮
    /// 当前选中索引+1
    /// </summary>
    private void NextFrameBtnClick()
    {
        CurrentSelectFrameIndex++;
        IsPlaying = false;
    }

    /// <summary>
    /// 修改当前帧框中的值时，当前选中帧也要同步更新
    /// </summary>
    private void CurrentFrameFieldValueChanged(ChangeEvent<int> evt)
    {
        CurrentSelectFrameIndex = evt.newValue;
    }

    /// <summary>
    /// 修改帧总值框中的值时，帧总值也要同步更新
    /// </summary>
    private void FrameCountFieldValueChanged(ChangeEvent<int> evt)
    {
        CurrentFrameCount = evt.newValue;
    }

    /// <summary>
    /// 是否循环播放轨道内容
    /// </summary>
    private void IsLoopToggleValueChanged(ChangeEvent<bool> evt)
    {
        isLoop = evt.newValue;
    }

    #endregion

    #region 轨道Track

    private VisualElement TrackMenuList;
    private VisualElement ContentListView;
    private readonly List<SkillTrackBase> trackList = new List<SkillTrackBase>();

    /// <summary>
    /// 初始化轨道的菜单以及窗口
    /// </summary>
    private void InitContent()
    {
        TrackMenuList = root.Q<VisualElement>(nameof(TrackMenuList));
        ContentListView = root.Q<VisualElement>(nameof(ContentListView));
        ScrollView TrackMenuScroll = root.Q<ScrollView>(nameof(TrackMenuScroll));
        ScrollView MainContentScroll = root.Q<ScrollView>(nameof(MainContentScroll));

        TrackMenuScroll.verticalScroller.valueChanged += (value) =>
        {
            MainContentScroll.verticalScroller.value = value;
        };
        MainContentScroll.verticalScroller.valueChanged += (value) =>
        {
            TrackMenuScroll.verticalScroller.value = value;
        };
        UpdateContentSize();
        InitTrack();
    }

    /// <summary>
    /// 初始化轨道窗口
    /// </summary>
    private void InitTrack()
    {
        if (SkillClip == null) return;
        InitCustomEventTrack();
        InitAnimationTrack();
        InitAudioTrack();
        InitEffectTrack();
        InitColliderTrack();
    }

    private void InitCustomEventTrack()
    {
        CustomEventTrack customEventTrack = new CustomEventTrack();
        customEventTrack.Init(TrackMenuList, ContentListView, skillEditorConfig.FrameUnitWidth);
        trackList.Add(customEventTrack);
    }

    private void InitAnimationTrack()
    {
        AnimationTrack animationTrack = new AnimationTrack();
        animationTrack.Init(TrackMenuList, ContentListView, skillEditorConfig.FrameUnitWidth);
        trackList.Add(animationTrack);
        getPositionForRootMotion = animationTrack.GetPositionForRootMotion;
    }

    private void InitAudioTrack()
    {
        AudioTrack audioTrack = new AudioTrack();
        audioTrack.Init(TrackMenuList, ContentListView, skillEditorConfig.FrameUnitWidth);
        trackList.Add(audioTrack);
    }

    private void InitEffectTrack()
    {
        EffectTrack effectTrack = new EffectTrack();
        effectTrack.Init(TrackMenuList, ContentListView, skillEditorConfig.FrameUnitWidth);
        trackList.Add(effectTrack);
    }

    private void InitColliderTrack()
    {
        ColliderTrack colliderTrack = new ColliderTrack();
        colliderTrack.Init(TrackMenuList, ContentListView, skillEditorConfig.FrameUnitWidth);
        trackList.Add(colliderTrack);
    }

    private void ResetTrack()
    {
        //如果配置文件是null,清理掉所有轨道
        if (SkillClip == null)
        {
            DestroyTrackList();
        }
        else
        {
            if (trackList.Count == 0)
            {
                InitTrack();
            }
            foreach (var track in trackList)
            {
                track.ResetView(skillEditorConfig.FrameUnitWidth);
            }
        }
    }

    private void DestroyTrackList()
    {
        foreach (var track in trackList)
        {
            track.Destroy();
        }
        trackList.Clear();
    }

    private void UpdateContentSize()
    {
        ContentListView.style.width = skillEditorConfig.FrameUnitWidth * CurrentFrameCount;
    }

    #endregion

    #region 编辑器预览PreView

    private bool isPlaying;
    /// <summary>
    /// 是否播放中
    /// 如果为True，则开始播放动画
    /// </summary>
    public bool IsPlaying
    {
        get => isPlaying;
        set
        {
            isPlaying = value;
            if (isPlaying)
            {
                startTime = DateTime.Now;
                startFrameIndex = currentSelectFrameIndex;

                foreach (var track in trackList)
                {
                    track.OnPlay(currentSelectFrameIndex);
                }
            }
            else
            {
                foreach (var track in trackList)
                {
                    track.OnStop();
                }
            }
        }
    }

    private DateTime startTime;
    private int startFrameIndex;
    private bool isLoop; //是否循环播放轨道

    /// <summary>
    /// IsPlaying为True时，处理播放动画逻辑
    /// 根据IsLoop按钮状态来判断是只播放一次还是循环播放
    /// </summary>
    private void Update()
    {
        if (IsPlaying)
        {
            //得到时间差
            float time = (float)DateTime.Now.Subtract(startTime).TotalSeconds;
            //确定时间轴的帧率
            float frameRote;
            if (SkillClip != null) frameRote = SkillClip.FrameRate;
            else frameRote = skillEditorConfig.DefaultFrameRote;

            //根据时间差计算当前的选中帧
            CurrentSelectFrameIndex = (int)((time * frameRote) + startFrameIndex);
            //到达最后一帧会置0
            if (CurrentSelectFrameIndex == CurrentFrameCount)
            {
                if (isLoop)
                {
                    CurrentSelectFrameIndex = 0;
                    startTime = DateTime.Now;
                    startFrameIndex = 0;
                }
                else
                {
                    IsPlaying = false;
                }
            }
        }
    }

    /// <summary>
    /// 驱动技能
    /// 通过调用它来采样并显示当前帧所应该播放的动画等
    /// 在选中帧更新时调用
    /// </summary>
    public void TickSkill()
    {
        //驱动技能表现
        if (SkillClip != null && PreviewCharacterObj != null)
        {
            foreach (var track in trackList)
            {
                track.TickView(currentSelectFrameIndex);
            }
        }
    }

    private Func<int, bool, Vector3> getPositionForRootMotion;

    /// <summary>
    /// 获取根运动下当前帧所在的位置
    /// </summary>
    public Vector3 GetPositionForRootMotion(int frameIndex, bool recover = false)
    {
        return getPositionForRootMotion(frameIndex, recover);
    }

    #endregion

    #region 场景绘制GizmoAndSceneGUI
    
    // /// <summary>
    // /// 绘制场景中物体
    // /// 比如攻击检测的范围等
    // /// 该Attribute是指无论选中不选中都会绘制
    // /// </summary>
    [DrawGizmo(GizmoType.NotInSelectionHierarchy | GizmoType.Selected)]
    private static void DrawGizmos(Transform player, GizmoType gizmoType)
    {
        if (Instance == null || Instance.PreviewCharacterObj == null ||
            Instance.PreviewCharacterObj.transform != player)
        {
            return;
        }
        foreach (var track in Instance.trackList)
        {
            track.DrawGizmos();
        }
    }

    /// <summary>
    /// 绘制编辑器场景GUI
    /// 比如类似能够移动物体的三个箭头等工具
    /// </summary>
    private void OnSceneGUI(SceneView sceneView)
    {
        if (PreviewCharacterObj == null)
        {
            return;
        }
        foreach (var track in Instance.trackList)
        {
            track.OnSceneGUI();
        }
    }

    #endregion

    #region 刷新保存

    public void SaveClip()
    {
        if (SkillClip)
        {
            EditorUtility.SetDirty(SkillClip);
            AssetDatabase.SaveAssetIfDirty(SkillClip);
            ResetFormConfig();
        }
    }

    /// <summary>
    /// 更新所有轨道的数据
    /// </summary>
    private void ResetFormConfig()
    {
        foreach (var track in trackList)
        {
            track.OnConfigChanged();
        }
    }

    /// <summary>
    /// 在Inspector面板中显示轨道中片段的详细信息
    /// </summary>
    public void ShowTrackItemOnInspector(TrackItemBase trackItem, SkillTrackBase track)
    {
        SkillEditorInspector.SetTrackItem(trackItem, track);
        Selection.activeObject = this;
    }

    #endregion
}

/// <summary>
/// 技能编辑器配置
/// </summary>
public class SkillEditorConfig
{
    public const int DefaultFrameUnitWidth = 5; //默认帧单位宽度
    public const int MaxFrameWidthLv = 10; //最大帧单位宽度等级
    public int FrameUnitWidth = 10; //当前帧单位宽度
    public float DefaultFrameRote = 30; //默认帧率
}
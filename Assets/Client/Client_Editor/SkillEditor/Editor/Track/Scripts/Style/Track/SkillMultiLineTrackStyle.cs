using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// 多行轨道的样式(视图)
/// </summary>
public class SkillMultiLineTrackStyle : SkillTrackStyleBase
{
    #region 常量

    private const string MenuAssetPath =
        SkillEditorWindow.SkillEditorAssetPath + "MultiLineTrackStyle/MultiLineTrackMenu.uxml";
    private const string ContentAssetPath =
        SkillEditorWindow.SkillEditorAssetPath + "MultiLineTrackStyle/MultiLineTrackContent.uxml";
    private const float headHeight = 35;
    private const float itemHeight = 32;

    #endregion

    private Action addChildTrackAction;
    private Func<int, bool> removeChildTrackFunc;
    private Action<int, int> swapChildTrackAction;
    private Action<ChildTrack, string> updateChildTrackNameAction;

    private VisualElement childMenuParent;
    private VisualElement contentItemParent;

    private List<ChildTrack> childTrackList;

    /// <summary>
    /// 初始化多行轨道的视图
    /// </summary>
    public void Init(VisualElement menuParent, VisualElement contentParent, string title
        , Action addChildTrackAction, Func<int, bool> removeChildTrackFunc, Action<int, int> swapChildTrackAction
        , Action<ChildTrack, string> updateChildTrackNameAction)
    {
        childTrackList = new List<ChildTrack>();
        this.menuParent = menuParent;
        this.contentParent = contentParent;
        this.addChildTrackAction = addChildTrackAction;
        this.removeChildTrackFunc = removeChildTrackFunc;
        this.swapChildTrackAction = swapChildTrackAction;
        this.updateChildTrackNameAction = updateChildTrackNameAction;

        menuRoot = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(MenuAssetPath).Instantiate().Query().ToList()[1];
        titleLabel = menuRoot.Q<Label>("Title");
        titleLabel.text = title;
        contentRoot = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(ContentAssetPath).Instantiate().Query()
            .ToList()[1];
        menuParent.Add(menuRoot);
        contentParent.Add(contentRoot);

        childMenuParent = menuRoot.Q<VisualElement>("TrackMenuList");
        childMenuParent.RegisterCallback<MouseDownEvent>(ChildMenuParentMouseDown);
        childMenuParent.RegisterCallback<MouseMoveEvent>(ChildMenuParentMouseMove);
        childMenuParent.RegisterCallback<MouseUpEvent>(ChildMenuParentMouseUp);
        childMenuParent.RegisterCallback<MouseOutEvent>(ChildMenuParentMouseOut);

        //添加子轨道按钮
        Button AddBtn = menuRoot.Q<Button>(nameof(AddBtn));
        AddBtn.clicked += AddBtnClick;
        UpdateSize();
    }

    #region 子轨道交互

    private bool isDragging;
    private int selectTrackIndex = -1;

    /// <summary>
    /// 鼠标在多行轨道菜单内按下
    /// </summary>
    private void ChildMenuParentMouseDown(MouseDownEvent evt)
    {
        if (selectTrackIndex != -1)
        {
            childTrackList[selectTrackIndex].UnSelect();
        }
        // 通过高度推导出当前交互的是第几个
        float mousePosition = evt.localMousePosition.y - itemHeight / 2;
        selectTrackIndex = GetChildIndexByMousePosition(mousePosition);
        childTrackList[selectTrackIndex].Select();
        //拖拽
        isDragging = true;
    }

    /// <summary>
    /// 鼠标在多行轨道菜单内移动
    /// </summary>
    private void ChildMenuParentMouseMove(MouseMoveEvent evt)
    {
        if (selectTrackIndex == -1 || isDragging == false) return;
        float mousePosition = evt.localMousePosition.y - itemHeight / 2;
        int mouseTrackIndex = GetChildIndexByMousePosition(mousePosition);
        if (mouseTrackIndex != selectTrackIndex)
        {
            SwapChildTrack(mouseTrackIndex, selectTrackIndex);
            selectTrackIndex = mouseTrackIndex;
        }
    }

    /// <summary>
    /// 鼠标在多行轨道菜单内抬起
    /// </summary>
    private void ChildMenuParentMouseUp(MouseUpEvent evt)
    {
        isDragging = false;
        if (selectTrackIndex != -1)
        {
            childTrackList[selectTrackIndex].UnSelect();
            selectTrackIndex = -1;
        }
    }

    /// <summary>
    /// 鼠标在多行轨道菜单内离开
    /// 这个函数经常会无意义调用，因为子物体经常和我们产生遮挡
    /// 所以会检测鼠标位置是否真的离开了范围
    /// </summary>
    private void ChildMenuParentMouseOut(MouseOutEvent evt)
    {
        if (!childMenuParent.contentRect.Contains(evt.localMousePosition))
        {
            isDragging = false;
            if (selectTrackIndex != -1)
            {
                childTrackList[selectTrackIndex].UnSelect();
                selectTrackIndex = -1;
            }
        }
    }

    /// <summary>
    /// 根据鼠标当前的位置获得子轨道的索引
    /// </summary>
    private int GetChildIndexByMousePosition(float mousePosY)
    {
        int trackIndex = Mathf.RoundToInt(mousePosY / itemHeight);
        trackIndex = Mathf.Clamp(trackIndex, 0, childTrackList.Count - 1);
        return trackIndex;
    }

    #endregion

    /// <summary>
    /// 交换两个子轨道
    /// 将两个子轨道的视图位置做交换
    /// 同时实际数据也会做变更
    /// </summary>
    private void SwapChildTrack(int index1, int index2)
    {
        if (index1 == index2) return;
        ChildTrack childTrack1 = childTrackList[index1];
        ChildTrack childTrack2 = childTrackList[index2];
        childTrackList[index1] = childTrack2;
        childTrackList[index2] = childTrack1;
        UpdateChildTracks();
        //上级轨道数据变更
        swapChildTrackAction(index1, index2);
    }

    /// <summary>
    /// 更新多行轨道的视图大小
    /// </summary>
    private void UpdateSize()
    {
        float height = headHeight + childTrackList.Count * itemHeight;
        contentRoot.style.height = height;
        menuRoot.style.height = height;
        childMenuParent.style.height = childTrackList.Count * itemHeight;
    }

    /// <summary>
    /// 按钮点击添加子轨道
    /// </summary>
    private void AddBtnClick()
    {
        addChildTrackAction?.Invoke();
    }

    /// <summary>
    /// 添加子轨道
    /// 在视图层面上添加
    /// </summary>
    public ChildTrack AddChildTrack()
    {
        ChildTrack childTrack = new ChildTrack();
        childTrack.Init(childTrackList.Count, childMenuParent, contentRoot,
            RemoveChildTrackAndData, RemoveChildTrack, updateChildTrackNameAction);
        childTrackList.Add(childTrack);
        UpdateSize();
        return childTrack;
    }

    /// <summary>
    /// 删除子轨道
    /// 只在视图层面做删除
    /// </summary>
    /// <param name="childTrack"></param>
    private void RemoveChildTrack(ChildTrack childTrack)
    {
        int index = childTrack.GetIndex();
        childTrack.DoDestroy();
        childTrackList.RemoveAt(index);
        //所有子轨道都需要更新索引
        UpdateChildTracks(index);
        UpdateSize();
    }

    /// <summary>
    /// 删除子轨道
    /// 在视图和数据层面做删除
    /// </summary>
    /// <param name="childTrack"></param>
    private void RemoveChildTrackAndData(ChildTrack childTrack)
    {
        if (removeChildTrackFunc == null) return;
        int index = childTrack.GetIndex();
        if (removeChildTrackFunc(index))
        {
            childTrack.DoDestroy();
            childTrackList.RemoveAt(index);
            //所有子轨道都需要更新索引
            UpdateChildTracks(index);
            UpdateSize();
        }
    }

    /// <summary>
    /// 从startIndex开始更新子轨道索引
    /// </summary>
    private void UpdateChildTracks(int startIndex = 0)
    {
        for (int i = startIndex; i < childTrackList.Count; i++)
        {
            childTrackList[i].SetIndex(i);
        }
    }


    /// <summary>
    /// 多行轨道的子轨道类
    /// </summary>
    public class ChildTrack
    {
        private const string ChildTrackMenuAssetPath =
            SkillEditorWindow.SkillEditorAssetPath + "MultiLineTrackStyle/MultiLineChildTrackMenu.uxml";
        private const string ChildTrackContentAssetPath =
            SkillEditorWindow.SkillEditorAssetPath + "MultiLineTrackStyle/MultiLineChildTrackContent.uxml";

        public VisualElement menuRoot;
        public VisualElement contentRoot;
        public VisualElement menuParent;
        public VisualElement contentParent;
        private VisualElement content;
        private TextField trackNameField;

        private Action<ChildTrack> removeAction;
        private Action<ChildTrack> destroyAction;
        private Action<ChildTrack, string> updateTrackNameAction;

        private static readonly Color NormalColor = new Color(0, 0, 0, 0);
        private static readonly Color SelectColor = Color.green;

        private int index;

        /// <summary>
        /// 初始化子轨道
        /// </summary>
        public void Init(int index, VisualElement menuParent, VisualElement contentParent
            , Action<ChildTrack> removeAction, Action<ChildTrack> destroyAction,
            Action<ChildTrack, string> updateTrackNameAction)
        {
            this.menuParent = menuParent;
            this.contentParent = contentParent;
            this.removeAction = removeAction;
            this.destroyAction = destroyAction;
            this.updateTrackNameAction = updateTrackNameAction;

            menuRoot = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(ChildTrackMenuAssetPath).Instantiate().Query()
                .ToList()[1];
            menuParent.Add(menuRoot);
            trackNameField = menuRoot.Q<TextField>("NameField");
            trackNameField.RegisterCallback<FocusInEvent>(TrackNameFieldFocusIn);
            trackNameField.RegisterCallback<FocusOutEvent>(TrackNameFieldFocusOut);

            contentRoot = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(ChildTrackContentAssetPath).Instantiate()
                .Query().ToList()[1];
            contentParent.Add(contentRoot);

            Button RemoveBtn = menuRoot.Q<Button>(nameof(RemoveBtn));
            RemoveBtn.clicked += () => removeAction(this);
            SetIndex(index);
        }

        private string oldTrackName;

        private void TrackNameFieldFocusIn(FocusInEvent evt)
        {
            oldTrackName = trackNameField.value;
        }

        /// <summary>
        /// 在子轨道名称框内修改完成后回车调用
        /// 保存修改后的值
        /// </summary>
        /// <param name="evt"></param>
        private void TrackNameFieldFocusOut(FocusOutEvent evt)
        {
            if (oldTrackName == trackNameField.value)
                return;
            if (String.IsNullOrEmpty(trackNameField.value))
            {
                updateTrackNameAction?.Invoke(this, oldTrackName);
                trackNameField.value = oldTrackName;
            }
            else
            {
                updateTrackNameAction?.Invoke(this, trackNameField.value);
            }
        }

        /// <summary>
        /// 初始化子轨道主体部分
        /// </summary>
        public void InitContent(VisualElement content)
        {
            this.content = content;
            contentRoot.Add(content);
        }

        /// <summary>
        /// 设置子轨道名称
        /// </summary>
        public void SetTrackName(string name)
        {
            trackNameField.value = name;
        }

        /// <summary>
        /// 设置子轨道所在的索引
        /// 视图会根据它的索引进行一个更新
        /// </summary>
        public void SetIndex(int index)
        {
            this.index = index;
            float height = 0;
            Vector3 pos = menuRoot.transform.position;
            height = index * itemHeight;
            pos.y = height;
            menuRoot.transform.position = pos;

            pos = contentRoot.transform.position;
            height += headHeight;
            pos.y = height;
            contentRoot.transform.position = pos;
        }

        /// <summary>
        /// 返回子轨道所在的索引
        /// </summary>
        public int GetIndex()
        {
            return index;
        }

        /// <summary>
        /// 子轨道选中
        /// </summary>
        public void Select()
        {
            menuRoot.style.backgroundColor = SelectColor;
        }

        /// <summary>
        /// 子轨道反选
        /// </summary>
        public void UnSelect()
        {
            menuRoot.style.backgroundColor = NormalColor;
        }

        /// <summary>
        /// 销毁子轨道
        /// </summary>
        public void Destroy()
        {
            destroyAction(this);
        }

        /// <summary>
        /// 实际删除子轨道
        /// </summary>
        public void DoDestroy()
        {
            if (menuRoot != null) menuParent.Remove(menuRoot);
            if (contentRoot != null) contentParent.Remove(contentRoot);
        }
    }
}
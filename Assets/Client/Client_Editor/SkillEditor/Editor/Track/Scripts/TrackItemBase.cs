using UnityEngine;

/// <summary>
/// 轨道片段基类
/// </summary>
public abstract class TrackItemBase
{
    protected float frameUnitWidth; //单位帧宽度

    protected int frameIndex; //起始帧索引
    /// <summary>
    /// 轨道片段的帧索引
    /// </summary>
    public int FrameIndex => frameIndex;
    
    public abstract void Select();
    public abstract void OnSelect();
    public abstract void OnUnSelect();
    
    /// <summary>
    /// 轨道片段每次修改时应当调用它，
    /// 保证轨道片段数据是实时的
    /// </summary>
    public virtual void OnConfigChanged() { }
    /// <summary>
    /// 重置视图,只是为了刷新
    /// </summary>
    public virtual void ResetView()
    {
        ResetView(frameUnitWidth);
    }
    /// <summary>
    /// 重置视图到指定帧宽度
    /// </summary>
    public virtual void ResetView(float frameUnitWidth)
    {
        this.frameUnitWidth = frameUnitWidth;
    }
}

/// <summary>
/// 轨道内的Item基类
/// </summary>
public abstract class TrackItemBase<T> : TrackItemBase where T : SkillTrackBase
{
    protected T track;                              //Item所在动画轨道

    protected Color normalColor;
    protected Color selectColor;

    /// <summary>
    /// 轨道片段的样式
    /// </summary>
    public SkillTrackItemStyleBase itemStyle {  get; protected set; }

    public override void Select()
    {
        SkillEditorWindow.Instance.ShowTrackItemOnInspector(this, track);
    }

    public override void OnSelect()
    {
        itemStyle.SetBGColor(selectColor);
    }

    public override void OnUnSelect()
    {
        itemStyle.SetBGColor(normalColor);
    }
}
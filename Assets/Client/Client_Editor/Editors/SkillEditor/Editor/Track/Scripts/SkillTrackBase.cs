using UnityEngine.UIElements;

/// <summary>
/// 技能轨道基类
/// </summary>
public abstract class SkillTrackBase
{
    protected float frameWidth; //单位帧宽度
    
    /// <summary>
    /// 初始化轨道
    /// </summary>
    public virtual void Init(VisualElement menuParent, VisualElement contentParent, float frameWidth)
    {
        this.frameWidth = frameWidth;
    }
    
    /// <summary>
    /// 默认地重置轨道，只是为了刷新，不改变大小
    /// </summary>
    public virtual void ResetView()
    {
        ResetView(frameWidth);
    }
    
    /// <summary>
    /// 重置轨道视图
    /// 当单位帧宽度更新时应当调用它来修改轨道长度
    /// </summary>
    /// <param name="frameWidth"></param>
    public virtual void ResetView(float frameWidth)
    {
        this.frameWidth = frameWidth;
    }
    
    /// <summary>
    /// 删除轨道中的指定帧索引的片段
    /// </summary>
    public virtual void DeleteTrackItem(int frameIndex) { }
    
    /// <summary>
    /// 轨道每次修改时应当调用它，
    /// 保证轨道数据是实时的
    /// </summary>
    public virtual void OnConfigChanged() { }
    
    /// <summary>
    /// 开始播放预览时
    /// </summary>
    public virtual void OnPlay(int startFrameIndex) { }
    
    /// <summary>
    /// 驱动视图
    /// 根据给定的帧索引去采样轨道并体现到视图中
    /// </summary>
    public virtual void TickView(int frameIndex) { }

    /// <summary>
    /// 结束播放预览时
    /// </summary>
    public virtual void OnStop() { }

    /// <summary>
    /// 销毁轨道
    /// </summary>
    public virtual void Destroy() { }

    /// <summary>
    /// 绘制当前轨道需要在场景中呈现的一些事物
    /// 比如攻击检测的范围等
    /// </summary>
    public virtual void DrawGizmos() { }

    /// <summary>
    /// 绘制当前轨道需要在场景中呈现的GUI
    /// </summary>
    public virtual void OnSceneGUI() { }
}
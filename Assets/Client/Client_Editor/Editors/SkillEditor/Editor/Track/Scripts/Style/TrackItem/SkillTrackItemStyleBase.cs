using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// 技能轨道片段的样式基类
/// </summary>
public abstract class SkillTrackItemStyleBase
{
    /// <summary>
    /// 轨道片段视图的根节点
    /// </summary>
    public VisualElement Root { get; protected set; }

    protected abstract string TrackItemAssetPath { get; }

    /// <summary>
    /// 设置背景颜色
    /// 调用它来改变轨道片段的背景颜色
    /// </summary>
    public virtual void SetBGColor(Color color)
    {
        Root.style.backgroundColor = color;
    }

    /// <summary>
    /// 设置轨道片段宽度
    /// </summary>
    public virtual void SetWidth(float width)
    {
        Root.style.width = width;
    }

    /// <summary>
    /// 设置轨道片段的位置(只需要设置x轴坐标)
    /// </summary>
    public virtual void SetPosition(float x)
    {
        Vector3 pos = Root.transform.position;
        pos.x = x;
        Root.transform.position = pos;
    }

}
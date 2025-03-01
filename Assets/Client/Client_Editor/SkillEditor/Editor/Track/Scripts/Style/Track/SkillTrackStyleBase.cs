using UnityEngine.UIElements;


/// <summary>
/// 技能轨道的样式基类
/// </summary>
public abstract class SkillTrackStyleBase
{
    public Label titleLabel;
    protected VisualElement menuParent;         //菜单父节点
    protected VisualElement contentParent;      //轨道父节点
    public VisualElement menuRoot;              //菜单根节点
    public VisualElement contentRoot;           //轨道根节点

    /// <summary>
    /// 添加样式到轨道节点中
    /// </summary>
    /// <param name="ve"></param>
    public virtual void AddItem(VisualElement ve)
    {
        contentRoot.Add(ve);
    }
      
    /// <summary>
    /// 从轨道节点中删除样式
    /// </summary>
    /// <param name="ve"></param>
    public virtual void RemoveItem(VisualElement ve)
    {
        contentRoot.Remove(ve);
    }

    /// <summary>
    /// 销毁轨道样式
    /// </summary>
    public virtual void Destroy()
    {
        if(menuRoot != null) menuParent.Remove(menuRoot);
        if(contentRoot != null) contentParent.Remove(contentRoot);
    }
}
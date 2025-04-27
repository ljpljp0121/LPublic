using UnityEditor;
using UnityEngine.UIElements;

/// <summary>
/// 单行轨道的样式(视图)
/// </summary>
public class SkillSingleLineTrackStyle : SkillTrackStyleBase
{
    private const string MenuAssetPath =
        SkillEditorWindow.SkillEditorAssetPath + "SingleLineTrackStyle/SingleLineTrackMenu.uxml";

    private const string ContentAssetPath =
        SkillEditorWindow.SkillEditorAssetPath + "SingleLineTrackStyle/SingleLineTrackContent.uxml";

    /// <summary>
    /// 初始化单行轨道的视图
    /// 主要是将基本的Menu和Content显示出来
    /// </summary>
    /// <param name="menuParent">menu视图的父节点</param>
    /// <param name="contentParent">content视图的父节点</param>
    /// <param name="title">标题</param>
    public void Init(VisualElement menuParent, VisualElement contentParent, string title)
    {
        this.menuParent = menuParent;
        this.contentParent = contentParent;
        menuRoot = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(MenuAssetPath).Instantiate().Query().ToList()[1];
        titleLabel = (Label)menuRoot;
        titleLabel.text = title;
        contentRoot = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(ContentAssetPath).Instantiate().Query()
            .ToList()[1];

        menuParent.Add(menuRoot);
        contentParent.Add(contentRoot);
    }
}
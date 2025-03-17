#if UNITY_EDITOR
using System;
using System.Linq;
using Sirenix.OdinInspector.Editor;


public class GASAssetWindow : OdinMenuEditorWindow
{
    protected override OdinMenuTree BuildMenuTree()
    {
        var tree = new OdinMenuTree();
        tree.Selection.SelectionChanged += OnMenuSelectionChange; 
    }

    void OnMenuSelectionChange(SelectionChangedType selectionChangedType)
    {
        var selected = MenuTree.Selection.FirstOrDefault();
        if (selected is { Value: AbilityOverview abilityOverview })
        {
            abilityOverview.Refresh();
        }
    }
}

#endif
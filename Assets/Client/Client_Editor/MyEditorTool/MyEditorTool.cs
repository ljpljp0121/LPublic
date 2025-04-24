
using Sirenix.OdinInspector.Editor;
using UnityEditor;

public class MyEditorTool : OdinMenuEditorWindow
{

    [MenuItem("Project/CustomTool")]
    private static void OpenWindow()
    {
        var window = GetWindow<MyEditorTool>();
    }

    protected override OdinMenuTree BuildMenuTree()
    {
        var tree = new OdinMenuTree();
        tree.Selection.SupportsMultiSelect = false;
        tree.Config.AutoScrollOnSelectionChanged = true;
        tree.Config.DrawScrollView = true;
        tree.Config.AutoHandleKeyboardNavigation = true;

        var uiTool = CreateInstance<UITool>();
        uiTool.Init();
        tree.Add("ShowUITool", uiTool);
        tree.Add("AtlasTool", CreateInstance<AtlasTool>());
        tree.Add("BuildTool", CreateInstance<BuildTool>());


        return tree;
    }
}

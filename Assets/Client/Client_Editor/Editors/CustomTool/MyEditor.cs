#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

public enum ToolTab
{
    FBXAnimationPanel,
}

public class MyEditor : EditorWindow
{
    private ToolTab currentTab = ToolTab.FBXAnimationPanel;

    [MenuItem("Project/综合工具集")]
    static void Init()
    {
        var window = GetWindow<MyEditor>();
        window.titleContent = new GUIContent("工具集");
        window.Show();
    }

    void OnGUI()
    {
        DrawToolbar();
        DrawCurrentPanel();
    }

    void DrawToolbar()
    {
        EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
        {
            if (GUILayout.Toggle(currentTab == ToolTab.FBXAnimationPanel, "动画快速改名工具", EditorStyles.toolbarButton))
            {
                if (currentTab != ToolTab.FBXAnimationPanel)
                    DrawCurrentPanel();
                currentTab = ToolTab.FBXAnimationPanel;
            }
        }
        EditorGUILayout.EndHorizontal();
    }

    void DrawCurrentPanel()
    {
        switch (currentTab)
        {
            case ToolTab.FBXAnimationPanel:
                FBXAnimationPanel.Instance.OnGUI();
                break;
        }
    }
}

#endif
using UnityEditor;

public class TestEditor : EditorWindow
{
    [MenuItem("Project/编辑器测试")]
    public static void ShowWindow()
    {
        GetWindow<TestEditor>("测试");
    }

    private void OnGUI()
    {
    }
}
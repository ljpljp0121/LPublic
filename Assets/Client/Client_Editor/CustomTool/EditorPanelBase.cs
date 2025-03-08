using UnityEditor;

#if UNITY_EDITOR
public abstract class EditorPanelBase
{
    public abstract void OnGUI();

    protected void DrawSectionTitle(string title)
    {
        EditorGUILayout.Space();
        EditorGUILayout.LabelField(title, EditorStyles.boldLabel);
        EditorGUILayout.Space();
    }
}
#endif
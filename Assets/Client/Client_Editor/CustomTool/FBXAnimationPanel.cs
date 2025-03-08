using System.IO;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
public class FBXAnimationPanel : EditorPanelBase
{
    public static FBXAnimationPanel Instance { get; } = new FBXAnimationPanel();

    public override void OnGUI()
    {
        EditorGUILayout.LabelField("FBX动画重命名工具", EditorStyles.boldLabel);
        
        if (GUILayout.Button("处理选中FBX文件", GUILayout.Height(30)))
        {
            ProcessSelectedFBX();
        }
        
        EditorGUILayout.HelpBox("选择包含动画的FBX文件后点击此按钮", MessageType.Info);
    }

    void ProcessSelectedFBX()
    {
        foreach (var obj in Selection.objects)
        {
            string path = AssetDatabase.GetAssetPath(obj);
            if (Path.GetExtension(path).ToLower() != ".fbx") continue;

            ModelImporter importer = AssetImporter.GetAtPath(path) as ModelImporter;
            if (importer == null) continue;

            string fileName = Path.GetFileNameWithoutExtension(path);
            
          
            ModelImporterClipAnimation[] clips = importer.defaultClipAnimations;
            if (clips.Length == 1) 
            {
                clips[0].name = fileName;
                Debug.Log($"已重命名: {path} => {fileName}");
            }
            else if (clips.Length > 1)
            {
                Debug.LogWarning($"文件包含多个动画片段: {path} (跳过处理)");
                continue;
            }

            importer.clipAnimations = clips;
            AssetDatabase.ImportAsset(path); 
        }
    }
}
#endif
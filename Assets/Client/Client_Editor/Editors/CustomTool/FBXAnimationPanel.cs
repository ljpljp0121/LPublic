#if UNITY_EDITOR

using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class FBXAnimationPanel : EditorPanelBase
{
    public static FBXAnimationPanel Instance { get; } = new FBXAnimationPanel();

    private string sourceFolder = "Assets/";
    private string targetFolder = "Assets/ExportedAnimations/";
    private Vector2 scrollPos;
    private List<string> logMessages = new List<string>();

    public override void OnGUI()
    {
        EditorGUILayout.LabelField("FBX动画重命名工具", EditorStyles.boldLabel);

        if (GUILayout.Button("处理选中FBX文件", GUILayout.Height(30)))
        {
            ProcessSelectedFBX();
        }

        EditorGUILayout.HelpBox("选择包含动画的FBX文件后点击此按钮", MessageType.Info);

        EditorGUILayout.Space();
        DrawPathSettings();
        EditorGUILayout.Space();
        DrawActionButtons();
        EditorGUILayout.Space();
        DrawLogPanel();
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


    void DrawPathSettings()
    {
        EditorGUILayout.LabelField("路径设置", EditorStyles.boldLabel);
        DrawFolderField("FBX源文件夹:", ref sourceFolder);
        DrawFolderField("保存目录:", ref targetFolder);
    }

    void DrawFolderField(string label, ref string path)
    {
        EditorGUILayout.BeginHorizontal();
        {
            path = EditorGUILayout.TextField(label, path);
            if (GUILayout.Button("浏览", GUILayout.Width(60)))
            {
                string newPath = EditorUtility.OpenFolderPanel("选择文件夹",
                    Application.dataPath, "");
                if (!string.IsNullOrEmpty(newPath))
                {
                    path = "Assets" + newPath.Replace(Application.dataPath, "");
                }
            }
        }
        EditorGUILayout.EndHorizontal();
    }

    void DrawActionButtons()
    {
        EditorGUILayout.BeginHorizontal();
        {
            if (GUILayout.Button("开始提取", GUILayout.Height(30)))
            {
                StartExtraction();
            }

            if (GUILayout.Button("清空日志", GUILayout.Height(30)))
            {
                logMessages.Clear();
            }
        }
        EditorGUILayout.EndHorizontal();
    }

    void DrawLogPanel()
    {
        EditorGUILayout.LabelField("操作日志:", EditorStyles.boldLabel);
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Height(200));
        foreach (var log in logMessages)
        {
            EditorGUILayout.LabelField(log);
        }
        EditorGUILayout.EndScrollView();
    }

    void StartExtraction()
    {
        try
        {
            logMessages.Add("▶️ 开始提取动画片段...");

            // 验证路径
            if (!Directory.Exists(sourceFolder))
            {
                logMessages.Add($"❌ 源文件夹不存在: {sourceFolder}");
                return;
            }

            // 创建目标目录
            Directory.CreateDirectory(targetFolder);

            // 获取所有FBX文件
            var fbxFiles = Directory.GetFiles(sourceFolder, "*.fbx", SearchOption.AllDirectories)
                .Where(p => !p.EndsWith(".meta"))
                .ToArray();

            int total = fbxFiles.Length;
            int processed = 0;

            foreach (var fbxPath in fbxFiles)
            {
                // 进度显示
                float progress = (float)processed / total;
                if (EditorUtility.DisplayCancelableProgressBar("处理中...",
                        $"{Path.GetFileName(fbxPath)} ({processed + 1}/{total})", progress))
                {
                    break;
                }

                ProcessFBXFile(fbxPath);
                processed++;
            }

            AssetDatabase.Refresh();
            logMessages.Add($"🎉 处理完成! 共处理{processed}个FBX文件");
        }
        catch (System.Exception e)
        {
            Debug.LogError(e);
            logMessages.Add($"🔥 发生严重错误: {e.Message}");
        }
        finally
        {
            EditorUtility.ClearProgressBar();
        }
    }

    void ProcessFBXFile(string fbxPath)
    {
        try
        {
            // 加载动画片段
            var clips = AssetDatabase.LoadAllAssetsAtPath(fbxPath)
                .OfType<AnimationClip>()
                .Where(c => !c.name.StartsWith("__preview__"))
                .ToList();

            if (clips.Count == 0)
            {
                logMessages.Add($"⚠️ 未找到有效动画: {Path.GetFileName(fbxPath)}");
                return;
            }

            // 提取每个动画
            foreach (var clip in clips)
            {
                string fileName = $"{Path.GetFileNameWithoutExtension(fbxPath)}.anim";
                string savePath = Path.Combine(targetFolder, fileName);

                // 确保路径唯一
                savePath = AssetDatabase.GenerateUniqueAssetPath(savePath);

                // 创建副本
                AnimationClip newClip = new AnimationClip();
                EditorUtility.CopySerialized(clip, newClip);

                // 保存文件
                AssetDatabase.CreateAsset(newClip, savePath);
                logMessages.Add($"✅ 已保存: {Path.GetFileName(savePath)}");
            }
        }
        catch (System.Exception e)
        {
            logMessages.Add($"❌ 处理失败: {Path.GetFileName(fbxPath)} - {e.Message}");
        }
    }
}

#endif
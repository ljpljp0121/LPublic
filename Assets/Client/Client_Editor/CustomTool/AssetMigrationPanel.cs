using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class AssetMigrationPanel : EditorPanelBase
{
    public static AssetMigrationPanel Instance { get; } = new AssetMigrationPanel();

    private string sourcePath = "Assets/";
    private string targetPath = "Assets/";
    private string fileFilter = "*.prefab;*.mat;*.png;*.fbx";
    private bool copyInsteadMove = false;

    public override void OnGUI()
    {
        EditorGUILayout.Space();
        DrawPathField("来源路径", ref sourcePath);
        DrawPathField("目标路径", ref targetPath);

        EditorGUILayout.Space();
        fileFilter = EditorGUILayout.TextField("文件过滤", fileFilter);
        EditorGUILayout.HelpBox("使用分号分隔多个类型，示例：*.prefab;*.png;*.fbx", MessageType.Info);

        EditorGUILayout.Space();
        copyInsteadMove = EditorGUILayout.Toggle("复制模式", copyInsteadMove);

        EditorGUILayout.Space();
        if (GUILayout.Button("执行迁移", GUILayout.Height(30)))
        {
            StartMigration();
        }

        EditorGUILayout.HelpBox("此操作将保留所有meta文件，确保引用关系不丢失", MessageType.Info);
    }

    void DrawPathField(string label, ref string path)
    {
        EditorGUILayout.BeginHorizontal();
        {
            path = EditorGUILayout.TextField(label, path);
            if (GUILayout.Button("选择", GUILayout.Width(50)))
            {
                string selectedPath = EditorUtility.OpenFolderPanel("选择目录", Application.dataPath, "");
                if (!string.IsNullOrEmpty(selectedPath))
                {
                    path = "Assets" + selectedPath.Replace(Application.dataPath, "");
                }
            }
        }
        EditorGUILayout.EndHorizontal();
    }

    void StartMigration()
    {
        try
        {
            // 获取所有资源文件（排除meta）
            List<string> files = GetAssetFiles(sourcePath);

            // 创建目标目录
            CreateDirectoryIfNeeded(targetPath);

            // 进度条设置
            int total = files.Count;
            int current = 0;

            foreach (string file in files)
            {
                current++;
                string fileName = Path.GetFileName(file);
                string relativePath = GetRelativePath(file);
                string newPath = Path.Combine(targetPath, relativePath).Replace("\\", "/");

                // 更新进度条
                if (EditorUtility.DisplayCancelableProgressBar(
                        "资源迁移中...",
                        $"{current}/{total} {fileName}",
                        (float)current / total))
                {
                    break;
                }

                // 处理主文件
                ProcessFile(file, newPath);

                // 处理meta文件
                string metaFile = file + ".meta";
                if (File.Exists(metaFile))
                {
                    ProcessFile(metaFile, newPath + ".meta");
                }
            }

            AssetDatabase.Refresh();
        }
        finally
        {
            EditorUtility.ClearProgressBar();
        }
    }

    void ProcessFile(string source, string destination)
    {
        if (copyInsteadMove)
        {
            File.Copy(source, destination, true);
        }
        else
        {
            File.Move(source, destination);
        }
    }

    List<string> GetAssetFiles(string path)
    {
        var files = new List<string>();
        var filters = ParseFileFilters(fileFilter);

        foreach (var filter in filters)
        {
            try
            {
                files.AddRange(Directory.GetFiles(path, filter, SearchOption.AllDirectories)
                    .Where(f => !f.EndsWith(".meta")));
            }
            catch (System.Exception e)
            {
                Debug.LogError($"过滤条件错误: {filter}\n{e.Message}");
            }
        }

        return files.Distinct()
            .OrderBy(f => f)
            .ToList();
    }

    List<string> ParseFileFilters(string input)
    {
        return input.Split(';')
            .Where(s => !string.IsNullOrWhiteSpace(s))
            .Select(s => s.Trim().ToLower())
            .Select(s => s.StartsWith("*.") ? s : (s.StartsWith(".") ? "*" + s : "*." + s))
            .Distinct()
            .ToList();
    }

    string GetRelativePath(string fullPath)
    {
        return fullPath.Substring(sourcePath.Length).TrimStart('/');
    }

    void CreateDirectoryIfNeeded(string path)
    {
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
    }
}
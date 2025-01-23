using System.IO;
using UnityEditor;
using UnityEngine;
using HybridCLR.Editor;
using HybridCLR.Editor.Commands;

public static class BuildMenu
{
    [MenuItem("Project/Build/NewClient")]
    public static void BuildNewClient()
    {
        Debug.Log("开始构建客户端");
        SettingsUtil.Enable = true;
        //华佗构建准备
        PrebuildCommand.GenerateAll();
        //搬运dll文本文件
        GenerateDllBytesFile();
    }

    [MenuItem("Project/Build/UpdateClient")]
    public static void BuildUpdateClient()
    {
        Debug.Log("开始更新客户端");
        CompileDllCommand.CompileDllActiveBuildTarget();
        //搬运dll文本文件
        GenerateDllBytesFile();
    }

    [MenuItem("Project/Build/GenerateDllBytesFile")]
    public static void GenerateDllBytesFile()
    {
        Debug.Log("开始生成dll文本文件");
        string hotupdateDllDirPath = System.Environment.CurrentDirectory + "\\" +
                                     SettingsUtil
                                         .GetHotUpdateDllsOutputDirByTarget(EditorUserBuildSettings.activeBuildTarget)
                                         .Replace('/', '\\');
        string aotDllDirPath = System.Environment.CurrentDirectory + "\\" +
                               SettingsUtil.GetAssembliesPostIl2CppStripDir(EditorUserBuildSettings.activeBuildTarget)
                                   .Replace('/', '\\');
        string hotUpdateDllTextDirPath = System.Environment.CurrentDirectory + "\\Assets\\Bundle\\HotUpdateDll";
        string aotDllTextDirPath = System.Environment.CurrentDirectory + "\\Assets\\Bundle\\AOTDll";
        foreach (string dllName in SettingsUtil.AOTAssemblyNames)
        {
            string path = $"{aotDllDirPath}\\{dllName}.dll";
            if (File.Exists(path))
            {
                File.Copy(path, $"{aotDllTextDirPath}\\{dllName}.dll.bytes", true);
            }
            else
            {
                path = $"{hotupdateDllDirPath}\\{dllName}.dll";
                File.Copy(path, $"{aotDllTextDirPath}\\{dllName}.dll.bytes", true);
            }
        }
        foreach (string dllName in SettingsUtil.HotUpdateAssemblyNamesExcludePreserved)
        {
            File.Copy($"{hotupdateDllDirPath}\\{dllName}.dll", $"{hotUpdateDllTextDirPath}\\{dllName}.dll.bytes", true);
        }
        AssetDatabase.Refresh();
        Debug.Log("完成生成dll文本文件");
    }
}
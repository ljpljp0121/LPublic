using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using HybridCLR.Editor;
using HybridCLR.Editor.Commands;
using UnityEditor;
using UnityEngine;
using YooAsset;
using YooAsset.Editor;

public enum EBuildBundleType
{
    /// <summary>
    /// 未知类型
    /// </summary>
    Unknown = 0,

    /// <summary>
    /// 虚拟资源包
    /// </summary>
    VirtualBundle = 1,

    /// <summary>
    /// AssetBundle
    /// </summary>
    AssetBundle = 2,

    /// <summary>
    /// 原生文件
    /// </summary>
    RawBundle = 3,
}

public class BuildAssetEditor : EditorWindow
{
    private int selectedPackageIndex = 0;
    private EBuildPipeline buildPipeline = EBuildPipeline.BuiltinBuildPipeline;
    private string bundleVersion = "v1.0.0";
    private bool foolMode = true;


    [MenuItem("Project/Build")]
    public static void ShowWindow()
    {
        GetWindow<BuildAssetEditor>("打包机");
    }

    private void OnGUI()
    {
        foolMode = EditorGUILayout.Toggle("傻瓜模式", foolMode);
        if (foolMode)
        {
            bundleVersion = EditorGUILayout.TextField("版本号", bundleVersion);
        }
        else
        {
            GUILayout.BeginHorizontal();
            List<string> packageNames = GetBuildPackageNames();
            string[] packageNamesArray = packageNames.ToArray();
            if (packageNamesArray.Length > 0)
            {
                selectedPackageIndex = EditorGUILayout.Popup(selectedPackageIndex, packageNamesArray);
            }
            else
            {
                EditorGUILayout.HelpBox("没有找到可用的资源包", MessageType.Warning);
            }

            buildPipeline = (EBuildPipeline)EditorGUILayout.EnumPopup(buildPipeline);

            bundleVersion = EditorGUILayout.TextField("版本号", bundleVersion);
            GUILayout.EndHorizontal();
        }

        BuildOptions();
    }

    private void BuildOptions()
    {
        if (!foolMode)
        {
            GUILayout.BeginHorizontal();

            if (GUILayout.Button("生成Dll文本文件"))
            {
                GenerateDllBytesFile();
            }

            if (GUILayout.Button("开始构建新包"))
            {
                Debug.Log("开始构建新包");
                PrebuildCommand.GenerateAll();
                GenerateDllBytesFile();
                BuildAsset(GetSelectedPackageName(), buildPipeline);
                OssHandler.PutObjectFromFolder(GetSelectedPackageName(), bundleVersion);
                GenerateStreamingAssets();
                AssetDatabase.Refresh();
            }

            if (GUILayout.Button("构建更新包"))
            {
                Debug.Log("开始构建更新包");
                CompileDllCommand.CompileDllActiveBuildTarget();
                GenerateDllBytesFile();
                BuildAsset(GetSelectedPackageName(), buildPipeline);
                OssHandler.PutObjectFromFolder(GetSelectedPackageName(), bundleVersion);
                AssetDatabase.Refresh();
            }

            GUILayout.EndHorizontal();
        }
        else
        {
            GUILayout.BeginHorizontal();

            if (GUILayout.Button("一键构建新包"))
            {
                Debug.Log("开始构建新包");
                PrebuildCommand.GenerateAll();
                GenerateDllBytesFile();
                BuildAsset("DllPackage", EBuildPipeline.RawFileBuildPipeline);
                BuildAsset("ResourcePackage", EBuildPipeline.BuiltinBuildPipeline);
                OssHandler.PutObjectFromFolder("DllPackage", bundleVersion);
                OssHandler.PutObjectFromFolder("ResourcePackage", bundleVersion);
                GenerateStreamingAssets();
                AssetDatabase.Refresh();
            }

            if (GUILayout.Button("一键构建更新包"))
            {
                Debug.Log("开始构建更新包");
                CompileDllCommand.CompileDllActiveBuildTarget();
                GenerateDllBytesFile();
                BuildAsset("DllPackage", EBuildPipeline.RawFileBuildPipeline);
                BuildAsset("ResourcePackage", EBuildPipeline.BuiltinBuildPipeline);

                OssHandler.PutObjectFromFolder("DllPackage", bundleVersion);
                OssHandler.PutObjectFromFolder("ResourcePackage", bundleVersion);
                AssetDatabase.Refresh();
            }

            GUILayout.EndHorizontal();
        }
    }

    private List<string> GetBuildPackageNames()
    {
        List<string> result = new List<string>();
        foreach (var package in AssetBundleCollectorSettingData.Setting.Packages)
        {
            result.Add(package.PackageName);
        }
        return result;
    }

    private string GetSelectedPackageName()
    {
        List<string> packageNames = GetBuildPackageNames();
        if (packageNames.Count == 0 || selectedPackageIndex >= packageNames.Count)
            return string.Empty;

        return packageNames[selectedPackageIndex];
    }

    /// <summary>
    /// 生成Dll文本
    /// </summary>
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

    /// <summary>
    /// 生成StreamingAssets首包资源
    /// </summary>
    private void GenerateStreamingAssets()
    {
        string dllBundleDirPath = System.Environment.CurrentDirectory +
                                  $"\\Bundles\\{EditorUserBuildSettings.activeBuildTarget}\\DllPackage\\{bundleVersion}";
        string resourceBundleDirPath = System.Environment.CurrentDirectory +
                                       $"\\Bundles\\{EditorUserBuildSettings.activeBuildTarget}\\ResourcePackage\\{bundleVersion}";
        string dllStreamingAssetsDirPath = Application.streamingAssetsPath + "\\yoo\\DllPackage";
        string resourceStreamingAssetsDirPath = Application.streamingAssetsPath + "\\yoo\\ResourcePackage";
        string[] versionParts = bundleVersion.Split('.');

        if (versionParts.Length == 3 && versionParts[1] == "0" && versionParts[2] == "0")
        {
            var files1 = Directory.GetFiles(dllBundleDirPath);
            var files2 = Directory.GetFiles(resourceBundleDirPath);
            foreach (var file in files1)
            {
                File.Copy(file, $"{dllStreamingAssetsDirPath}\\{Path.GetFileName(file)}", true);
            }
            foreach (var file in files2)
            {
                File.Copy(file, $"{resourceStreamingAssetsDirPath}\\{Path.GetFileName(file)}", true);
            }
        }
    }

    /// <summary>
    /// 打包资源
    /// </summary>
    private void BuildAsset(string packageName, EBuildPipeline buildPipeline)
    {
        var buildResult = new BuildResult();
        if (buildPipeline == EBuildPipeline.BuiltinBuildPipeline)
        {
            BuiltinBuildParameters builtinParameters =
                BuildCommonParameters<BuiltinBuildParameters>(packageName, buildPipeline);
            builtinParameters.BuildBundleType = (int)EBuildBundleType.AssetBundle;
            builtinParameters.EnableSharePackRule = true;
            builtinParameters.CompressOption =
                AssetBundleBuilderSetting.GetPackageCompressOption(packageName, buildPipeline);
            var pipeline = CreateBuildPipeline(buildPipeline);
            buildResult = pipeline.Run(builtinParameters, true);
        }
        else if (buildPipeline == EBuildPipeline.RawFileBuildPipeline)
        {
            RawFileBuildParameters rawFileParameters =
                BuildCommonParameters<RawFileBuildParameters>(packageName, buildPipeline);
            rawFileParameters.BuildBundleType = (int)EBuildBundleType.RawBundle;
            var pipeline = CreateBuildPipeline(buildPipeline);
            buildResult = pipeline.Run(rawFileParameters, true);
        }
        // 显示构建结果
        if (buildResult.Success)
        {
            EditorUtility.RevealInFinder(buildResult.OutputPackageDirectory);
        }
    }

    /// <summary>
    /// 构建公共参数
    /// </summary>
    private T BuildCommonParameters<T>(string packageName, EBuildPipeline buildPipeline)
        where T : BuildParameters, new()
    {
        T parameters = new T();
        parameters.BuildOutputRoot = AssetBundleBuilderHelper.GetDefaultBuildOutputRoot();
        parameters.BuildinFileRoot = AssetBundleBuilderHelper.GetStreamingAssetsRoot();
        parameters.BuildPipeline = buildPipeline.ToString();
        parameters.BuildTarget = EditorUserBuildSettings.activeBuildTarget;
        parameters.PackageName = packageName;
        parameters.PackageVersion = bundleVersion;
        parameters.VerifyBuildingResult = true;
        parameters.FileNameStyle = AssetBundleBuilderSetting.GetPackageFileNameStyle(packageName, buildPipeline);
        parameters.BuildinFileCopyOption =
            AssetBundleBuilderSetting.GetPackageBuildinFileCopyOption(packageName, buildPipeline);
        parameters.BuildinFileCopyParams =
            AssetBundleBuilderSetting.GetPackageBuildinFileCopyParams(packageName, buildPipeline);
        parameters.ClearBuildCacheFiles =
            AssetBundleBuilderSetting.GetPackageClearBuildCache(packageName, buildPipeline);
        parameters.UseAssetDependencyDB =
            AssetBundleBuilderSetting.GetPackageUseAssetDependencyDB(packageName, buildPipeline);
        parameters.EncryptionServices = CreateEncryptionInstance(packageName, buildPipeline);
        return parameters;
    }

    /// <summary>
    /// 创建构造管线
    /// </summary>
    private IBuildPipeline CreateBuildPipeline(EBuildPipeline buildPipeline)
    {
        switch (buildPipeline)
        {
            case EBuildPipeline.BuiltinBuildPipeline:
                return new BuiltinBuildPipeline();
            case EBuildPipeline.RawFileBuildPipeline:
                return new RawFileBuildPipeline();
            default:
                throw new ArgumentOutOfRangeException(nameof(buildPipeline), buildPipeline, null);
        }
    }

    /// <summary>
    /// 创建加密类实例
    /// </summary>
    private IEncryptionServices CreateEncryptionInstance(string PackageName, EBuildPipeline BuildPipeline)
    {
        var encyptionClassName = AssetBundleBuilderSetting.GetPackageEncyptionClassName(PackageName, BuildPipeline);
        var encryptionClassTypes = EditorTools.GetAssignableTypes(typeof(IEncryptionServices));
        var classType = encryptionClassTypes.Find(x => x.FullName.Equals(encyptionClassName));
        if (classType != null)
            return (IEncryptionServices)Activator.CreateInstance(classType);
        else
            return null;
    }
}
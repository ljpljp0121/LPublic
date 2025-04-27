
using System.Collections;
using System.IO;
using HybridCLR.Editor;
using HybridCLR.Editor.Commands;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;
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

public class BuildTool : OdinEditorWindow
{
    public void Init()
    {
        SetPackageChoices();
    }

    private static IEnumerable PackageChoices = new ValueDropdownList<string>();

    [Title("基本配置")]
    [SerializeField, LabelText("Build管线")]
    private EBuildPipeline buildPipeline = EBuildPipeline.BuiltinBuildPipeline;

    [SerializeField, LabelText("Bundle版本")]
    private string bundleVersion = "v1.0.0";

    [SerializeField, LabelText("资源包")]
    [ValueDropdown("PackageChoices", HideChildProperties = true)]
    private string packageName;

    [Title("CDN配置")]
    [SerializeField, LabelText("CDN地址")]
    private string CdnUrl;


    private static void SetPackageChoices()
    {
        var packageNameChoices = new ValueDropdownList<string>();
        foreach (var package in AssetBundleCollectorSettingData.Setting.Packages)
        {
            packageNameChoices.Add(package.PackageName);
        }
        PackageChoices = packageNameChoices;
    }

    [TabGroup("打包流程","Dll文本生成")]
    [SerializeField, LabelText("热更新Dll生成Bytes位置")]
    private string hotUpdateDllPath = "Assets/Bundle/HotUpdateDll";

    [TabGroup("打包流程", "Dll文本生成")]
    [SerializeField, LabelText("AOTDll生成Bytes位置")]
    private string aotDllPath = "Assets/Bundle/AOTDll";

    [TabGroup("打包流程", "Dll文本生成")]
    [Button("生成Dll文本文件")]
    private void GenerateDllBytesFile()
    {
        Debug.Log("开始生成Dll文本文件");
        string hotUpdateDllDirPath =
            System.Environment.CurrentDirectory
            + "\\"
            + SettingsUtil.GetHotUpdateDllsOutputDirByTarget(EditorUserBuildSettings.activeBuildTarget)
                .Replace('/', '\\');

        string aotDllDirPath =
            System.Environment.CurrentDirectory
            + "\\"
            + SettingsUtil.GetAssembliesPostIl2CppStripDir(EditorUserBuildSettings.activeBuildTarget)
                .Replace('/', '\\');

        string hotUpdateDllTextDirPath =
            System.Environment.CurrentDirectory + "\\" + hotUpdateDllPath.Replace('/', '\\');
        string aotDllTextDirPath = System.Environment.CurrentDirectory + "\\" + aotDllPath.Replace("/", "\\");
        foreach (var dllName in SettingsUtil.AOTAssemblyNames)
        {
            string path = $"{aotDllDirPath}\\{dllName}.dll";
            if (File.Exists(path))
            {
                File.Copy(path, $"{aotDllTextDirPath}\\{dllName}.dll.bytes", true);
            }
            else
            {
                path = $"{hotUpdateDllDirPath}\\{dllName}.dll";
                File.Copy(path, $"{aotDllTextDirPath}\\{dllName}.dll.bytes", true);
            }
        }

        foreach (string dllName in SettingsUtil.HotUpdateAssemblyNamesExcludePreserved)
        {
            File.Copy($"{hotUpdateDllDirPath}\\{dllName}.dll", $"{hotUpdateDllTextDirPath}\\{dllName}.dll.bytes", true);
        }
        AssetDatabase.Refresh();
        Debug.Log("完成生成Dll文件");
    }

    [TabGroup("打包流程", "新包流程")]
    [Button("构建新资源包")]
    private void BuildNew()
    {
        Debug.Log("开始构建新包");
        PrebuildCommand.GenerateAll();
        GenerateDllBytesFile();

    }


}

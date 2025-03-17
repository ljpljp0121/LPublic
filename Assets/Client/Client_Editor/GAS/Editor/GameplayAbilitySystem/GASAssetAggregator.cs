#if UNITY_EDITOR
namespace GAS.Editor
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using GAS.Editor.General;
    using GAS.Editor.Validation;
    using GAS.Runtime;
    using Sirenix.OdinInspector.Editor;
    using Sirenix.Utilities;
    using Sirenix.Utilities.Editor;
    using UnityEditor;
    using UnityEngine;
    using Debug = UnityEngine.Debug;

    /// <summary>
    /// GAS 资源聚合编辑器窗口，用于集中管理各种 GAS 相关资源
    /// </summary>
    public class GASAssetAggregator : OdinMenuEditorWindow
    {
        // 支持的资源类型数组（修饰器计算/游戏特效/效果/能力/ASC预设）
        private static readonly Type[] Types = new Type[5]
        {
            typeof(ModifierMagnitudeCalculation),
            typeof(GameplayCue),
            typeof(GameplayEffectAsset),
            typeof(AbilityAsset),
            typeof(AbilitySystemComponentPreset)
        };

        private static string[] libPaths;

        // 资源库路径属性（延迟初始化）
        static string[] LibPaths
        {
            get
            {
                if (libPaths == null) CheckLibPaths();
                return libPaths;
            }
        }
        // 目录信息存储数组（根目录）
        private static readonly DirectoryInfo[] DirectoryInfos = new DirectoryInfo[5];
        // 子目录列表
        private static readonly List<DirectoryInfo> SubDirectoryInfos = new List<DirectoryInfo>();
        // 菜单显示名称（带排序前缀）
        private static readonly string[] MenuNames = new string[5]
        {
            "A- Mod Magnitude Calculation",
            "A- Gameplay Cue",
            "B- Gameplay Effect",
            "C- Ability",
            "D- Ability System Component"
        };

        [MenuItem("Project/GAS/Asset Aggregator", priority = 1)]
        private static void OpenWindow()
        {
            CheckLibPaths();
            var window = GetWindow<GASAssetAggregator>();
            window.position = GUIHelper.GetEditorWindowRect().AlignCenter(1050, 625);
            window.MenuWidth = 220;
        }

        // 初始化资源库路径
        private static void CheckLibPaths()
        {
            libPaths = new[]
            {
                GASSettingAsset.MMCLibPath,
                GASSettingAsset.GameplayCueLibPath,
                GASSettingAsset.GameplayEffectLibPath,
                GASSettingAsset.GameplayAbilityLibPath,
                GASSettingAsset.ASCLibPath,
            };

            SubDirectoryInfos.Clear();
            for (var i = 0; i < DirectoryInfos.Length; i++)
            {
                var rootMenuName = MenuNames[i];
                DirectoryInfos[i] = new DirectoryInfo(rootMenuName, libPaths[i], libPaths[i], Types[i], true);

                foreach (var subDir in DirectoryInfos[i].SubDirectory)
                    SubDirectoryInfos.Add(new DirectoryInfo(rootMenuName, libPaths[i], subDir, Types[i], false));
            }
        }

        // 构建 Odin 菜单树
        protected override OdinMenuTree BuildMenuTree()
        {
            var tree = new OdinMenuTree();
            tree.Selection.SelectionChanged += OnMenuSelectionChange;
            for (var i = 0; i < MenuNames.Length; i++)
            {
                var menuName = MenuNames[i];
                var libPath = LibPaths[i];
                var type = Types[i];
                tree.Add(menuName, DirectoryInfos[i]);
                // 特殊处理能力菜单，添加概览面板
                if (menuName == MenuNames[3])
                {
                    tree.Add(menuName, new AbilityOverview());
                }
                // 添加该路径下的所有资产
                tree.AddAllAssetsAtPath(menuName, libPath, type, true)
                    .AddThumbnailIcons();
            }

            // 添加所有子目录节点
            foreach (var subDirectoryInfo in SubDirectoryInfos) tree.Add(subDirectoryInfo.MenuName, subDirectoryInfo);

            // 配置菜单树
            tree.Config.DrawSearchToolbar = true; // 显示搜索栏
            tree.Config.SearchToolbarHeight = 30;
            tree.Config.AutoScrollOnSelectionChanged = true;
            tree.Config.DrawScrollView = true;
            tree.Config.AutoHandleKeyboardNavigation = true;
            tree.SortMenuItemsByName(true); // 按名称排序

            return tree;
        }

        // 绘制编辑器顶部工具栏
        protected override void OnBeginDrawEditors()
        {
            var selected = MenuTree.Selection.FirstOrDefault();
            var toolbarHeight = MenuTree.Config.SearchToolbarHeight;

            // Draws a toolbar with the name of the currently selected menu item.
            SirenixEditorGUI.BeginHorizontalToolbar(toolbarHeight);
            {
                if (selected != null) GUILayout.Label(selected.Name);

                // 目录或概览面板的工具栏按钮
                if (selected is { Value: DirectoryInfo or AbilityOverview })
                {
                    DirectoryInfo directoryInfo = selected.Value is AbilityOverview
                        ? DirectoryInfos[3]
                        : selected.Value as DirectoryInfo;

                    // 打开目录按钮
                    if (SirenixEditorGUI.ToolbarButton(new GUIContent("Open In Explorer")))
                        OpenDirectoryInExplorer(directoryInfo);

                    // 创建子目录按钮
                    if (SirenixEditorGUI.ToolbarButton(new GUIContent("Create Sub Directory")))
                    {
                        CreateNewSubDirectory(directoryInfo);
                        GUIUtility
                            .ExitGUI(); // In order to solve: "EndLayoutGroup: BeginLayoutGroup must be called first."
                    }

                    // 创建资源按钮
                    if (SirenixEditorGUI.ToolbarButton(new GUIContent("Create Asset")))
                    {
                        CreateNewAsset(directoryInfo);
                        GUIUtility
                            .ExitGUI(); // In order to solve: "EndLayoutGroup: BeginLayoutGroup must be called first."
                    }

                    // 删除子目录按钮（非根目录）
                    if (directoryInfo is { Root: false })
                        if (SirenixEditorGUI.ToolbarButton(new GUIContent("Remove")))
                        {
                            RemoveSubDirectory(directoryInfo);
                            GUIUtility
                                .ExitGUI(); // In order to solve: "EndLayoutGroup: BeginLayoutGroup must be called first."
                        }
                }

                // 资产对象的工具栏按钮
                if (selected is { Value: ScriptableObject asset })
                {
                    // 在项目中定位
                    if (SirenixEditorGUI.ToolbarButton(new GUIContent("Show In Project")))
                        ShowInProject(asset);

                    // 打开资源所在目录
                    if (SirenixEditorGUI.ToolbarButton(new GUIContent("Open In Explorer")))
                        OpenAssetInExplorer(asset);

                    // 删除资源
                    if (SirenixEditorGUI.ToolbarButton(new GUIContent("Remove")))
                        RemoveAsset(asset);
                }
            }
            SirenixEditorGUI.EndHorizontalToolbar();
        }

        // 刷新界面和资源
        private void Refresh()
        {
            AssetDatabase.Refresh(); // 刷新资产数据库
            CheckLibPaths(); // 重新加载路径
            ForceMenuTreeRebuild(); // 重建菜单树
        }

        // 打开目录资源管理器
        private void OpenDirectoryInExplorer(DirectoryInfo directoryInfo)
        {
            var path = directoryInfo.Directory.Replace("/", "\\");
            Process.Start("explorer.exe", path);
        }

        // 在Project窗口定位资源
        private void ShowInProject(ScriptableObject asset)
        {
            if (asset != null)
            {
                EditorGUIUtility.PingObject(asset);
                Selection.SetActiveObjectWithContext(asset, null);
            }
        }

        // 打开资源所在文件夹
        private void OpenAssetInExplorer(ScriptableObject asset)
        {
            var path = AssetDatabase.GetAssetPath(asset).Replace("/", "\\");
            Process.Start("explorer.exe", path);
        }

        // 创建子目录（带验证）
        private void CreateNewSubDirectory(DirectoryInfo directoryInfo)
        {
            StringEditWindow.OpenWindow("Sub Directory Name", "",
                s =>
                {
                    var newPath = directoryInfo.Directory + "/" + s;
                    if (!AssetDatabase.IsValidFolder(newPath))
                    {
                        return ValidationResult.Invalid("Folder already exists!");
                    }

                    return ValidationResult.Valid;
                },
                s =>
                {
                    var newPath = directoryInfo.Directory + "/" + s;
                    AssetDatabase.CreateFolder(directoryInfo.Directory, s);
                    Refresh();
                    Debug.Log($"[EX] {newPath} folder created!");
                });
        }

        // 删除子目录（带二次确认）
        private void RemoveSubDirectory(DirectoryInfo directoryInfo)
        {
            if (!EditorUtility.DisplayDialog("Warning", "Are you sure you want to delete this folder?", "Yes",
                    "No")) return;

            if (!EditorUtility.DisplayDialog("Second Warning", "ALL FILES in this folder will be DELETED!" +
                                                               "\nAre you sure you want to DELETE this Folder?", "Yes",
                    "No")) return;

            AssetDatabase.DeleteAsset(directoryInfo.Directory);
            Refresh();
            Debug.Log($"[EX] {directoryInfo.Directory} folder deleted!");
        }

        // 创建新资产（根据类型调用不同创建方法）
        private void CreateNewAsset(DirectoryInfo directoryInfo)
        {
            if (directoryInfo.AssetType == Types[0])
                ScriptableObjectCreator.ShowDialog<ModifierMagnitudeCalculation>(directoryInfo.RootDirectory,
                    TrySelectMenuItemWithObject);
            else if (directoryInfo.AssetType == Types[1])
                ScriptableObjectCreator.ShowDialog<GameplayCue>(directoryInfo.RootDirectory,
                    TrySelectMenuItemWithObject);
            else if (directoryInfo.AssetType == Types[2])
                ScriptableObjectCreator.ShowDialog<GameplayEffectAsset>(directoryInfo.RootDirectory,
                    TrySelectMenuItemWithObject);
            else if (directoryInfo.AssetType == Types[3])
                ScriptableObjectCreator.ShowDialog<AbilityAsset>(directoryInfo.RootDirectory,
                    TrySelectMenuItemWithObject);
            else if (directoryInfo.AssetType == Types[4])
                ScriptableObjectCreator.ShowDialog<AbilitySystemComponentPreset>(directoryInfo.RootDirectory,
                    TrySelectMenuItemWithObject);
        }

        // 删除资产（带确认）
        private void RemoveAsset(ScriptableObject asset)
        {
            if (!EditorUtility.DisplayDialog("Warning", "Are you sure you want to delete this asset?", "Yes",
                    "No")) return;

            var name = asset.name; // Get the name before deleting
            AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(asset));
            Refresh();
            Debug.Log($"[EX] {name} asset deleted!");
        }

        // 菜单选择变化回调（用于刷新能力概览）
        private void OnMenuSelectionChange(SelectionChangedType selectionChangedType)
        {
            var selected = MenuTree.Selection.FirstOrDefault();
            if (selected is { Value: AbilityOverview abilityOverview })
            {
                abilityOverview.Refresh();
            }
        }
    }
}
#endif
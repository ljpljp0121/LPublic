using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR

/// <summary>
/// 技能系统全局检视面板（编辑器专用）
/// 功能：验证技能资源配置，生成代码映射，提供可视化错误提示
/// </summary>
public class AbilityOverview
{
    #region 警告信息显示区域

    [BoxGroup("Warning", order: -1)]
    [HideLabel]
    [ShowIf("ExistAbilityWithEmptyUniqueName")] // 当存在空名称时自动显示
    [DisplayAsString(TextAlignment.Left, true)]
    public string WarningAbilityUniqueNameIsNull =
        "<size=13><color=yellow>The <color=orange>Unique Name</color> of the ability must not be <color=red><b>EMPTY</b></color>! " +
        "Please check!</color></size>";

    [BoxGroup("Warning", order: -1)]
    [HideLabel]
    [ShowIf("ExistAbilityWithDuplicatedUniqueName")]// 当存在重复名称时自动显示
    [DisplayAsString(TextAlignment.Left, true)]
    public string WarningAbilityUniqueNameRepeat =
        "<size=13><color=yellow>The <color=orange>Unique Name</color> of the ability must not be <color=red><b>DUPLICATED</b></color>! " +
        "The duplicated abilities are as follows:<color=white> Move,Attack </color>.</color></size>";

    #endregion
    
    [VerticalGroup("Abilities", order: 1)]
    [ListDrawerSettings(ShowFoldout = true, ShowIndexLabels = false, ShowItemCount = false, IsReadOnly = true)]
    [DisplayAsString]
    public List<string> Abilities = new List<string>();

    public AbilityOverview()
    {
        Refresh();
    }

    [HorizontalGroup("Buttons", order: 0, MarginRight = 0.2f)]
    [GUIColor(0, 0.9f, 0.1f, 1)]
    [Button("Generate Ability Collection", ButtonSizes.Large, ButtonStyle.Box, Expanded = true)]
    void GenerateAbilityCollection()
    {
        if (ExistAbilityWithEmptyUniqueName() || ExistAbilityWithDuplicatedUniqueName())
        {
            EditorUtility.DisplayDialog("Warning", "Please check the warning message!\n" +
                                                   "Fix the Unique Name Error!\n" +
                                                   "(If you have fixed all and the warning still exist," +
                                                   " try to refresh the abilities with the REFRESH button.)", "OK");
            return;
        }
        AbilityCollectionGenerator.Gen();
        AssetDatabase.Refresh();
    }


    /// <summary>
    /// 刷新技能数据列表
    /// 遍历指定路径下的所有AbilityAsset资源
    /// </summary>
    [HorizontalGroup("Buttons", width: 50)]
    [GUIColor(1, 1f, 0)]
    [Button(SdfIconType.ArrowRepeat, "", ButtonHeight = 30)]
    [HideLabel]
    public void Refresh()
    {
        Abilities.Clear();
        //var abilityAssets = EditorUtil.FindAssetsByType<AbilityAsset>(GASSettingAsset.GameplayAbilityLibPath);
        //abilityAssets.ForEach(ability =>
        //{
        //    Abilities.Add(ability.UniqueName);
        //});
    }

    /// <summary>
    /// 检测空名称存在性
    /// </summary>
    bool ExistAbilityWithEmptyUniqueName()
    {
        bool existEmpty = Abilities.Exists(string.IsNullOrEmpty);
        return existEmpty;
    }

    /// <summary>
    /// 检测重复名称存在性
    /// 动态更新警告信息中的重复项列表
    /// </summary>
    bool ExistAbilityWithDuplicatedUniqueName()
    {
        var duplicateStrings = FindDuplicateStrings(Abilities);
        bool existDuplicated = duplicateStrings.Count > 0;
        if (existDuplicated)
        {
            string duplicatedUniqueName = duplicateStrings.Aggregate("", (current, d) => current + (d + ","));
            duplicatedUniqueName = duplicatedUniqueName.Remove(duplicatedUniqueName.Length - 1, 1);
            WarningAbilityUniqueNameRepeat =
                "<size=13><color=yellow>The <color=orange>Unique Name</color> of the ability must not be <color=red><b>DUPLICATED</b></color>! " +
                $"The duplicated abilities are as follows: \n <size=15><b><color=white> {duplicatedUniqueName} </color></b></size>.</color></size>";
        }

        return existDuplicated;
    }

    /// <summary>
    /// 查找重复字符串算法
    /// </summary>
    List<string> FindDuplicateStrings(List<string> names)
    {
        List<string> duplicates = names
            .Where(name => !string.IsNullOrEmpty(name))
            .GroupBy(name => name)
            .Where(group => group.Count() > 1)
            .Select(group => group.Key)
            .ToList();

        return duplicates;
    }
}

#endif

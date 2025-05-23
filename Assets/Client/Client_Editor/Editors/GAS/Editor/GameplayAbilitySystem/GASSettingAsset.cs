﻿using System;

#if UNITY_EDITOR
namespace GAS.Editor
{
    using System.IO;
    using GAS;
    using GAS.General;
    using Sirenix.OdinInspector;
    using UnityEditor;
    using UnityEngine;

    /// <summary>
    /// GAS设置面板
    /// </summary>
    [FilePath(GasDefine.GAS_BASE_SETTING_PATH)]
    public class GASSettingAsset : ScriptableSingleton<GASSettingAsset>
    {
        private const int LABEL_WIDTH = 200;
        private const int SHORT_LABEL_WIDTH = 200;
        private static GASSettingAsset setting;
        public static GASSettingAsset Setting
        {
            get
            {
                if (setting == null) setting = LoadOrCreate();
                return setting;
            }
        }

        public static string CodeGenPath => Setting.CodeGeneratePath;

        /// <summary>
        /// 代码生成路径
        /// </summary>
        [Title(SkillDefine.TITLE_SETTING, Bold = true)]
        [BoxGroup("A", false, order: 1)]
        [LabelText(SkillDefine.LABLE_OF_CodeGeneratePath)]
        [LabelWidth(LABEL_WIDTH)]
        [FolderPath]
        [OnValueChanged("SaveAsset")]
        public string CodeGeneratePath = "Assets/Scripts/Gen";

        /// <summary>
        /// 配置资源路径
        /// </summary>
        [BoxGroup("A")]
        [LabelText(SkillDefine.LABLE_OF_GASConfigAssetPath)]
        [LabelWidth(LABEL_WIDTH)]
        [FolderPath]
        [OnValueChanged("SaveAsset")]
        public string GASConfigAssetPath = "Assets/GAS/Config";

        /// <summary>
        /// 能力系统组件路径
        /// </summary>
        [Title(SkillDefine.TITLE_PATHS,Bold = true)]
        [PropertySpace(10)]
        [ShowInInspector]
        [BoxGroup("A")]
        [DisplayAsString(TextAlignment.Left,true)]
        [LabelWidth(SHORT_LABEL_WIDTH)] 
        public static string ASCLibPath => $"{Setting.GASConfigAssetPath}/{GasDefine.GAS_ASC_LIBRARY_FOLDER}";

        /// <summary>
        /// 游戏效果路径
        /// </summary>
        [ShowInInspector]
        [BoxGroup("A")]
        [DisplayAsString(TextAlignment.Left,true)]
        [LabelWidth(SHORT_LABEL_WIDTH)]
        public static string GameplayEffectLibPath =>
            $"{Setting.GASConfigAssetPath}/{GasDefine.GAS_EFFECT_LIBRARY_FOLDER}";

        /// <summary>
        /// 游戏能力库
        /// </summary>
        [ShowInInspector]
        [BoxGroup("A")]
        [DisplayAsString(TextAlignment.Left,true)]
        [LabelWidth(SHORT_LABEL_WIDTH)]
        public static string GameplayAbilityLibPath =>
            $"{Setting.GASConfigAssetPath}/{GasDefine.GAS_ABILITY_LIBRARY_FOLDER}";

        /// <summary>
        /// 游戏提示库
        /// </summary>
        [ShowInInspector]
        [BoxGroup("A")]
        [DisplayAsString(TextAlignment.Left,true)]
        [LabelWidth(SHORT_LABEL_WIDTH)]
        public static string GameplayCueLibPath => $"{Setting.GASConfigAssetPath}/{GasDefine.GAS_CUE_LIBRARY_FOLDER}";

        /// <summary>
        /// ModifierMagnitudeCalculation修改器路径
        /// </summary>
        [ShowInInspector]
        [BoxGroup("A")]
        [DisplayAsString(TextAlignment.Left,true)]
        [LabelWidth(SHORT_LABEL_WIDTH)]
        public static string MMCLibPath => $"{Setting.GASConfigAssetPath}/{GasDefine.GAS_MMC_LIBRARY_FOLDER}";


        [ShowInInspector]
        [BoxGroup("A")]
        [DisplayAsString(TextAlignment.Left,true)]
        [LabelWidth(SHORT_LABEL_WIDTH)]
        public static string AbilityTaskLib => $"{Setting.GASConfigAssetPath}/{GasDefine.GAS_ABILITY_TASK_LIBRARY_FOLDER}";

        [ShowInInspector]
        [BoxGroup("A")]
        [DisplayAsString(TextAlignment.Left, true)]
        [LabelWidth(SHORT_LABEL_WIDTH)]
        [LabelText("Tag Asset Path")]
        public static string GAS_TAG_ASSET_PATH => GasDefine.GAS_TAGS_MANAGER_ASSET_PATH;

        [ShowInInspector]
        [BoxGroup("A")]
        [DisplayAsString(TextAlignment.Left, true)]
        [LabelWidth(SHORT_LABEL_WIDTH)]
        [LabelText("Attribute Asset Path")]
        public static string GAS_ATTRIBUTE_ASSET_PATH => GasDefine.GAS_ATTRIBUTE_ASSET_PATH;

        [ShowInInspector]
        [BoxGroup("A")]
        [DisplayAsString(TextAlignment.Left, true)]
        [LabelWidth(SHORT_LABEL_WIDTH)]
        [LabelText("AttributeSet Asset Path")]
        public static string GAS_ATTRIBUTESET_ASSET_PATH => GasDefine.GAS_ATTRIBUTE_SET_ASSET_PATH;

        /// <summary>
        /// 检查路径文件夹是否存在
        /// </summary>
        /// <param name="folderPath"></param>
        void CheckPathFolderExist(string folderPath)
        {
            var folders = folderPath.Split('/');
            if (folders[0] != "Assets")
            {
                EditorUtility.DisplayDialog("Error!", "'Config Asset Path/Code Gen Path' must start with Assets!",
                    "OK");
                return;
            }

            string parentFolderPath = folders[0];
            for (var i = 1; i < folders.Length; i++)
            {
                string newFolderName = folders[i];
                if (newFolderName == "") continue;

                string newFolderPath = parentFolderPath + "/" + newFolderName;
                if (!AssetDatabase.IsValidFolder(newFolderPath))
                {
                    AssetDatabase.CreateFolder(parentFolderPath, newFolderName);
                    Debug.Log("Folder created at path: " + newFolderPath);
                }

                parentFolderPath += "/" + newFolderName;
            }
        }
        
        [BoxGroup("A")]
        [DisplayAsString(TextAlignment.Left,true)]
        [GUIColor(0,0.8f,0)]
        [PropertySpace(10)]
        // [InfoBox(GASTextDefine.TIP_CREATE_FOLDERS)]
        [Button(SdfIconType.FolderCheck,SkillDefine.BUTTON_CheckAllPathFolderExist,ButtonHeight = 38)]
        void CheckAllPathFolderExist()
        {
            CheckPathFolderExist(GASConfigAssetPath);
            CheckPathFolderExist(CodeGeneratePath);
            CheckPathFolderExist(ASCLibPath);
            CheckPathFolderExist(GameplayAbilityLibPath);
            CheckPathFolderExist(GameplayEffectLibPath);
            CheckPathFolderExist(GameplayCueLibPath);
            CheckPathFolderExist(MMCLibPath);
            CheckPathFolderExist(AbilityTaskLib);
            AssetDatabase.Refresh();
        }

        [BoxGroup("A")]
        [DisplayAsString(TextAlignment.Left, true)]
        [GUIColor(0.8f, 0.8f, 0)]
        [PropertySpace(10)]
        // [InfoBox(GASTextDefine.TIP_CREATE_GEN_AscUtilCode)]
        [Button(SdfIconType.Upload, SkillDefine.BUTTON_GenerateAscExtensionCode, ButtonHeight = 38)]
        void GenerateAscExtensionCode()
        {
            string pathWithoutAssets = Application.dataPath.Substring(0, Application.dataPath.Length - 6);
            var filePath =
                $"{pathWithoutAssets}/{GASSettingAsset.CodeGenPath}/{GasDefine.GAS_ATTRIBUTESET_LIB_CSHARP_SCRIPT_NAME}";
            
            if (!File.Exists(filePath))
            {
                EditorUtility.DisplayDialog("Error!", "Please generate AttributeSetAsset first!", "OK");
                return;
            }
            
            AbilitySystemComponentUtilGenerator.Gen();
            AssetDatabase.Refresh();
        }

        private void SaveAsset()
        {
            if (Instance == this) return;
            UpdateAsset(this);
            Save();
        }
    }
}
#endif
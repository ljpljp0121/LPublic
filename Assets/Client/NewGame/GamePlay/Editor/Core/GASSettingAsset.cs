#if UNITY_EDITOR
namespace Game.Editor
{
    using Sirenix.OdinInspector;
    using UnityEditor;
    using UnityEngine;
    [FilePath(GameDefine.GAME_SETTING_ASSET_PATH)]
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

        [Title("设置")]
        [BoxGroup("A", false, order: 1)]
        [LabelText("脚本生成路径")]
        [LabelWidth(LABEL_WIDTH)]
        [FolderPath]
        [OnValueChanged("SaveAsset")]
        public string CodeGeneratePath = "Assets/Scripts/Gen";

        [Title("路径")]
        [BoxGroup("A")]
        [DisplayAsString(TextAlignment.Left, true)]
        [LabelWidth(SHORT_LABEL_WIDTH)]
        public static string GAS_TAG_ASSET_PATH => "ProjectSettings/GameplayTagsAsset.asset";

        private void CheckPathFolderExist(string folderPath)
        {
            var folders = folderPath.Split('/');
            if (folders[0] != "Assets")
            {
                EditorUtility.DisplayDialog("Error!", "代码生成路径必须在Assets之下", "OK");
                return;
            }

            string parentFolderPath = folders[0];
            for (int i = 1; i < folders.Length; i++)
            {
                string newFolderName = folders[i];
                if (newFolderName == "") continue;

                string newfolderPath = $"{parentFolderPath}/{newFolderName}";
                if (!AssetDatabase.IsValidFolder(newfolderPath))
                {
                    AssetDatabase.CreateFolder(parentFolderPath, newFolderName);
                    Debug.Log("创建文件夹,路径:" + newfolderPath);
                }

                parentFolderPath += "/" + newFolderName;
            }
        }

        [BoxGroup("A")]
        [DisplayAsString(TextAlignment.Left, true)]
        [GUIColor(0, 0.6f, 0)]
        [PropertySpace(10)]
        [InfoBox("请点击该按钮来保证代码生成路径正确")]
        [Button(SdfIconType.FolderCheck, "检查路径配置", ButtonHeight = 30)]
        void CheckAllPathFolderExist()
        {
            CheckPathFolderExist(CodeGeneratePath);
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
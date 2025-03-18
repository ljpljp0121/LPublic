#if UNITY_EDITOR
namespace GAS.Editor
{
    using Sirenix.OdinInspector.Editor;
    using Sirenix.Utilities;
    using Sirenix.Utilities.Editor;
    using UnityEditor;

    /// <summary>
    /// GAS 设置聚合编辑器窗口，用于集中管理各种 GAS 相关资源配置
    /// </summary>
    public class GASSettingAggregator : OdinMenuEditorWindow
    {
        private static GASSettingAsset settingAsset;

        private static GameplayTagsAsset tagsAsset; 

        private static AttributeAsset attributeAsset;

        private static AttributeSetAsset attributeSetAsset;

        /// <summary>
        /// GAS核心设置资源属性
        /// </summary>
        private static GASSettingAsset SettingAsset
        {
            get
            {
                if (settingAsset == null) settingAsset = GASSettingAsset.LoadOrCreate();
                return settingAsset;
            }
        }

        /// <summary>
        /// 标签资源属性
        /// </summary>
        private static GameplayTagsAsset TagsAsset
        {
            get
            {
                if (tagsAsset == null) tagsAsset = GameplayTagsAsset.LoadOrCreate();
                return tagsAsset;
            }
        }

        /// <summary>
        /// 属性资源
        /// </summary>
        private static AttributeAsset AttributeAsset
        {
            get
            {
                if (attributeAsset == null) attributeAsset = AttributeAsset.LoadOrCreate();
                return attributeAsset;
            }
        }

        /// <summary>
        /// 属性集合资源
        /// </summary>
        private static AttributeSetAsset AttributeSetAsset
        {
            get
            {
                if (attributeSetAsset == null) attributeSetAsset = AttributeSetAsset.LoadOrCreate();
                return attributeSetAsset;
            }
        }

        [MenuItem("Project/GAS/Settings", priority = 0)]
        private static void OpenWindow()
        {
            var window = GetWindow<GASSettingAggregator>();
            window.position = GUIHelper.GetEditorWindowRect().AlignCenter(900, 600);
        }

        protected override OdinMenuTree BuildMenuTree()
        {
            var tree = new OdinMenuTree();

            // 添加菜单项（显示名称，关联对象）
            tree.Add("Setting", SettingAsset);
            tree.Add("Tags", TagsAsset);
            tree.Add("Attribute", AttributeAsset);
            tree.Add("Attribute Set", AttributeSetAsset);

            // 配置菜单树属性
            tree.Config.AutoScrollOnSelectionChanged = true;
            tree.Config.DrawScrollView = true;
            tree.Config.AutoHandleKeyboardNavigation = true;
            tree.Selection.SelectionChanged += type =>
            {
                GASSettingAsset.Save();
                GameplayTagsAsset.Save();
                AttributeAsset.Save();
                AttributeSetAsset.Save();
            };
            return tree;
        }
    }
}
#endif
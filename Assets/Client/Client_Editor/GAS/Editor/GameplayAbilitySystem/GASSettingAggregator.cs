#if UNITY_EDITOR
namespace GAS.Editor
{
    using Sirenix.OdinInspector.Editor;
    using Sirenix.Utilities;
    using Sirenix.Utilities.Editor;
    using UnityEditor;
    
    public class GASSettingAggregator : OdinMenuEditorWindow
    {
        private static GASSettingAsset settingAsset;

        private static GameplayTagsAsset tagsAsset;

        private static AttributeAsset attributeAsset;

        private static AttributeSetAsset attributeSetAsset;

        private static GASSettingAsset SettingAsset
        {
            get
            {
                if (settingAsset == null) settingAsset = GASSettingAsset.LoadOrCreate();
                return settingAsset;
            }
        }

        private static GameplayTagsAsset TagsAsset
        {
            get
            {
                if (tagsAsset == null) tagsAsset = GameplayTagsAsset.LoadOrCreate();
                return tagsAsset;
            }
        }

        private static AttributeAsset AttributeAsset
        {
            get
            {
                if (attributeAsset == null) attributeAsset = AttributeAsset.LoadOrCreate();
                return attributeAsset;
            }
        }

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

            tree.Add("Setting", SettingAsset);
            tree.Add("Tags", TagsAsset);
            tree.Add("Attribute", AttributeAsset);
            tree.Add("Attribute Set", AttributeSetAsset);

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
#if UNITY_EDITOR
namespace Game.Editor
{
    using Sirenix.OdinInspector.Editor;
    using Sirenix.Utilities;
    using Sirenix.Utilities.Editor;
    using UnityEditor;

    public class GASAggregator : OdinMenuEditorWindow
    {
        private static GASSettingAsset settingAsset;
        private static GameTagsAsset gameTagsAsset;

        private static GASSettingAsset SettingAsset
        {
            get
            {
                if (settingAsset == null) settingAsset = GASSettingAsset.LoadOrCreate();
                return settingAsset;
            }
        }
        private static GameTagsAsset GameTagsAsset
        {
            get
            {
                if (gameTagsAsset == null) gameTagsAsset = GameTagsAsset.LoadOrCreate();
                return gameTagsAsset;
            }
        }

        [MenuItem("Project/Gameplay/Settings", priority = 0)]
        private static void OpenWindow()
        {
            var window = GetWindow<GASAggregator>();
            window.position = GUIHelper.GetEditorWindowRect().AlignCenter(900, 600);
        }

        protected override OdinMenuTree BuildMenuTree()
        {
            var tree = new OdinMenuTree();
            tree.Add("Setting", SettingAsset);
            tree.Add("Tags", GameTagsAsset);

            tree.Config.AutoScrollOnSelectionChanged = true;
            tree.Config.DrawScrollView = true;
            tree.Config.AutoHandleKeyboardNavigation = true;
            tree.Selection.SelectionChanged += type =>
            {
                GASSettingAsset.Save();
                GameTagsAsset.Save();
            };
            return tree;
        }
    }
}
#endif
#if UNITY_EDITOR

using System.Collections.Generic;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEditor.TreeDataModel;

namespace Game.Editor
{
    [CustomEditor(typeof(GameTagsAsset))]
    public class GameTagsTreeAssetEditor : UnityEditor.Editor
    {
        private const string SessionStateKeyPrefix = "TVS";
        private SearchField searchField;
        private GameTagTreeView treeView;

        private GameTagsAsset Asset => (GameTagsAsset)target;


        private class GameTagTreeView : TreeViewWithTreeModel<GameTagTreeElement>
        {
            private readonly GameTagsAsset asset;
            public GameTagTreeView(TreeViewState state, TreeModel<GameTagTreeElement> model, GameTagsAsset asset) : base(state, model)
            {
                showBorder = true;
                showAlternatingRowBackgrounds = true;
                this.asset = asset;
            }

            public override void OnDropDraggedElementsAtIndex(List<TreeViewItem> draggedRows, GameTagTreeElement parent, int insertIndex)
            {
                base.OnDropDraggedElementsAtIndex(draggedRows, parent, insertIndex);
                asset.CacheTags();
                EditorUtility.SetDirty(asset);
                GameTagsAsset.Save();
            }
        }
    }


}

#endif
#if UNITY_EDITOR

using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEditor.TreeDataModel;
using UnityEditor.UIElements;
using UnityEngine;

namespace Game.Editor
{
    [CustomEditor(typeof(GameTagsAsset))]
    public class GameTagsTreeAssetEditor : UnityEditor.Editor
    {
        private const string SessionStateKeyPrefix = "TVS";
        private SearchField searchField;
        private GameTagTreeView treeView;

        private GameTagsAsset Asset => (GameTagsAsset)target;

        private void OnEnable()
        {
            Undo.undoRedoPerformed += OnUndoRedoPerformed;

            var treeViewState = new TreeViewState();
            var jsonState = SessionState.GetString(SessionStateKeyPrefix + Asset.GetInstanceID(), "");
            if (!string.IsNullOrEmpty(jsonState))
                JsonUtility.FromJsonOverwrite(jsonState, treeViewState);
            var treeModel = new TreeModel<GameTagTreeElement>(Asset.GameTagTreeElements);
            treeView = new GameTagTreeView(treeViewState, treeModel, Asset);
            treeView.BeforeDroppingDraggedItems += OnBeforeDroppingDraggedItems;
            treeView.Reload();

            searchField = new SearchField();
            searchField.downOrUpArrowKeyPressed += treeView.SetFocusAndEnsureSelectedItem;

            if (!treeView.TreeModel.Root.HasChildren) CreateFirstTag();
        }


        private void OnDisable()
        {
            Undo.undoRedoPerformed -= OnUndoRedoPerformed;

            SessionState.SetString(SessionStateKeyPrefix + Asset.GetInstanceID(), JsonUtility.ToJson(treeView.state));
        }

        /// <summary>
        /// 撤销/重做操作
        /// </summary>
        private void OnUndoRedoPerformed()
        {
            if (treeView != null)
            {
                treeView.TreeModel.SetData(Asset.GameTagTreeElements);
                treeView.Reload();
            }
        }

        private void OnBeforeDroppingDraggedItems(IList<TreeViewItem> draggedRows)
        {
            Undo.RecordObject(Asset, $"Moving{draggedRows.Count}Tag{(draggedRows.Count > 1 ? "s" : "")}");
        }

        public override void OnInspectorGUI()
        {
            GUILayout.Space(5f);
            DrawToolBar();
            GUILayout.Space(3f);

            const float topToolbarHeight = 20f;
            const float spacing = 2f;
            var totalHeight = treeView.totalHeight + topToolbarHeight + 2 * spacing;
            var rect = GUILayoutUtility.GetRect(0, 10000, 0, totalHeight);
            var toolbarRect = new Rect(rect.x, rect.y, rect.width, topToolbarHeight);
            var multiColumnTreeViewRect = new Rect(rect.x, rect.y + topToolbarHeight + spacing, rect.width,
                rect.height - topToolbarHeight - 2 * spacing);
            DrawSearchBar(toolbarRect);
            DrawTreeView(multiColumnTreeViewRect);
        }

        private void DrawToolBar()
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                var style = "miniButton";
                if (GUILayout.Button("展开全部", style)) treeView.ExpandAll();
                if (GUILayout.Button("折叠全部", style)) treeView.CollapseAll();
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("添加标签", style)) CreateTag();
                if (GUILayout.Button("移除标签", style)) RemoveTags();
                if (GUILayout.Button("生成代码", style)) GenCode();
            }
        }
        private void DrawSearchBar(Rect rect)
        {
            treeView.searchString = searchField.OnGUI(rect, treeView.searchString);
        }

        private void DrawTreeView(Rect rect)
        {
            treeView.OnGUI(rect);
        }


        private void CreateTag()
        {
            Undo.RecordObject(Asset, "Add Tag To Asset");
            StringEditWindow.OpenWindow("Tag", "", Validations.ValidateVariableName, AddTag, "创建新标签");
            GUIUtility.ExitGUI();
        }

        private void AddTag(string tagName)
        {
            Undo.RecordObject(Asset, "Add Tag To Asset");
            var selection = treeView.GetSelection();
            TreeElement parent = (selection.Count == 1 ? treeView.TreeModel.Find(selection[0]) : null) ??
                treeView.TreeModel.Root;
            var depth = parent != null ? parent.Depth + 1 : 0;
            var id = treeView.TreeModel.GenerateUniqueId();
            var element = new GameTagTreeElement(tagName, depth, id);
            treeView.TreeModel.AddElement(element, parent, 0);

            treeView.SetSelection(new[] { id }, TreeViewSelectionOptions.RevealAndFrame);
            SaveAsset();
        }

        private void RemoveTags()
        {
            var selection = treeView.GetSelection();

            var sb = new StringBuilder("Tags: \n");
            foreach (var selectedItem in selection)
            {
                TreeElement tag = treeView.TreeModel.Find(selectedItem);
                BuildTagPath(sb, tag, "    " + GetPrefix(tag));
            }

            var result = EditorUtility.DisplayDialog("Remove Tags Confirmation",
                "Are you sure you want to REMOVE the following tags?\n" +
                "Note: All associated sub tags will also be removed.\n" +
                sb.ToString(),
                "Remove Tags",
                "Cancel");

            if (result)
            {
                Undo.RecordObject(Asset, "Remove Tag From Asset");

                treeView.TreeModel.RemoveElements(selection);
                SaveAsset();
            }
        }

        private void BuildTagPath(StringBuilder sb, TreeElement tag, string prefix, bool isSubTag = false)
        {
            var tagName = prefix + tag.Name;
            var s = tagName + (isSubTag ? " (sub tag)" : "");
            sb.AppendLine(s);

            if (tag.Children != null)
            {
                foreach (var child in tag.Children)
                {
                    BuildTagPath(sb, child, tagName + ".", true);
                }
            }
        }

        private string GetPrefix(TreeElement tag)
        {
            string prefix = "";
            var parent = tag.Parent;
            while (parent != null && parent.Depth >= 0)
            {
                prefix = parent.Name + "." + prefix;
                parent = parent.Parent;
            }

            return prefix;
        }

        private void CreateFirstTag()
        {
            AddTag("Ability");
        }

        private void SaveAsset()
        {
            Asset.CacheTags();
            EditorUtility.SetDirty(Asset);
            GameTagsAsset.UpdateAsset(Asset);
            GameTagsAsset.Save();
        }

        private void GenCode()
        {
            GameTagLibGen.Gen();
            AssetDatabase.Refresh();
        }


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
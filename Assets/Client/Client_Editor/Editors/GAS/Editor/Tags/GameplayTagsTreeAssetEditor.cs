#if UNITY_EDITOR
using System.Collections.Generic;
using System.Text;
using GAS.Editor.General;
using GAS.Editor.Validation;
using GAS.General;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEditor.TreeDataModel;
using UnityEngine;

namespace GAS.Editor
{
    /// <summary>
    /// 游戏标签资产的编辑器扩展类
    /// 功能：提供可视化的树形标签编辑界面
    /// </summary>
    [CustomEditor(typeof(GameplayTagsAsset))]
    public class GameplayTagsTreeAssetEditor : UnityEditor.Editor
    {
        private const string KSessionStateKeyPrefix = "TVS";
        private SearchField _searchField;
        private GameplayTagTreeView _treeView;

        private GameplayTagsAsset Asset => (GameplayTagsAsset)target;

        private void OnEnable()
        {
            Undo.undoRedoPerformed += OnUndoRedoPerformed;
            // 初始化树视图状态
            var treeViewState = new TreeViewState();
            var jsonState = SessionState.GetString(KSessionStateKeyPrefix + Asset.GetInstanceID(), "");
            if (!string.IsNullOrEmpty(jsonState))
                JsonUtility.FromJsonOverwrite(jsonState, treeViewState);
            // 创建树形数据模型
            var treeModel = new TreeModel<GameplayTagTreeElement>(Asset.GameplayTagTreeElements);
            // 初始化自定义树视图
            _treeView = new GameplayTagTreeView(treeViewState, treeModel, Asset);
            _treeView.beforeDroppingDraggedItems += OnBeforeDroppingDraggedItems;
            _treeView.Reload();
            // 初始化搜索框
            _searchField = new SearchField();
            _searchField.downOrUpArrowKeyPressed += _treeView.SetFocusAndEnsureSelectedItem;

            // 自动创建初始标签
            if (!_treeView.treeModel.Root.HasChildren) CreateFirstTag();
        }

        private void OnDisable()
        {
            Undo.undoRedoPerformed -= OnUndoRedoPerformed;

            SessionState.SetString(KSessionStateKeyPrefix + Asset.GetInstanceID(),
                JsonUtility.ToJson(_treeView.state));
        }

        // 处理撤销/重做操作
        private void OnUndoRedoPerformed()
        {
            if (_treeView != null)
            {
                _treeView.treeModel.SetData(Asset.GameplayTagTreeElements);
                _treeView.Reload();
            }
        }

        // 拖拽操作前的记录
        private void OnBeforeDroppingDraggedItems(IList<TreeViewItem> draggedRows)
        {
            Undo.RecordObject(Asset,
                $"Moving {draggedRows.Count} Tag{(draggedRows.Count > 1 ? "s" : "")}");
        }

        // 主绘制逻辑
        public override void OnInspectorGUI()
        {
            GUILayout.Space(5f);
            ToolBar();
            GUILayout.Space(3f);

            const float topToolbarHeight = 20f;
            const float spacing = 2f;
            var totalHeight = _treeView.totalHeight + topToolbarHeight + 2 * spacing;
            var rect = GUILayoutUtility.GetRect(0, 10000, 0, totalHeight);
            var toolbarRect = new Rect(rect.x, rect.y, rect.width, topToolbarHeight);
            var multiColumnTreeViewRect = new Rect(rect.x, rect.y + topToolbarHeight + spacing, rect.width,
                rect.height - topToolbarHeight - 2 * spacing);
            SearchBar(toolbarRect);
            DoTreeView(multiColumnTreeViewRect);
        }

        private void SearchBar(Rect rect)
        {
            _treeView.searchString = _searchField.OnGUI(rect, _treeView.searchString);
        }

        private void DoTreeView(Rect rect)
        {
            _treeView.OnGUI(rect);
        }

        // 绘制顶部工具栏
        private void ToolBar()
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                var style = "miniButton";
                if (GUILayout.Button(SkillDefine.BUTTON_ExpandAllTag, style)) _treeView.ExpandAll();

                if (GUILayout.Button(SkillDefine.BUTTON_CollapseAllTag, style)) _treeView.CollapseAll();

                GUILayout.FlexibleSpace();

                if (GUILayout.Button(SkillDefine.BUTTON_AddTag, style)) CreateTag();

                if (GUILayout.Button(SkillDefine.BUTTON_RemoveTag, style)) RemoveTags();

                if (GUILayout.Button(SkillDefine.BUTTON_GenTagCode, style)) GenCode();
            }
        }

        private void AddTag(string tagName)
        {
            Undo.RecordObject(Asset, "Add Item To Asset");
            var selection = _treeView.GetSelection();
            TreeElement parent = (selection.Count == 1 ? _treeView.treeModel.Find(selection[0]) : null) ??
                                 _treeView.treeModel.Root;
            var depth = parent != null ? parent.Depth + 1 : 0;
            var id = _treeView.treeModel.GenerateUniqueID();
            var element = new GameplayTagTreeElement(tagName, depth, id);
            _treeView.treeModel.AddElement(element, parent, 0);

            // Select newly created element
            _treeView.SetSelection(new[] { id }, TreeViewSelectionOptions.RevealAndFrame);
            SaveAsset();
        }

        public void CreateTag()
        {
            Undo.RecordObject(Asset, "Add Item To Asset");
            StringEditWindow.OpenWindow("Tag", "", Validations.ValidateVariableName, AddTag, "Create new Tag");
            GUIUtility.ExitGUI(); // In order to solve: "EndLayoutGroup: BeginLayoutGroup must be called first."
        }

        public void RemoveTags()
        {
            var selection = _treeView.GetSelection();

            var sb = new StringBuilder("Tags: \n");
            foreach (var selectedItem in selection)
            {
                TreeElement tag = _treeView.treeModel.Find(selectedItem);
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

                _treeView.treeModel.RemoveElements(selection);
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
            GameplayTagsAsset.UpdateAsset(Asset);
            GameplayTagsAsset.Save();
        }

        void GenCode()
        {
            GTagLibGenerator.Gen();
            AssetDatabase.Refresh();
        }

        private class GameplayTagTreeView : TreeViewWithTreeModel<GameplayTagTreeElement>
        {
            private readonly GameplayTagsAsset _asset;

            public GameplayTagTreeView(TreeViewState state, TreeModel<GameplayTagTreeElement> model,
                GameplayTagsAsset asset)
                : base(state, model)
            {
                showBorder = true;
                showAlternatingRowBackgrounds = true;
                _asset = asset;
            }

            public override void OnDropDraggedElementsAtIndex(List<TreeViewItem> draggedRows,
                GameplayTagTreeElement parent, int insertIndex)
            {
                base.OnDropDraggedElementsAtIndex(draggedRows, parent, insertIndex);
                _asset.CacheTags();
                EditorUtility.SetDirty(_asset);
                GameplayTagsAsset.Save();
            }
        }
    }
}
#endif
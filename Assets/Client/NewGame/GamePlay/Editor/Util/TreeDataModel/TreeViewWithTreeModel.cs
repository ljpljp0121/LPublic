#if UNITY_EDITOR

namespace UnityEditor.TreeDataModel
{
    using System;
    using System.Linq;
    using UnityEditor.IMGUI.Controls;
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// 树视图项 绑定数据模型与视图
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal class TreeViewItem<T> : TreeViewItem where T : TreeElement
    {
        public TreeViewItem(int id, int depth, string displayName, T data) : base(id, depth, displayName)
        {
            this.Data = data;
        }
        public T Data { get; set; }
    }

    public class TreeViewWithTreeModel<T> : TreeView where T : TreeElement
    {
        private const string GenericDragID = "GenericDragColumnDragging";
        private readonly List<TreeViewItem> rows = new List<TreeViewItem>(100);

        public TreeViewWithTreeModel(TreeViewState state, TreeModel<T> model) : base(state)
        {
            Init(model);
        }
        public TreeViewWithTreeModel(TreeViewState state, MultiColumnHeader multiColumnHeader, TreeModel<T> model)
            : base(state, multiColumnHeader)
        {
            Init(model);
        }

        public TreeModel<T> TreeModel { get; private set; }
        public event Action TreeChanged;
        public event Action<IList<TreeViewItem>> BeforeDroppingDraggedItems;

        /// 初始化模型绑定
        private void Init(TreeModel<T> model)
        {
            TreeModel = model;
            TreeModel.ModelChanged += ModelChanged;
        }

        private void ModelChanged()
        {
            if (TreeChanged != null)
                TreeChanged();

            Reload();
        }

        /// <summary>
        /// 构建TreeView根节点（隐藏根）
        /// </summary>
        protected override TreeViewItem BuildRoot()
        {
            var depthForHiddenRoot = -1;
            return new TreeViewItem<T>(TreeModel.Root.ID, depthForHiddenRoot, TreeModel.Root.Name, TreeModel.Root);
        }

        /// <summary>
        /// 构建可见的行数据（处理展开/搜索状态）
        /// </summary>
        protected override IList<TreeViewItem> BuildRows(TreeViewItem root)
        {
            if (TreeModel.Root == null) Debug.LogError("tree model root is null. did you call SetData()?");

            rows.Clear();
            if (!string.IsNullOrEmpty(searchString))
            {
                Search(TreeModel.Root, searchString, rows);
            }
            else
            {
                if (TreeModel.Root.HasChildren)
                    AddChildrenRecursive(TreeModel.Root, 0, rows);
            }

            SetupParentsAndChildrenFromDepths(root, rows);

            return rows;
        }

        private void AddChildrenRecursive(T parent, int depth, IList<TreeViewItem> newRows)
        {
            foreach (T child in parent.Children)
            {
                var item = new TreeViewItem<T>(child.ID, depth, child.Name, child);
                newRows.Add(item);

                if (child.HasChildren)
                {
                    if (IsExpanded(child.ID))
                        AddChildrenRecursive(child, depth + 1, newRows);
                    else
                        item.children = CreateChildListForCollapsedParent();
                }
            }
        }

        private void Search(T searchFromThis, string search, List<TreeViewItem> result)
        {
            if (string.IsNullOrEmpty(search))
                throw new ArgumentException("Invalid search: cannot be null or empty", "search");

            const int kItemDepth = 0; 

            var stack = new Stack<T>();
            foreach (var element in searchFromThis.Children)
                stack.Push((T)element);
            while (stack.Count > 0)
            {
                var current = stack.Pop();
   
                if (current.Name.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0)
                    result.Add(new TreeViewItem<T>(current.ID, kItemDepth, current.Name, current));

                if (current.Children != null && current.Children.Count > 0)
                    foreach (var element in current.Children)
                        stack.Push((T)element);
            }

            SortSearchResult(result);
        }

        /// <summary>
        /// 自定义搜索结果的排序规则（按名称自然排序）
        /// </summary>
        protected virtual void SortSearchResult(List<TreeViewItem> rows)
        {
            rows.Sort((x, y) =>
                EditorUtility.NaturalCompare(x.displayName,
                    y.displayName)); 
        }

        protected override IList<int> GetAncestors(int id)
        {
            return TreeModel.GetFathers(id);
        }

        protected override IList<int> GetDescendantsThatHaveChildren(int id)
        {
            return TreeModel.GetChildren(id);
        }

        protected override bool CanStartDrag(CanStartDragArgs args)
        {
            return true;
        }

        protected override void SetupDragAndDrop(SetupDragAndDropArgs args)
        {
            if (hasSearch)
                return;

            DragAndDrop.PrepareStartDrag();
            var draggedRows = GetRows().Where(item => args.draggedItemIDs.Contains(item.id)).ToList();
            DragAndDrop.SetGenericData(GenericDragID, draggedRows);
            DragAndDrop.objectReferences = new UnityEngine.Object[] { }; // this IS required for dragging to work
            var title = draggedRows.Count == 1 ? draggedRows[0].displayName : "< Multiple >";
            DragAndDrop.StartDrag(title);
        }

        /// <summary>
        /// 处理拖拽操作（验证+插入元素）
        /// </summary>
        protected override DragAndDropVisualMode HandleDragAndDrop(DragAndDropArgs args)
        {
            var draggedRows = DragAndDrop.GetGenericData(GenericDragID) as List<TreeViewItem>;
            if (draggedRows == null)
                return DragAndDropVisualMode.None;

            switch (args.dragAndDropPosition)
            {
                case DragAndDropPosition.UponItem:
                case DragAndDropPosition.BetweenItems:
                    {
                        var validDrag = ValidDrag(args.parentItem, draggedRows);
                        if (args.performDrop && validDrag)
                        {
                            var parentData = ((TreeViewItem<T>)args.parentItem).Data;
                            OnDropDraggedElementsAtIndex(draggedRows, parentData,
                                args.insertAtIndex == -1 ? 0 : args.insertAtIndex);
                        }

                        return validDrag ? DragAndDropVisualMode.Move : DragAndDropVisualMode.None;
                    }

                case DragAndDropPosition.OutsideItems:
                    {
                        if (args.performDrop)
                            OnDropDraggedElementsAtIndex(draggedRows, TreeModel.Root, TreeModel.Root.Children.Count);

                        return DragAndDropVisualMode.Move;
                    }
                default:
                    Debug.LogError("Unhandled enum " + args.dragAndDropPosition);
                    return DragAndDropVisualMode.None;
            }
        }

        /// <summary>
        /// 拖拽完成时的实际数据操作
        /// </summary>
        public virtual void OnDropDraggedElementsAtIndex(List<TreeViewItem> draggedRows, T parent, int insertIndex)
        {
            if (BeforeDroppingDraggedItems != null)
                BeforeDroppingDraggedItems(draggedRows);

            var draggedElements = new List<TreeElement>();
            foreach (var x in draggedRows)
                draggedElements.Add(((TreeViewItem<T>)x).Data);

            var selectedIDs = draggedElements.Select(x => x.ID).ToArray();
            TreeModel.MoveElements(parent, insertIndex, draggedElements);
            SetSelection(selectedIDs, TreeViewSelectionOptions.RevealAndFrame);
        }


        private bool ValidDrag(TreeViewItem parent, List<TreeViewItem> draggedItems)
        {
            var currentParent = parent;
            while (currentParent != null)
            {
                if (draggedItems.Contains(currentParent))
                    return false;
                currentParent = currentParent.parent;
            }

            return true;
        }
    }
}

#endif
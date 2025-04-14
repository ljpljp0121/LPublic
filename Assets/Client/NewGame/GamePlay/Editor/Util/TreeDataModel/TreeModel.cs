#if UNITY_EDITOR

namespace UnityEditor.TreeDataModel
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// 树形数据模型，管理树形结构的增删改查操作
    /// </summary>
    public class TreeModel<T> where T : TreeElement
    {
        private IList<T> data;
        private int maxID;

        public T Root { get; private set; }
        public event Action ModelChanged;
        public int NumberOfDataElements => data.Count;

        public TreeModel(IList<T> data)
        {
            SetData(data);
        }

        /// <summary>
        /// 根据ID查找元素
        /// </summary>
        public T Find(int id)
        {
            return data.FirstOrDefault(element => element.ID == id);
        }

        /// <summary>
        /// 设置新数据并重建树结构
        /// </summary>
        public void SetData(IList<T> data)
        {
            Init(data);
        }

        private void Init(IList<T> data)
        {
            this.data = data ?? throw new ArgumentNullException("data", "Input data is null. Ensure input is a non-null list.");
            if (this.data.Count > 0)
            {
                Root = TreeElementUtility.ListToTree(data);
            }
            else
            {
                T root = Activator.CreateInstance(typeof(T), "Root", -1, 0) as T;
                AddRoot(root);
                Root = root;
            }
            maxID = this.data.Max(e => e.ID);
        }

        /// <summary>
        /// 生成唯一递增ID
        /// </summary>
        public int GenerateUniqueId()
        {
            return ++maxID;
        }

        /// <summary>
        /// 获取指定节点的所有父节点ID（从近到远）
        /// </summary>
        public IList<int> GetFathers(int id)
        {
            var parents = new List<int>();
            TreeElement T = Find(id);
            if (T != null)
            {
                while (T.Parent != null)
                {
                    parents.Add(T.Parent.ID);
                    T = T.Parent;
                }
            }
            return parents;
        }

        /// <summary>
        /// 获取指定节点的所有子节点ID（深度优先）
        /// </summary>
        public IList<int> GetChildren(int id)
        {
            T item = Find(id);
            if (item != null)
            {
                return GetChildren(item);
            }

            return new List<int>();
        }

        private IList<int> GetChildren(TreeElement item)
        {
            Stack<TreeElement> stack = new Stack<TreeElement>();
            stack.Push(item);

            var result = new List<int>();
            while (stack.Count > 0)
            {
                TreeElement current = stack.Pop();
                if (current.HasChildren)
                {
                    result.Add(current.ID);
                    foreach (var T in current.Children)
                    {
                        stack.Push(T);
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// 通过ID列表删除多个元素
        /// </summary>
        public void RemoveElements(IList<int> elementIds)
        {
            IList<T> elements = data.Where(element => elementIds.Contains(element.ID)).ToArray();
            RemoveElements(elements);
        }

        public void RemoveElements(IList<T> elements)
        {
            foreach (var element in elements)
            {
                if (element == Root)
                {
                    throw new ArgumentException("It is not allowed to remove the root element");
                }
            }

            var commonFathers = TreeElementUtility.FindCommonFathersWithSelf(elements);
            foreach (var element in commonFathers)
            {
                element.Parent.Children.Remove(element);
                element.Parent = null;
            }

            TreeElementUtility.TreeToList(Root, data);

            Changed();
        }

        /// <summary>
        /// 添加多个元素到指定父节点下
        /// </summary>
        public void AddElements(IList<T> elements, TreeElement parent, int insertPosition)
        {
            if (elements == null)
                throw new ArgumentNullException("elements", "elements is null");
            if (elements.Count == 0)
                throw new ArgumentNullException("elements", "elements Count is 0: nothing to add");
            if (parent == null)
                throw new ArgumentNullException("parent", "parent is null");
            if (parent.Children == null)
                parent.Children = new List<TreeElement>();

            parent.Children.InsertRange(insertPosition, elements.Cast<TreeElement>());
            foreach (var element in elements)
            {
                element.Parent = parent;
                element.Depth = parent.Depth + 1;
                TreeElementUtility.UpdateDepthValues(element);
            }

            TreeElementUtility.TreeToList(Root, data);

            Changed();
        }

        /// <summary>
        /// 创建初始根节点（仅在数据为空时调用）
        /// </summary>
        public void AddRoot(T root)
        {
            if (root == null)
                throw new ArgumentNullException("root", "root is null");

            if (data == null)
                throw new InvalidOperationException("Internal Error: data list is null");

            if (data.Count != 0)
                throw new InvalidOperationException("AddRoot is only allowed on empty data list");

            root.Name = "Root";
            root.ID = GenerateUniqueId();
            root.Depth = -1;
            data.Add(root);
        }

        public void AddElement(T element, TreeElement parent, int insertPosition)
        {
            if (element == null)
                throw new ArgumentNullException("element", "element is null");
            if (parent == null)
                throw new ArgumentNullException("parent", "parent is null");

            if (parent.Children == null)
                parent.Children = new List<TreeElement>();

            parent.Children.Insert(insertPosition, element);
            element.Parent = parent;

            TreeElementUtility.UpdateDepthValues(parent);
            TreeElementUtility.TreeToList(Root, data);

            Changed();
        }

        /// <summary>
        /// 移动多个元素到新父节点下
        /// </summary>
        public void MoveElements(TreeElement parentElement, int insertionIndex, List<TreeElement> elements)
        {
            if (insertionIndex < 0)
                throw new ArgumentException("Invalid input: insertionIndex is -1, client needs to decide what index elements should be reparented at");

            if (parentElement == null)
                return;

            if (insertionIndex > 0)
                insertionIndex -= parentElement.Children.GetRange(0, insertionIndex).Count(elements.Contains);

            foreach (var draggedItem in elements)
            {
                draggedItem.Parent.Children.Remove(draggedItem);    
                draggedItem.Parent = parentElement;                 
            }

            if (parentElement.Children == null)
                parentElement.Children = new List<TreeElement>();

            parentElement.Children.InsertRange(insertionIndex, elements);

            TreeElementUtility.UpdateDepthValues(Root);
            TreeElementUtility.TreeToList(Root, data);

            Changed();
        }

        void Changed()
        {
            if (ModelChanged != null)
                ModelChanged();
        }
    }
}

#endif
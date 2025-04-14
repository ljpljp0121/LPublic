#if UNITY_EDITOR

namespace UnityEditor.TreeDataModel
{
    using System;
    using System.Collections.Generic;

    public class TreeElementUtility
    {
        /// <summary>
        /// 将树形结构展开为深度优先的扁平化列表
        /// </summary>
        public static void TreeToList<T>(T root, IList<T> result) where T : TreeElement
        {
            if (result == null)
                throw new NullReferenceException("The input 'IList<T> result' list is null");
            result.Clear();

            Stack<T> stack = new Stack<T>();
            stack.Push(root);

            while (stack.Count > 0)
            {
                T current = stack.Pop();
                result.Add(current);

                if (current.Children != null && current.Children.Count > 0)
                {
                    for (int i = current.Children.Count - 1; i >= 0; i--)
                    {
                        stack.Push((T)current.Children[i]);
                    }
                }
            }
        }

        /// <summary>
        /// 将扁平化列表重建为树形结构（基于Depth属性）
        /// </summary>
        public static T ListToTree<T>(IList<T> list) where T : TreeElement
        {
            ValidateDepthValues(list);
            foreach (var element in list)
            {
                element.Parent = null;
                element.Children = null;
            }

            for (int parentIndex = 0; parentIndex < list.Count; parentIndex++)
            {
                var parent = list[parentIndex];
                bool alreadyHasValidChildren = parent.Children != null;
                if (alreadyHasValidChildren)
                    continue;

                int parentDepth = parent.Depth;
                int childCount = 0;

                for (int i = parentIndex + 1; i < list.Count; i++)
                {
                    if (list[i].Depth == parentDepth + 1)
                        childCount++;
                    if (list[i].Depth <= parentDepth)
                        break;
                }

                List<TreeElement> childList = null;
                if (childCount != 0)
                {
                    childList = new List<TreeElement>(childCount);
                    childCount = 0;
                    for (int i = parentIndex + 1; i < list.Count; i++)
                    {
                        if (list[i].Depth == parentDepth + 1)
                        {
                            list[i].Parent = parent;
                            childList.Add(list[i]);
                            childCount++;
                        }

                        if (list[i].Depth <= parentDepth)
                            break;
                    }
                }

                parent.Children = childList;
            }
            return list[0];
        }

        /// <summary>
        /// 校验列表的深度值是否符合树形结构规则
        /// </summary>
        public static void ValidateDepthValues<T>(IList<T> list) where T : TreeElement
        {
            if (list.Count == 0)
                throw new ArgumentException("list should have items, count is 0, check before calling ValidateDepthValues", "list");

            if (list[0].Depth != -1)
                throw new ArgumentException("list item at index 0 should have a depth of -1 (since this should be the hidden root of the tree). Depth is: " + list[0].Depth, "list");

            for (int i = 0; i < list.Count - 1; i++)
            {
                int depth = list[i].Depth;
                int nextDepth = list[i + 1].Depth;
                if (nextDepth > depth && nextDepth - depth > 1)
                    throw new ArgumentException(
                        $"Invalid depth info in input list. Depth cannot increase more than 1 per row. Index {i} has depth {depth} while index {i + 1} has depth {nextDepth}");
            }

            for (int i = 1; i < list.Count; ++i)
                if (list[i].Depth < 0)
                    throw new ArgumentException("Invalid depth value for item at index " + i + ". Only the first item (the root) should have depth below 0.");

            if (list.Count > 1 && list[1].Depth != 0)
                throw new ArgumentException("Input list item at index 1 is assumed to have a depth of 0", "list");
        }

        /// <summary>
        /// 从根节点开始更新所有子节点的深度值
        /// </summary>
        public static void UpdateDepthValues<T>(T root) where T : TreeElement
        {
            if (root == null)
                throw new ArgumentNullException("root", "The root is null");

            if (!root.HasChildren)
                return;

            Stack<TreeElement> stack = new Stack<TreeElement>();
            stack.Push(root);
            while (stack.Count > 0)
            {
                TreeElement current = stack.Pop();
                if (current.Children != null)
                {
                    foreach (var child in current.Children)
                    {
                        child.Depth = current.Depth + 1;
                        stack.Push(child);
                    }
                }
            }
        }

        static bool IsChildOf<T>(T child, IList<T> elements) where T : TreeElement
        {
            while (child != null)
            {
                child = (T)child.Parent;
                if (elements.Contains(child))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// 查找列表中多个元素的公共祖先（去除冗余子节点）
        /// </summary>
        public static IList<T> FindCommonFathersWithSelf<T>(IList<T> elements) where T : TreeElement
        {
            if (elements.Count == 1)
                return new List<T>(elements);

            List<T> result = new List<T>(elements);
            result.RemoveAll(g => IsChildOf(g, elements));
            return result;
        }
    }
}

#endif
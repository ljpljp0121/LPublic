
#if UNITY_EDITOR

namespace UnityEditor.TreeDataModel
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// 树形结构的基础数据结构
    /// </summary>
    public class TreeElement
    {
        [SerializeField] private int id;
        [SerializeField] private string name;
        [SerializeField] private int depth;

        [NonSerialized] private List<TreeElement> children;
        [NonSerialized] private TreeElement parent;

        public TreeElement()
        {
        }

        public TreeElement(string name, int depth, int id)
        {
            this.name = name;
            this.id = id;
            this.depth = depth;
        }

        public int Depth
        {
            get => depth;
            set => depth = value;
        }

        public TreeElement Parent
        {
            get => parent;
            set => parent = value;
        }

        public List<TreeElement> Children
        {
            get => children;
            set => children = value;
        }

        public bool HasChildren => Children != null && Children.Count > 0;

        public string Name
        {
            get => name;
            set => name = value;
        }

        public int ID
        {
            get => id;
            set => id = value;
        }
    }
}

#endif
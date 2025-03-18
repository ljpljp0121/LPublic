#if UNITY_EDITOR
namespace UnityEditor.TreeDataModel
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;


    /// <summary>
    /// 树形结构元素基类，用于构建可序列化的树形数据结构
    /// 典型应用场景：编辑器中的树状视图（如资源管理器、层级树等）
    /// </summary>
    [Serializable]
    public class TreeElement
    {
        [SerializeField] private int _id;

        [SerializeField] private string _name;

        [SerializeField] private int _depth;

        [NonSerialized] private List<TreeElement> _children;
        [NonSerialized] private TreeElement _parent;

        public TreeElement()
        {
        }

        public TreeElement(string name, int depth, int id)
        {
            _name = name;
            _id = id;
            _depth = depth;
        }

        public int Depth
        {
            get => _depth;
            set => _depth = value;
        }

        public TreeElement Parent
        {
            get => _parent;
            set => _parent = value;
        }

        public List<TreeElement> Children
        {
            get => _children;
            set => _children = value;
        }

        public bool HasChildren => Children != null && Children.Count > 0;

        public string Name
        {
            get => _name;
            set => _name = value;
        }

        public int ID
        {
            get => _id;
            set => _id = value;
        }
    }
}
#endif
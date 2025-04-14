#if UNITY_EDITOR

namespace Game.Editor
{
    using System;
    using UnityEditor.TreeDataModel;

    [Serializable]
    public class GameTagTreeElement : TreeElement
    {
        public GameTagTreeElement(string name, int depth, int id) : base(name, depth, id)
        {
        }
    }
}

#endif
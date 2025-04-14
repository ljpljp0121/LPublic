
#if UNITY_EDITOR
namespace Game.Editor
{
    using System.Collections.Generic;
    using Game.RunTime;
    using UnityEditor.TreeDataModel;
    using UnityEngine;

    [FilePath(GameDefine.GAME_TAG_ASSET_PATH)]
    public class GameTagsAsset : ScriptableSingleton<GameTagsAsset>
    {
        [SerializeField] private List<GameTagTreeElement> gameTagTreeElements = new List<GameTagTreeElement>();

        internal List<GameTagTreeElement> GameTagTreeElements => gameTagTreeElements;

        [SerializeField] public List<GameTag> Tags = new List<GameTag>();

        public void CacheTags()
        {
            Tags.Clear();
            for (int i = 0; i < gameTagTreeElements.Count; i++)
            {
                TreeElement tag = gameTagTreeElements[i];
                if (tag.Depth == -1) continue;
                string tagName = tag.Name;
                while (tag.Parent.Depth >= 0)
                {
                    tagName = tag.Parent.Name + "." + tagName;
                    tag = tag.Parent;
                }

                Tags.Add(new GameTag(tagName));
            }
        }
    }
}
#endif
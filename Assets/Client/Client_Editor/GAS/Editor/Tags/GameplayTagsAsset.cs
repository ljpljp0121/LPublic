#if UNITY_EDITOR
namespace GAS.Editor
{
	using GAS.Runtime;
	using System.Collections.Generic;
	using UnityEditor.TreeDataModel;
	using UnityEngine;

    /// <summary>
    /// Tag标签资源,它用于分类和描述对象的状态，非常有用于控制游戏逻辑
    /// </summary>
    [FilePath(GasDefine.GAS_TAGS_MANAGER_ASSET_PATH)]
	public class GameplayTagsAsset : ScriptableSingleton<GameplayTagsAsset>
	{
		[SerializeField] List<GameplayTagTreeElement> gameplayTagTreeElements = new List<GameplayTagTreeElement>();

		internal List<GameplayTagTreeElement> GameplayTagTreeElements => gameplayTagTreeElements;
		
		[SerializeField] public List<GameplayTag> Tags = new List<GameplayTag>();


		public void CacheTags()
		{
			Tags.Clear();
			for (int i = 0; i < gameplayTagTreeElements.Count; i++)
			{
				TreeElement tag = gameplayTagTreeElements[i];
				if (tag.Depth == -1) continue;
				string tagName = tag.Name;
				while (tag.Parent.Depth >=0)
				{
					tagName = tag.Parent.Name + "." + tagName;
					tag = tag.Parent;
				}

				Tags.Add(new GameplayTag(tagName));
			}
		}
	}
}
#endif

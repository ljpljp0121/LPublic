using System.Collections.Generic;

namespace Game.RunTime
{
    public class GameTagAggregator
    {
        private Dictionary<GameTag, List<object>> dynamicAddedTags = new();
        private Dictionary<GameTag, List<object>> dynamicRemovedTags = new();
        private readonly List<GameTag> fixedTags = new List<GameTag>();
        private static Pool pool = new Pool(typeof(List<object>), 1024);

        public GameTagAggregator()
        {

        }

        public void Init(GameTag[] tags)
        {
            fixedTags.Clear();
            fixedTags.AddRange(tags);
        }

        public static bool IsTagInList(GameTag tag, List<GameTag> tags)
        {
            foreach (var t in tags)
            {
                if (t == tag)
                {
                    return true;
                }
            }
            return false;
        }

        private bool TryAddFixedTag(GameTag tag)
        {
            var dirty = !IsTagInList(tag, fixedTags);
            if (dirty) fixedTags.Add(tag);
            var dynamicRemovedTagsRemoved = dynamicRemovedTags.Remove(tag);
            dirty = dirty || dynamicRemovedTagsRemoved;
            var dynamicAddedTagsRemoved = dynamicAddedTags.Remove(tag);
            dirty = dirty || dynamicAddedTagsRemoved;
            return dirty;
        }

        public void AddFixedTag(GameTag tag)
        {
            TryAddFixedTag(tag);
        }

        public void AddFixedTag(GameTagSet tagSet)
        {
            if (tagSet.IsEmpty) return;
            foreach (var tag in tagSet.Tags)
            {
                TryAddFixedTag(tag);
            }
        }

        private bool TryRemoveFixedTag(GameTag tag)
        {
            var dirty = fixedTags.Remove(tag);
            var dynamicAddedTagsRemoved = dynamicAddedTags.Remove(tag);
            dirty = dirty || dynamicAddedTagsRemoved;
            var dynamicRemovedTagsRemoved = dynamicRemovedTags.Remove(tag);
            dirty = dirty || dynamicRemovedTagsRemoved;
            return dirty;
        }

        public void RemoveFixedTag(GameTag tag)
        {
            TryRemoveFixedTag(tag);
        }

        public void RemoveFixedTag(GameTagSet tagSet)
        {
            if (tagSet.IsEmpty) return;
            foreach (var tag in tagSet.Tags)
            {
                TryRemoveFixedTag(tag);
            }
        }
    }
}
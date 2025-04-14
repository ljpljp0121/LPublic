using System.Collections.Generic;
using System.Linq;

namespace Game.RunTime
{
    public class GameTagContainer
    {
        public List<GameTag> Tags { get; }

        public GameTagContainer(params GameTag[] tags)
        {
            Tags = new List<GameTag>(tags);
        }

        public void AddTag(GameTag tag)
        {
            if (Tags.Contains(tag)) return;
            Tags.Add(tag);
        }

        public void RemoveTag(GameTag tag)
        {
            Tags.Remove(tag);
        }

        public void AddTag(GameTagSet tagSet)
        {
            if (tagSet.IsEmpty) return;
            foreach (var tag in tagSet.Tags)
            {
                AddTag(tag);
            }
        }

        public void RemoveTag(GameTagSet tagSet)
        {
            if (tagSet.IsEmpty) return;
            foreach (var tag in tagSet.Tags)
            {
                RemoveTag(tag);
            }
        }

        public bool HasTag(GameTag tag)
        {
            return Tags.Any(t => t.HasTag(tag));
        }

        public bool HasAllTags(GameTagSet tagSet)
        {
            return tagSet.IsEmpty || tagSet.Tags.All(HasTag);
        }

        public bool HasAllTags(params GameTag[] tags)
        {
            return tags.All(HasTag);
        }

        public bool HasAnyTags(GameTagSet other)
        {
            return other.IsEmpty || other.Tags.Any(HasTag);
        }

        public bool HasAnyTags(params GameTag[] tags)
        {
            return tags.Any(HasTag);
        }

        public bool HasNoneTags(GameTagSet other)
        {
            return other.IsEmpty || !other.Tags.Any(HasTag);
        }

        public bool HasNoneTags(params GameTag[] tags)
        {
            return !tags.Any(HasTag);
        }
    }
}
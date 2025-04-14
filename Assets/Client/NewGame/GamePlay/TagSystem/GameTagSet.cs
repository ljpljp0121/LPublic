using System;

namespace Game.RunTime
{
    public readonly struct GameTagSet
    {
        public readonly GameTag[] Tags;

        public bool IsEmpty => Tags.Length == 0;

        public GameTagSet(string[] tagNames)
        {
            Tags = new GameTag[tagNames.Length];
            for (int i = 0; i < tagNames.Length; i++)
            {
                Tags[i] = new GameTag(tagNames[i]);
            }
        }

        public GameTagSet(params GameTag[] tags)
        {
            Tags = tags ?? Array.Empty<GameTag>();
        }

        public bool HasTag(GameTag tag)
        {
            foreach (var t in Tags)
            {
                if (t.HasTag(tag)) return true;
            }

            return false;
        }

        public bool HasAllTags(GameTagSet other)
        {
            return HasAllTags(other.Tags);
        }

        public bool HasAllTags(params GameTag[] tags)
        {
            foreach (var tag in tags)
            {
                if (!HasTag(tag)) return false;
            }

            return true;
        }

        public bool HasAnyTags(GameTagSet other)
        {
            return HasAnyTags(other.Tags);
        }

        public bool HasAnyTags(params GameTag[] tags)
        {
            foreach (var tag in tags)
            {
                if (HasTag(tag)) return true;
            }

            return false;
        }

        public bool HasNoneTags(GameTagSet other)
        {
            return HasNoneTags(other.Tags);
        }

        public bool HasNoneTags(params GameTag[] tags)
        {
            foreach (var tag in tags)
            {
                if (HasTag(tag)) return false;
            }

            return true;
        }
    }
}
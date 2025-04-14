using System;
using System.Linq;
using UnityEngine;

namespace Game.RunTime
{
    [Serializable]
    public struct GameTag
    {
        [SerializeField] private string name;
        [SerializeField] private int hashCode;
        [SerializeField] private string shortName;
        [SerializeField] private int[] fatherHashCodes;
        [SerializeField] private string[] fatherNames;

        public GameTag(string name)
        {
            this.name = name;
            hashCode = name.GetHashCode();
            var tags = name.Split(',');

            fatherNames = new string[tags.Length - 1];
            fatherHashCodes = new int[tags.Length - 1];
            var i = 0;
            var fatherTag = "";
            while (i < tags.Length - 1)
            {
                fatherTag += tags[i];
                fatherHashCodes[i] = fatherTag.GetHashCode();
                fatherNames[i] = fatherTag;
                fatherTag += ".";
                i++;
            }

            shortName = tags.Last();
        }

        public string Name => name;
        public string ShortName => shortName;
        public int HashCode => hashCode;
        public string[] FatherNames => fatherNames;
        public bool Root => fatherHashCodes.Length == 0;
        public int[] FatherHashCodes => fatherHashCodes;
        public bool IsSonOf(GameTag other)
        {
            return other.FatherHashCodes.Contains(hashCode);
        }

        public override bool Equals(object obj)
        {
            return obj is GameTag tag && this == tag;
        }


        public override int GetHashCode()
        {
            return HashCode;
        }

        public static bool operator ==(GameTag a, GameTag b)
        {
            return a.HashCode == b.HashCode;
        }

        public static bool operator !=(GameTag a, GameTag b)
        {
            return a.HashCode != b.HashCode;
        }

        public bool HasTag(GameTag tag)
        {
            foreach (var fatherHashCode in fatherHashCodes)
            {
                if (fatherHashCode == tag.HashCode)
                    return true;
            }

            return this == tag;
        }
    }

}

///////////////////////////////////
//// This is a generated file. ////
////     Do not modify it.     ////
///////////////////////////////////

using System;
using System.Collections.Generic;

namespace GAS.Runtime
{
    public class AS_Fight : AttributeSet
    {
        #region HP

        private AttributeBase _HP = new AttributeBase("AS_Fight", "HP");

        public AttributeBase HP => _HP;

        public void InitHP(float value)
        {
            _HP.SetBaseValue(value);
            _HP.SetCurrentValue(value);
        }

        public void SetCurrentHP(float value)
        {
            _HP.SetCurrentValue(value);
        }

        public void SetBaseHP(float value)
        {
            _HP.SetBaseValue(value);
        }

        #endregion HP

        public override AttributeBase this[string key]
        {
            get
            {
                switch (key)
                {
                    case "HP":
                        return _HP;
                }

                return null;
            }
        }

        public override string[] AttributeNames { get; } =
        {
            "HP",
        };

        public override void SetOwner(AbilitySystemComponent owner)
        {
            _owner = owner;
            _HP.SetOwner(owner);
        }
    }

    public static class GAttrSetLib
    {
        public static readonly Dictionary<string, Type> AttrSetTypeDict = new Dictionary<string, Type>()
        {
            { "Fight", typeof(AS_Fight) },
        };
        public static readonly Dictionary<Type,string> TypeToName = new Dictionary<Type,string>
        {
            {  typeof(AS_Fight),nameof(AS_Fight) },
        };

        public static List<string> AttributeFullNames = new List<string>()
        {
            "AS_Fight.HP",
        };
    }
}
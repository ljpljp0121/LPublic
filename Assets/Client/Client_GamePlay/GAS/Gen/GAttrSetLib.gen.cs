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

        #region ATK

        private AttributeBase _ATK = new AttributeBase("AS_Fight", "ATK");

        public AttributeBase ATK => _ATK;

        public void InitATK(float value)
        {
            _ATK.SetBaseValue(value);
            _ATK.SetCurrentValue(value);
        }

        public void SetCurrentATK(float value)
        {
            _ATK.SetCurrentValue(value);
        }

        public void SetBaseATK(float value)
        {
            _ATK.SetBaseValue(value);
        }

        #endregion ATK

        #region SPEED

        private AttributeBase _SPEED = new AttributeBase("AS_Fight", "SPEED");

        public AttributeBase SPEED => _SPEED;

        public void InitSPEED(float value)
        {
            _SPEED.SetBaseValue(value);
            _SPEED.SetCurrentValue(value);
        }

        public void SetCurrentSPEED(float value)
        {
            _SPEED.SetCurrentValue(value);
        }

        public void SetBaseSPEED(float value)
        {
            _SPEED.SetBaseValue(value);
        }

        #endregion SPEED

        #region Sprite

        private AttributeBase _Sprite = new AttributeBase("AS_Fight", "Sprite");

        public AttributeBase Sprite => _Sprite;

        public void InitSprite(float value)
        {
            _Sprite.SetBaseValue(value);
            _Sprite.SetCurrentValue(value);
        }

        public void SetCurrentSprite(float value)
        {
            _Sprite.SetCurrentValue(value);
        }

        public void SetBaseSprite(float value)
        {
            _Sprite.SetBaseValue(value);
        }

        #endregion Sprite

        public override AttributeBase this[string key]
        {
            get
            {
                switch (key)
                {
                    case "HP":
                        return _HP;
                    case "ATK":
                        return _ATK;
                    case "SPEED":
                        return _SPEED;
                    case "Sprite":
                        return _Sprite;
                }

                return null;
            }
        }

        public override string[] AttributeNames { get; } =
        {
            "HP",
            "ATK",
            "SPEED",
            "Sprite",
        };

        public override void SetOwner(SkillSystemComponent owner)
        {
            _owner = owner;
            _HP.SetOwner(owner);
            _ATK.SetOwner(owner);
            _SPEED.SetOwner(owner);
            _Sprite.SetOwner(owner);
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
            "AS_Fight.ATK",
            "AS_Fight.SPEED",
            "AS_Fight.Sprite",
        };
    }
}
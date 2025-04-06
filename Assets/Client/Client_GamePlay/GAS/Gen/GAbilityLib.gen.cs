///////////////////////////////////
//// This is a generated file. ////
////     Do not modify it.     ////
///////////////////////////////////

using System;
using System.Collections.Generic;

namespace GAS.Runtime
{
    public static class GAbilityLib
    {
        public struct AbilityInfo
        {
            public string Name;
            public string AssetPath;
            public Type AbilityClassType;
        }

        public static AbilityInfo CommonAttack = new AbilityInfo { Name = "CommonAttack", AssetPath = "Assets/Bundle/SOData/GameplayAbilityLib/CommonAttack.asset",AbilityClassType = typeof(GAS.Runtime.TimelineAbility) };


        public static Dictionary<string, AbilityInfo> AbilityMap = new Dictionary<string, AbilityInfo>
        {
            ["CommonAttack"] = CommonAttack,
        };
    }
}
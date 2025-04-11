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

        public static AbilityInfo CommonAttack1 = new AbilityInfo { Name = "CommonAttack1", AssetPath = "Assets/Bundle/SOData/GameplayAbilityLib/CommonAttack1.asset",AbilityClassType = typeof(GAS.Runtime.TimelineAbility) };

        public static AbilityInfo CommonAttack2 = new AbilityInfo { Name = "CommonAttack2", AssetPath = "Assets/Bundle/SOData/GameplayAbilityLib/CommonAttack2.asset",AbilityClassType = typeof(GAS.Runtime.TimelineAbility) };

        public static AbilityInfo CommonAttack3 = new AbilityInfo { Name = "CommonAttack3", AssetPath = "Assets/Bundle/SOData/GameplayAbilityLib/CommonAttack3.asset",AbilityClassType = typeof(GAS.Runtime.TimelineAbility) };

        public static AbilityInfo CommonAttack4 = new AbilityInfo { Name = "CommonAttack4", AssetPath = "Assets/Bundle/SOData/GameplayAbilityLib/CommonAttack4.asset",AbilityClassType = typeof(GAS.Runtime.TimelineAbility) };

        public static AbilityInfo CommonAttack5 = new AbilityInfo { Name = "CommonAttack5", AssetPath = "Assets/Bundle/SOData/GameplayAbilityLib/CommonAttack5.asset",AbilityClassType = typeof(GAS.Runtime.TimelineAbility) };


        public static Dictionary<string, AbilityInfo> AbilityMap = new Dictionary<string, AbilityInfo>
        {
            ["CommonAttack1"] = CommonAttack1,
            ["CommonAttack2"] = CommonAttack2,
            ["CommonAttack3"] = CommonAttack3,
            ["CommonAttack4"] = CommonAttack4,
            ["CommonAttack5"] = CommonAttack5,
        };
    }
}
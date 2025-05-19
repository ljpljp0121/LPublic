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
            public int ID;
            public string Name;
            public string AssetPath;
            public Type AbilityClassType;
        }

        public static AbilityInfo CommonAttack1 = new AbilityInfo { ID = 1001, Name = "CommonAttack1", AssetPath = "Assets/Bundle/SOData/GameplayAbilityLib/CommonAttack1.asset",AbilityClassType = typeof(GAS.Runtime.TimelineAbility) };

        public static AbilityInfo CommonAttack2 = new AbilityInfo { ID = 1002, Name = "CommonAttack2", AssetPath = "Assets/Bundle/SOData/GameplayAbilityLib/CommonAttack2.asset",AbilityClassType = typeof(GAS.Runtime.TimelineAbility) };

        public static AbilityInfo CommonAttack3 = new AbilityInfo { ID = 1003, Name = "CommonAttack3", AssetPath = "Assets/Bundle/SOData/GameplayAbilityLib/CommonAttack3.asset",AbilityClassType = typeof(GAS.Runtime.TimelineAbility) };

        public static AbilityInfo CommonAttack4 = new AbilityInfo { ID = 1004, Name = "CommonAttack4", AssetPath = "Assets/Bundle/SOData/GameplayAbilityLib/CommonAttack4.asset",AbilityClassType = typeof(GAS.Runtime.TimelineAbility) };

        public static AbilityInfo CommonAttack5 = new AbilityInfo { ID = 1005, Name = "CommonAttack5", AssetPath = "Assets/Bundle/SOData/GameplayAbilityLib/CommonAttack5.asset",AbilityClassType = typeof(GAS.Runtime.TimelineAbility) };

        public static AbilityInfo Move = new AbilityInfo { ID = 1000, Name = "Move", AssetPath = "Assets/Bundle/SOData/GameplayAbilityLib/Move.asset",AbilityClassType = typeof(Move) };


        public static Dictionary<string, AbilityInfo> AbilityMap = new Dictionary<string, AbilityInfo>
        {
            ["CommonAttack1"] = CommonAttack1,
            ["CommonAttack2"] = CommonAttack2,
            ["CommonAttack3"] = CommonAttack3,
            ["CommonAttack4"] = CommonAttack4,
            ["CommonAttack5"] = CommonAttack5,
            ["Move"] = Move,
        };

        public static Dictionary<int, AbilityInfo> AbilityMapByID = new Dictionary<int, AbilityInfo>()
        {
            [1001] = CommonAttack1,
            [1002] = CommonAttack2,
            [1003] = CommonAttack3,
            [1004] = CommonAttack4,
            [1005] = CommonAttack5,
            [1000] = Move,
        };
    }
}
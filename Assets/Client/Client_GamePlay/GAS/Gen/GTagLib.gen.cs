///////////////////////////////////
//// This is a generated file. ////
////     Do not modify it.     ////
///////////////////////////////////

using System.Collections.Generic;

namespace GAS.Runtime
{
    public static class GTagLib
    {
        public static GameplayTag Faction { get; } = new GameplayTag("Faction");
        public static GameplayTag Ability { get; } = new GameplayTag("Ability");
        public static GameplayTag State { get; } = new GameplayTag("State");
        public static GameplayTag Event { get; } = new GameplayTag("Event");
        public static GameplayTag CD { get; } = new GameplayTag("CD");
        public static GameplayTag Ban { get; } = new GameplayTag("Ban");

        public static Dictionary<string, GameplayTag> TagMap = new Dictionary<string, GameplayTag>
        {
            ["Faction"] = Faction,
            ["Ability"] = Ability,
            ["State"] = State,
            ["Event"] = Event,
            ["CD"] = CD,
            ["Ban"] = Ban,
        };
    }
}
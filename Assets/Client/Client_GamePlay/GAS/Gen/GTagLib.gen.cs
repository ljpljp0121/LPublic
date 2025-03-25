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
        public static GameplayTag Faction_Player { get; } = new GameplayTag("Faction.Player");
        public static GameplayTag Faction_Enemy { get; } = new GameplayTag("Faction.Enemy");
        public static GameplayTag Ability { get; } = new GameplayTag("Ability");
        public static GameplayTag Ability_Dash { get; } = new GameplayTag("Ability.Dash");
        public static GameplayTag Ability_Attack { get; } = new GameplayTag("Ability.Attack");
        public static GameplayTag Ability_Move { get; } = new GameplayTag("Ability.Move");
        public static GameplayTag Ability_Defend { get; } = new GameplayTag("Ability.Defend");
        public static GameplayTag Ability_Die { get; } = new GameplayTag("Ability.Die");
        public static GameplayTag State { get; } = new GameplayTag("State");
        public static GameplayTag State_DeBuff { get; } = new GameplayTag("State.DeBuff");
        public static GameplayTag State_Buff { get; } = new GameplayTag("State.Buff");
        public static GameplayTag Event { get; } = new GameplayTag("Event");
        public static GameplayTag Event_Moving { get; } = new GameplayTag("Event.Moving");
        public static GameplayTag CD { get; } = new GameplayTag("CD");
        public static GameplayTag CD_SuperSkill { get; } = new GameplayTag("CD.SuperSkill");
        public static GameplayTag CD_CommonSkill { get; } = new GameplayTag("CD.CommonSkill");
        public static GameplayTag Ban { get; } = new GameplayTag("Ban");
        public static GameplayTag Ban_Motion { get; } = new GameplayTag("Ban.Motion");

        public static Dictionary<string, GameplayTag> TagMap = new Dictionary<string, GameplayTag>
        {
            ["Faction"] = Faction,
            ["Faction.Player"] = Faction_Player,
            ["Faction.Enemy"] = Faction_Enemy,
            ["Ability"] = Ability,
            ["Ability.Dash"] = Ability_Dash,
            ["Ability.Attack"] = Ability_Attack,
            ["Ability.Move"] = Ability_Move,
            ["Ability.Defend"] = Ability_Defend,
            ["Ability.Die"] = Ability_Die,
            ["State"] = State,
            ["State.DeBuff"] = State_DeBuff,
            ["State.Buff"] = State_Buff,
            ["Event"] = Event,
            ["Event.Moving"] = Event_Moving,
            ["CD"] = CD,
            ["CD.SuperSkill"] = CD_SuperSkill,
            ["CD.CommonSkill"] = CD_CommonSkill,
            ["Ban"] = Ban,
            ["Ban.Motion"] = Ban_Motion,
        };
    }
}
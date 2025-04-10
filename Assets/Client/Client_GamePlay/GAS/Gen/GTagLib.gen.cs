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
        public static GameplayTag Ability_XingJianYa { get; } = new GameplayTag("Ability.XingJianYa");
        public static GameplayTag Ability_XingJianYa_ComAttack1 { get; } = new GameplayTag("Ability.XingJianYa.ComAttack1");
        public static GameplayTag Ability_XingJianYa_ComAttack2 { get; } = new GameplayTag("Ability.XingJianYa.ComAttack2");
        public static GameplayTag Ability_XingJianYa_ComAttack3 { get; } = new GameplayTag("Ability.XingJianYa.ComAttack3");
        public static GameplayTag Ability_XingJianYa_ComAttack4 { get; } = new GameplayTag("Ability.XingJianYa.ComAttack4");
        public static GameplayTag Ability_XingJianYa_Move { get; } = new GameplayTag("Ability.XingJianYa.Move");
        public static GameplayTag Ability_Dash { get; } = new GameplayTag("Ability.Dash");
        public static GameplayTag Ability_Defend { get; } = new GameplayTag("Ability.Defend");
        public static GameplayTag Ability_Die { get; } = new GameplayTag("Ability.Die");
        public static GameplayTag State { get; } = new GameplayTag("State");
        public static GameplayTag State_DeBuff { get; } = new GameplayTag("State.DeBuff");
        public static GameplayTag State_Buff { get; } = new GameplayTag("State.Buff");
        public static GameplayTag Event { get; } = new GameplayTag("Event");
        public static GameplayTag Event_BlockAbility { get; } = new GameplayTag("Event.BlockAbility");
        public static GameplayTag Event_UseAbility { get; } = new GameplayTag("Event.UseAbility");
        public static GameplayTag Event_BlockMove { get; } = new GameplayTag("Event.BlockMove");
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
            ["Ability.XingJianYa"] = Ability_XingJianYa,
            ["Ability.XingJianYa.ComAttack1"] = Ability_XingJianYa_ComAttack1,
            ["Ability.XingJianYa.ComAttack2"] = Ability_XingJianYa_ComAttack2,
            ["Ability.XingJianYa.ComAttack3"] = Ability_XingJianYa_ComAttack3,
            ["Ability.XingJianYa.ComAttack4"] = Ability_XingJianYa_ComAttack4,
            ["Ability.XingJianYa.Move"] = Ability_XingJianYa_Move,
            ["Ability.Dash"] = Ability_Dash,
            ["Ability.Defend"] = Ability_Defend,
            ["Ability.Die"] = Ability_Die,
            ["State"] = State,
            ["State.DeBuff"] = State_DeBuff,
            ["State.Buff"] = State_Buff,
            ["Event"] = Event,
            ["Event.BlockAbility"] = Event_BlockAbility,
            ["Event.UseAbility"] = Event_UseAbility,
            ["Event.BlockMove"] = Event_BlockMove,
            ["CD"] = CD,
            ["CD.SuperSkill"] = CD_SuperSkill,
            ["CD.CommonSkill"] = CD_CommonSkill,
            ["Ban"] = Ban,
            ["Ban.Motion"] = Ban_Motion,
        };
    }
}
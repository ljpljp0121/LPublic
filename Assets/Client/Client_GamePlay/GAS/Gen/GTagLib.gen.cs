///////////////////////////////////
//// This is a generated file. ////
////     Do not modify it.     ////
///////////////////////////////////

using System.Collections.Generic;

namespace GAS.Runtime
{
    public static class GTagLib
    {
        public static GameplayTag State { get; } = new GameplayTag("State");
        public static GameplayTag State_Ability { get; } = new GameplayTag("State.Ability");
        public static GameplayTag State_Ability_CancelWindow { get; } = new GameplayTag("State.Ability.CancelWindow");
        public static GameplayTag State_Moving { get; } = new GameplayTag("State.Moving");
        public static GameplayTag Element { get; } = new GameplayTag("Element");
        public static GameplayTag Ability { get; } = new GameplayTag("Ability");
        public static GameplayTag Ability_Name { get; } = new GameplayTag("Ability.Name");
        public static GameplayTag Ability_Name_Movement { get; } = new GameplayTag("Ability.Name.Movement");
        public static GameplayTag Ability_Type { get; } = new GameplayTag("Ability.Type");
        public static GameplayTag Ability_Type_Move { get; } = new GameplayTag("Ability.Type.Move");
        public static GameplayTag Ability_Type_Self { get; } = new GameplayTag("Ability.Type.Self");
        public static GameplayTag Ability_Type_Melee { get; } = new GameplayTag("Ability.Type.Melee");
        public static GameplayTag Ability_Type_Ranged { get; } = new GameplayTag("Ability.Type.Ranged");
        public static GameplayTag Env { get; } = new GameplayTag("Env");
        public static GameplayTag Effect { get; } = new GameplayTag("Effect");
        public static GameplayTag Effect_Debuff { get; } = new GameplayTag("Effect.Debuff");
        public static GameplayTag Effect_Buff { get; } = new GameplayTag("Effect.Buff");
        public static GameplayTag Role { get; } = new GameplayTag("Role");
        public static GameplayTag System { get; } = new GameplayTag("System");

        public static Dictionary<string, GameplayTag> TagMap = new Dictionary<string, GameplayTag>
        {
            ["State"] = State,
            ["State.Ability"] = State_Ability,
            ["State.Ability.CancelWindow"] = State_Ability_CancelWindow,
            ["State.Moving"] = State_Moving,
            ["Element"] = Element,
            ["Ability"] = Ability,
            ["Ability.Name"] = Ability_Name,
            ["Ability.Name.Movement"] = Ability_Name_Movement,
            ["Ability.Type"] = Ability_Type,
            ["Ability.Type.Move"] = Ability_Type_Move,
            ["Ability.Type.Self"] = Ability_Type_Self,
            ["Ability.Type.Melee"] = Ability_Type_Melee,
            ["Ability.Type.Ranged"] = Ability_Type_Ranged,
            ["Env"] = Env,
            ["Effect"] = Effect,
            ["Effect.Debuff"] = Effect_Debuff,
            ["Effect.Buff"] = Effect_Buff,
            ["Role"] = Role,
            ["System"] = System,
        };
    }
}
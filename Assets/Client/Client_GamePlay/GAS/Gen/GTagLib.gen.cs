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
        public static GameplayTag State_Ability_Attacking { get; } = new GameplayTag("State.Ability.Attacking");
        public static GameplayTag State_Moving { get; } = new GameplayTag("State.Moving");
        public static GameplayTag Element { get; } = new GameplayTag("Element");
        public static GameplayTag Ability { get; } = new GameplayTag("Ability");
        public static GameplayTag Ability_Type { get; } = new GameplayTag("Ability.Type");
        public static GameplayTag Ability_Type_OnSite { get; } = new GameplayTag("Ability.Type.OnSite");
        public static GameplayTag Ability_Type_HandsOff { get; } = new GameplayTag("Ability.Type.HandsOff");
        public static GameplayTag Ability_Type_Move { get; } = new GameplayTag("Ability.Type.Move");
        public static GameplayTag Ability_Type_Self { get; } = new GameplayTag("Ability.Type.Self");
        public static GameplayTag Ability_Type_Melee { get; } = new GameplayTag("Ability.Type.Melee");
        public static GameplayTag Ability_Type_Ranged { get; } = new GameplayTag("Ability.Type.Ranged");
        public static GameplayTag Env { get; } = new GameplayTag("Env");
        public static GameplayTag Effect { get; } = new GameplayTag("Effect");
        public static GameplayTag Effect_Common { get; } = new GameplayTag("Effect.Common");
        public static GameplayTag Effect_Common_LockWindow { get; } = new GameplayTag("Effect.Common.LockWindow");
        public static GameplayTag Effect_Common_ComboWindow { get; } = new GameplayTag("Effect.Common.ComboWindow");
        public static GameplayTag Effect_Common_CancelWindow { get; } = new GameplayTag("Effect.Common.CancelWindow");
        public static GameplayTag Effect_Debuff { get; } = new GameplayTag("Effect.Debuff");
        public static GameplayTag Effect_Buff { get; } = new GameplayTag("Effect.Buff");
        public static GameplayTag Role { get; } = new GameplayTag("Role");
        public static GameplayTag System { get; } = new GameplayTag("System");

        public static Dictionary<string, GameplayTag> TagMap = new Dictionary<string, GameplayTag>
        {
            ["State"] = State,
            ["State.Ability"] = State_Ability,
            ["State.Ability.Attacking"] = State_Ability_Attacking,
            ["State.Moving"] = State_Moving,
            ["Element"] = Element,
            ["Ability"] = Ability,
            ["Ability.Type"] = Ability_Type,
            ["Ability.Type.OnSite"] = Ability_Type_OnSite,
            ["Ability.Type.HandsOff"] = Ability_Type_HandsOff,
            ["Ability.Type.Move"] = Ability_Type_Move,
            ["Ability.Type.Self"] = Ability_Type_Self,
            ["Ability.Type.Melee"] = Ability_Type_Melee,
            ["Ability.Type.Ranged"] = Ability_Type_Ranged,
            ["Env"] = Env,
            ["Effect"] = Effect,
            ["Effect.Common"] = Effect_Common,
            ["Effect.Common.LockWindow"] = Effect_Common_LockWindow,
            ["Effect.Common.ComboWindow"] = Effect_Common_ComboWindow,
            ["Effect.Common.CancelWindow"] = Effect_Common_CancelWindow,
            ["Effect.Debuff"] = Effect_Debuff,
            ["Effect.Buff"] = Effect_Buff,
            ["Role"] = Role,
            ["System"] = System,
        };
    }
}
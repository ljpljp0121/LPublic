﻿using System;
using System.Collections.Generic;

namespace GAS.Runtime
{
    public interface ISkillSystemComponent
    {
        void SetPreset(SkillSystemComponentPreset ascPreset);

        void Init(GameplayTag[] baseTags, Type[] attrSetTypes, AbilityAsset[] baseAbilities, int level);
        bool HasTag(GameplayTag tag);

        bool HasAllTags(GameplayTagSet tags);

        bool HasAnyTags(GameplayTagSet tags);

        void AddFixedTags(GameplayTagSet tags);
        void AddFixedTag(GameplayTag tag);

        void RemoveFixedTags(GameplayTagSet tags);
        void RemoveFixedTag(GameplayTag tag);

        SkillEffectSpec ApplyGameplayEffectTo(SkillEffect skillEffect, SkillSystemComponent target);

        SkillEffectSpec ApplyGameplayEffectToSelf(SkillEffect skillEffect);

        void ApplyModFromInstantGameplayEffect(SkillEffectSpec spec);

        void RemoveGameplayEffect(SkillEffectSpec spec);

        void Tick();

        Dictionary<string, float> DataSnapshot();

        void GrantAbility(AbstractAbility ability);

        void RemoveAbility(string abilityName);

        float? GetAttributeCurrentValue(string setName, string attributeShortName);
        float? GetAttributeBaseValue(string setName, string attributeShortName);

        bool TryActivateAbility(string abilityName, bool isStart = false, params object[] args);
        void TryEndAbility(string abilityName);

        CooldownTimer CheckCooldownFromTags(GameplayTagSet tags);

        T AttrSet<T>() where T : AttributeSet;

        void ClearGameplayEffect();
    }
}
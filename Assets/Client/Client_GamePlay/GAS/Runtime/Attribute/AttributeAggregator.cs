using System;
using System.Collections.Generic;
using UnityEngine;

namespace GAS.Runtime
{
    /// <summary>
    /// 缓存所有影响该属性的 ​游戏效果（GameplayEffect）​ 和其 ​修饰器（Modifier）​。
    /// 根据修饰器的操作类型（加法、乘法、覆盖）动态计算当前值。
    /// </summary>
    public class AttributeAggregator
    {
        AttributeBase _processedAttribute;
        SkillSystemComponent _owner;

        /// <summary>
        ///  modifiers的顺序很重要，因为modifiers的执行是按照顺序来的。
        /// </summary>
        private List<Tuple<SkillEffectSpec, GameplayEffectModifier>> _modifierCache =
            new List<Tuple<SkillEffectSpec, GameplayEffectModifier>>();
        
        public AttributeAggregator(AttributeBase attribute , SkillSystemComponent owner)
        {
            _processedAttribute = attribute;
            _owner = owner;

            OnCreated();
        }

        void OnCreated()
        {
            _processedAttribute.RegisterPostBaseValueChange(UpdateCurrentValueWhenBaseValueIsDirty);
            _owner.SkillEffectContainer.RegisterOnGameplayEffectContainerIsDirty(RefreshModifierCache);
        }
        
        public void OnDispose()
        {
            _processedAttribute.UnregisterPostBaseValueChange(UpdateCurrentValueWhenBaseValueIsDirty);
            _owner.SkillEffectContainer.UnregisterOnGameplayEffectContainerIsDirty(RefreshModifierCache);
        }
        
        /// <summary>
        /// it's triggered only when the owner's gameplay effect is added or removed. 
        /// </summary>
        void RefreshModifierCache()
        {
            // 注销属性变化监听回调
            UnregisterAttributeChangedListen();
            _modifierCache.Clear();
            var gameplayEffects = _owner.SkillEffectContainer.GameplayEffects();
            foreach (var geSpec in gameplayEffects)
            {
                if (geSpec.IsActive)
                {
                    foreach (var modifier in geSpec.SkillEffect.Modifiers)
                    {
                        if (modifier.AttributeName == _processedAttribute.Name)
                        {
                            _modifierCache.Add(new Tuple<SkillEffectSpec, GameplayEffectModifier>(geSpec, modifier));
                            TryRegisterAttributeChangedListen(geSpec, modifier);
                        }
                    }
                }
            }
            
            UpdateCurrentValueWhenModifierIsDirty();
        }
        
        /// <summary>
        /// 为CurrentValue计算新值。 (BaseValue的变化依赖于instant型GameplayEffect.)
        /// 这个方法的触发时机为：
        /// 1._modifierCache变化时
        /// 2._processedAttribute的BaseValue变化时
        /// 3._modifierCache的AttributeBased类的MMC，Track类属性变化时
        /// </summary>
        /// <returns></returns>
        float CalculateNewValue()
        {
            float newValue = _processedAttribute.BaseValue;
            foreach (var tuple in _modifierCache)
            {
                var spec = tuple.Item1;
                var modifier = tuple.Item2;
                var magnitude = modifier.CalculateMagnitude(spec,modifier.ModiferMagnitude);
                switch (modifier.Operation)
                {
                    case GEOperation.Add:
                        newValue += magnitude;
                        break;
                    case GEOperation.Multiply:
                        newValue *= magnitude;
                        break;
                    case GEOperation.Override:
                        newValue = magnitude;
                        break;
                }
            }
            return newValue;
        }
        
        void UpdateCurrentValueWhenBaseValueIsDirty(AttributeBase attribute, float oldBaseValue, float newBaseValue)
        {
            if(oldBaseValue == newBaseValue) return;
            
            float newValue = CalculateNewValue();
            _processedAttribute.SetCurrentValue(newValue);
        }
        
        void UpdateCurrentValueWhenModifierIsDirty()
        {
            float newValue = CalculateNewValue();
            _processedAttribute.SetCurrentValue(newValue);
        }

        private void UnregisterAttributeChangedListen()
        {
            foreach (var tuple in _modifierCache)
                TryUnregisterAttributeChangedListen(tuple.Item1, tuple.Item2);
        }

        private void TryUnregisterAttributeChangedListen(SkillEffectSpec ge, GameplayEffectModifier modifier)
        {
            if (modifier.MMC is AttributeBasedModCalculation mmc &&
                mmc.captureType == AttributeBasedModCalculation.GEAttributeCaptureType.Track)
            {
                if (mmc.attributeFromType == AttributeBasedModCalculation.AttributeFrom.Target)
                {
                    if (ge.Owner != null)
                        ge.Owner.AttributeSetContainer.Sets[mmc.attributeSetName][mmc.attributeShortName]
                            .UnregisterPostCurrentValueChange(OnAttributeChanged);
                }
                else
                {
                    if (ge.Source != null)
                        ge.Source.AttributeSetContainer.Sets[mmc.attributeSetName][mmc.attributeShortName]
                            .UnregisterPostCurrentValueChange(OnAttributeChanged);
                }
            }
        }
        
        private void TryRegisterAttributeChangedListen(SkillEffectSpec ge, GameplayEffectModifier modifier)
        {
            if (modifier.MMC is AttributeBasedModCalculation mmc &&
                mmc.captureType == AttributeBasedModCalculation.GEAttributeCaptureType.Track)
            {
                if (mmc.attributeFromType == AttributeBasedModCalculation.AttributeFrom.Target)
                {
                    if (ge.Owner != null)
                        ge.Owner.AttributeSetContainer.Sets[mmc.attributeSetName][mmc.attributeShortName]
                            .RegisterPostCurrentValueChange(OnAttributeChanged);
                }
                else
                {
                    if (ge.Source != null)
                        ge.Source.AttributeSetContainer.Sets[mmc.attributeSetName][mmc.attributeShortName]
                            .RegisterPostCurrentValueChange(OnAttributeChanged);
                }
            }
        }
        
        private void OnAttributeChanged(AttributeBase attribute,float oldValue,float newValue)
        {
            if(_modifierCache.Count == 0) return;
            foreach (var tuple in _modifierCache)
            {
                var ge = tuple.Item1;
                var modifier = tuple.Item2;
                if (modifier.MMC is AttributeBasedModCalculation mmc &&
                    mmc.captureType == AttributeBasedModCalculation.GEAttributeCaptureType.Track &&
                    attribute.Name == mmc.attributeName)
                {
                    if ((mmc.attributeFromType == AttributeBasedModCalculation.AttributeFrom.Target &&
                         attribute.Owner == ge.Owner) ||
                        (mmc.attributeFromType == AttributeBasedModCalculation.AttributeFrom.Source &&
                         attribute.Owner == ge.Source))
                    {
                        UpdateCurrentValueWhenModifierIsDirty();
                        break;
                    }
                }
            }
        }
    }
}
using System;
using System.Collections.Generic;
using GAS.General;
using UnityEngine;

namespace GAS.Runtime
{
    public class SkillEffectSpec
    {
        private Dictionary<GameplayTag, float> _valueMapWithTag = new Dictionary<GameplayTag, float>();
        private Dictionary<string, float> _valueMapWithName = new Dictionary<string, float>();
        private List<SkillCueDurationalSpec> _cueDurationalSpecs = new List<SkillCueDurationalSpec>();
        
        public event Action<SkillSystemComponent,SkillEffectSpec> onImmunity; 

        public SkillEffectSpec(
            SkillEffect skillEffect,
            SkillSystemComponent source,
            SkillSystemComponent owner)
        {
            SkillEffect = skillEffect;
            Source = source;
            Owner = owner;
            Duration = SkillEffect.Duration;
            DurationPolicy = SkillEffect.DurationPolicy;
            if (skillEffect.DurationPolicy != EffectsDurationPolicy.Instant)
            {
                PeriodExecution = SkillEffect.PeriodExecution?.CreateSpec(source, owner);
                PeriodTicker = new SkillEffectPeriodTicker(this);
            }

            CaptureDataFromSource();
        }

        public SkillEffect SkillEffect { get; }
        public long ActivationTime { get; private set; }
        public float Level { get; private set; }
        public SkillSystemComponent Source { get; }
        public SkillSystemComponent Owner { get; }
        public bool IsApplied { get; private set; }
        public bool IsActive { get; private set; }
        public SkillEffectPeriodTicker PeriodTicker { get; }
        public float Duration { get; private set; }
        public EffectsDurationPolicy DurationPolicy { get; private set; }
        public SkillEffectSpec PeriodExecution{ get; private set; }

        public Dictionary<string, float> SnapshotAttributes { get; private set; }

        public float DurationRemaining()
        {
            if (DurationPolicy == EffectsDurationPolicy.Infinite)
                return -1;

            return Mathf.Max(0, Duration - (SkillTimer.Timestamp() - ActivationTime) / 1000f);
        }

        public void SetLevel(float level)
        {
            Level = level;
        }

        public void SetDuration(float duration)
        {
            Duration = duration;
        }
        
        public void SetDurationPolicy(EffectsDurationPolicy durationPolicy)
        {
            DurationPolicy = durationPolicy;
        }
        
        public void SetPeriodExecution(SkillEffectSpec periodExecution)
        {
            PeriodExecution = periodExecution;
        }

        public void Apply()
        {
            if (IsApplied) return;
            IsApplied = true;
            Activate();
        }

        public void DisApply()
        {
            if (!IsApplied) return;
            IsApplied = false;
            Deactivate();
        }

        public void Activate()
        {
            if (IsActive) return;
            IsActive = true;
            ActivationTime = SkillTimer.Timestamp();
            TriggerOnActivation();
        }

        public void Deactivate()
        {
            if (!IsActive) return;
            IsActive = false;
            TriggerOnDeactivation();
        }

        public bool CanRunning()
        {
            return Owner.HasAllTags(SkillEffect.TagContainer.OngoingRequiredTags);
        }

        public void Tick()
        {
            PeriodTicker?.Tick();
        }

        void TriggerInstantCues(SkillCueInstant[] cues)
        {
            foreach (var cue in cues) cue.ApplyFrom(this);
        }
        
        private void TriggerCueOnExecute()
        {
            if (SkillEffect.CueOnExecute == null || SkillEffect.CueOnExecute.Length <= 0) return;
            TriggerInstantCues(SkillEffect.CueOnExecute);
        }

        private void TriggerCueOnAdd()
        {
            if (SkillEffect.CueOnAdd != null && SkillEffect.CueOnAdd.Length > 0)
                TriggerInstantCues(SkillEffect.CueOnAdd);

            if (SkillEffect.CueDurational != null && SkillEffect.CueDurational.Length > 0)
            {
                _cueDurationalSpecs.Clear();
                foreach (var cueDurational in SkillEffect.CueDurational)
                {
                    var cueSpec = cueDurational.ApplyFrom(this);
                    if (cueSpec != null) _cueDurationalSpecs.Add(cueSpec);
                }

                foreach (var cue in _cueDurationalSpecs) cue.OnAdd();
            }
        }

        private void TriggerCueOnRemove()
        {
            if (SkillEffect.CueOnRemove != null && SkillEffect.CueOnRemove.Length > 0)
                TriggerInstantCues(SkillEffect.CueOnRemove);

            if (SkillEffect.CueDurational != null && SkillEffect.CueDurational.Length > 0)
            {
                foreach (var cue in _cueDurationalSpecs) cue.OnRemove();

                _cueDurationalSpecs = null;
            }
        }

        private void TriggerCueOnActivation()
        {
            if (SkillEffect.CueOnActivate != null && SkillEffect.CueOnActivate.Length > 0)
                TriggerInstantCues(SkillEffect.CueOnActivate);

            if (SkillEffect.CueDurational != null && SkillEffect.CueDurational.Length > 0)
                foreach (var cue in _cueDurationalSpecs)
                    cue.OnGameplayEffectActivate();
        }

        private void TriggerCueOnDeactivation()
        {
            if (SkillEffect.CueOnDeactivate != null && SkillEffect.CueOnDeactivate.Length > 0)
                TriggerInstantCues(SkillEffect.CueOnDeactivate);

            if (SkillEffect.CueDurational != null && SkillEffect.CueDurational.Length > 0)
                foreach (var cue in _cueDurationalSpecs)
                    cue.OnGameplayEffectDeactivate();
        }

        private void CueOnTick()
        {
            if (SkillEffect.CueDurational == null || SkillEffect.CueDurational.Length <= 0) return;
            foreach (var cue in _cueDurationalSpecs) cue.OnTick();
        }

        public void TriggerOnExecute()
        {
            TriggerCueOnExecute();

            Owner.SkillEffectContainer.RemoveGameplayEffectWithAnyTags(SkillEffect.TagContainer
                .RemoveGameplayEffectsWithTags);
            Owner.ApplyModFromInstantGameplayEffect(this);
        }

        public void TriggerOnAdd()
        {
            TriggerCueOnAdd();
        }

        public void TriggerOnRemove()
        {
            TriggerCueOnRemove();
        }

        private void TriggerOnActivation()
        {
            TriggerCueOnActivation();
            Owner.GameplayTagAggregator.ApplyGameplayEffectDynamicTag(this);
            Owner.SkillEffectContainer.RemoveGameplayEffectWithAnyTags(SkillEffect.TagContainer
                .RemoveGameplayEffectsWithTags);
        }

        private void TriggerOnDeactivation()
        {
            TriggerCueOnDeactivation();
            Owner.GameplayTagAggregator.RestoreGameplayEffectDynamicTags(this);
        }

        public void TriggerOnTick()
        {
            if (DurationPolicy == EffectsDurationPolicy.Duration ||
                DurationPolicy == EffectsDurationPolicy.Infinite)
                CueOnTick();
        }

        public void TriggerOnImmunity()
        {
            onImmunity?.Invoke(Owner, this);
            onImmunity = null;
        }
        
        public void RemoveSelf()
        {
            Owner.SkillEffectContainer.RemoveGameplayEffectSpec(this);
        }

        private void CaptureDataFromSource()
        {
            SnapshotAttributes = Source.DataSnapshot();
        }

        public void RegisterValue(GameplayTag tag, float value)
        {
            _valueMapWithTag[tag] = value;
        }
        
        public void RegisterValue(string name, float value)
        {
            _valueMapWithName[name] = value;
        }
        
        public bool UnregisterValue(GameplayTag tag)
        {
            return _valueMapWithTag.Remove(tag);
        }
        
        public bool UnregisterValue(string name)
        {
            return _valueMapWithName.Remove(name);
        }
        
        public float? GetMapValue(GameplayTag tag)
        {
            return _valueMapWithTag.TryGetValue(tag, out var value) ? value : (float?) null;
        }
        
        public float? GetMapValue(string name)
        {
            return _valueMapWithName.TryGetValue(name, out var value) ? value : (float?) null;
        }
    }
}
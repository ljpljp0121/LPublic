
namespace GAS.Runtime
{
    public enum EffectsDurationPolicy
    {
        Instant = 1,
        Infinite,
        Duration
    }

    public class SkillEffect
    {
        public readonly string GameplayEffectName;
        public readonly EffectsDurationPolicy DurationPolicy;
        public readonly float Duration; // -1 represents infinite duration
        public readonly float Period;
        public readonly SkillEffect PeriodExecution;
        public readonly SkillEffectTagContainer TagContainer;

        // Cues
        public readonly SkillCueInstant[] CueOnExecute;
        public readonly SkillCueInstant[] CueOnRemove;
        public readonly SkillCueInstant[] CueOnAdd;
        public readonly SkillCueInstant[] CueOnActivate;
        public readonly SkillCueInstant[] CueOnDeactivate;
        public readonly SkillCueDurational[] CueDurational;

        public readonly GameplayEffectModifier[] Modifiers;
        public readonly ExecutionCalculation[] Executions;

        public SkillEffectSpec CreateSpec(
            SkillSystemComponent creator,
            SkillSystemComponent owner)
        {
            return new SkillEffectSpec(this, creator, owner);
        }

        public SkillEffect(SkillEffectAsset asset)
        {
            GameplayEffectName = asset.name;
            DurationPolicy = asset.DurationPolicy;
            Duration = asset.Duration;
            Period = asset.Period;
            TagContainer = new SkillEffectTagContainer(
                asset.AssetTags,
                asset.GrantedTags,
                asset.ApplicationRequiredTags,
                asset.OngoingRequiredTags,
                asset.RemoveGameplayEffectsWithTags,
                asset.ApplicationImmunityTags);
            PeriodExecution = asset.PeriodExecution != null ? new SkillEffect(asset.PeriodExecution) : null;
            CueOnExecute = asset.CueOnExecute;
            CueOnRemove = asset.CueOnRemove;
            CueOnAdd = asset.CueOnAdd;
            CueOnActivate = asset.CueOnActivate;
            CueOnDeactivate = asset.CueOnDeactivate;
            CueDurational = asset.CueDurational;
            Modifiers = asset.Modifiers;
            Executions = asset.Executions;
        }

        public SkillEffect(
            EffectsDurationPolicy durationPolicy,
            float duration,
            float period,
            SkillEffect periodExecution,
            SkillEffectTagContainer tagContainer,
            SkillCueInstant[] cueOnExecute,
            SkillCueInstant[] cueOnAdd,
            SkillCueInstant[] cueOnRemove,
            SkillCueInstant[] cueOnActivate,
            SkillCueInstant[] cueOnDeactivate,
            SkillCueDurational[] cueDurational,
            GameplayEffectModifier[] modifiers,
            ExecutionCalculation[] executions)
        {
            GameplayEffectName = null;
            DurationPolicy = durationPolicy;
            Duration = duration;
            Period = period;
            PeriodExecution = periodExecution;
            TagContainer = tagContainer;
            CueOnExecute = cueOnExecute;
            CueOnRemove = cueOnRemove;
            CueOnAdd = cueOnAdd;
            CueOnActivate = cueOnActivate;
            CueOnDeactivate = cueOnDeactivate;
            CueDurational = cueDurational;
            Modifiers = modifiers;
            Executions = executions;
        }

        public bool CanApplyTo(ISkillSystemComponent target)
        {
            return target.HasAllTags(TagContainer.ApplicationRequiredTags);
        }
    }
}
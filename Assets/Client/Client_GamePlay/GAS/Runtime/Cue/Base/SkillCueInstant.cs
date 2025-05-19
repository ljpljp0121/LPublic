using UnityEngine;

namespace GAS.Runtime
{
    /// <summary>
    /// 即时型提示
    /// </summary>
    public abstract class SkillCueInstant : SkillCue<SkillCueInstantSpec>
    {
        public virtual void ApplyFrom(SkillEffectSpec skillEffectSpec)
        {
            if (Triggerable(skillEffectSpec.Owner))
            {
                var instantCue = CreateSpec(new SkillCueParameters
                    { SourceSkillEffectSpec = skillEffectSpec });
                instantCue?.Trigger();
            }
        }

        public virtual void ApplyFrom(AbilitySpec abilitySpec, params object[] customArguments)
        {
            if (Triggerable(abilitySpec.Owner))
            {
                var instantCue = CreateSpec(new SkillCueParameters
                    { sourceAbilitySpec = abilitySpec, customArguments = customArguments });
                instantCue?.Trigger();
            }
        }

#if UNITY_EDITOR
        public virtual void OnEditorPreview(GameObject previewObject, int frame, int startFrame)
        {
        }
#endif
    }

    /// <summary>
    /// 即时型提示运行时实例逻辑
    /// </summary>
    public abstract class SkillCueInstantSpec : SkillCueSpec
    {
        public SkillCueInstantSpec(SkillCueInstant cue, SkillCueParameters parameters) : base(cue,
            parameters)
        {
        }
        
        public abstract void Trigger();
    }
    
    public abstract class SkillCueInstantSpec<T>:SkillCueInstantSpec where T:SkillCueInstant
    {
        public readonly T cue;
        
        public SkillCueInstantSpec(T cue, SkillCueParameters parameters) : base(cue, parameters)
        {
            this.cue = cue;
        }
    }
}
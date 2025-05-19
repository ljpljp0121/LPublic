using UnityEngine;

namespace GAS.Runtime
{
    /// <summary>
    ///  持续型提示
    /// </summary>
    public abstract class SkillCueDurational : SkillCue<SkillCueDurationalSpec>
    {
        public SkillCueDurationalSpec ApplyFrom(SkillEffectSpec skillEffectSpec)
        {
            if (!Triggerable(skillEffectSpec.Owner)) return null;
            var durationalCue = CreateSpec(new SkillCueParameters
                { SourceSkillEffectSpec = skillEffectSpec });
            return durationalCue;
        }
        
        public SkillCueDurationalSpec ApplyFrom(AbilitySpec abilitySpec,params object[] customArguments)
        {
            if (!Triggerable(abilitySpec.Owner)) return null;
            var durationalCue = CreateSpec(new SkillCueParameters
                { sourceAbilitySpec = abilitySpec, customArguments = customArguments});
            return durationalCue;
        }
        
#if UNITY_EDITOR
        public virtual void OnEditorPreview(GameObject previewObject,int frameIndex,int startFrame,int endFrame)
        {
        }
#endif
    }

    /// <summary>
    /// 持续型提示运行时实例逻辑
    /// </summary>
    public abstract class SkillCueDurationalSpec : SkillCueSpec
    {
        protected SkillCueDurationalSpec(SkillCueDurational cue, SkillCueParameters parameters) : 
            base(cue, parameters)
        {
        }

        public abstract void OnAdd();
        public abstract void OnRemove();
        public abstract void OnGameplayEffectActivate();
        public abstract void OnGameplayEffectDeactivate();
        public abstract void OnTick();
    }

    public abstract class SkillCueDurationalSpec<T> : SkillCueDurationalSpec where T : SkillCueDurational
    {
        public readonly T cue;

        protected SkillCueDurationalSpec(T cue, SkillCueParameters parameters) : base(cue, parameters)
        {
            this.cue = cue;
        }
    }
}
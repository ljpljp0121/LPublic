
namespace GAS.Runtime
{
    /// <summary>
    /// 效果的运行时实例逻辑
    /// </summary>
    public abstract class SkillCueSpec
    {
        protected readonly SkillCue _cue;
        protected readonly SkillCueParameters _parameters;
        public SkillSystemComponent Owner { get; protected set; }

        public virtual bool Triggerable()
        {
            return _cue.Triggerable(Owner);
        }
        
        public SkillCueSpec(SkillCue cue, SkillCueParameters cueParameters)
        {
            _cue = cue;
            _parameters = cueParameters;
            if (_parameters.SourceSkillEffectSpec != null)
            {
                Owner = _parameters.SourceSkillEffectSpec.Owner;
            }
            else if (_parameters.sourceAbilitySpec != null)
            {
                Owner = _parameters.sourceAbilitySpec.Owner;
            }
        }
    }
}
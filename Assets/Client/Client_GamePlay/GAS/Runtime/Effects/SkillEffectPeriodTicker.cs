using UnityEngine;

namespace GAS.Runtime
{
    public class SkillEffectPeriodTicker
    {
        private float _periodRemaining;
        private readonly SkillEffectSpec _spec;

        public SkillEffectPeriodTicker(SkillEffectSpec spec)
        {
            _spec = spec;
            _periodRemaining = Period;
        }

        private float Period => _spec.SkillEffect.Period;

        public void Tick()
        {
            _spec.TriggerOnTick();
            
            if (_periodRemaining <= 0)
            {
                _periodRemaining = Period;
                _spec.PeriodExecution?.TriggerOnExecute();
            }
            else
            {
                _periodRemaining -= Time.deltaTime;
            }

            if (_spec.DurationPolicy== EffectsDurationPolicy.Duration && _spec.DurationRemaining() <= 0)
            {
                _spec.RemoveSelf();
            }
        }
    }
}
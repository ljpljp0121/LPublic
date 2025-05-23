using UnityEngine;

namespace GAS.Runtime
{
    /// <summary>
    /// （运行时动态传值）
    /// </summary>
    [CreateAssetMenu(fileName = "SetByCallerFromTag", menuName = "GAS/MMC/SetByCallerFromTagModCalculation")]
    public class SetByCallerFromTagModCalculation:ModifierMagnitudeCalculation
    {
        [SerializeField] private GameplayTag _tag;
        public override float CalculateMagnitude(SkillEffectSpec spec  ,float input)
        {
            var value = spec.GetMapValue(_tag);
#if UNITY_EDITOR
            if(value==null) Debug.LogWarning($"SetByCallerModCalculation: GE's '{_tag.Name}' value(tag map) is not set");
#endif
            return value ?? 0;
        }
    }
}
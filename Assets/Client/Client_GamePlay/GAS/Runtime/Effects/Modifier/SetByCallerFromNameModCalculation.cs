using UnityEngine;

namespace GAS.Runtime
{
    /// <summary>
    /// （运行时动态传值）
    /// </summary>
    [CreateAssetMenu(fileName = "SetByCallerFromName", menuName = "GAS/MMC/SetByCallerFromNameModCalculation")]
    public class SetByCallerFromNameModCalculation : ModifierMagnitudeCalculation
    {
        [SerializeField] private string valueName;
        public override float CalculateMagnitude(SkillEffectSpec spec,float input)
        {
            var value = spec.GetMapValue(valueName);
#if UNITY_EDITOR
            if(value==null) Debug.LogWarning($"SetByCallerModCalculation: GE's '{valueName}' value(name map) is not set");
#endif
            return value ?? 0;
        }
    }
}
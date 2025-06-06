using System.Collections.Generic;
using GAS.Runtime;

namespace GAS.Runtime
{
    /// <summary>
    /// 能力实现基类
    /// 连接 数据层 与 逻辑层 的桥梁
    /// </summary>
    public abstract class AbstractAbility
    {
        public readonly string Name;
        public readonly AbilityAsset DataReference;

        // TODO : AbilityTask
        // public List<OngoingAbilityTask> OngoingAbilityTasks=new List<OngoingAbilityTask>();
        // public List<AsyncAbilityTask> AsyncAbilityTasks = new List<AsyncAbilityTask>();

        public AbilityTagContainer Tag { get; protected set; }

        public SkillEffect Cooldown { get; protected set; }

        public float CooldownTime { get; protected set; }

        public SkillEffect Cost { get; protected set; }

        public AbstractAbility(AbilityAsset abilityAsset)
        {
            DataReference = abilityAsset;

            Name = DataReference.UniqueName;
            Tag = new AbilityTagContainer(
                DataReference.AssetTag, DataReference.CancelAbilityTags, DataReference.BlockAbilityTags,
                DataReference.ActivationOwnedTag, DataReference.ActivationRequiredTags, DataReference.ActivationBlockedTags);
            Cooldown = DataReference.Cooldown ? new SkillEffect(DataReference.Cooldown) : default;
            Cost = DataReference.Cost ? new SkillEffect(DataReference.Cost) : default;

            CooldownTime = DataReference.CooldownTime;
        }

        public abstract AbilitySpec CreateSpec(SkillSystemComponent owner);

        public void SetCooldown(SkillEffect coolDown)
        {
            if (coolDown.DurationPolicy == EffectsDurationPolicy.Duration)
            {
                Cooldown = coolDown;
            }
#if UNITY_EDITOR
            else
            {
                UnityEngine.Debug.LogError("Cooldown must be duration policy!");
            }
#endif
        }

        public void SetCost(SkillEffect cost)
        {
            if (cost.DurationPolicy == EffectsDurationPolicy.Instant)
            {
                Cost = cost;
            }
#if UNITY_EDITOR
            else
            {
                UnityEngine.Debug.LogError("Cost must be instant policy!");
            }
#endif
        }
    }

    /// <summary>
    /// 可以直接通过AbilityAsset属性获取到具体的资源,不用强制转换
    /// </summary>
    public abstract class AbstractAbility<T> : AbstractAbility where T : AbilityAsset
    {
        public T AbilityAsset => DataReference as T;

        protected AbstractAbility(T abilityAsset) : base(abilityAsset)
        {
        }
    }
}
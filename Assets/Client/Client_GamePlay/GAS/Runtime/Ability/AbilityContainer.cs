using System.Collections.Generic;
using System.Linq;

namespace GAS.Runtime
{
    /// <summary>
    /// ���� ?ASC���е����м���ʵ��
    /// </summary>
    public class AbilityContainer
    {
        private readonly SkillSystemComponent _owner;
        private readonly Dictionary<string, AbilitySpec> _abilities = new Dictionary<string, AbilitySpec>();

        public AbilityContainer(SkillSystemComponent owner)
        {
            _owner = owner;
        }

        public void Tick()
        {
            var enumerable = _abilities.Values.ToArray();
            foreach (var abilitySpec in enumerable) abilitySpec.Tick();
        }

        public void GrantAbility(AbstractAbility ability)
        {
            if (_abilities.ContainsKey(ability.Name)) return;
            var abilitySpec = ability.CreateSpec(_owner);
            _abilities.Add(ability.Name, abilitySpec);
        }

        public void RemoveAbility(AbstractAbility ability)
        {
            RemoveAbility(ability.Name);
        }

        public void RemoveAbility(string abilityName)
        {
            if (!_abilities.ContainsKey(abilityName)) return;

            EndAbility(abilityName);
            _abilities.Remove(abilityName);
        }

        public bool TryActivateAbility(string abilityName, bool isReStart = false, params object[] args)
        {
            if (!_abilities.ContainsKey(abilityName)) return false;
            if (!_abilities[abilityName].TryActivateAbility(isReStart, args)) return false;

            var tags = _abilities[abilityName].Ability.Tag.CancelAbilitiesWithTags;
            foreach (var kv in _abilities)
            {
                if (kv.Key.Equals(abilityName))
                    continue;
                var abilityTag = kv.Value.Ability.Tag;
                if (abilityTag.AssetTag.HasAnyTags(tags))
                {
                    _abilities[kv.Key].TryCancelAbility();
                }
            }

            return true;
        }

        public void EndAbility(string abilityName)
        {
            if (!_abilities.ContainsKey(abilityName)) return;
            _abilities[abilityName].TryEndAbility();
        }

        public void CancelAbility(string abilityName)
        {
            if (!_abilities.ContainsKey(abilityName)) return;
            _abilities[abilityName].TryCancelAbility();
        }

        void CancelAbilitiesByTag(GameplayTagSet tags)
        {
            foreach (var kv in _abilities)
            {
                var abilityTag = kv.Value.Ability.Tag;
                if (abilityTag.AssetTag.HasAnyTags(tags))
                {
                    _abilities[kv.Key].TryCancelAbility();
                }
            }
        }

        public Dictionary<string, AbilitySpec> AbilitySpecs() => _abilities;

        public void CancelAllAbilities()
        {
            foreach (var kv in _abilities)
                _abilities[kv.Key].TryCancelAbility();
        }
    }
}
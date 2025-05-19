namespace GAS.Runtime
{
    public struct SkillEffectTagContainer
    {
        public GameplayTagSet AssetTags;
        
        public GameplayTagSet GrantedTags;
        
        public GameplayTagSet ApplicationRequiredTags;
        public GameplayTagSet OngoingRequiredTags;
        
        public GameplayTagSet RemoveGameplayEffectsWithTags;
        
        public GameplayTagSet ApplicationImmunityTags;

        public SkillEffectTagContainer(
            GameplayTag[] assetTags, 
            GameplayTag[] grantedTags,
            GameplayTag[] applicationRequiredTags,
            GameplayTag[] ongoingRequiredTags,
            GameplayTag[] removeGameplayEffectsWithTags,
            GameplayTag[] applicationImmunityTags)
        {
            AssetTags = new GameplayTagSet(assetTags);
            GrantedTags = new GameplayTagSet(grantedTags);
            ApplicationRequiredTags = new GameplayTagSet(applicationRequiredTags);
            OngoingRequiredTags = new GameplayTagSet(ongoingRequiredTags);
            RemoveGameplayEffectsWithTags = new GameplayTagSet(removeGameplayEffectsWithTags);
            ApplicationImmunityTags = new GameplayTagSet(applicationImmunityTags);
        }
    }
}
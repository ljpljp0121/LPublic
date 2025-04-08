namespace GAS.Runtime
{
    /// <summary>
    /// 将相关属性组织成逻辑组
    /// 用于组织和管理一组属性
    /// </summary>
    public abstract class AttributeSet
    {
        protected AbilitySystemComponent _owner;
        
        public abstract AttributeBase this[string key] { get; }
        public abstract string[] AttributeNames { get; }
        public abstract void SetOwner(AbilitySystemComponent owner);
        public void ChangeAttributeBase(string attributeShortName, float value)
        {
            if (this[attributeShortName] != null)
            {
                this[attributeShortName].SetBaseValue(value);
            }
        }
    }
}
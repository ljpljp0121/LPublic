namespace GAS.Runtime
{
    /// <summary>
    /// GAS属性结构体，
    /// 存储属性的基础值和实时计算后的当前值
    /// 
    /// </summary>
    public struct AttributeValue
    {
        public AttributeValue(float baseValue)
        {
            _baseValue = baseValue;
            _currentValue = baseValue;
        }

        float _baseValue;
        public float BaseValue => _baseValue;

        float _currentValue;
        public float CurrentValue => _currentValue;

        public void SetCurrentValue(float value)
        {
            _currentValue = value;
        }

        public void SetBaseValue(float value)
        {
            _baseValue = value;
        }
    }
}
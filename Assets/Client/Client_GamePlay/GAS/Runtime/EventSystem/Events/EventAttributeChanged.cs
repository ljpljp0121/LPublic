using System;

namespace GAS.Runtime
{
    public class AttributeChangedEventArgs : EventArgs
    {
        public AttributeChangedEventArgs(SkillSystemComponent owner, AttributeBase attribute, float oldValue,
            float newValue)
        {
            Owner = owner;
            Attribute = attribute;
            OldValue = oldValue;
            NewValue = newValue;
        }

        public SkillSystemComponent Owner { get; }
        public AttributeBase Attribute { get; }
        public float OldValue { get; }
        public float NewValue { get; }
    }

    public class AttributeChangedEvent:EventBase<AttributeChangedEventArgs>
    {
        public void Publish(SkillSystemComponent owner, AttributeBase attribute, float oldValue, float newValue)
        {
            Publish(new AttributeChangedEventArgs(owner, attribute, oldValue, newValue));
        }
    }
}
using System;
using System.Collections.Generic;

namespace GAS.Runtime
{
    /// <summary>
    /// 作为所有 AttributeSet 的容器，统一管理生命周期
    /// </summary>
    public class AttributeSetContainer
    {
        private readonly AbilitySystemComponent _owner;
        private readonly Dictionary<string,AttributeSet> _attributeSets = new Dictionary<string,AttributeSet>();
        private readonly Dictionary<AttributeBase,AttributeAggregator> _attributeAggregators = new Dictionary<AttributeBase, AttributeAggregator>();
        
        public Dictionary<string,AttributeSet> Sets => _attributeSets;
        
        public AttributeSetContainer(AbilitySystemComponent owner)
        {
            _owner = owner;
        }
        
        public void AddAttributeSet<T>() where T : AttributeSet
        {
            if (TryGetAttributeSet<T>(out _)) return;
            
            var setName = AttributeSetUtil.AttributeSetName(typeof(T));
            _attributeSets.Add(setName,Activator.CreateInstance<T>());
            
            var attrSet = _attributeSets[setName];
            foreach (var attr in attrSet.AttributeNames)
            {
                if (!_attributeAggregators.ContainsKey(attrSet[attr]))
                {
                    _attributeAggregators.Add(attrSet[attr],new AttributeAggregator(attrSet[attr],_owner));
                }
            }
        }
        
        /// <summary>
        /// 添加属性集
        /// </summary>
        public void AddAttributeSet(Type attrSetType)
        {
            if (TryGetAttributeSet(attrSetType,out _)) return;
            var setName = AttributeSetUtil.AttributeSetName(attrSetType);
            _attributeSets.Add(setName,Activator.CreateInstance(attrSetType) as AttributeSet);
            
            AttributeSet attrSet = _attributeSets[setName];
            foreach (var attr in attrSet.AttributeNames)
            {
                if (!_attributeAggregators.ContainsKey(attrSet[attr]))
                {
                    _attributeAggregators.Add(attrSet[attr],new AttributeAggregator(attrSet[attr],_owner));
                }
            }
            attrSet.SetOwner(_owner);
        }
        
        /// <summary>
        /// Be careful when using this method, it may cause unexpected errors(when using network sync).
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void RemoveAttributeSet<T>()where T : AttributeSet
        {
            var setName = AttributeSetUtil.AttributeSetName(typeof(T));
            var attrSet = _attributeSets[setName];
            foreach (var attr in attrSet.AttributeNames)
            {
                _attributeAggregators.Remove(attrSet[attr]);
            }

            _attributeSets.Remove(setName);
        }

        /// <summary>
        /// 获取属性集
        /// </summary>
        /// <typeparam name="T">属性集类型</typeparam>
        /// <param name="attributeSet">属性集</param>
        /// <returns>是否获取成功</returns>
        public bool TryGetAttributeSet<T>(out T attributeSet) where T : AttributeSet
        {
            if(_attributeSets.TryGetValue(AttributeSetUtil.AttributeSetName(typeof(T)), out var set))
            {
                attributeSet = (T)set;
                return true;
            }
            
            attributeSet = null;
            return false;
        }

        /// <summary>
        /// 获取属性集
        /// </summary>
        /// <param name="attrSetType">属性集类型</param>
        /// <param name="attributeSet">属性集</param>
        /// <returns>是否获取成功</returns>
        bool TryGetAttributeSet(Type attrSetType,out AttributeSet attributeSet)
        {
            if(_attributeSets.TryGetValue(AttributeSetUtil.AttributeSetName(attrSetType), out var set))
            {
                attributeSet = set;
                return true;
            }
            
            attributeSet = null;
            return false;
        }

        /// <summary>
        /// 获取属性基础值
        /// </summary>
        /// <param name="attrSetName">属性集名称</param>
        /// <param name="attrShortName">属性短名称</param>
        /// <returns>属性值</returns>
        public float? GetAttributeBaseValue(string attrSetName,string attrShortName)
        {
            return _attributeSets.TryGetValue(attrSetName, out var set) ? set[attrShortName].BaseValue : (float?)null;
        }

        /// <summary>
        /// 获取属性当前值。
        /// </summary>
        /// <param name="attrSetName">属性集名称</param>
        /// <param name="attrShortName">属性短名称</param>
        /// <returns>属性值</returns>
        public float? GetAttributeCurrentValue(string attrSetName,string attrShortName)
        {
            return _attributeSets.TryGetValue(attrSetName, out var set) ? set[attrShortName].CurrentValue : (float?)null;
        }

        /// <summary>
        /// 生成所有属性当前值的快照，用于存档或同步。
        /// </summary>
        /// <returns>Key:属性名 Value:属性值</returns>
        public Dictionary<string, float> Snapshot()
        {
            Dictionary<string, float> snapshot = new Dictionary<string, float>();
            foreach (var attributeSet in _attributeSets)
            {
                foreach (var name in attributeSet.Value.AttributeNames)
                {
                    var attr = attributeSet.Value[name];
                    snapshot.Add(attr.Name, attr.CurrentValue);
                }
            }
            return snapshot;
        }

        /// <summary>
        /// 清理所有计算器资源
        /// </summary>
        public void Dispose()
        {
            foreach (var aggregator in _attributeAggregators)
                aggregator.Value.OnDispose();
        }
    }
}
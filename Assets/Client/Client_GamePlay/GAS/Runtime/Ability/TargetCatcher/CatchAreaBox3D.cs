using System;
using System.Collections.Generic;
using GAS.General;
using UnityEngine;

namespace GAS.Runtime
{
    [Serializable]
    public class CatchAreaBox3D : CatchAreaBase
    {
        public Vector3 offset;
        public Vector3 size;
        public Vector3 rotation;
        public EffectCenterType centerType;

        public void Init(SkillSystemComponent owner, LayerMask checkLayer, Vector3 offset, Vector3 size, Vector3 rotation)
        {
            base.Init(owner, checkLayer);
            this.offset = offset;
            this.size = size;
            this.rotation = rotation;
        }

        public override List<SkillSystemComponent> CatchTargets(SkillSystemComponent mainTarget)
        {
            var result = new List<SkillSystemComponent>();

            Collider[] targets = centerType switch
            {
                EffectCenterType.SelfOffset => Owner.OverlapBox3D(offset, size, rotation, checkLayer),
                EffectCenterType.WorldSpace => Physics.OverlapBox(offset, size, Quaternion.Euler(rotation), checkLayer),
                EffectCenterType.TargetOffset => mainTarget.OverlapBox3D(offset, size, rotation, checkLayer),
                _ => null
            };

            if (targets == null) return result;
            foreach (Collider target in targets)
            {
                var targetUnit = target.GetComponent<SkillSystemComponent>();
                if (targetUnit != null) result.Add(targetUnit);
            }
            return result;
        }
#if UNITY_EDITOR

        public override void OnEditorPreview(GameObject previewObject)
        {
            float showTime = 1;
            Color color = Color.green;
            var relativeTransform = previewObject.transform;
            var center = offset;
            Quaternion angle = relativeTransform.rotation * Quaternion.Euler(rotation);
            switch (centerType)
            {
                case EffectCenterType.SelfOffset:
                    Vector3 worldOffset = relativeTransform.TransformDirection(offset);
                    center = relativeTransform.position + worldOffset;
                    angle = relativeTransform.rotation * Quaternion.Euler(rotation);
                    break;
                case EffectCenterType.WorldSpace:
                    center = offset;
                    angle = Quaternion.Euler(rotation);
                    break;
                case EffectCenterType.TargetOffset:
                    return;
            }
            //DebugExtension.DebugBox(center,size,angle,color,showTime);
        }

#endif

    }
}
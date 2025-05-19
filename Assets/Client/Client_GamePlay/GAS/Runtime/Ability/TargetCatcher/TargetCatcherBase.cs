using System.Collections.Generic;
using GAS.Runtime;
using UnityEngine;

namespace GAS.Runtime
{
    public abstract class TargetCatcherBase
    {
        public SkillSystemComponent Owner;

        public TargetCatcherBase()
        {

        }

        public virtual void Init(SkillSystemComponent owner)
        {
            Owner = owner;
        }

        public abstract List<SkillSystemComponent> CatchTargets(SkillSystemComponent mainTarget);

#if UNITY_EDITOR
        public virtual void OnEditorPreview(GameObject obj)
        {
        }
#endif
    }
}
﻿using System;
using GAS.Runtime;
using UnityEngine;

namespace GAS.Runtime
{
    [Serializable]
    public abstract class CatchAreaBase : TargetCatcherBase
    {
        public LayerMask checkLayer;

        public void Init(SkillSystemComponent owner, LayerMask checkLayer) 
        {
            base.Init(owner);
            this.checkLayer = checkLayer;
        }
    }
}
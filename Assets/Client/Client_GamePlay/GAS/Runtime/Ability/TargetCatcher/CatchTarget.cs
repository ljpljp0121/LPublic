using System.Collections.Generic;
using GAS.Runtime;

namespace GAS.Runtime
{
    public class CatchTarget : TargetCatcherBase
    {
        public override List<SkillSystemComponent> CatchTargets(SkillSystemComponent target)
        {
            return new List<SkillSystemComponent>() { target };
        }
    }
}
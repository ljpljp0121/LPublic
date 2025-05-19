using System.Collections.Generic;
using GAS.Runtime;

namespace GAS.Runtime
{
    public class CatchSelf : TargetCatcherBase
    {
        public override List<SkillSystemComponent> CatchTargets(SkillSystemComponent mainTarget)
        {
            return new List<SkillSystemComponent> { Owner };
        }
    }
}
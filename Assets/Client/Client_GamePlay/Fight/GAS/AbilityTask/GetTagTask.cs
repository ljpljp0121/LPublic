using System;
using System.Collections;
using System.Collections.Generic;
using GAS.Runtime;
using UnityEngine;

[Serializable]
public class GetTagTask : OngoingAbilityTask
{
    [SerializeField] private GameplayTag tag;

    private AbilitySystemComponent asc;

    public override void Init(AbilitySpec spec)
    {
        base.Init(spec);
        asc = _spec.Owner;
    }
    public override void OnStart(int startFrame)
    {
        asc.AddFixedTag(tag);
    }

    public override void OnEnd(int endFrame)
    {
        asc.RemoveFixedTag(tag);
    }

    public override void OnTick(int frameIndex, int startFrame, int endFrame)
    {

    }

    public void SetGameplayTag(GameplayTag tag)
    {
        this.tag = tag;
    }
}
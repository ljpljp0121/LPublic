using System.Collections;
using System.Collections.Generic;
using Game.RunTime;
using UnityEngine;

public class GameAbilityComponent : GameComponent
{
    public GameTagAggregator GameTagAggregator { get; private set; }


    private bool ready;
    private void Awake()
    {
        Prepare();
    }

    public override void Enable()
    {
        Prepare();
    }

    private void Prepare()
    {
        if (ready) return;
        GameTagAggregator = new GameTagAggregator();
        ready = true;
    }

}

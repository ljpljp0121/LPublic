
using UnityEngine;

public class ChronosSystem : SingletonMono<ChronosSystem>
{
    protected override void Awake()
    {
        base.Awake();
        InitTimeScales();
    }

    private void InitTimeScales()
    {
        
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IChronosComponent
{
    float LocalTimeScaleValue { get; set; }
    public TimeScaleGroup SelfTimeScale { get; }
    void RegisterToLocalTimeScale(TimeScaleGroup timeScaleGroup);
    void UnRegisterFromLocalTimeScale(TimeScaleGroup timeScaleGroup);
    List<TimeScaleGroup> TimeScaleGroupContainer { get; }
    float TimeScale { get; }
    float DeltaTime { get; }
    float UnscaledDeltaTime { get; }
    float FixedDeltaTime { get; }
    void OnTimeScaleGroupDestroyed(TimeScaleGroup source);
    void OnTimeScaleChanged(float timeScale, TimeScaleGroup source);
    public GameObject GameObject { get; }
}
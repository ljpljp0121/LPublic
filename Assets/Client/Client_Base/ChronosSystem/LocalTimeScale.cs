
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "TimeScale", menuName = "Chronos/TimeScale")]
public class LocalTimeScale : ScriptableObject
{
    [SerializeField]
    protected float scaleValue = 1f;
    [SerializeField]
    protected bool autoRemoveWhenEmpty = true;

    protected float lastValue = 1f;
    protected bool isPaused = false;
    protected readonly HashSet<ILocalTimer> localTimeContainer = new();

    public bool AutoRemoveWhenEmpty
    {
        get => autoRemoveWhenEmpty;
        set => autoRemoveWhenEmpty = value;
    }

    public bool IsPaused => isPaused;
    public string[] LocalTimerNames => localTimeContainer.Select(s => s.ToString()).ToArray();
    public GameObject[] LocalTimerObjects => localTimeContainer.Select(s => s.GameObject).ToArray();
    public float ScaleValue
    {
        get => scaleValue;
        set => scaleValue = value;
    }

}

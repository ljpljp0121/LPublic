using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 时间组件
/// </summary>
public class ChronosComponent : MonoBehaviour, IChronosComponent
{
    [Min(0f)] [Tooltip("默认TimeScale,在初始化时会以这个值初始化时间刻度")] [SerializeField]
    protected float defaultTimeScale = 1f;

    protected TimeScaleGroup localTimeScale;
    protected float currentTimeScale;
    protected float desiredTimeScale;

    public List<TimeScaleGroup> TimeScaleGroupContainer { get; } = new List<TimeScaleGroup>();
    public float TimeScale => currentTimeScale;
    public float DeltaTime => Time.deltaTime * TimeScale;
    public float UnscaledDeltaTime => Time.unscaledDeltaTime * TimeScale;
    public float FixedDeltaTime => Time.fixedDeltaTime * TimeScale;
    public virtual float InverseTimeScale => Time.timeScale / TimeScale;
    public TimeScaleGroup SelfTimeScale => localTimeScale;
    public float LocalTimeScaleValue
    {
        get => localTimeScale.ScaleValue;
        set => localTimeScale.SetScaleValue(value);
    }
    public GameObject GameObject => gameObject;
    protected virtual void SetCurrentTimeScale(float value) => currentTimeScale = value;
    protected virtual void SetDesiredTimeScale(float value) => desiredTimeScale = value;

    protected virtual void Awake()
    {
        localTimeScale = new TimeScaleGroup();
        localTimeScale.Init($"{name}_LocalTimeScale", 1f);
    }

    protected virtual void OnEnable()
    {
        SetCurrentTimeScale(defaultTimeScale);
    }

    protected virtual void OnDestroy()
    {
        UnRegisterFromAll();
        if (localTimeScale == null) return;
        localTimeScale.Dispose();
        localTimeScale = null;
    }

    public virtual void RegisterToLocalTimeScale(TimeScaleGroup timeScaleGroup)
    {
        if (timeScaleGroup == null)
        {
            LogSystem.Error($"timeScaleGroup is null, please check it!");
            return;
        }
        if (TimeScaleGroupContainer.Contains(timeScaleGroup)) return;
        TimeScaleGroupContainer.Add(timeScaleGroup);
        timeScaleGroup.Register(this);
        timeScaleGroup.OnTimeScaleChanged += OnTimeScaleChanged;
        ResetDesiredTimeScale(bool applyImmediately = false);
    }

    public virtual void UnRegisterFromLocalTimeScale(TimeScaleGroup timeScaleGroup)
    {
        if (timeScaleGroup == null)
            return;
        if (TimeScaleGroupContainer.Contains(timeScaleGroup))
        {
            timeScaleGroup.UnRegister(this);
            TimeScaleGroupContainer.Remove(timeScaleGroup);
        }
    }

    public virtual void UnRegisterFromAll()
    {
        for (var i = TimeScaleGroupContainer.Count - 1; i >= 0; i--)
        {
            var timeScaleGroup = TimeScaleGroupContainer[i];
            UnRegisterFromLocalTimeScale(timeScaleGroup);
        }
        TimeScaleGroupContainer.Clear();
    }


    public virtual void OnTimeScaleGroupDestroyed(TimeScaleGroup source)
    {
        if (source == null)
            return;
        if (TimeScaleGroupContainer.Contains(source))
            TimeScaleGroupContainer.Remove(source);
    }

    public virtual void OnTimeScaleChanged(float timeScale, TimeScaleGroup source)
    {
        HandleChangedTimeScaleValues();
    }

    public virtual void HandleChangedTimeScaleValues() { }

    public virtual void ResetDesiredTimeScale(bool applyImmediately = false)
    {
        if (applyImmediately)
        {
            EndTransition();
            return;
        }
        StartTransition();
    }

    protected virtual void StartTransition() { }

    protected virtual void EndTransition() { }
}
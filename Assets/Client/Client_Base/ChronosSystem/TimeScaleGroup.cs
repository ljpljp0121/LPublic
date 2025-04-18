using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class TimeScaleGroup
{
    [SerializeField] protected float scaleValue = 1f;
    [SerializeField] protected bool autoRemoveWhenEmpty = true;

    protected string name;
    protected float lastValue = 1f;
    protected bool isPaused = false;
    protected readonly HashSet<IChronosComponent> localTimeContainer = new();

    public string Name => name;
    public int HashCode => name.GetHashCode();

    public bool AutoRemoveWhenEmpty
    {
        get => autoRemoveWhenEmpty;
        set => autoRemoveWhenEmpty = value;
    }

    public bool IsPaused => isPaused;
    public string[] LocalTimerNames => localTimeContainer.Select(s => s.ToString()).ToArray();
    public GameObject[] LocalTimerObjects => localTimeContainer.Select(s => s.GameObject).ToArray();
    public int LocalTimerCount => localTimeContainer.Count;
    public float ScaleValue
    {
        get => scaleValue;
        set
        {
            if (value < 0f)
            {
                Debug.LogWarning("Negative TimeScale is not supported. Setting to 0.");
                scaleValue = 0f;
            }
            this.scaleValue = value;
            OnTimeScaleChanged?.Invoke(this.scaleValue, this);
            OnTimeScaleChangedUnityEvent?.Invoke(this.scaleValue);

            if (this.scaleValue == 0f && !isPaused)
            {
                isPaused = true;
                OnPaused?.Invoke(this);
                OnPausedUnityEvent?.Invoke();
            }
            else if (this.scaleValue > 0f && isPaused)
            {
                isPaused = false;
                OnResumed?.Invoke(this);
                OnResumedUnityEvent?.Invoke();
            }
        }
    }
    public float DeltaTime => Time.deltaTime * scaleValue;
    public float UnscaledDeltaTime => Time.unscaledDeltaTime * scaleValue;
    public float FixedDeltaTime => Time.fixedDeltaTime * scaleValue;
    public event Action<float, TimeScaleGroup> OnTimeScaleChanged;
    public event Action<TimeScaleGroup> OnPaused;
    public event Action<TimeScaleGroup> OnResumed;
    public event Action<object> OnRegister;
    public event Action<object> OnUnRegister;

    #region Unity事件接口

    [SerializeField] protected UnityEvent<float> onTimeScaleChangedUnityEvent = new UnityEvent<float>();
    [SerializeField] protected UnityEvent<object> onRegisterUnityEvent = new UnityEvent<object>();
    [SerializeField] protected UnityEvent<object> onUnRegisterUnityEvent = new UnityEvent<object>();
    [SerializeField] protected UnityEvent onPausedUnityEvent = new UnityEvent();
    [SerializeField] protected UnityEvent onResumedUnityEvent = new UnityEvent();

    public UnityEvent<float> OnTimeScaleChangedUnityEvent => onTimeScaleChangedUnityEvent;
    public UnityEvent<object> OnRegisterUnityEvent => onRegisterUnityEvent;
    public UnityEvent<object> OnUnRegisterUnityEvent => onUnRegisterUnityEvent;
    public UnityEvent OnPausedUnityEvent => onPausedUnityEvent;
    public UnityEvent OnResumedUnityEvent => onResumedUnityEvent;

    #endregion

    public void SetScaleValue(float newValue)
    {
        if (Mathf.Approximately(ScaleValue, newValue)) return;
        ScaleValue = newValue;
    }

    public void Pause()
    {
        lastValue = ScaleValue;
        ScaleValue = 0f;
    }

    public void Resume() => ScaleValue = lastValue;

    public void Init(string nameValue, float initValue = 1f, bool autoRemoveWhenEmpty = true)
    {
        name = nameValue;
        ScaleValue = initValue;
        this.autoRemoveWhenEmpty = autoRemoveWhenEmpty;
    }

    public void Register(IChronosComponent chronosComponent)
    {
        localTimeContainer.Add(chronosComponent);
        OnRegister?.Invoke(chronosComponent);
        OnRegisterUnityEvent?.Invoke(chronosComponent);
    }

    public void UnRegister(IChronosComponent chronosComponent)
    {
        if (!localTimeContainer.Contains(chronosComponent))
            return;

        Debug.Log($"UnRegister {chronosComponent.GameObject.name} from {name}");
        localTimeContainer.Remove(chronosComponent);
        chronosComponent.UnRegisterFromLocalTimeScale(this);
        OnUnRegister?.Invoke(chronosComponent);
        OnUnRegisterUnityEvent?.Invoke(chronosComponent);

        if (localTimeContainer.Count == 0 && AutoRemoveWhenEmpty)
            ChronosSystem.Instance.RemoveTimeScaleGroup(name);
    }

    public void UnRegisterAll()
    {
        foreach (var localTimer in localTimeContainer.ToArray())
        {
            UnRegister(localTimer);
        }
    }

    public void Dispose()
    {
        foreach (var localTimer in localTimeContainer)
            localTimer.OnTimeScaleGroupDestroyed(this);
        UnRegisterAll();
        localTimeContainer.Clear();
        if (AutoRemoveWhenEmpty && ChronosSystem.Instance != null)
            ChronosSystem.Instance.RemoveTimeScaleGroup(name);
    }
}
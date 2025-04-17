
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

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

    public event Action<float, LocalTimeScale> OnTimeScaleChanged;
    public event Action<LocalTimeScale> OnPaused;
    public event Action<LocalTimeScale> OnResumed;
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

}

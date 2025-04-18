using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

/// <summary>
/// 时间组件
/// 为所有需要使用Chronos的对象提供时间刻度功能
/// 不要使用Unity的Time.deltaTime,为对象添加该组件后,使用该组件的DeltaTime
/// </summary>
public class ChronosComponent : MonoBehaviour, IChronosComponent
{
    [Min(0f)]
    [Tooltip("默认TimeScale,在初始化时会以这个值初始化时间刻度")]
    [SerializeField]
    protected float defaultTimeScale = 1f;

    [Header("过渡设置")]
    [InfoBox("transitionDuration是过渡时间，也就是说当该对象的时间刻度改变的时候(通过时间刻度组改变),会有一个过渡时间,是效果更加" +
             "丝滑,而transitionCurve是过渡曲线,,x轴0 - 1为过渡时间0 - transitionDuration,y轴 0 - 1 为 当前TimeScale到目标TimeScale")]
    [SerializeField] protected float transitionDuration = 0.2f;
    [SerializeField] protected AnimationCurve transitionCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);


    protected TimeScaleGroup localTimeScale;
    protected float currentTimeScale; //当前对象时间刻度
    protected float desiredTimeScale;
    protected float transitionStartTime;
    protected float transitionStartValue;
    protected bool isTransitioning = false;
    /// <summary>
    /// 注册的所有时间刻度组
    /// </summary>
    public List<TimeScaleGroup> TimeScaleGroupContainer { get; } = new List<TimeScaleGroup>();
    /// <summary>
    /// 当前时间刻度
    /// </summary>
    public float TimeScale => currentTimeScale;
    public float DeltaTime => Time.deltaTime * TimeScale;
    public float UnscaledDeltaTime => Time.unscaledDeltaTime * TimeScale;
    public float FixedDeltaTime => Time.fixedDeltaTime * TimeScale;
    /// <summary>
    /// 时间刻度的倒数
    /// </summary>
    public virtual float InverseTimeScale => Time.timeScale / TimeScale;
    public TimeScaleGroup SelfTimeScale => localTimeScale;
    public float LocalTimeScaleValue
    {
        get => localTimeScale.ScaleValue;
        set => localTimeScale.SetScaleValue(value);
    }
    public GameObject GameObject => gameObject;
    /// <summary>
    /// 设置当前时间刻度
    /// </summary>
    protected virtual void SetCurrentTimeScale(float value) => currentTimeScale = value;
    /// <summary>
    /// 设置目标时间刻度
    /// </summary>
    protected virtual void SetDesiredTimeScale(float value) => desiredTimeScale = value;

    protected virtual void Awake()
    {
        localTimeScale = new TimeScaleGroup($"{this.gameObject.name}_LocalTimeScale", 1f);
    }

    protected virtual void OnEnable()
    {
        SetDesiredTimeScale(defaultTimeScale);
        SetCurrentTimeScale(defaultTimeScale);
        RegisterToLocalTimeScale(localTimeScale);
    }

    protected virtual void Update() => HandleTransition();

    protected virtual void OnDisable() => UnRegisterFromAll();

    protected virtual void OnDestroy()
    {
        UnRegisterFromAll();
        if (localTimeScale == null) return;
        localTimeScale.Dispose();
        localTimeScale = null;
    }

    /// <summary>
    /// 注册对象到时间刻度组
    /// </summary>
    /// <param name="timeScaleGroup"></param>
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
        ResetDesiredTimeScale();
    }

    /// <summary>
    /// 从时间刻度组注销对象
    /// </summary>
    public virtual void UnRegisterFromLocalTimeScale(TimeScaleGroup timeScaleGroup)
    {
        if (timeScaleGroup == null)
            return;
        if (TimeScaleGroupContainer.Contains(timeScaleGroup))
        {
            timeScaleGroup.UnRegister(this);
            TimeScaleGroupContainer.Remove(timeScaleGroup);
        }

        ResetDesiredTimeScale();
    }

    /// <summary>
    /// 注销对象从所有时间刻度组
    /// </summary>
    public virtual void UnRegisterFromAll()
    {
        for (var i = TimeScaleGroupContainer.Count - 1; i >= 0; i--)
        {
            var timeScaleGroup = TimeScaleGroupContainer[i];
            UnRegisterFromLocalTimeScale(timeScaleGroup);
        }
        TimeScaleGroupContainer.Clear();
        ResetDesiredTimeScale();
    }

    /// <summary>
    /// 合并所有时间刻度
    /// </summary>
    protected virtual float CombinedTimeScale()
    {
        RemoveNullTimeScaleGroup();
        var combinedTimeScale = localTimeScale.ScaleValue;
        foreach (var group in TimeScaleGroupContainer)
            combinedTimeScale *= group.ScaleValue;
        return combinedTimeScale;
    }

    /// <summary>
    /// 将已经销毁的时间刻度组移除
    /// </summary>
    protected virtual void RemoveNullTimeScaleGroup() => TimeScaleGroupContainer.RemoveAll(group => group == null);

    /// <summary>
    /// 应用过渡,会在Update中调用
    /// </summary>
    protected virtual void HandleTransition()
    {
        if (!isTransitioning) return;

        var elapsedTime = Time.time - transitionStartTime;
        var t = transitionDuration > 0 ? Mathf.Clamp01(elapsedTime / transitionDuration) : 1f;
        var curveValue = transitionCurve.Evaluate(t);
        var newValue = Mathf.Lerp(transitionStartValue, desiredTimeScale, curveValue);

        LogSystem.Log($"transitionStartValue {transitionStartValue}, desiredTimeScale {desiredTimeScale}, curveValue {curveValue} newValue {newValue} ");
        SetCurrentTimeScale(newValue);

        if (t >= 1.0f || Mathf.Approximately(newValue, desiredTimeScale))
        {
            EndTransition();
        }
    }

    /// <summary>
    /// 设置目标时间刻度
    /// 可以选择过渡模式，或者立即应用
    /// </summary>
    /// <param name="applyImmediately">是否立即应用</param>
    protected virtual void ResetDesiredTimeScale(bool applyImmediately = false)
    {
        SetDesiredTimeScale(CombinedTimeScale());
        if (applyImmediately)
        {
            EndTransition();
            return;
        }
        StartTransition();
    }

    /// <summary>
    /// 开始过渡
    /// </summary>
    protected virtual void StartTransition()
    {
        if (transitionDuration <= 0)
        {
            SetDesiredTimeScale(CombinedTimeScale());
            SetCurrentTimeScale(CombinedTimeScale());
            isTransitioning = false;
            return;
        }

        isTransitioning = true;
        transitionStartTime = Time.time;
        transitionStartValue = currentTimeScale;
        SetDesiredTimeScale(CombinedTimeScale());
    }

    /// <summary>
    /// 结束过渡
    /// </summary>
    protected virtual void EndTransition()
    {
        isTransitioning = false;
        SetCurrentTimeScale(desiredTimeScale);
    }

    #region 回调

    /// <summary>
    /// 时间刻度组销毁回调
    /// </summary>
    public virtual void OnTimeScaleGroupDestroyed(TimeScaleGroup source)
    {
        if (source == null)
            return;
        if (TimeScaleGroupContainer.Contains(source))
            TimeScaleGroupContainer.Remove(source);
    }

    /// <summary>
    /// 时间刻度组修改时间刻度回调
    /// </summary>
    public virtual void OnTimeScaleChanged(float timeScale, TimeScaleGroup source)
    {
        if (transitionDuration <= 0)
        {
            SetDesiredTimeScale(CombinedTimeScale());
            SetCurrentTimeScale(CombinedTimeScale());
            isTransitioning = false;
            return;
        }

        ResetDesiredTimeScale();
    }

    #endregion
}
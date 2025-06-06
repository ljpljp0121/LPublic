using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using GAS.Runtime;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;


[RequireComponent(typeof(Animator))]
public class AnimationCom : GameComponent
{
    private Dictionary<string, AnimationClip> animClipDic = new Dictionary<string, AnimationClip>();

    public Transform ModelTransform => this.transform;

    private Animator animator;
    private PlayableGraph graph;
    private AnimationMixerPlayable mixer;

    private AnimationNodeBase previousNode; // 上一个节点
    private AnimationNodeBase currentNode; // 当前节点
    private int inputPort0 = 0;
    private int inputPort1 = 1;

    private Action onAnimEnd;
    private float animEndTime;
    private SingleAnimationNode trackNode;

    private Coroutine transitionCoroutine;

    private float speed;
    public float Speed
    {
        get => speed;
        set
        {
            speed = value;
            currentNode.SetSpeed(speed);
        }
    }

    public void Awake()
    {
        animator = GetComponent<Animator>();
        // 创建图
        graph = PlayableGraph.Create("AnimationPlayer");
        if (!graph.IsValid())
        {
            LogSystem.Error("PlayableGraph 创建失败！");
            return;
        }
        // 设置图的时间模式
        graph.SetTimeUpdateMode(DirectorUpdateMode.GameTime);
        // 创建混合器
        mixer = AnimationMixerPlayable.Create(graph, 3);
        if (!mixer.IsValid())
        {
            LogSystem.Error("AnimationMixerPlayable 创建失败！");
            graph.Destroy();
            return;
        }
        // 创建Output
        AnimationPlayableOutput playableOutput = AnimationPlayableOutput.Create(graph, "Animation", animator);
        // 让混合器链接上Output
        playableOutput.SetSourcePlayable(mixer);
    }

    public override void Enable()
    {
        try
        {
            graph.Play();
        }
        catch
        {
            LogSystem.Error("graph 没有初始化，为null");
        }
    }

    public override void Disable()
    {
        try
        {
            graph.Stop();
        }
        catch
        {
            LogSystem.Error("graph 没有初始化，为null");
        }
    }

    public void OnDestroy()
    {
        try
        {
            graph.Destroy();
        }
        catch
        {
            LogSystem.Error("graph 没有初始化，为null");
        }
    }

    private void DestroyNode(AnimationNodeBase node)
    {
        if (node != null)
        {
            graph.Disconnect(mixer, node.InputPort);
            node.PushPool();
        }
    }

    /// <summary>
    /// 播放动画过度，在切换AnimationClip时会调用
    /// </summary>
    /// <param name="fixedTime"></param>
    private void StartTransitionAnimation(float fixedTime)
    {
        if (transitionCoroutine != null) StopCoroutine(transitionCoroutine);
        transitionCoroutine = StartCoroutine(TransitionAnimation(fixedTime));
    }

    // 动画过渡
    private IEnumerator TransitionAnimation(float fixedTime)
    {
        // 交换端口号
        (inputPort0, inputPort1) = (inputPort1, inputPort0);

        // 硬切判断
        if (fixedTime == 0)
        {
            mixer.SetInputWeight(inputPort1, 0);
            mixer.SetInputWeight(inputPort0, 1);
        }

        // 当前的权重
        float currentWeight = 1;
        float _speed = 1 / fixedTime;

        while (currentWeight > 0)
        {
            // 权重在减少
            currentWeight = Mathf.Clamp01(currentWeight - Time.deltaTime * _speed);
            mixer.SetInputWeight(inputPort1, currentWeight); // 减少
            mixer.SetInputWeight(inputPort0, 1 - currentWeight); // 增加
            yield return null;
        }
        transitionCoroutine = null;
    }

    #region 单独动画

    public void PlaySingleAnimation(string clipName, float speed = 1, bool refreshAnimation = false,
        float transitionFixedTime = 0f)
    {
        string realClipName = Path.GetFileNameWithoutExtension(clipName);
        if (!animClipDic.TryGetValue(realClipName, out var animationClip))
        {
            animationClip = AssetSystem.LoadAsset<AnimationClip>(clipName);
            if (animationClip == null)
            {
                LogSystem.Error($"没有找到AnimationClip: {clipName}");
                return;
            }
            animClipDic.Add(realClipName, animationClip);
        }
        PlaySingleAnimation(animationClip, speed, refreshAnimation, transitionFixedTime);
    }

    public void PlaySingleAnimation(string clipName, Action onAnimEnd)
    {
        string realClipName = Path.GetFileNameWithoutExtension(clipName);
        if (!animClipDic.TryGetValue(realClipName, out var animationClip))
        {
            animationClip = AssetSystem.LoadAsset<AnimationClip>(clipName);
            if (animationClip == null)
            {
                LogSystem.Error($"没有找到AnimationClip: {clipName}");
                return;
            }
            animClipDic.Add(realClipName, animationClip);
        }
        PlaySingleAnimation(animationClip, onAnimEnd);
    }

    public void PlaySingleAnimation(AnimationClip animationClip, float speed = 1, bool refreshAnimation = false,
        float transitionFixedTime = 0f)
    {
        SingleAnimationNode singleAnimationNode = null;
        if (currentNode == null) // 首次播放
        {
            singleAnimationNode = AssetSystem.GetOrNew<SingleAnimationNode>();
            singleAnimationNode.Init(graph, mixer, animationClip, speed, inputPort0);
            mixer.SetInputWeight(inputPort0, 1);
        }
        else
        {
            // 相同的动画
            if (!refreshAnimation && currentNode is SingleAnimationNode preNode &&
                preNode.GetAnimationClip() == animationClip) return;
            // 销毁掉当前可能被占用的Node
            DestroyNode(previousNode);
            singleAnimationNode = AssetSystem.GetOrNew<SingleAnimationNode>();
            singleAnimationNode.Init(graph, mixer, animationClip, speed, inputPort1);
            previousNode = currentNode;
            StartTransitionAnimation(transitionFixedTime);
        }
        this.speed = speed;
        currentNode = singleAnimationNode;
        if (graph.IsPlaying() == false) graph.Play();
        LogSystem.Log($"开始播放动画: {animationClip.name}");
    }

    public void PlaySingleAnimation(AnimationClip animationClip, Action onAnimEnd)
    {
        PlaySingleAnimation(animationClip);
        if (onAnimEnd != null)
        {
            this.onAnimEnd = onAnimEnd;
            trackNode = currentNode as SingleAnimationNode;
            if (trackNode != null && trackNode.GetAnimationClip() != null)
            {
                float clipLength = trackNode.GetAnimationClip().length;
                animEndTime = Time.time + clipLength / speed;
            }
            else
            {
                onAnimEnd?.Invoke();
                ClearCallback();
            }
        }
    }

    #endregion

    #region 混合动画

    public void PlayBlendAnimation(List<AnimationClip> clips, float speed = 1, float transitionFixedTime = 0.25f)
    {
        BlendAnimationNode blendAnimationNode = AssetSystem.GetOrNew<BlendAnimationNode>();
        // 如果是第一次播放，不存在过渡
        if (currentNode == null)
        {
            blendAnimationNode.Init(graph, mixer, clips, speed, inputPort0);
            mixer.SetInputWeight(inputPort0, 1);
        }
        else
        {
            DestroyNode(previousNode);
            blendAnimationNode.Init(graph, mixer, clips, speed, inputPort1);
            previousNode = currentNode;
            StartTransitionAnimation(transitionFixedTime);
        }
        this.speed = speed;
        currentNode = blendAnimationNode;
        if (graph.IsPlaying() == false) graph.Play();
    }

    public void PlayBlendAnimation(AnimationClip clip1, AnimationClip clip2, float speed = 1,
        float transitionFixedTime = 0.25f)
    {
        BlendAnimationNode blendAnimationNode = AssetSystem.GetOrNew<BlendAnimationNode>();
        // 如果是第一次播放，不存在过渡
        if (currentNode == null)
        {
            blendAnimationNode.Init(graph, mixer, clip1, clip2, speed, inputPort0);
            mixer.SetInputWeight(inputPort0, 1);
        }
        else
        {
            DestroyNode(previousNode);
            blendAnimationNode.Init(graph, mixer, clip1, clip2, speed, inputPort1);
            previousNode = currentNode;
            StartTransitionAnimation(transitionFixedTime);
        }
        this.speed = speed;
        currentNode = blendAnimationNode;
        if (graph.IsPlaying() == false) graph.Play();
    }

    public void SetBlendWeight(List<float> weightList)
    {
        (currentNode as BlendAnimationNode)?.SetBlendWeight(weightList);
    }

    public void SetBlendWeight(float clip1Weight)
    {
        (currentNode as BlendAnimationNode)?.SetBlendWeight(clip1Weight);
    }

    #endregion

    public void ClearCallback()
    {
        onAnimEnd = null;
        animEndTime = 0;
    }

    public override void Tick()
    {
        if (onAnimEnd != null)
        {
            if (currentNode != trackNode)
            {
                ClearCallback();
                return;
            }

            if (Time.time >= animEndTime)
            {
                onAnimEnd?.Invoke();
                ClearCallback();
            }
        }
    }

    #region RootMotion

    private Action<Vector3, Quaternion> rootMotionAction;

    private void OnAnimatorMove()
    {
        rootMotionAction?.Invoke(animator.deltaPosition, animator.deltaRotation);
    }

    public void SetRootMotionAction(Action<Vector3, Quaternion> rootMotionAction)
    {
        this.rootMotionAction = rootMotionAction;
    }

    public void ClearRootMotionAction()
    {
        rootMotionAction = null;
    }

    #endregion

    #region 动画事件

    private readonly Dictionary<string, Action> eventDic = new Dictionary<string, Action>();

    private void AnimationEvent(string eventName)
    {
        if (eventDic.TryGetValue(eventName, out Action action))
        {
            action?.Invoke();
        }
    }

    public void AddAnimationEvent(string eventName, Action action)
    {
        if (eventDic.TryGetValue(eventName, out Action act))
        {
            act += action;
        }
        else
        {
            eventDic.Add(eventName, action);
        }
    }

    public void RemoveAnimationEvent(string eventName)
    {
        eventDic.Remove(eventName);
    }

    public void RemoveAnimationEvent(string eventName, Action action)
    {
        if (eventDic.TryGetValue(eventName, out Action act))
        {
            act -= action;
        }
    }

    public void CleanAllActionEvent()
    {
        eventDic.Clear();
    }

    #endregion
}
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;


[InitializeOrder(100)]
public class AnimationComponent : MonoBehaviour, IComponent, IRequire<Animator>
{
    private Animator animator;
    private PlayableGraph graph;
    private AnimationMixerPlayable mixer;

    private AnimationNodeBase previousNode; // 上一个节点
    private AnimationNodeBase currentNode; // 当前节点
    private int inputPort0 = 0;
    private int inputPort1 = 1;

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

    public void SetDependency(Animator dependency) => animator = dependency;

    public void Init()
    {
        // 创建图
        graph = PlayableGraph.Create("AnimationPlayer");
        // 设置图的时间模式
        graph.SetTimeUpdateMode(DirectorUpdateMode.GameTime);
        // 创建混合器
        mixer = AnimationMixerPlayable.Create(graph, 3);
        // 创建Output
        AnimationPlayableOutput playableOutput = AnimationPlayableOutput.Create(graph, "Animation", animator);
        // 让混合器链接上Output
        playableOutput.SetSourcePlayable(mixer);
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

    
    
    /// <summary>
    /// 播放单个动画
    /// </summary>
    public void PlaySingleAnimation(AnimationClip animationClip, float speed = 1, bool refreshAnimation = false,
        float transitionFixedTime = 0.25f)
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
    }

    /// <summary>
    /// 播放混合动画
    /// </summary>
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

    /// <summary>
    /// 播放混合动画
    /// 如果只是播放两个混合动画使用这个
    /// </summary>
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

    private void OnDestroy()
    {
        graph.Destroy();
    }

    private void OnDisable()
    {
        graph.Stop();
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

    // Animator会触发的实际事件函数
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
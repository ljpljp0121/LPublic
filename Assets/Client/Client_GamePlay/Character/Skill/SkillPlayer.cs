using UnityEngine;

public class SkillPlayer : MonoBehaviour, IComponent, IUpdatable, IRequire<AnimationComponent>
{
    private AnimationComponent animationComponent;

    public bool IsPlaying { get; private set; }

    private int currentFrameIndex;
    private float playerTotalTime;
    private int frameRate;

    public void SetDependency(AnimationComponent dependency) => animationComponent = dependency;

    public void Init()
    {
        
    }

    void IUpdatable.OnUpdate()
    {
        OnUpdate();
    }

    public void UnInit()
    {
        MonoSystem.RemoveUpdate(OnUpdate);
    }

    private void OnUpdate()
    {
        if (!IsPlaying) return;
    }
}
using UnityEngine;

public class SkillPlayer : MonoBehaviour
{
    private AnimationComponent animationComponent;

    public bool IsPlaying { get; private set; }

    private int currentFrameIndex;
    private float playerTotalTime;
    private int frameRate;

    public void Init(AnimationComponent animationComponent)
    {
        MonoSystem.AddUpdate(OnUpdate);
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
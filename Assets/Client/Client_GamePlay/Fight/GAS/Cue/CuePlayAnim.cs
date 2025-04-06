using GAS.Runtime;
using UnityEngine;

public class CuePlayAnim : GameplayCueInstant
{
    [SerializeField] private AnimationClip AnimClip;
    public AnimationClip AnimationClip => AnimClip;

    public override GameplayCueInstantSpec CreateSpec(GameplayCueParameters parameters) =>
        new CuePlayAnimSpec(this, parameters);
#if UNITY_EDITOR
    public override void OnEditorPreview(GameObject previewObject, int frame, int startFrame)
    {
        var entity = previewObject.GetComponentInChildren<AnimationCom>();
        var animObject = entity.gameObject;
        var anim = entity.GetComponent<Animator>();
        if (startFrame <= frame)
        {
            float clipFrameCount = (int)(AnimClip.frameRate * AnimClip.length);
            if (frame < startFrame + clipFrameCount)
            {
                var progress = (frame - startFrame) / clipFrameCount;
                if (progress > 1 && AnimClip.isLooping) progress -= (int)progress;
                AnimClip.SampleAnimation(animObject, progress * AnimClip.length);
            }
        }
    }
#endif
}

public class CuePlayAnimSpec : GameplayCueInstantSpec
{
    private readonly CuePlayAnim cuePlayAnim;

    public CuePlayAnimSpec(GameplayCueInstant cue, GameplayCueParameters parameters) : base(cue, parameters)
    {
        cuePlayAnim = cue as CuePlayAnim;
    }

    public override void Trigger()
    {
        var animCom = Owner.GetComponentInChildren<AnimationCom>();
        animCom.PlaySingleAnimation(cuePlayAnim.AnimationClip);
    }
}
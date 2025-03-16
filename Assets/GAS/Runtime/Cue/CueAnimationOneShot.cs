using GAS.General;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GAS.Runtime
{
    [CreateAssetMenu(fileName = "CuePlayAnimation", menuName = "GAS/Cue/CuePlayAnimation")]
    public class CueAnimationOneShot : GameplayCueInstant
    {
        [BoxGroup]
        [InfoBox(GASTextDefine.CUE_ANIMATION_PATH_TIP)]
        [LabelText(GASTextDefine.CUE_ANIMATION_PATH)]
        [SerializeField]
        private string _animatorRelativePath;

        [BoxGroup] [LabelText(GASTextDefine.CUE_ANIMATION_STATE)] [SerializeField]
        private string _stateName;

        public string AnimatorRelativePath => _animatorRelativePath;
        public string StateName => _stateName;


        public override GameplayCueInstantSpec CreateSpec(GameplayCueParameters parameters)
        {
            return new CueAnimationOneShotSpec(this, parameters);
        }

#if UNITY_EDITOR
        public override void OnEditorPreview(GameObject previewObject, int frame, int startFrame)
        {
            if (startFrame <= frame)
            {
                var animatorObject = previewObject.transform.Find(AnimatorRelativePath);
                var animator = animatorObject.GetComponent<Animator>();
                var stateMap = animator.GetAllAnimationState();
                if (stateMap.TryGetValue(StateName, out var clip))
                {
                    float clipFrameCount = (int)(clip.frameRate * clip.length);
                    if (frame <= clipFrameCount + startFrame)
                    {
                        var progress = (frame - startFrame) / clipFrameCount;
                        if (progress > 1 && clip.isLooping) progress -= (int)progress;
                        clip.SampleAnimation(animatorObject.gameObject, progress * clip.length);
                    }
                }
            }
        }
#endif
    }

    public class CueAnimationOneShotSpec : GameplayCueInstantSpec<CueAnimationOneShot>
    {
        private readonly Animator _animator;

        public CueAnimationOneShotSpec(CueAnimationOneShot cue, GameplayCueParameters parameters) : base(cue,
            parameters)
        {
            var animatorTransform = Owner.transform.Find(cue.AnimatorRelativePath);
            _animator = animatorTransform.GetComponent<Animator>();
        }

        public override void Trigger()
        {
            _animator.Play(cue.StateName);
        }
    }
}
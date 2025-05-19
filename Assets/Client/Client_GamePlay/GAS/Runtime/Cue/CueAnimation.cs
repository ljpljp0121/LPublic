using GAS.General;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GAS.Runtime
{
    public class CueAnimation:SkillCueDurational
    {
        [BoxGroup]
        [InfoBox(SkillDefine.CUE_ANIMATION_PATH_TIP)]
        [LabelText(SkillDefine.CUE_ANIMATION_PATH)]
        [SerializeField]
        private string _animatorRelativePath;

        [BoxGroup] [LabelText(SkillDefine.CUE_ANIMATION_STATE)] [SerializeField]
        private string _stateName;

        public string AnimatorRelativePath => _animatorRelativePath;
        public string StateName => _stateName;


        public override SkillCueDurationalSpec CreateSpec(SkillCueParameters parameters)
        {
            return new CueAnimationSpec(this, parameters);
        }
        
#if UNITY_EDITOR
        public override void OnEditorPreview(GameObject previewObject, int frame, int startFrame,int endFrame)
        {
            if (startFrame <= frame && frame <= endFrame)
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
    
    public class CueAnimationSpec : SkillCueDurationalSpec<CueAnimation>
    {
        private readonly Animator _animator;

        public CueAnimationSpec(CueAnimation cue, SkillCueParameters parameters) : base(cue,
            parameters)
        {
            var animatorTransform = Owner.transform.Find(cue.AnimatorRelativePath);
            _animator = animatorTransform.GetComponent<Animator>();
        }

        public override void OnAdd()
        {
            _animator.Play(cue.StateName);
        }

        public override void OnRemove()
        {
        }

        public override void OnGameplayEffectActivate()
        {
        }

        public override void OnGameplayEffectDeactivate()
        {
        }

        public override void OnTick()
        {
        }
    }
}
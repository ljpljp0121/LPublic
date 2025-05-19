using System;
using GAS.General;
using Sirenix.OdinInspector;
using UnityEngine;
using Object = UnityEngine.Object;

namespace GAS.Runtime
{
    public class CuePlaySound : SkillCueDurational
    {
        [BoxGroup]
        [LabelText(SkillDefine.CUE_SOUND_EFFECT)]
        public AudioClip soundEffect; 
        
        [BoxGroup]
        [LabelText(SkillDefine.CUE_ATTACH_TO_OWNER)]
        public bool isAttachToOwner = true;
        
        public override SkillCueDurationalSpec CreateSpec(SkillCueParameters parameters)
        {
            return new CuePlaySoundSpec(this, parameters);
        }
    }
    
    public class CuePlaySoundSpec : SkillCueDurationalSpec<CuePlaySound>
    {
        private AudioSource _audioSource;
        
        public CuePlaySoundSpec(CuePlaySound cue, SkillCueParameters parameters) : base(cue,
            parameters)
        {
            if (cue.isAttachToOwner)
            {
                _audioSource = Owner.gameObject.GetComponent<AudioSource>();
                if (_audioSource == null)
                {
                    _audioSource = Owner.gameObject.AddComponent<AudioSource>();
                }
            }
            else
            {
                var soundRoot = new GameObject("SoundRoot");
                soundRoot.transform.position = Owner.transform.position;
                _audioSource = soundRoot.AddComponent<AudioSource>();
            }
        }

        public override void OnAdd()
        {
            _audioSource.clip = cue.soundEffect;
            _audioSource.Play();
        }

        public override void OnRemove()
        {
            if (!cue.isAttachToOwner)
            {
                Object.Destroy(_audioSource.gameObject);
            }else
            {
                _audioSource.Stop();
                _audioSource.clip = null;
            }
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
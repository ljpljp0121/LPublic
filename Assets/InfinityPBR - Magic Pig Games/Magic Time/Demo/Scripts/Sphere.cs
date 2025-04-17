using InfinityPBR;
using UnityEngine;

/*
 * This is the "MagicTimeUser" class that is the "brains" of the spheres. The additional logic handles the audio
 * and bumping the camera when the sphere enters or exists a trigger zone.
 */

namespace MagicPigGames.MagicTime.Demo
{
    public class Sphere : MagicTimeUser
    {
        [Header("Audio Clips")] 
        public AudioClip[] enterClips;
        public AudioClip[] exitClips;
        
        [Header("Plumbing")]
        public AudioSource audioSource;

        public void Enter()
        {
            PlayClip(enterClips.TakeRandom());
            BumpCamera();
        }

        public void Exit()
        {
            PlayClip(exitClips.TakeRandom());
            BumpCamera();
        }

        public void PlayClip(AudioClip clip)
        {
            if (audioSource.isPlaying)
                return;
            
            audioSource.clip = clip;
            audioSource.Play();
        }

        private CameraControl _cameraControl;
        private void BumpCamera()
        {
            if (_cameraControl == null)
                _cameraControl = CameraControl.Instance;
            
            _cameraControl.Bump();
        }

        private void OnValidate()
        {
            if (audioSource == null)
                audioSource = GetComponent<AudioSource>();
        }
    }
}